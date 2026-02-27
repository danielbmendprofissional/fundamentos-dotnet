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
    internal class AuditoriaInterna : IAuditoriaInterna
    {
        private readonly IUserProvider _userProvider;
        private readonly IEfMetadataAdapter _metadataAdapter;
        private readonly IAuditBuffer<AuditTrailCreatedEvent> _auditBuffer;
        private readonly IAuditBuffer<AuditLogEvent> _logBuffer;

        public AuditoriaInterna(
            IUserProvider userProvider,
            IEfMetadataAdapter metadataAdapter,
            IAuditBuffer<AuditTrailCreatedEvent> auditBuffer,
            IAuditBuffer<AuditLogEvent> logBuffer)
        {
            _userProvider = userProvider;
            _metadataAdapter = metadataAdapter;
            _auditBuffer = auditBuffer;
            _logBuffer = logBuffer;
        }

        public List<IAuditEntry> GerarSnapshot(IAuditTracker tracker)
        {
            return tracker.Entries()
                .Where(ValidarEstadoAuditoria)
                .Select(e =>
                {
                    var snapshot = new AuditEntrySnapshot
                    {
                        Entity = e.Entity,
                        State = e.State,
                        TypeMetadata = e.TypeMetadata,
                        Properties = e.Properties.Select(p =>
                        {
                            var propSnapshot = new AuditPropertySnapshot
                            {
                                Name = p.Name,
                                OriginalValue = p.OriginalValue,
                                IsModified = p.IsModified,
                                IsTemporary = p.IsTemporary,
                                Metadata = p.Metadata
                            };

                            if (p.IsTemporary)
                            {
                                var entidadeViva = e.Entity;
                                var nomeProp = p.Name;
                                propSnapshot.SetLazyLoader(() =>
                                    entidadeViva.GetType().GetProperty(nomeProp)?.GetValue(entidadeViva, null));
                            }
                            else
                            {
                                propSnapshot.CurrentValue = p.CurrentValue;
                            }

                            return propSnapshot;
                        }).Cast<IAuditProperty>().ToList()
                    };

                    var collections = _metadataAdapter.GetCollectionsMetadata(e.TypeMetadata);
                    if (collections != null)
                    {
                        foreach (var metadata in collections)
                        {
                            string navName = _metadataAdapter.GetPropertyName(metadata);
                            var navEntry = e.Navigation(navName);

                            if (navEntry != null)
                            {
                                snapshot.NavEntries[navName] = navEntry;
                            }
                        }
                    }

                    var references = _metadataAdapter.GetReferencesMetadata(e.TypeMetadata);
                    if (references != null)
                    {
                        foreach (var navMetadata in references)
                        {
                            string navName = _metadataAdapter.GetPropertyName(navMetadata);
                            var navEntry = e.Navigation(navName);

                            if (navEntry != null)
                            {
                                snapshot.NavEntries[navName] = navEntry;
                            }
                        }
                    }

                    return (IAuditEntry)snapshot;
                }).ToList();
        }

        public async Task ProcessarAuditoriaAsync(List<IAuditEntry> snapshots, CancellationToken cancellationToken = default)
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

        private bool ValidarEstadoAuditoria(IAuditEntry entry)
        {
            if (entry.State == AuditEntryState.Unchanged)
                return false;

            var attribute = _metadataAdapter.GetAuditableAttribute(entry.Entity.GetType());
            if (attribute == null) return false;

            return VerificarPermissaoEstado(entry.State, attribute.Estados);
        }

        private bool VerificarPermissaoEstado(AuditEntryState state, AuditoriaEstado estadosPermitidos)
        {
            switch (state)
            {
                case AuditEntryState.Added: return (estadosPermitidos & AuditoriaEstado.Create) != 0;
                case AuditEntryState.Deleted: return (estadosPermitidos & AuditoriaEstado.Delete) != 0;
                case AuditEntryState.Modified: return (estadosPermitidos & AuditoriaEstado.Update) != 0;
                default: return false;
            }
        }

        private void IniciarAuditoria(AuditProcessingContextHelper ctx, List<IAuditEntry> entries)
        {
            ctx.Step = AuditConstants.PreSaveCapturando;
            var usuario = _userProvider?.GetCurrentUser() ?? AuditConstants.NaoInformado;

            foreach (var entry in entries)
            {
                ctx.EntriesMap[entry.Entity] = entry;
                ctx.Track(entry, AuditConstants.PreSaveCriando);

                var trail = CriarTrailEntity(entry, usuario, ctx);
                ctx.Helpers.Add(new AuditHelper(entry, trail));
            }
        }

        private AuditTrails CriarTrailEntity(IAuditEntry entry, string usuario, AuditProcessingContextHelper ctx)
        {
            var type = entry.Entity.GetType();

            var entityName = _metadataAdapter.GetEntityName(type);
            var aplicacao = _metadataAdapter.GetNamespace(type);

            ctx.Track(entry, AuditConstants.PreSaveMetadados + entityName);

            var trailEntry = AuditTrails.Criar(usuario, aplicacao, entityName);

            DefinirValorPropriedades(entry, trailEntry);
            DefinirValoresNavegacao(entry, trailEntry);
            DefinirValoresReferencia(entry, trailEntry);

            return trailEntry;
        }

        private void DefinirValorPropriedades(IAuditEntry entry, AuditTrails trailEntry)
        {
            var trailType = ObterTipoTrilha(entry.State);
            trailEntry.DefinirTrailType(trailType);

            var properties = entry.Properties.Where(x => !x.IsTemporary);
            foreach (var property in properties)
            {
                string propertyName = _metadataAdapter.GetPropertyName(property.Metadata);
                ProcessarValoresPropriedade(entry.State, trailEntry, property, propertyName);
            }
        }

        private TrailType ObterTipoTrilha(AuditEntryState state)
        {
            switch (state)
            {
                case AuditEntryState.Added: return TrailType.Create;
                case AuditEntryState.Deleted: return TrailType.Delete;
                case AuditEntryState.Modified: return TrailType.Update;
                default: return TrailType.None;
            }
        }

        private void ProcessarValoresPropriedade(AuditEntryState state, AuditTrails trailEntry, IAuditProperty property, string propertyName)
        {
            switch (state)
            {
                case AuditEntryState.Added:
                    trailEntry.AdicionarNewValues(propertyName, property.CurrentValue);
                    break;
                case AuditEntryState.Deleted:
                    trailEntry.AdicionarOldValues(propertyName, property.OriginalValue);
                    break;
                case AuditEntryState.Modified:
                    if (property.IsModified && (property.OriginalValue == null || !property.OriginalValue.Equals(property.CurrentValue)))
                    {
                        trailEntry.AdicionarChangedColumns(propertyName);
                        trailEntry.AdicionarOldValues(propertyName, property.OriginalValue);
                        trailEntry.AdicionarNewValues(propertyName, property.CurrentValue);
                    }
                    break;
            }
        }

        private void DefinirValoresNavegacao(IAuditEntry entry, AuditTrails trailEntry)
        {
            var collections = _metadataAdapter.GetCollectionsMetadata(entry.TypeMetadata);
            if (collections == null) return;

            foreach (var metadata in collections)
            {
                string navigationName = _metadataAdapter.GetPropertyName(metadata);
                if (string.IsNullOrEmpty(navigationName) || navigationName == AuditConstants.Unknown)
                    continue;

                var navEntry = entry.Navigation(navigationName);
                if (navEntry == null) continue;

                if (!(_metadataAdapter.GetCurrentValue(navEntry) is IEnumerable values))
                    continue;

                var changes = ObterMudancasColecao(values, entry.State);

                ProcessarInclusaoColecao(trailEntry, navigationName, changes);
            }
        }

        private IEnumerable<NavigationHelper> ObterMudancasColecao(IEnumerable values, AuditEntryState state)
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

        private void DefinirValoresReferencia(IAuditEntry entry, AuditTrails trailEntry)
        {
            var references = _metadataAdapter.GetReferencesMetadata(entry.TypeMetadata);
            if (references == null) return;

            foreach (var metadata in references)
            {
                string referenceName = _metadataAdapter.GetPropertyName(metadata);
                if (string.IsNullOrEmpty(referenceName) || referenceName == AuditConstants.Unknown)
                    continue;

                var navEntry = entry.Navigation(referenceName);

                if (navEntry == null || !_metadataAdapter.IsNavigationModified(navEntry))
                    continue;

                trailEntry.AdicionarChangedColumns(referenceName);
            }
        }

        private void ProcessarMetadadosPosSave(AuditProcessingContextHelper ctx)
        {
            ctx.Step = AuditConstants.PosSaveMetadados;
            if (ctx.Helpers == null || ctx.EntriesMap == null) return;

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

        private void DefinirChavePrimaria(IAuditEntry entry, AuditTrails trail, AuditProcessingContextHelper ctx)
        {
            if (!string.IsNullOrEmpty(trail.PrimaryKey)) return;

            var entityType = entry.Entity.GetType();
            var entityName = _metadataAdapter.GetEntityName(entityType);

            ctx.Step = AuditConstants.PosSavePk + entityName;

            var idValue = ObterIdDaEntry(entry);

            if (idValue != AuditConstants.NotAvailable)
                trail.DefinirPrimaryKey(idValue);
        }

        private void DefinirIdColecoes(AuditTrails trail, AuditProcessingContextHelper ctx)
        {
            var map = ctx.EntriesMap;
            if (map == null || trail.NewValues.Count == 0) return;

            foreach (var entry in trail.NewValues)
            {
                if (!(entry.Value is IEnumerable<NavigationHelper> collection)) continue;

                foreach (var change in collection)
                {
                    ProcessarIdNavegacao(change, map);
                }
            }
        }

        private void ProcessarIdNavegacao(NavigationHelper change, Dictionary<object, IAuditEntry> map)
        {
            var instance = change.Instance;

            if (instance == null) return;

            if (map.TryGetValue(instance, out var itemEntry))
            {
                var id = ObterIdDaEntry(itemEntry);

                if (id != AuditConstants.NotAvailable)
                {
                    change.Id = id;
                }
            }

            change.Instance = null;
        }

        private string ObterIdDaEntry(IAuditEntry entry)
        {
            var typeMetadata = entry.TypeMetadata;
            if (typeMetadata == null) return AuditConstants.NotAvailable;

            var primaryKeyMetadata = _metadataAdapter.FindPrimaryKey(typeMetadata);
            if (primaryKeyMetadata == null) return AuditConstants.NotAvailable;

            var properties = _metadataAdapter.GetPropertiesFromMetadata(primaryKeyMetadata);
            if (properties == null) return AuditConstants.NotAvailable;

            string firstValue = null;
            StringBuilder compositeId = null;

            foreach (var prop in properties)
            {
                string propName = _metadataAdapter.GetPropertyName(prop);
                var property = entry.Property(propName);
                if (property == null) continue;

                object val = property.CurrentValue;
                string valStr = val?.ToString();

                if (string.IsNullOrEmpty(valStr)) continue;

                if (firstValue == null)
                {
                    firstValue = valStr;
                }
                else
                {
                    if (compositeId == null) compositeId = new StringBuilder(firstValue);
                    compositeId.Append("|").Append(valStr);
                }
            }

            if (compositeId != null) return compositeId.ToString();
            return firstValue ?? AuditConstants.NotAvailable;
        }

        private void DefinirRoots(IAuditEntry entry, AuditTrails trail, AuditProcessingContextHelper ctx)
        {
            var entityType = entry.TypeMetadata;
            if (entityType == null) return;

            var foreignKeys = _metadataAdapter.GetForeignKeys(entityType);
            if (foreignKeys == null) return;

            foreach (var fk in foreignKeys)
            {
                if (TentarProcessarRoot(entry, trail, ctx, fk))
                    break;
            }
        }

        private bool TentarProcessarRoot(IAuditEntry entry, AuditTrails trail, AuditProcessingContextHelper ctx, object fk)
        {
            var navigationName = _metadataAdapter.GetNavigationNameFromForeignKey(fk);
            if (string.IsNullOrWhiteSpace(navigationName)) return false;

            var navEntry = entry.Navigation(navigationName);
            var principalInstance = _metadataAdapter.GetCurrentValue(navEntry);

            if (principalInstance == null || !ctx.EntriesMap.TryGetValue(principalInstance, out var principalEntry))
                return false;

            return DefinirRootFinal(entry, trail, principalEntry, fk);
        }

        private bool DefinirRootFinal(IAuditEntry entry, AuditTrails trail, IAuditEntry principalEntry, object fk)
        {
            var fkName = _metadataAdapter.GetFirstFkPropertyName(fk);
            if (string.IsNullOrWhiteSpace(fkName)) return false;

            var rootId = entry.Property(fkName).CurrentValue?.ToString();
            if (string.IsNullOrWhiteSpace(rootId)) return false;

            var rootName = _metadataAdapter.GetCleanEntityName(principalEntry.TypeMetadata);

            trail.DefinirRoot(rootName, rootId);
            return true;
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
                    var nome = _metadataAdapter?.GetCleanEntityName(ctx.CurrentEntry.TypeMetadata)
                               ?? ctx.CurrentEntry.Entity.GetType().Name;

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
