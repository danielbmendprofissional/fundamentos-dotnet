# Clean Architecture Studies + CQRS + Mediator

![License](https://img.shields.io/badge/license-MIT-blue)
![Language](https://img.shields.io/github/languages/top/danibmend/CleanArchitecture)
![Stars](https://img.shields.io/github/stars/danibmend/CleanArchitecture?style=social)

## 📌 About

This repository is a **hands-on study of Clean Architecture** applied in **C# / .NET**, with a clear separation of responsibilities across **Domain**, **Application**, **Infrastructure**, and **Presentation** layers.  
It also explores **CQRS** and the **Mediator** pattern.

The goal here is **not** to ship a production-ready system, but to **solidify architectural concepts**, design decisions, and best practices used in real-world and scalable projects.

> ⚠️ **Important:** this is a **personal study project**.  
> The main focus is **not** the features themselves, but the **reading, understanding, and conscious application of the concepts**.

---

## 🚀 Motivation

- Gain a deeper understanding of **Clean Architecture (Robert C. Martin)**.
- Practice **architectural-level separation of concerns**.
- Apply principles like **SOLID**, **DIP**, **DDD**, and solid **layering practices**.
- Build a **personal reference** for future projects.
- Reduce coupling between business rules and technical details.

---

## 📂 Project Structure

The solution follows the classic **Clean Architecture** model, where dependencies always point **inward**.  
Also, check the **"Possibilities of Concepts"** file for more examples of what can live in each layer.

```text
/src
 ├── CleanArchitecture.Domain          # Domain layer (business rules)
 │   ├── Entities                      # Domain entities
 │   └── Interfaces                    # Domain contracts
 │
 ├── CleanArchitecture.Application     # Use cases and orchestration
 │   ├── Mapper                        # Debated, but commonly used in .NET contexts
 │   ├── Services                      # Service extensions (Application DI)
 │   ├── Shares                        # Behaviors (pipeline, cross-cutting concerns)
 │   ├── UseCases                      # Use cases (Commands / Queries)
 │   └── Validators                    # Input validation (commonly accepted in .NET)
 │
 ├── CleanArchitecture.Infrastructure  # Technical details
 │   ├── Context                       # DbContext / EF Core
 │   ├── Repositories                  # Repository implementations
 │   └── Services                      # External services (e.g., Email, Files)
 │
 ├── CleanArchitecture.WebAPI          # Presentation layer
 │   ├── Controllers                   # HTTP endpoints
 │   └── Program.cs                    # Application bootstrap
 │
 ├── CleanArchitecture.sln             # .NET solution
 ├── LICENSE                           # MIT License
 └── README.md                         # Main documentation


```

## 🚀 Running the Examples

Make sure you have the **.NET SDK** installed (recommended: **.NET 7+**).

1. Clone the repository:
   ```bash
   git clone https://github.com/danibmend/Solid.git
2. Navigate to the project root:
   ```bash
   cd Solid
3. Open the solution in your preferred IDE (Visual Studio, VS Code, Rider, etc.).
4. Run each example project (e.g., `Solid.SRP.Console`) to see the results for each principle.

## Requirements
- .NET 7+ SDK  
- A C#/.NET-capable editor or IDE  
- Basic familiarity with OOP, SOLID, and DDD  


## License
This project is licensed under the **MIT License** — see the `LICENSE` file for more details.


## Docker Image Build Commands

# Gerando a imagem a partir do arquivo otimizado
docker build -t minha-api:otimizada -f src/Presentation/CleanArchitecture.WebAPI/Dockerfile.otimizado .

# Gerando a imagem a partir do arquivo de dev
docker build -t minha-api:dev -f src/Presentation/CleanArchitecture.WebAPI/Dockerfile.dev .
