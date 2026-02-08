using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Memento.Bad
{
    // O "Caretaker" ruim que conhece as entranhas do objeto.
    public class ServicoHistoricoBad
    {
        // O histórico guarda cópias manuais das propriedades.
        // Se eu adicionar "FotoUrl" no Perfil, tenho que mudar este dicionário/lista.
        private readonly List<dynamic> _snapshots = new();

        public void SalvarEstado(PerfilUsuarioBad perfil)
        {
            // VIOLAÇÃO OCP: O histórico está acoplado às propriedades do Perfil.
            _snapshots.Add(new
            {
                perfil.Nome,
                perfil.Bio,
                perfil.Idade,
                perfil.TemaHex
            });
        }

        public void Desfazer(PerfilUsuarioBad perfil)
        {
            var ultimo = _snapshots.LastOrDefault();
            if (ultimo == null) return;

            // VIOLAÇÃO SRP: O serviço "seta" o estado do objeto na mão.
            perfil.Nome = ultimo.Nome;
            perfil.Bio = ultimo.Bio;
            perfil.Idade = ultimo.Idade;
            perfil.TemaHex = ultimo.TemaHex;
        }
    }
}
