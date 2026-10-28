using DesignPatterns.Adapter._1;
using DesignPatterns.Adapter._1.Interface;
using DesignPatterns.Adapter._2;
using DesignPatterns.Adapter._2.Interface;
using DesignPatterns.Adapter._2.Model;

ILumiaXMLTarget lumiaXMLTarget = new LumiaXMLAdapter();
var xmlLumiaMobile = lumiaXMLTarget.GetLumiaMobilesXMLSpecifications();
Console.WriteLine(xmlLumiaMobile.InnerText);


// =======================================================

IEnderecoProvider enderecoProvider = new EnderecoProvider();
Endereco endereco = await enderecoProvider.Buscar("01001000");
Console.WriteLine($"CEP: {endereco.Cep}, Rua: {endereco.Rua}");