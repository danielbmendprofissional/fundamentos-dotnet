using DesignPatterns.AbstractFactory.Solution.Microsoft;
using DesignPatterns.AbstractFactory.Solution.Microsoft.AbstractFactory;
using DesignPatterns.AbstractFactory.Solution.Microsoft.AbstractFactory.Factory.Documento;
using DesignPatterns.AbstractFactory.Solution.Microsoft.AbstractFactory.Factory.Documento.Brasil;
using DesignPatterns.AbstractFactory.Solution.Microsoft.AbstractFactory.Factory.Documento.USA;
using DesignPatterns.AbstractFactory.Solution.Microsoft.AbstractFactory.Factory.Pagamento;
using DesignPatterns.AbstractFactory.Solution.Microsoft.AbstractFactory.Factory.Pagamento.Brasil;
using DesignPatterns.AbstractFactory.Solution.Microsoft.AbstractFactory.Factory.Pagamento.USA;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Dictionary = DesignPatterns.AbstractFactory.Solution.Dictionary.AbstractFactory;

var builder = Host.CreateApplicationBuilder(args);

//CLIENTE NAO SABE DE NADA DISSO, AQUI É SOMENTE UM EXEMPLO ESSES REGISTROS FICAM TUDO NO REGITRO DE DI,
// SERVICE EXTENSIONS OU PROGRAM.CS

// --- REGISTROS BRASIL ---
// Substitui o Dictionary: Cada chave mapeia para uma classe
// Chave composta: (Pais, Tipo)
builder.Services.AddKeyedTransient<IPagamento, Pix>((Pais.Brasil, TipoPagamento.Pix));
builder.Services.AddKeyedTransient<IPagamento, Boleto>((Pais.Brasil, TipoPagamento.Debito));
builder.Services.AddKeyedTransient<IPagamento, Boleto>((Pais.Brasil, TipoPagamento.Credito));
builder.Services.AddKeyedTransient<IPagamento, Boleto>((Pais.Brasil, TipoPagamento.Boleto));
builder.Services.AddKeyedTransient<IDocumento, NFe>(TipoDocumento.NFe);
// A Factory do Brasil também é registrada como um serviço
builder.Services.AddKeyedTransient<IInternacionalCheckout, BrasilCheckout>(Pais.Brasil);

// --- REGISTROS USA ---
builder.Services.AddKeyedTransient<IPagamento, Paypal>((Pais.Usa, TipoPagamento.PayPal));
builder.Services.AddKeyedTransient<IDocumento, UsaInvoice>(TipoDocumento.UsaInvoice);
builder.Services.AddKeyedTransient<IInternacionalCheckout, UsaCheckout>(Pais.Usa);

var host = builder.Build();


var checkout = host.Services.GetRequiredKeyedService<IInternacionalCheckout>(Pais.Brasil);

var pag = checkout.CriarPagamento("pix");
var doc = checkout.CriarDocumento();

pag.Processar(150.00m);
doc.Gerar(150.00m);





// ========================================================
//Dictionary

Dictionary.IInternacionalCheckout checkout = new Dictionary.BrasilCheckout();

var pix = checkout.CriarPagamento("pix");
var documento = checkout.CriarDocumento();

pix.Processar(100);
documento.Gerar(100);