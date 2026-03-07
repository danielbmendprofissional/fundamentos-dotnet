using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Valuto.AuditTrail.Abstractions.Model.Enums;
using Valuto.AuditTrail.EFCore;
using Valuto.AuditTrail.Test8;


var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var connectionString = "Data Source=users.db";

        services.AddDbContext<TestDbContext>(options =>
        {
            options.UseSqlite(connectionString);
        });

        services.AddAuditTrail<TestDbContext>(options =>
        {
            options.ConfigureEnviroment(Enviroments.Homologacao);
        });
    });
    using var host = builder.Build();
    await host.StartAsync();
{
    using var scope = host.Services.CreateScope();

    var db = scope.ServiceProvider.GetRequiredService<TestDbContext>();

    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();
}

{
    using var scope = host.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<TestDbContext>();


    var solicitacao = new SOLICITACAO
    {
        COD_SOLICITACAO = 1,
        NUM_SOLICITACAO = 2026001,
        NUM_PROTO_EXTERNO = "EXT-12345",
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

    for (int a = 1; a <= 2; a++)
    {
        solicitacao.SOLICITACAO_ANEXO.Add(new SOLICITACAO_ANEXO
        {
            COD_ANEXO = a,
            COD_SOLICITACAO = solicitacao.COD_SOLICITACAO,
            NOM_ARQUIVO = $"documento_anexo_{a}.pdf",
            DSC_EXTENSAO = ".pdf"
        });
    }

    for (int h = 1; h <= 2; h++)
    {
        solicitacao.SOLICITACAO_HISTORICO_STATUS.Add(new SOLICITACAO_HISTORICO_STATUS
        {
            COD_HISTORICO = h,
            COD_SOLICITACAO = solicitacao.COD_SOLICITACAO,
            DAT_ALTERACAO = DateTime.Now.AddHours(-h),
            COD_IND_SITUACAO = h
        });
    }

    for (int i = 1; i <= 2; i++)
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

        for (int c = 1; c <= 2; c++)
        {
            solicitacaoTramite.COMENTARIOS.Add(new SOLICITACAO_TRAMITE_COMENTARIO
            {
                COD_COMENTARIO = (i * 100) + c,
                COD_SOLICIT_TRAMITE = i,
                TXT_COMENTARIO = $"Comentário {c} do trâmite {i}"
            });
        }

        for (int l = 1; l <= 2; l++)
        {
            solicitacaoTramite.LOGS.Add(new SOLICITACAO_TRAMITE_LOG
            {
                COD_LOG_TRAMITE = (i * 1000) + l,
                COD_SOLICIT_TRAMITE = i,
                DSC_LOG = $"Log técnico {l} do trâmite {i} - Processado com sucesso."
            });
        }

        solicitacao.SOLICITACAO_TRAMITE.Add(solicitacaoTramite);
    }

    db.Solicitacao.Add(solicitacao);
    db.SaveChanges();

    var soliticacoes = await db.Solicitacao.ToListAsync();
    foreach (var s in soliticacoes)
    {
        Console.WriteLine($"{s.COD_SOLICITACAO} ({s.NUM_SOLICITACAO})");
    }

    await host.StopAsync();

    Console.WriteLine("Agora sim, o sistema esperou o envio antes de fechar.");
}