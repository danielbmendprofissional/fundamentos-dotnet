using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Valuto.AuditTrail.Abstractions.Model.Enums;
using Valuto.AuditTrail.Abstractions.Model.Events;
using Valuto.AuditTrail.EF;

namespace Valuto.AuditTrail.TestEF
{
    class Program
    {
        static void Main(string[] args)
        {
            DbConfiguration.SetConfiguration(new SQLiteDbConfig());

            RunAsync(args).GetAwaiter().GetResult();
        }

        static async Task RunAsync(string[] args)
        {
            DbConfiguration.SetConfiguration(new SQLiteDbConfig());

            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    var sQLiteConn = "SQLiteConn";
                    services.AddScoped<TestDbContext>(sp => new TestDbContext(sQLiteConn));
                    services.AddAuditTrail<TestDbContext>(options =>
                    {
                        options.ConfigureEnviroment(Enviroments.Homologacao);

                        options.ConfigureBuffer(b =>
                        {
                            b.MaxBufferSize = 50000;
                            b.MaxParallelism = 20;
                        });

                        options.ConfigurePublisher(p =>
                        {
                            p.RetryOnFailure = true;
                        });
                    });
                });

            using (var host = builder.Build())
            {
                await host.StartAsync();

                using (var scope = host.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<TestDbContext>();

                    if (!db.Database.Exists())
                    {
                        db.Database.Create();
                    }

                    db.Database.ExecuteSqlCommand(@"
    CREATE TABLE IF NOT EXISTS SOLICITACAO (
        COD_SOLICITACAO INTEGER PRIMARY KEY AUTOINCREMENT,
        NUM_SOLICITACAO INTEGER,
        NUM_PROTO_EXTERNO TEXT,
        DAT_CADASTRO DATETIME,
        IND_RESP_PARC_PEND TEXT,
        COD_IND_TIPO INTEGER,
        COD_IND_FORMA INTEGER,
        COD_IND_SATISFACAO INTEGER,
        DSC_SATISFACAO TEXT,
        DSC_SATISFACAO_HTML TEXT,
        NUM_PROTOCOLO TEXT,
        IND_FECHADO TEXT NOT NULL,
        NUM_DIAS_ATRASO INTEGER,
        NUM_DIAS_VENCER INTEGER,
        DAT_VENCIMENTO DATETIME,
        IND_VENCE_HOJE TEXT NOT NULL,
        COD_IND_CANAL INTEGER,
        COD_ULT_RESPONSAVEL INTEGER,
        NOM_ULT_RESPONSAVEL TEXT,
        COD_ULT_DEPTO_RESP INTEGER,
        NOM_ULT_DEPTO_RESP TEXT,
        DSC_TEMPO_FILA TEXT,
        NUM_TEMPO_FILA INTEGER,
        COD_ULT_SIT_TRAMITE INTEGER,
        DSC_ULT_SIT_TRAMITE TEXT,
        COD_DEPTO_ORIGEM INTEGER,
        NOM_DEPTO_ORIGEM TEXT,
        COD_USU_DEMANDANTE INTEGER,
        NOM_USU_DEMANDANTE TEXT,
        DSC_ABERTURA TEXT,
        DSC_ABERTURA_HTML TEXT,
        IND_ENVIO_MSG_WHATSAPP TEXT,
        COD_OCORRENCIA_LEGADO INTEGER,
        IND_EXIBE_RESPONSAVEL_SOLICIT TEXT
    );

    CREATE TABLE IF NOT EXISTS SOLICITACAO_TRAMITE (
        COD_SOLICIT_TRAMITE INTEGER PRIMARY KEY AUTOINCREMENT,
        COD_SOLICITACAO INTEGER NOT NULL,
        IND_RESOLVIDO TEXT NOT NULL,
        COD_IND_SITUACAO INTEGER NOT NULL,
        COD_IND_TRAMITACAO INTEGER NOT NULL,
        DSC_TRAMITE TEXT NOT NULL,
        COD_DOC_ANEXO INTEGER,
        COD_USU_SIGESP INTEGER,
        NOM_USU_SIGESP TEXT,
        COD_LOTACAO_SIGESP INTEGER,
        DSC_LOTACAO_SIGESP TEXT,
        COD_CIDADAO INTEGER,
        COD_ENC_SOLICIT_TRAMITE INTEGER,
        DAT_REAGENDAMENTO DATETIME,
        DAT_CADASTRO DATETIME NOT NULL,
        COD_USU_SGI INTEGER,
        NOM_USU_SGI TEXT,
        DSC_TRAMITE_HTML TEXT NOT NULL,
        COD_PESSOA_OUV INTEGER,
        COD_USU_INCLUSAO INTEGER,
        COD_IND_CANAL INTEGER,
        COD_EQUIPE INTEGER,
        DSC_CONCLUSAO_TECNICA TEXT,
        DSC_DEPTO_NOTIFICADOS TEXT,
        DSC_DIAS_TRAMITE TEXT,
        IND_NOTIF_WP TEXT,
        COD_REQUEST_WPP TEXT,

        COD_USU_ENCAMINHADO INTEGER,
        COD_DEPTO_ENCAMINHADO INTEGER,
        NOM_DESTINATARIO TEXT,

        FOREIGN KEY(COD_SOLICITACAO) REFERENCES SOLICITACAO(COD_SOLICITACAO)
    );

    CREATE TABLE IF NOT EXISTS SOLICITACAO_ANEXO (
        COD_ANEXO INTEGER PRIMARY KEY AUTOINCREMENT,
        COD_SOLICITACAO INTEGER NOT NULL,
        NOM_ARQUIVO TEXT NOT NULL,
        DSC_EXTENSAO TEXT NOT NULL,
        -- OBRIGATORIEDADE: FK para manter a integridade no teste
        FOREIGN KEY (COD_SOLICITACAO) REFERENCES SOLICITACAO (COD_SOLICITACAO) ON DELETE CASCADE
    );

    CREATE TABLE IF NOT EXISTS SOLICITACAO_HISTORICO_STATUS (
        COD_HISTORICO INTEGER PRIMARY KEY AUTOINCREMENT,
        COD_SOLICITACAO INTEGER NOT NULL,
        DAT_ALTERACAO DATETIME NOT NULL,
        COD_IND_SITUACAO INTEGER NOT NULL,
        -- OBRIGATORIEDADE: Integridade referencial para o histórico
        FOREIGN KEY (COD_SOLICITACAO) REFERENCES SOLICITACAO (COD_SOLICITACAO) ON DELETE CASCADE
    );

    CREATE TABLE IF NOT EXISTS SOLICITACAO_TRAMITE_COMENTARIO (
        COD_COMENTARIO INTEGER PRIMARY KEY AUTOINCREMENT,
        COD_SOLICIT_TRAMITE INTEGER NOT NULL,
        TXT_COMENTARIO TEXT NOT NULL,
        -- OBRIGATORIEDADE: Relacionamento com a tabela de trâmite
        FOREIGN KEY (COD_SOLICIT_TRAMITE) REFERENCES SOLICITACAO_TRAMITE (COD_SOLICIT_TRAMITE) ON DELETE CASCADE
    );

    CREATE TABLE IF NOT EXISTS SOLICITACAO_TRAMITE_LOG (
        COD_LOG_TRAMITE INTEGER PRIMARY KEY AUTOINCREMENT,
        COD_SOLICIT_TRAMITE INTEGER NOT NULL,
        DSC_LOG TEXT NOT NULL,
        -- OBRIGATORIEDADE: Integridade referencial conforme mapeamento OnDelete(DeleteBehavior.Cascade)
        FOREIGN KEY (COD_SOLICIT_TRAMITE) REFERENCES SOLICITACAO_TRAMITE (COD_SOLICIT_TRAMITE) ON DELETE CASCADE
    );

");

                    db.Solicitacao.Add(CreateSolicitacaoMock());
                    await db.SaveChangesAsync();
                }

                Console.WriteLine("Sucesso. Pressione qualquer tecla...");
                Console.ReadKey();
                await host.StopAsync();
            }
        }

        private static SOLICITACAO CreateSolicitacaoMock()
        {
            var solicitacao = new SOLICITACAO
            {
                COD_SOLICITACAO = 1,
                NUM_SOLICITACAO = 2026001,
                NUM_PROTO_EXTERNO = "EXT1",
                DAT_CADASTRO = DateTime.Now,
                IND_RESP_PARC_PEND = "N",
                COD_IND_TIPO = 10,
                COD_IND_FORMA = 2,
                COD_IND_SATISFACAO = null,
                DSC_SATISFACAO = "null",
                DSC_SATISFACAO_HTML = "null",
                NUM_PROTOCOLO = "PROTO-001",
                IND_FECHADO = "N",
                NUM_DIAS_ATRASO = 0,
                NUM_DIAS_VENCER = 5,
                DAT_VENCIMENTO = DateTime.Now.AddDays(5),
                IND_VENCE_HOJE = "N",
                COD_IND_CANAL = 3,
                COD_ULT_RESPONSAVEL = 101,
                NOM_ULT_RESPONSAVEL = "João Silva",
                COD_ULT_DEPTO_RESP = 20,
                NOM_ULT_DEPTO_RESP = "Atendimento",
                DSC_TEMPO_FILA = "10 minutos",
                NUM_TEMPO_FILA = 10,
                COD_ULT_SIT_TRAMITE = 1,
                DSC_ULT_SIT_TRAMITE = "Aberto",
                COD_DEPTO_ORIGEM = 5,
                NOM_DEPTO_ORIGEM = "Ouvidoria",
                COD_USU_DEMANDANTE = 5001,
                NOM_USU_DEMANDANTE = "Maria Oliveira",
                DSC_ABERTURA_HTML = "<p>Abertura da solicitação</p>",
                DSC_ABERTURA = "Abertura da solicitação",
                IND_ENVIO_MSG_WHATSAPP = "S",
                COD_OCORRENCIA_LEGADO = null,
                IND_EXIBE_RESPONSAVEL_SOLICIT = "S"
            };

            for (int i = 1; i <= 5; i++)
            {
                var solicitacaoTramite = new SOLICITACAO_TRAMITE
                {
                    COD_SOLICIT_TRAMITE = i,
                    COD_SOLICITACAO = solicitacao.COD_SOLICITACAO,
                    IND_RESOLVIDO = "N",
                    COD_IND_SITUACAO = i,
                    COD_IND_TRAMITACAO = i * 10,
                    DSC_TRAMITE = $"Trâmite número {i}",
                    COD_DOC_ANEXO = null,
                    COD_USU_SIGESP = 200 + i,
                    NOM_USU_SIGESP = $"Usuário SIGESP {i}",
                    COD_LOTACAO_SIGESP = 300 + i,
                    DSC_LOTACAO_SIGESP = $"Lotação {i}",
                    COD_CIDADAO = 1000 + i,
                    COD_ENC_SOLICIT_TRAMITE = null,
                    DAT_REAGENDAMENTO = null,
                    DAT_CADASTRO = DateTime.Now.AddMinutes(i),
                    COD_USU_SGI = 400 + i,
                    NOM_USU_SGI = $"Usuário SGI {i}",
                    DSC_TRAMITE_HTML = $"<p>Trâmite HTML {i}</p>",
                    COD_PESSOA_OUV = null,
                    COD_USU_INCLUSAO = 10 + i,
                    COD_IND_CANAL = 1,
                    COD_EQUIPE = 50 + i,
                    DSC_CONCLUSAO_TECNICA = $"Conclusão técnica {i}",
                    DSC_DEPTO_NOTIFICADOS = $"Deptos notificados {i}",
                    COD_USU_ENCAMINHADO = null,
                    COD_DEPTO_ENCAMINHADO = null,
                    NOM_DESTINATARIO = $"Destinatário {i}",
                    DSC_DIAS_TRAMITE = $"{i} dias",
                    IND_NOTIF_WP = "N",
                    COD_REQUEST_WPP = $"REQ-WPP-{i}",
                    SOLICITACAO = solicitacao
                };

                solicitacao.SOLICITACAO_TRAMITE.Add(solicitacaoTramite);
            }
            return solicitacao;
        }
    }
}
