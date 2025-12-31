# SOLID Studies

![License](https://img.shields.io/badge/license-MIT-blue)
![Language](https://img.shields.io/github/languages/top/danibmend/Solid)
![Stars](https://img.shields.io/github/stars/danibmend/Solid?style=social)

## 📌 Sobre

Este projeto é um **estudo prático dos princípios SOLID** aplicados em **C# / .NET**.  
Ele contém exemplos de como aplicar cada um dos cinco princípios de design de software orientado a objetos — **Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation e Dependency Inversion**.
Lembrando que é um projeto para estudo PESSOAL, talvez não esteja tão didático assim...
Mais importante do projeto é ler os CONCEITOS.

---

## 🚀 Motivação

- Compreender profundamente os **princípios SOLID**.
- Ver exemplos reais de código que demonstram boas práticas.
- Aplicar conceitos de **Clean Code**, **Design Patterns** e **Arquitetura Limpa** em exemplos simples.
- Criar um material de referência pessoal para estudo e aprimoramento.

---

## 📂 Estrutura do Projeto

O repositório está organizado para facilitar o estudo de cada princípio:

```text

/src
 ├── Solid.SRP/        # Single Responsibility Principle
 ├── Solid.OCP/        # Open/Closed Principle
 ├── Solid.LSP/        # Liskov Substitution Principle
 ├── Solid.ISP/        # Interface Segregation Principle
 ├── Solid.DIP/        # Dependency Inversion Principle
 ├── *.Console/        # Apps para executar os exemplos
 ├── Solid.sln         # Solution .NET
 ├── LICENSE           # MIT License
 └── README.md         # Documentação principal
```


---

## 📖 Conceitos Abordados

Este projeto cobre os **5 princípios SOLID**:

| Princípio | Significado |
|-----------|-------------|
| **S** – Single Responsibility Principle (SRP) | Cada classe/módulo tem uma única responsabilidade. |
| **O** – Open/Closed Principle (OCP) | Entidades devem estar abertas à extensão, mas fechadas à modificação. |
| **L** – Liskov Substitution Principle (LSP) | Subtipos podem substituir seus tipos base sem quebrar o programa. |
| **I** – Interface Segregation Principle (ISP) | Interfaces específicas são melhores que interfaces “gordas”. |
| **D** – Dependency Inversion Principle (DIP) | Dependa de abstrações, não de implementações. |

> Estes princípios foram definidos por Robert C. Martin e são essenciais para escrever código **limpo, sustentável e testável**.

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
  Familiaridade básica com POO

Licença
  Este projeto está licenciado sob a MIT License — veja o arquivo LICENSE para mais detalhes.
