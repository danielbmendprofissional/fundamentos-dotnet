using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
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
using Valuto.AuditTrail.Logic.Extensions;
using Valuto.AuditTrail.Logic.Paralel;

namespace Valuto.AuditTrail.Logic
{
    //[System.Diagnostics.DebuggerStepThrough]
    internal class AuditoriaInterna : IAuditoriaInterna
    {
        private readonly IUserProvider _userProvider;
        private readonly IMetadataAdapter _metadataAdapter;
        private readonly IAuditBuffer<AuditEntryCreatedEvent> _auditBuffer;
        private readonly IAuditBuffer<AuditLogEvent> _logBuffer;
        private readonly AuditProcessingContextHelper _contextHelper;

        public AuditoriaInterna(
            IUserProvider userProvider,
            IMetadataAdapter metadataAdapter,
            IAuditBuffer<AuditEntryCreatedEvent> auditBuffer,
            IAuditBuffer<AuditLogEvent> logBuffer)
        {
            _userProvider = userProvider;
            _metadataAdapter = metadataAdapter;
            _auditBuffer = auditBuffer;
            _logBuffer = logBuffer;
            _contextHelper = new AuditProcessingContextHelper();
        }

        public (List<EntitySnapshot> Snapshots, List<Action> Materializadores) GerarSnapshot(ITrackerAdapter tracker)
        {
            var materializadores = new List<Action>();

            try
            {
                var snapshots = tracker.Entries(_metadataAdapter)
                    .Where(e => ValidarEstadoAuditoria(e.State, e.Type))
                    .Select(e => MapToSnapshot(e, materializadores))
                    .ToList();

                return (snapshots, materializadores);
            }
            catch (Exception)
            {
                return (new List<EntitySnapshot>(), materializadores);
            }
        }

        public async Task ProcessarAuditoriaAsync(List<EntitySnapshot> snapshots, CancellationToken cancellationToken = default)
        {
            if (snapshots == null || snapshots.Count == 0) return;

            var usuario = _userProvider?.GetCurrentUser() ?? AuditConstants.NaoInformado;

            List<AuditEntry> auditEntries = new List<AuditEntry>(snapshots.Count);
            
            try
            {
                foreach (var snapshot in snapshots)
                {
                    _contextHelper.Start(snapshot);

                    var entry = CriarTrailEntity(snapshot, usuario);
                    auditEntries.Add(entry);
                }

                await EnviarParaFilaAsync(auditEntries, cancellationToken);
            }
            catch (Exception ex)
            {
                await EnviarLog(ex, _contextHelper);
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

        private EntitySnapshot MapToSnapshot(IEntityAdapter e, List<Action> materializadores)
        {
            var snapshot = EntitySnapshot.Create(e.Name, e.Type?.Namespace, e.State);

            snapshot.Properties.AddRange(e.Properties.Select(prop =>
                PropertySnapshot.Create(prop, e.State)));

            snapshot.ForeignKeys.AddRange(e.ForeignKeys.Select(fk =>
                ForeignKeySnapshot.Create(fk)));


            snapshot.NavEntries.AddRange(e.Navigations.Select(nav =>
                NavigationSnapshot.Create(nav)));

            snapshot.References.AddRange(e.References.Select(@ref =>
                ReferenceSnapshot.Create(@ref)));

            if (e.State == EntryState.Added)
            {
                materializadores.Add(() =>
                {
                    foreach (var prop in e.Properties.Where(p => p.IsPrimaryKey))
                    {
                        var snapshotProp = snapshot.Properties.FirstOrDefault(p => p.Name == prop.Name);
                        if (snapshotProp != null)
                        {
                            snapshotProp.UpdateValueAfterSave(prop.CurrentValue);
                        }
                    }
                });
            }

            return snapshot;
        }

        private AuditEntry CriarTrailEntity(EntitySnapshot entry, string usuario)
        {
            var entityName = entry.Name;
            var aplicacao = entry.Namespace;

            var auditEntity = AuditEntry.Criar(usuario, aplicacao, entityName);

            DefinirType(entry, auditEntity);
            DefinirPropriedades(entry, auditEntity);
            DefinirValoresNavegacao(entry, auditEntity);
            DefinirValoresReferencia(entry, auditEntity);
            DefinirChavePrimaria(entry, auditEntity);
            DefinirRoots(entry, auditEntity);

            return auditEntity;
        }

        private void DefinirType(EntitySnapshot entry, AuditEntry auditEntity)
        {
            _contextHelper.Track(AuditConstants.DefinirType);

            var trailType = entry.State.ObterTipoTrilha();
            auditEntity.DefinirTrailType(trailType);
        }

        private void DefinirPropriedades(EntitySnapshot entry, AuditEntry auditEntity)
        {
            _contextHelper.Track(AuditConstants.DefinirPropriedades);

            var properties = entry.Properties.Where(x => !x.IsTemporary);
            foreach (var property in properties)
            {
                DefinirValoresPropriedade(entry.State, auditEntity, property);
            }
        }

        private void DefinirValoresNavegacao(EntitySnapshot entry, AuditEntry auditEntity)
        {
            if (entry.NavEntries.Count == 0) return;

            _contextHelper.Track(AuditConstants.DefinirValoresNavegacao);

            foreach (var nav in entry.NavEntries)
            {
                var ids = nav.ObterIdsFormatados();

                var changes = ids.Select(id => new NavigationHelper
                {
                    Status = entry.State.ToString(),
                    Id = id
                }).ToList();

                if (changes.Count > 0)
                {
                    auditEntity.AdicionarNewValues(nav.Name, changes);
                    auditEntity.AdicionarChangedColumns(nav.Name);
                }
            }
        }

        private void DefinirValoresReferencia(EntitySnapshot entry, AuditEntry auditEntity)
        {
            var references = entry.References;
            if (references == null) return;

            _contextHelper.Track(AuditConstants.DefinirValoresReferencia);

            foreach (var reference in references)
            {
                if (!reference.IsModified)
                {
                    string referenceName = reference.Name;
                    auditEntity.AdicionarChangedColumns(referenceName);
                }
            }
        }

        private void DefinirRoots(EntitySnapshot snapshot, AuditEntry entry)
        {
            var foreignKeys = snapshot.ForeignKeys;
            if (foreignKeys == null) return;

            _contextHelper.Track(AuditConstants.DefinirRoots);

            foreach (var foreignKey in foreignKeys)
            {
                var id = foreignKey.CurrentValue?.ToString();
                if (string.IsNullOrWhiteSpace(id)) continue;

                var name = foreignKey.Name;
                if (string.IsNullOrWhiteSpace(name)) continue;

                entry.AdicionarBinding(
                    _contextHelper.TransactionId,
                    id,
                    entry.PrimaryKey, 
                    name, 
                    foreignKey.PropertyName);
            }
        }

        private void DefinirChavePrimaria(EntitySnapshot snapshot, AuditEntry entry)
        {
            if (!string.IsNullOrEmpty(entry.PrimaryKey)) return;

            _contextHelper.Track(AuditConstants.DefinirPrimaryKey);

            var primaryKeys = snapshot.Properties.Where(x => x.IsPrimaryKey);

            var primaryKey = primaryKeys.MontarChavePrimaria();

            entry.DefinirPrimaryKey(primaryKey);
        }

        private void DefinirValoresPropriedade(EntryState state, AuditEntry trailEntry, PropertySnapshot property)
        {
            string name = property.Name;

            switch (state)
            {
                case EntryState.Added:
                    trailEntry.AdicionarNewValues(name, property.CurrentValue);
                    break;
                case EntryState.Deleted:
                    trailEntry.AdicionarOldValues(name, property.OriginalValue);
                    break;
                case EntryState.Modified:
                    if (property.IsModified && !object.Equals(property.OriginalValue, property.CurrentValue))
                    {
                        trailEntry.AdicionarChangedColumns(name);
                        trailEntry.AdicionarOldValues(name, property.OriginalValue);
                        trailEntry.AdicionarNewValues(name, property.CurrentValue);
                    }
                    break;
            }
        }

        private async Task EnviarParaFilaAsync(IEnumerable<AuditEntry> entries, CancellationToken ct = default)
        {
            foreach (var e in entries)
            {
                var bindingEvents = e.AuditBindings.Select(x => new AuditBindingCreatedEvent(
                    x.Id,
                    e.Id, // AuditEntryId
                    e.TransactionId,
                    x.PrimaryKey,
                    x.ForeignKey,
                    x.Name,
                    x.PropertyName
                )).ToList();

                var @event = new AuditEntryCreatedEvent(
                    e.Id,
                    e.TransactionId,
                    e.Usuario,
                    e.EntityName,
                    e.SistemaName,
                    e.PrimaryKey,
                    e.OldValues.ToDictionary(kv => kv.Key, kv => kv.Value),
                    e.NewValues.ToDictionary(kv => kv.Key, kv => kv.Value),
                    e.ChangedColumns.ToList(),
                    bindingEvents,
                    e.Tipo
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

                    var pks = ctx.CurrentEntry.Properties
                        .Where(p => p.IsPrimaryKey)
                        .Select(p => p.CurrentValue?.ToString() ?? AuditConstants.Unknown);

                    var idValue = string.Join("|", pks);

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
