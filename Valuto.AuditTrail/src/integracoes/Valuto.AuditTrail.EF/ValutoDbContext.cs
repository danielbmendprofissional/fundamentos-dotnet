using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Valuto.AuditTrail.Abstractions.Commom;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;
using Valuto.AuditTrail.Abstractions.Model.Helpers;
using Valuto.AuditTrail.Abstractions.Model.Snapshots;

namespace Valuto.AuditTrail.EF
{
    /// <summary>
    /// Classe base para o contexto do banco de dados que integra automaticamente o sistema de auditoria.
    /// </summary>
    /// <typeparam name="TContext">O tipo do DbContext derivado.</typeparam>
    public abstract class ValutoDbContext<TContext> : DbContext
            where TContext : DbContext
    {
        private bool _isSaving;
        /// <summary>
        /// Serviço de auditoria que será iniciado no DI
        /// </summary>
        public IAuditoriaInterna AuditoriaInterna { get; set; }
        /// <summary>
        /// Inicializa uma nova instância do contexto de auditoria.
        /// </summary>
        /// <param name="nameOrConnectionString">As opções de configuração do DbContext.</param>
        protected ValutoDbContext(string nameOrConnectionString) : base(nameOrConnectionString) { }
        /// <summary>
        /// Método estendido disparado imediatamente antes do processamento da auditoria e persistência síncrona.
        /// Para o client colocar suas configurações especificas de saveChanges
        /// </summary>
        protected virtual void OnBeforeSaveChanges() { }
        /// <summary>
        /// Método estendido disparado imediatamente antes do processamento da auditoria e persistência assíncrona.
        /// Para o client colocar suas configurações especificas de saveChanges
        /// </summary>
        protected virtual Task OnBeforeSaveChangesAsync() => Task.FromResult(0);
        /// <summary>
        /// Sobrescrita selada para garantir que o fluxo de 
        /// Iniciar, Persistir e Finalizar auditoria seja respeitado.
        /// </summary>
        public sealed override int SaveChanges()
        {
            if (_isSaving) throw new InvalidOperationException(AuditConstants.RecursionError);

            _isSaving = true;
            var tracker = new AuditTracker(this);

            try
            {
                OnBeforeSaveChanges();

                var snapshots = AuditoriaInterna.GerarSnapshot(tracker);
                var result = base.SaveChanges();

                if (snapshots.Count > 0)
                {
                    MaterializarSnapshots(snapshots);

                    _ = Task.Run(() =>
                    {
                        SafeAction(() =>
                            AuditoriaInterna.ProcessarAuditoriaAsync(snapshots));

                    }, CancellationToken.None);
                }

                return result;
            }
            finally { _isSaving = false; }
        }
        /// <summary>
        /// Sobrescrita selada para garantir que o fluxo de 
        /// Iniciar, Persistir e Finalizar auditoria seja respeitado.
        /// </summary>
        public sealed override Task<int> SaveChangesAsync()
            => SaveChangesAsync(CancellationToken.None);
        /// <summary>
        /// Sobrescrita selada para garantir que o fluxo de 
        /// Iniciar, Persistir e Finalizar auditoria seja respeitado.
        /// </summary>
        public sealed override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            if (_isSaving) throw new InvalidOperationException(AuditConstants.RecursionError);

            _isSaving = true;
            var tracker = new AuditTracker(this);

            try
            {
                await OnBeforeSaveChangesAsync();

                var snapshots = AuditoriaInterna.GerarSnapshot(tracker);
                var result = await base.SaveChangesAsync(cancellationToken);

                if (snapshots.Count > 0)
                {
                    MaterializarSnapshots(snapshots);

                    _ = Task.Run(async () =>
                    {
                        await SafeActionAsync(async () =>
                        {
                            await AuditoriaInterna.ProcessarAuditoriaAsync(snapshots, cancellationToken);
                        });
                    });
                }

                return result;
            }
            finally { _isSaving = false; }
        }

        private void MaterializarSnapshots(List<IAuditEntry> snapshots)
        {
            snapshots.SelectMany(s => s.Properties)
                     .Cast<AuditPropertySnapshot>()
                     .Where(p => p.IsTemporary)
                     .ToList()
                     .ForEach(p => {
                         var id = p.CurrentValue;
                         p.IsTemporary = false;
                         p.CurrentValue = id;
                     });
        }

        private void SafeAction(Action action) 
        { 
            try 
            { 
                action(); 
            } 
            catch { } 
        }
        private async Task SafeActionAsync(Func<Task> action)
        {
            try
            {
               await action();
            }
            catch { }
        }
    }
}
