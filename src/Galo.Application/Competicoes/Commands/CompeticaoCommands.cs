using Galo.Application.Common.Exceptions;
using Galo.Application.Common.Interfaces.Persistence;
using Galo.Domain.Competicoes.Entities;
using Galo.Domain.Competicoes.Enums;
using Galo.Domain.Competicoes.Interfaces;
using MediatR;

namespace Galo.Application.Competicoes.Commands
{
    // ---------------- Cadastros básicos ----------------

    public sealed record CadastrarClubeCommand(
        string Nome, string Sigla, string Cidade, string Uf, int AnoFundacao, bool EhOGalo = false) : IRequest<Guid>;

    public sealed class CadastrarClubeCommandHandler : IRequestHandler<CadastrarClubeCommand, Guid>
    {
        private readonly IClubeRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CadastrarClubeCommandHandler(IClubeRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CadastrarClubeCommand request, CancellationToken cancellationToken)
        {
            var clube = Clube.Criar(request.Nome, request.Sigla, request.Cidade, request.Uf, request.AnoFundacao, request.EhOGalo);
            await _repository.AdicionarAsync(clube, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return clube.Id;
        }
    }

    public sealed record CadastrarEstadioCommand(string Nome, string Cidade, int Capacidade) : IRequest<Guid>;

    public sealed class CadastrarEstadioCommandHandler : IRequestHandler<CadastrarEstadioCommand, Guid>
    {
        private readonly IEstadioRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CadastrarEstadioCommandHandler(IEstadioRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CadastrarEstadioCommand request, CancellationToken cancellationToken)
        {
            var estadio = Estadio.Criar(request.Nome, request.Cidade, request.Capacidade);
            await _repository.AdicionarAsync(estadio, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return estadio.Id;
        }
    }

    public sealed record CriarCampeonatoCommand(string Nome, int Temporada, TipoCampeonato Tipo) : IRequest<Guid>;

    public sealed class CriarCampeonatoCommandHandler : IRequestHandler<CriarCampeonatoCommand, Guid>
    {
        private readonly ICampeonatoRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CriarCampeonatoCommandHandler(ICampeonatoRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CriarCampeonatoCommand request, CancellationToken cancellationToken)
        {
            var campeonato = Campeonato.Criar(request.Nome, request.Temporada, request.Tipo);
            await _repository.AdicionarAsync(campeonato, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return campeonato.Id;
        }
    }

    // ---------------- Partida ----------------

    public sealed record AgendarPartidaCommand(
        Guid CampeonatoId, Guid MandanteId, Guid VisitanteId, Guid EstadioId, DateTime DataHoraUtc, int Rodada) : IRequest<Guid>;

    public sealed class AgendarPartidaCommandHandler : IRequestHandler<AgendarPartidaCommand, Guid>
    {
        private readonly IPartidaRepository _partidas;
        private readonly ICampeonatoRepository _campeonatos;
        private readonly IClubeRepository _clubes;
        private readonly IEstadioRepository _estadios;
        private readonly IUnitOfWork _unitOfWork;

        public AgendarPartidaCommandHandler(IPartidaRepository partidas, ICampeonatoRepository campeonatos,
            IClubeRepository clubes, IEstadioRepository estadios, IUnitOfWork unitOfWork)
        {
            _partidas = partidas;
            _campeonatos = campeonatos;
            _clubes = clubes;
            _estadios = estadios;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(AgendarPartidaCommand request, CancellationToken cancellationToken)
        {
            // Agregados diferentes se referenciam por Id: a EXISTÊNCIA do outro lado é verificada aqui (Application),
            // não por chave estrangeira navegável dentro do domínio.
            _ = await _campeonatos.ObterPorIdAsync(request.CampeonatoId, cancellationToken)
                ?? throw new NotFoundException("Campeonato", request.CampeonatoId);
            _ = await _clubes.ObterPorIdAsync(request.MandanteId, cancellationToken)
                ?? throw new NotFoundException("Clube mandante", request.MandanteId);
            _ = await _clubes.ObterPorIdAsync(request.VisitanteId, cancellationToken)
                ?? throw new NotFoundException("Clube visitante", request.VisitanteId);
            _ = await _estadios.ObterPorIdAsync(request.EstadioId, cancellationToken)
                ?? throw new NotFoundException("Estádio", request.EstadioId);

            var partida = Partida.Agendar(request.CampeonatoId, request.MandanteId, request.VisitanteId,
                request.EstadioId, request.DataHoraUtc, request.Rodada);

            await _partidas.AdicionarAsync(partida, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return partida.Id;
        }
    }

    public sealed record AdicionarSetorCommand(Guid PartidaId, string Nome, int Capacidade, decimal Preco) : IRequest<Guid>;

    public sealed class AdicionarSetorCommandHandler : IRequestHandler<AdicionarSetorCommand, Guid>
    {
        private readonly IPartidaRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public AdicionarSetorCommandHandler(IPartidaRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(AdicionarSetorCommand request, CancellationToken cancellationToken)
        {
            var partida = await _repository.ObterPorIdAsync(request.PartidaId, cancellationToken)
                          ?? throw new NotFoundException("Partida", request.PartidaId);

            var setorId = partida.AdicionarSetor(request.Nome, request.Capacidade, request.Preco);

            _repository.Atualizar(partida);
            await _unitOfWork.CommitAsync(cancellationToken);
            return setorId;
        }
    }

    public sealed record IniciarPartidaCommand(Guid PartidaId) : IRequest;

    public sealed class IniciarPartidaCommandHandler : IRequestHandler<IniciarPartidaCommand>
    {
        private readonly IPartidaRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public IniciarPartidaCommandHandler(IPartidaRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(IniciarPartidaCommand request, CancellationToken cancellationToken)
        {
            var partida = await _repository.ObterPorIdAsync(request.PartidaId, cancellationToken)
                          ?? throw new NotFoundException("Partida", request.PartidaId);
            partida.Iniciar();
            _repository.Atualizar(partida);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }

    public sealed record RegistrarGolCommand(
        Guid PartidaId, Guid ClubeBeneficiadoId, Guid? JogadorId, int Minuto, TipoGol Tipo = TipoGol.Normal) : IRequest;

    public sealed class RegistrarGolCommandHandler : IRequestHandler<RegistrarGolCommand>
    {
        private readonly IPartidaRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public RegistrarGolCommandHandler(IPartidaRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(RegistrarGolCommand request, CancellationToken cancellationToken)
        {
            var partida = await _repository.ObterPorIdAsync(request.PartidaId, cancellationToken)
                          ?? throw new NotFoundException("Partida", request.PartidaId);

            partida.RegistrarGol(request.ClubeBeneficiadoId, request.JogadorId, request.Minuto, request.Tipo);

            _repository.Atualizar(partida);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }

    public sealed record EncerrarPartidaCommand(Guid PartidaId) : IRequest;

    public sealed class EncerrarPartidaCommandHandler : IRequestHandler<EncerrarPartidaCommand>
    {
        private readonly IPartidaRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public EncerrarPartidaCommandHandler(IPartidaRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(EncerrarPartidaCommand request, CancellationToken cancellationToken)
        {
            var partida = await _repository.ObterPorIdAsync(request.PartidaId, cancellationToken)
                          ?? throw new NotFoundException("Partida", request.PartidaId);
            partida.Encerrar();
            _repository.Atualizar(partida);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }

    public sealed record CancelarPartidaCommand(Guid PartidaId, string Motivo) : IRequest;

    public sealed class CancelarPartidaCommandHandler : IRequestHandler<CancelarPartidaCommand>
    {
        private readonly IPartidaRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CancelarPartidaCommandHandler(IPartidaRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CancelarPartidaCommand request, CancellationToken cancellationToken)
        {
            var partida = await _repository.ObterPorIdAsync(request.PartidaId, cancellationToken)
                          ?? throw new NotFoundException("Partida", request.PartidaId);
            partida.Cancelar(request.Motivo);
            _repository.Atualizar(partida);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
