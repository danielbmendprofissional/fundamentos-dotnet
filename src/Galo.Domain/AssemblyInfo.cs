using System.Runtime.CompilerServices;

// Guard e os construtores internos das entidades filhas ficam visíveis só para
// quem precisa (mapeamento EF, laboratório e testes) - encapsulamento do domínio.
[assembly: InternalsVisibleTo("Galo.Infrastructure")]
[assembly: InternalsVisibleTo("Galo.Estudos")]
[assembly: InternalsVisibleTo("Galo.Domain.Tests")]
