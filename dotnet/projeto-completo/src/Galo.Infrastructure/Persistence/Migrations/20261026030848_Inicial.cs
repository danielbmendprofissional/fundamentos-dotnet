using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Galo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Campeonatos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Temporada = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campeonatos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clubes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Sigla = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Cidade = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Uf = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    AnoFundacao = table.Column<int>(type: "integer", nullable: false),
                    EhOGalo = table.Column<bool>(type: "boolean", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clubes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Estadios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Cidade = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Capacidade = table.Column<int>(type: "integer", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estadios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jogadores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Apelido = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    Camisa = table.Column<int>(type: "integer", nullable: false),
                    Posicao = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    DataNascimento = table.Column<DateOnly>(type: "date", nullable: false),
                    Nacionalidade = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    ValorMercado = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jogadores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Torcedores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Cpf = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Telefone = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    DataNascimento = table.Column<DateOnly>(type: "date", nullable: false),
                    Plano = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    EnderecoPrincipalId = table.Column<Guid>(type: "uuid", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Torcedores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Partidas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CampeonatoId = table.Column<Guid>(type: "uuid", nullable: false),
                    MandanteId = table.Column<Guid>(type: "uuid", nullable: false),
                    VisitanteId = table.Column<Guid>(type: "uuid", nullable: false),
                    EstadioId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataHoraUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Rodada = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    GolsMandante = table.Column<int>(type: "integer", nullable: false),
                    GolsVisitante = table.Column<int>(type: "integer", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partidas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Partidas_Campeonatos_CampeonatoId",
                        column: x => x.CampeonatoId,
                        principalTable: "Campeonatos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Partidas_Clubes_MandanteId",
                        column: x => x.MandanteId,
                        principalTable: "Clubes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Partidas_Clubes_VisitanteId",
                        column: x => x.VisitanteId,
                        principalTable: "Clubes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Partidas_Estadios_EstadioId",
                        column: x => x.EstadioId,
                        principalTable: "Estadios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Contratos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    JogadorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Inicio = table.Column<DateOnly>(type: "date", nullable: false),
                    Fim = table.Column<DateOnly>(type: "date", nullable: false),
                    SalarioMensal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ClausulaRescisoria = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contratos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contratos_Jogadores_JogadorId",
                        column: x => x.JogadorId,
                        principalTable: "Jogadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Enderecos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TorcedorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Cep = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Logradouro = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Numero = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Bairro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Cidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Estado = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Pais = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Complemento = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enderecos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Enderecos_Torcedores_TorcedorId",
                        column: x => x.TorcedorId,
                        principalTable: "Torcedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComprasIngresso",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TorcedorId = table.Column<Guid>(type: "uuid", nullable: false),
                    PartidaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Numero = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ValorTotal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ReservaConfirmada = table.Column<bool>(type: "boolean", nullable: false),
                    MotivoCancelamento = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComprasIngresso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComprasIngresso_Partidas_PartidaId",
                        column: x => x.PartidaId,
                        principalTable: "Partidas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComprasIngresso_Torcedores_TorcedorId",
                        column: x => x.TorcedorId,
                        principalTable: "Torcedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Gols",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PartidaId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClubeId = table.Column<Guid>(type: "uuid", nullable: false),
                    JogadorId = table.Column<Guid>(type: "uuid", nullable: true),
                    Minuto = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gols", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gols_Clubes_ClubeId",
                        column: x => x.ClubeId,
                        principalTable: "Clubes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Gols_Jogadores_JogadorId",
                        column: x => x.JogadorId,
                        principalTable: "Jogadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Gols_Partidas_PartidaId",
                        column: x => x.PartidaId,
                        principalTable: "Partidas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartidaSetores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PartidaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Capacidade = table.Column<int>(type: "integer", nullable: false),
                    Disponiveis = table.Column<int>(type: "integer", nullable: false),
                    Preco = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartidaSetores", x => x.Id);
                    table.CheckConstraint("CK_PartidaSetores_Disponiveis", "\"Disponiveis\" >= 0 AND \"Disponiveis\" <= \"Capacidade\"");
                    table.ForeignKey(
                        name: "FK_PartidaSetores_Partidas_PartidaId",
                        column: x => x.PartidaId,
                        principalTable: "Partidas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItensCompra",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompraId = table.Column<Guid>(type: "uuid", nullable: false),
                    Setor = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false),
                    PorcentagemDesconto = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensCompra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensCompra_ComprasIngresso_CompraId",
                        column: x => x.CompraId,
                        principalTable: "ComprasIngresso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PagamentosCompra",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompraId = table.Column<Guid>(type: "uuid", nullable: false),
                    Metodo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Valor = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DataPagamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CodigoTransacao = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagamentosCompra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PagamentosCompra_ComprasIngresso_CompraId",
                        column: x => x.CompraId,
                        principalTable: "ComprasIngresso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Campeonatos_Temporada",
                table: "Campeonatos",
                column: "Temporada");

            migrationBuilder.CreateIndex(
                name: "IX_Clubes_Sigla",
                table: "Clubes",
                column: "Sigla",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComprasIngresso_Numero",
                table: "ComprasIngresso",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComprasIngresso_PartidaId",
                table: "ComprasIngresso",
                column: "PartidaId");

            migrationBuilder.CreateIndex(
                name: "IX_ComprasIngresso_Status",
                table: "ComprasIngresso",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ComprasIngresso_TorcedorId",
                table: "ComprasIngresso",
                column: "TorcedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Contratos_JogadorId",
                table: "Contratos",
                column: "JogadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Enderecos_TorcedorId",
                table: "Enderecos",
                column: "TorcedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Gols_ClubeId",
                table: "Gols",
                column: "ClubeId");

            migrationBuilder.CreateIndex(
                name: "IX_Gols_JogadorId",
                table: "Gols",
                column: "JogadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Gols_PartidaId",
                table: "Gols",
                column: "PartidaId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensCompra_CompraId",
                table: "ItensCompra",
                column: "CompraId");

            migrationBuilder.CreateIndex(
                name: "IX_Jogadores_Nome",
                table: "Jogadores",
                column: "Nome");

            migrationBuilder.CreateIndex(
                name: "IX_Jogadores_Posicao_Status",
                table: "Jogadores",
                columns: new[] { "Posicao", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_PagamentosCompra_CompraId",
                table: "PagamentosCompra",
                column: "CompraId");

            migrationBuilder.CreateIndex(
                name: "IX_Partidas_CampeonatoId_Rodada",
                table: "Partidas",
                columns: new[] { "CampeonatoId", "Rodada" });

            migrationBuilder.CreateIndex(
                name: "IX_Partidas_DataHoraUtc",
                table: "Partidas",
                column: "DataHoraUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Partidas_EstadioId",
                table: "Partidas",
                column: "EstadioId");

            migrationBuilder.CreateIndex(
                name: "IX_Partidas_MandanteId",
                table: "Partidas",
                column: "MandanteId");

            migrationBuilder.CreateIndex(
                name: "IX_Partidas_Status",
                table: "Partidas",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Partidas_VisitanteId",
                table: "Partidas",
                column: "VisitanteId");

            migrationBuilder.CreateIndex(
                name: "IX_PartidaSetores_PartidaId_Nome",
                table: "PartidaSetores",
                columns: new[] { "PartidaId", "Nome" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Torcedores_Cpf",
                table: "Torcedores",
                column: "Cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Torcedores_Email",
                table: "Torcedores",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Torcedores_Plano",
                table: "Torcedores",
                column: "Plano");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contratos");

            migrationBuilder.DropTable(
                name: "Enderecos");

            migrationBuilder.DropTable(
                name: "Gols");

            migrationBuilder.DropTable(
                name: "ItensCompra");

            migrationBuilder.DropTable(
                name: "PagamentosCompra");

            migrationBuilder.DropTable(
                name: "PartidaSetores");

            migrationBuilder.DropTable(
                name: "Jogadores");

            migrationBuilder.DropTable(
                name: "ComprasIngresso");

            migrationBuilder.DropTable(
                name: "Partidas");

            migrationBuilder.DropTable(
                name: "Torcedores");

            migrationBuilder.DropTable(
                name: "Campeonatos");

            migrationBuilder.DropTable(
                name: "Clubes");

            migrationBuilder.DropTable(
                name: "Estadios");
        }
    }
}
