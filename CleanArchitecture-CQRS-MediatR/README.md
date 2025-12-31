# Clean Architecture Studies + CQRS + MEDIATOR

![License](https://img.shields.io/badge/license-MIT-blue)
![Language](https://img.shields.io/github/languages/top/danibmend/CleanArchitecture)
![Stars](https://img.shields.io/github/stars/danibmend/CleanArchitecture?style=social)

## 📌 Sobre

Este projeto é um **estudo prático da Clean Architecture**, aplicada em **C# / .NET**, com separação clara de responsabilidades entre **Domain**, **Application**, **Infrastructure** e **Presentation**.
Usando também conceitos de CQRS e MEDIATOR

O objetivo **não é entregar um sistema pronto**, mas sim **consolidar conceitos arquiteturais**, decisões de design e boas práticas utilizadas em projetos reais e escaláveis.

> ⚠️ **Importante:** este é um projeto de **estudo pessoal**.  
> O foco principal **não é a funcionalidade em si**, mas **a leitura, entendimento e aplicação consciente dos conceitos**.

---

## 🚀 Motivação

- Entender profundamente a **Clean Architecture (Robert C. Martin)**.
- Praticar **separação de responsabilidades** em nível arquitetural.
- Aplicar princípios como **SOLID**, **DIP**, **DDD** e **boas práticas de camadas**.
- Criar um **material de referência pessoal** para projetos futuros.
- Evitar acoplamento entre regras de negócio e detalhes técnicos.

---

## 📂 Estrutura do Projeto

O projeto está organizado seguindo o modelo clássico da **Clean Architecture**, onde as dependências sempre apontam **para dentro**.
LEMBRANDO (Leia o arquivo Possibilidades de conceitos para ver mais sobre possíveis objetos na estrutura.

```text
/src
 ├── CleanArchitecture.Domain          # Camada de domínio (regras de negócio)
 │   ├── Entities                      # Entidades do domínio
 │   └── Interfaces                    # Contratos do domínio
 │
 ├── CleanArchitecture.Application     # Casos de uso e orquestração
 │   ├── Mapper                        # Existem discordancias, mas no contexto .NET é aceito
 │   ├── Services                      # Services Extensions (Application DI)
 │   ├── Shares                        # Behaviors
 │   ├── UseCases                      # Casos de uso (Commands / Queries)
 │   ├── Validators                    # Validações de entrada. Existem discordancias, mas no contexto .NET é aceito
 │
 ├── CleanArchitecture.Infrastructure  # Detalhes técnicos
 │   ├── Context                      # DbContext / EF Core
 │   ├── Repositories                 # Implementações de repositórios
 │   ├── Services                     # Serviços externos (ex: Email, Files)
 │
 ├── CleanArchitecture.WebAPI          # Camada de apresentação
 │   ├── Controllers                  # Endpoints HTTP
 │   └── Program.cs                   # Bootstrap da aplicação
 │
 ├── CleanArchitecture.sln             # Solution .NET
 ├── LICENSE                           # MIT License
 └── README.md                         # Documentação principal

```

## 🚀 Como Rodar os Exemplos

  Certifique-se de ter o **.NET SDK** instalado (recomendado .NET 7 ou superior).

  1. Clone o repositório:
     ```bash
     git clone https://github.com/danibmend/Solid.git
  2. Navegue até a pasta raiz:
    cd Solid
  4. Abra no IDE preferido (Visual Studio, VS Code, Rider etc.)
  5. Execute cada projeto exemplo (ex.: Solid.SRP.Console) para ver os resultados de cada princípio.

Requisitos
  .NET 7+ SDK
  Editor/IDE com suporte C# (.NET)
  Familiaridade com POO, SOLID e DDD (básico).

Licença
  Este projeto está licenciado sob a MIT License — veja o arquivo LICENSE para mais detalhes.
