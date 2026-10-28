using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Proxy.Protection.Good
{
    public class ProntuarioProxy : IProntuarioService
    {
        private readonly IProntuarioService _serviceReal;
        private readonly ProntuarioRepository _repo;
        private readonly int _medicoLogadoId;

        public ProntuarioProxy(IProntuarioService serviceReal, ProntuarioRepository repo, int medicoId)
        {
            _serviceReal = serviceReal;
            _repo = repo;
            _medicoLogadoId = medicoId;
        }

        public void DeletarProntuario(int id)
        {
            // Proxy calls repository directly for validation, not the real service, otherwise we would need 
            // to validate the new service method too.
            var prontuario = _repo.ObterPorId(id);

            if (prontuario == null)
            {
                Console.WriteLine($"[Proxy] Erro: Prontuário {id} não encontrado.");
                return;
            }

            // Validação de Posse (Dono do Recurso)
            if (prontuario.MedicoId != _medicoLogadoId)
            {
                Console.WriteLine($"[BLOQUEADO] Dr. {_medicoLogadoId} tentou deletar prontuário do Dr. {prontuario.MedicoId}.");
                return;
            }

            Console.WriteLine($"[Proxy] Acesso autorizado para o Dr. {_medicoLogadoId}.");
            _serviceReal.DeletarProntuario(id);
        }
    }
}
