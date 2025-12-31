# SOLID Studies

![License](https://img.shields.io/badge/license-MIT-blue)
![Language](https://img.shields.io/github/languages/top/danibmend/Solid)
![Stars](https://img.shields.io/github/stars/danibmend/Solid?style=social)

## 📌 About

This repository is a **hands-on study of the SOLID principles** applied in **C# / .NET**.  
It includes practical examples showing how to apply each of the five object-oriented design principles — **Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, and Dependency Inversion**.

Note: this is a **personal learning project**, so it may not be perfectly “teaching-friendly” in every section. The main goal here is to focus on the **core concepts**.

---

## 🚀 Motivation

- Build a solid understanding of the **SOLID principles**.
- Explore real code examples that demonstrate good practices.
- Apply ideas from **Clean Code**, **Design Patterns**, and **Clean Architecture** through simple scenarios.
- Keep a personal reference to revisit and improve over time.

---

## 📂 Project Structure

The repository is organized to make it easy to study each principle:

```text
/src
 ├── Solid.SRP/        # Single Responsibility Principle
 ├── Solid.OCP/        # Open/Closed Principle
 ├── Solid.LSP/        # Liskov Substitution Principle
 ├── Solid.ISP/        # Interface Segregation Principle
 ├── Solid.DIP/        # Dependency Inversion Principle
 ├── *.Console/        # Console apps to run the examples
 ├── Solid.sln         # .NET solution
 ├── LICENSE           # MIT License
 └── README.md         # Main documentation

```


---

## 📖 Covered Concepts

This project focuses on the **five SOLID principles**:

| Principle | Meaning |
|----------|---------|
| **S** – Single Responsibility Principle (SRP) | Each class/module should have a single responsibility (one reason to change). |
| **O** – Open/Closed Principle (OCP) | Entities should be open for extension, but closed for modification. |
| **L** – Liskov Substitution Principle (LSP) | Subtypes should be able to replace their base types without breaking expected behavior. |
| **I** – Interface Segregation Principle (ISP) | Small, specific interfaces are better than large “fat” ones. |
| **D** – Dependency Inversion Principle (DIP) | Depend on abstractions, not on concrete implementations. |

> These principles were popularized by Robert C. Martin and are essential for writing **clean, maintainable, and testable** code.

## 🚀 Running the Examples

Make sure you have the **.NET SDK** installed (recommended: **.NET 7+**).

1. Clone the repository:
   ```bash
   git clone https://github.com/danibmend/Solid.git
2. Navigate to the project root:
   ```bash
   cd Solid
3. Open the solution in your preferred IDE (Visual Studio, VS Code, Rider, etc.).
4. Run each example project (e.g., `Solid.SRP.Console`) to see the output for each principle.

## Requirements
- .NET 7+ SDK  
- A C#/.NET-capable editor or IDE  
- Basic knowledge of OOP  

## License
This project is licensed under the **MIT License** — see the `LICENSE` file for more details.

