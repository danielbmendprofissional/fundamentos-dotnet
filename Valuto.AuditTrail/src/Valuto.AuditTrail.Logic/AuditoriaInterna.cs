using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Valuto.AuditTrail.Abstractions;
using Valuto.AuditTrail.Abstractions.Commom;
using Valuto.AuditTrail.Abstractions.Contracts;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;
using Valuto.AuditTrail.Abstractions.Model.Entities;
using Valuto.AuditTrail.Abstractions.Model.Enums;
using Valuto.AuditTrail.Abstractions.Model.Enums.Valuto.AuditTrail.Abstractions.Model.Enums;
using Valuto.AuditTrail.Abstractions.Model.Events;
using Valuto.AuditTrail.Abstractions.Model.Helpers;
using Valuto.AuditTrail.Abstractions.Model.Snapshots;
using Valuto.AuditTrail.Logic.Paralel;

namespace Valuto.AuditTrail.Logic
{
    //[System.Diagnostics.DebuggerStepThrough]
    internal class AuditoriaInterna : IAuditoriaInterna
    {
        private readonly IUserProvider _userProvider;
        private readonly IMetadataAdapter _metadataAdapter;
        private readonly IAuditBuffer<AuditTrailCreatedEvent> _auditBuffer;
        private readonly IAuditBuffer<AuditLogEvent> _logBuffer;

        public AuditoriaInterna(
            IUserProvider userProvider,
            IMetadataAdapter metadataAdapter,
            IAuditBuffer<AuditTrailCreatedEvent> auditBuffer,
            IAuditBuffer<AuditLogEvent> logBuffer)
        {
            _userProvider = userProvider;
            _metadataAdapter = metadataAdapter;
            _auditBuffer = auditBuffer;
            _logBuffer = logBuffer;
        }

        public (List<EntitySnapshot> Snapshots, List<Action> Materializadores) GerarSnapshot(ITrackerAdapter tracker)
        {
            var materializadores = new List<Action>();

            try
            {
                var snapshots = tracker.Entries(_metadataAdapter)
                    .Where(e => ValidarEstadoAuditoria(e.State, e.Type))
                    .Select(e =>
                    {
                        var snapshot = EntitySnapshot.Create(
                            e.Name,
                            e.Type.Namespace ?? string.Empty,
                            e.State);

                        foreach (var prop in e.Properties)
                        {
                            snapshot.Properties.Add(PropertySnapshot.Create(prop, e.State));
                        }

                        foreach (var foreign in e.ForeignKeys)
                        {
                            snapshot.ForeignKeys.Add(ForeignKeySnapshot.Create(foreign));
                        }

                        foreach (var nav in e.Navigations)
                        {
                            snapshot.NavEntries.Add(NavigationSnapshot.Create(nav.Name, nav.Values));
                        }

                        foreach (var @ref in e.References)
                        {
                            snapshot.References.Add(ReferenceSnapshot.Create(@ref.Name, @ref.IsModified));
                        }

                        return snapshot;
                    }).ToList();

                return (snapshots, materializadores);
            }
            catch
            {
                return (Enumerable.Empty<EntitySnapshot>().ToList(), materializadores);
            }
        }

        public async Task ProcessarAuditoriaAsync(List<EntitySnapshot> snapshots, CancellationToken cancellationToken = default)
        {
            var ctx = new AuditProcessingContextHelper();
            try
            {
                IniciarAuditoria(ctx, snapshots);
                await FinalizarAuditoriaAsync(ctx, cancellationToken);
            }
            catch (Exception ex)
            {
                await EnviarLog(ex, ctx);
            }
        }

        public async Task FinalizarAuditoriaAsync(AuditProcessingContextHelper ctx, CancellationToken cancellationToken = default)
        {
            try
            {
                if (ctx.Helpers == null || ctx.Helpers.Count == 0) return;

                ProcessarMetadadosPosSave(ctx);

                var trails = ctx.Helpers.Select(x => x.Trail);

                await EnviarParaFilaAsync(trails, cancellationToken);
            }
            catch (Exception ex)
            {
                await EnviarLog(ex, ctx);
            }
        }

        private bool ValidarEstadoAuditoria(EntryState state, Type? type)
        {
            if (type == null || state == EntryState.Unchanged)
                return false;

            var attribute = _metadataAdapter.GetAuditableAttribute(type);
            if (attribute == null) return false;

            return VerificarPermissaoEstado(state, attribute.Estados);
        }

        private bool VerificarPermissaoEstado(EntryState state, AuditoriaEstado estadosPermitidos)
        {
            switch (state)
            {
                case EntryState.Added: return (estadosPermitidos & AuditoriaEstado.Create) != 0;
                case EntryState.Deleted: return (estadosPermitidos & AuditoriaEstado.Delete) != 0;
                case EntryState.Modified: return (estadosPermitidos & AuditoriaEstado.Update) != 0;
                default: return false;
            }
        }

        private void IniciarAuditoria(AuditProcessingContextHelper ctx, List<EntitySnapshot> entries)
        {
            ctx.Step = AuditConstants.PreSaveCapturando;
            var usuario = _userProvider?.GetCurrentUser() ?? AuditConstants.NaoInformado;

            foreach (var entry in entries)
            {
                ctx.Track(entry, AuditConstants.PreSaveCriando);

                var trail = CriarTrailEntity(entry, usuario, ctx);
                ctx.Helpers.Add(new AuditHelper(entry, trail));
            }
        }

        private AuditTrails CriarTrailEntity(EntitySnapshot entry, string usuario, AuditProcessingContextHelper ctx)
        {
            var entityName = entry.Name;
            var aplicacao = entry.Namespace;

            ctx.Track(entry, AuditConstants.PreSaveMetadados + entityName);

            var trailEntry = AuditTrails.Criar(usuario, aplicacao, entityName);

            DefinirValorPropriedades(entry, trailEntry);
            DefinirValoresNavegacao(entry, trailEntry);
            DefinirValoresReferencia(entry, trailEntry);

            return trailEntry;
        }

        private void DefinirValorPropriedades(EntitySnapshot entry, AuditTrails trailEntry)
        {
            var trailType = ObterTipoTrilha(entry.State);
            trailEntry.DefinirTrailType(trailType);

            var properties = entry.Properties.Where(x => !x.IsTemporary);
            foreach (var property in properties)
            {
                ProcessarValoresPropriedade(entry.State, trailEntry, property, property.Name);
            }
        }

        private TrailType ObterTipoTrilha(EntryState state)
        {
            switch (state)
            {
                case EntryState.Added: return TrailType.Create;
                case EntryState.Deleted: return TrailType.Delete;
                case EntryState.Modified: return TrailType.Update;
                default: return TrailType.None;
            }
        }

        private void ProcessarValoresPropriedade(EntryState state, AuditTrails trailEntry, PropertySnapshot property, string propertyName)
        {
            switch (state)
            {
                case EntryState.Added:
                    trailEntry.AdicionarNewValues(propertyName, property.CurrentValue);
                    break;
                case EntryState.Deleted:
                    trailEntry.AdicionarOldValues(propertyName, property.OriginalValue);
                    break;
                case EntryState.Modified:
                    if (property.IsModified && (property.OriginalValue == null || !property.OriginalValue.Equals(property.CurrentValue)))
                    {
                        trailEntry.AdicionarChangedColumns(propertyName);
                        trailEntry.AdicionarOldValues(propertyName, property.OriginalValue);
                        trailEntry.AdicionarNewValues(propertyName, property.CurrentValue);
                    }
                    break;
            }
        }

        private void DefinirValoresNavegacao(EntitySnapshot entry, AuditTrails trailEntry)
        {
            var collections = entry.NavEntries;
            if (collections == null) return;

            foreach (var collection in collections)
            {
                var collectionName = collection.Name;
                var values = collection.Values;

                var changes = ObterMudancasColecao(values, entry.State);

                ProcessarInclusaoColecao(trailEntry, collectionName, changes);
            }
        }

        private IEnumerable<NavigationHelper> ObterMudancasColecao(IEnumerable values, EntryState state)
        {
            if (values == null) yield break;

            foreach (var item in values)
            {
                if (item == null) continue;

                yield return new NavigationHelper
                {
                    Status = state.ToString(),
                    Instance = item
                };
            }
        }

        private void ProcessarInclusaoColecao(AuditTrails trail, string name, IEnumerable<NavigationHelper> changes)
        {
            var materializedChanges = changes.Cast<NavigationHelper>().ToList();

            if (materializedChanges.Count > 0)
            {
                trail.AdicionarNewValues(name, materializedChanges);
                trail.AdicionarChangedColumns(name);
            }
        }

        private void DefinirValoresReferencia(EntitySnapshot entry, AuditTrails trailEntry)
        {
            var references = entry.References;
            if (references == null) return;

            foreach (var reference in references)
            {
                string referenceName = reference.Name;
                if (!reference.IsModified)
                    trailEntry.AdicionarChangedColumns(referenceName);
            }
        }

        private void ProcessarMetadadosPosSave(AuditProcessingContextHelper ctx)
        {
            ctx.Step = AuditConstants.PosSaveMetadados;
            if (ctx.Helpers == null) return;

            foreach (var audit in ctx.Helpers)
            {
                ctx.CurrentEntry = audit.Entry;
                var trail = audit.Trail;
                var entry = audit.Entry;

                DefinirChavePrimaria(entry, trail, ctx);
                DefinirRoots(entry, trail, ctx);
                DefinirIdColecoes(trail, ctx);

                trail.DefinirTransactionId(ctx.TransactionId);
            }
        }

        private void DefinirChavePrimaria(EntitySnapshot entry, AuditTrails trail, AuditProcessingContextHelper ctx)
        {
            if (!string.IsNullOrEmpty(trail.PrimaryKey)) return;

            var entityName = entry.Name;
            ctx.Step = AuditConstants.PosSavePk + entityName;

            var idValue = ObterIdDaEntry(entry);
            trail.DefinirPrimaryKey(idValue);
        }

        private void DefinirIdColecoes(AuditTrails trail, AuditProcessingContextHelper ctx)
        {
            if (ctx == null || trail.NewValues.Count == 0) return;

            foreach (var entry in trail.NewValues)
            {
                if (entry.Value is IEnumerable<NavigationHelper> collection)
                {
                    foreach (var change in collection)
                    {
                        ProcessarIdNavegacao(change, ctx);
                    }
                }
            }
        }

        private void ProcessarIdNavegacao(NavigationHelper change, AuditProcessingContextHelper ctx)
        {
            if (change.Instance == null) return;

            //var pkNames = change.PrimaryKeyNames;

            //if (pkNames != null && pkNames.Any())
            //{
            //    var values = new List<string>();

            //    foreach (var name in pkNames)
            //    {
            //        var prop = change.Instance.GetType().GetProperty(name);
            //        var val = prop?.GetValue(change.Instance);
            //        if (val != null) values.Add(val.ToString());
            //    }

            //    change.Id = values.Count > 1
            //        ? string.Join("|", values)
            //        : values.FirstOrDefault();
            //}

            //change.Instance = null;
        }

        private string ObterIdDaEntry(EntitySnapshot entry)
        {
            string firstValue = null;
            StringBuilder compositeId = null;

            var primaryKeys = entry.Properties.Where(x => x.IsPrimaryKey);

            foreach (var key in primaryKeys)
            {
                string propName = key.Name;
                string propVal = key.CurrentValue?.ToString();

                if (string.IsNullOrEmpty(propVal)) continue;

                if (firstValue == null)
                {
                    firstValue = propVal;
                }
                else
                {
                    if (compositeId == null) compositeId = new StringBuilder(firstValue);
                    compositeId.Append("|").Append(propVal);
                }
            }

            if (compositeId != null) return compositeId.ToString();
            return firstValue ?? AuditConstants.NotAvailable;
        }

        private void DefinirRoots(EntitySnapshot entry, AuditTrails trail, AuditProcessingContextHelper ctx)
        {
            //ctx.Step = AuditConstants.

            foreach (var fk in entry.ForeignKeys)
            {
                var rootId = fk.CurrentValue?.ToString();
                if (string.IsNullOrWhiteSpace(rootId)) continue;

                var rootName = fk.PrincipalEntityName;
                if (string.IsNullOrWhiteSpace(rootName)) continue;

                trail.DefinirRoot(rootName, rootId);
                return;
            }
        }

        private async Task EnviarParaFilaAsync(IEnumerable<AuditTrails> entries, CancellationToken ct = default)
        {
            foreach (var e in entries)
            {
                var @event = new AuditTrailCreatedEvent(
                    e.Id,
                    e.TransactionId,
                    e.Usuario,
                    e.EntityName,
                    e.SistemaName,
                    e.PrimaryKey,
                    e.OldValues.ToDictionary(kv => kv.Key, kv => kv.Value),
                    e.NewValues.ToDictionary(kv => kv.Key, kv => kv.Value),
                    e.ChangedColumns.ToList(),
                    e.Tipo,
                    e.Root,
                    e.RootId
                );

                await _auditBuffer.EnqueueAsync(@event, ct);
            }
        }

        private async Task EnviarLog(Exception ex, AuditProcessingContextHelper ctx, CancellationToken ct = default)
        {
            try
            {
                var sistema = _metadataAdapter?.GetNamespace(GetType()) ?? AuditConstants.NotAvailable;
                var entityInfo = AuditConstants.NotAvailable;

                if (ctx.CurrentEntry != null)
                {
                    var nome = ctx.CurrentEntry.Name;

                    var idValue = ObterIdDaEntry(ctx.CurrentEntry);
                    entityInfo = $"{nome} [ID: {idValue}]";
                }

                var fullMessage = $"Etapa: {ctx.Step} | Contexto: {entityInfo}";

                await _logBuffer.EnqueueAsync(new AuditLogEvent(
                            ex.Message,
                            ex.StackTrace,
                            sistema,
                            fullMessage), CancellationToken.None);
            }
            catch { }
        }
    }
}
