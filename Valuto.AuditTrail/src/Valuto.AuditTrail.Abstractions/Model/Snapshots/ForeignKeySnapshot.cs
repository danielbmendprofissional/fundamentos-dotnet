using System;
using System.Collections.Generic;
using System.Text;
using Valuto.AuditTrail.Abstractions.Contracts.Auditoria.Adapters;

namespace Valuto.AuditTrail.Abstractions.Model.Snapshots
{
    public sealed class ForeignKeySnapshot
    {
        /// <summary>
        /// Obtém o nome da restrição de chave estrangeira ou da propriedade de sombra.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Obtém o nome da propriedade de navegação associada a esta chave.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Indica se o valor é temporário (ex: FK vinculada a uma PK ainda não persistida).
        /// </summary>
        public bool IsTemporary { get; set; }

        /// <summary>
        /// Obtém ou define o valor da chave estrangeira.
        /// </summary>
        public object? CurrentValue { get; set; }

        /// <summary>
        /// Construtor privado para garantir a integridade do estado inicial através do Factory Method.
        /// </summary>
        private ForeignKeySnapshot(
            string name,
            string propertyName,
            object? currentValue,
            bool isTemporary)
        {
            Name = name;
            PropertyName = propertyName;
            CurrentValue = currentValue;
            IsTemporary = isTemporary;
        }

        /// <summary>
        /// Cria uma nova instância de <see cref="ForeignKeySnapshot"/> a partir de um adaptador de dados.
        /// </summary>
        /// <param name="adapter">O adaptador contendo os dados vivos do rastreador.</param>
        /// <returns>Uma instância de snapshot desacoplada do Entity Framework.</returns>
        public static ForeignKeySnapshot Create(IForeignKeyAdapter adapter)
        {
            // OBRIGATORIEDADE: Determinar estado temporário e valor inicial de forma atômica
            bool isTemp = adapter.IsTemporary;
            object? value = isTemp ? null : adapter.CurrentValue;

            return new ForeignKeySnapshot(
                adapter.Name,
                adapter.PropertyName,
                value,
                isTemp
            );
        }
    }
}
