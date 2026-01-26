using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Proxy.Protection.Bad
{
    //Internal because cliente should not access it directly
    internal sealed class ProntuarioServiceReal : IProntuarioService
    {
        private readonly ProntuarioRepository _repo;

        public ProntuarioServiceReal(ProntuarioRepository repo)
        {
            _repo = repo;
        }

        public void DeletarProntuario(int id)
        {
            // Disprotected access to the repository
            _repo.Remover(id);
        }
    }
}
