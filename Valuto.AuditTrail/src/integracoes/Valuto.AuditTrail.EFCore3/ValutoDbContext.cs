using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Valuto.AuditTrail.Abstractions.Commom;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria;
using Valuto.AuditTrail.Abstractions.Model.Helpers;
using Valuto.AuditTrail.EFCore3.Adapters;

namespace Valuto.AuditTrail.EFCore3
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
        /// <param name="options">As opções de configuração do DbContext.</param>
        protected ValutoDbContext(DbContextOptions<TContext> options) : base(options) { }
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
            => SaveChanges(acceptAllChangesOnSuccess: true);
        /// <summary>
        /// Sobrescrita selada para garantir que o fluxo de 
        /// Iniciar, Persistir e Finalizar auditoria seja respeitado.
        /// </summary>
        public sealed override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => SaveChangesAsync(acceptAllChangesOnSuccess: true, cancellationToken);
        /// <summary>
        /// Sobrescrita selada para garantir que o fluxo de 
        /// Iniciar, Persistir e Finalizar auditoria seja respeitado.
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">Padrão do EF Core</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Caso o client chame o base.saveChanges de maneira equivocada</exception>
        public sealed override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            if (_isSaving) throw new InvalidOperationException(AuditConstants.RecursionError);

            _isSaving = true;
            var ctx = new AuditProcessingContextHelper();
            var tracker = new AuditTracker(this.ChangeTracker);

            try
            {
                OnBeforeSaveChanges();

                SafeAction(() 
                    => AuditoriaInterna?.IniciarAuditoria(ctx, tracker));

                var result = base.SaveChanges(acceptAllChangesOnSuccess);

                SafeAction(() => {
                    _ = AuditoriaInterna?.FinalizarAuditoriaAsync(ctx, CancellationToken.None);
                });

                return result;
            }
            finally { _isSaving = false; }
        }
        /// <summary>
        /// Sobrescrita selada para garantir que o fluxo de 
        /// Iniciar, Persistir e Finalizar auditoria seja respeitado.
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">Padrão do EF Core</param>
        /// <param name="ct">Padrão do EF Core</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Caso o client chame o base.saveChanges de maneira equivocada</exception>
        public sealed override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken ct = default)
        {
            if (_isSaving) throw new InvalidOperationException(AuditConstants.RecursionError);

            _isSaving = true;
            var ctx = new AuditProcessingContextHelper();
            var tracker = new AuditTracker(this.ChangeTracker);

            try
            {
                await OnBeforeSaveChangesAsync();

                SafeAction(()
                    => AuditoriaInterna?.IniciarAuditoria(ctx, tracker));

                var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, ct);

                await SafeActionAsync(async () 
                    => await AuditoriaInterna?.FinalizarAuditoriaAsync(ctx, ct));

                return result;
            }
            finally { _isSaving = false; }
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
