using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Proxy.Protection
{
    public class ProntuarioRepository
    {
        private List<Prontuario> _banco = new() {
        new Prontuario { Id = 55, Paciente = "Paciente A", MedicoId = 1 }, // Dr. João
        new Prontuario { Id = 99, Paciente = "Paciente B", MedicoId = 2 }  // Dr. Rodrigo
    };

        public Prontuario? ObterPorId(int id) => _banco.FirstOrDefault(p => p.Id == id);

        public void Remover(int id)
        {
            _banco.RemoveAll(p => p.Id == id);
            Console.WriteLine($"[Banco] Prontuário {id} removido fisicamente.");
        }
    }
}
