# LogisticsHub — Enterprise E-Commerce & Logistics Engine 🚀

[![.NET 8](https://img.shields.io/badge/.NET-8.0-blueviolet.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean_Architecture-blue.svg)]()
[![Stripe Integration](https://img.shields.io/badge/Payments-Stripe_API-orange.svg)]()
[![Repository Pattern](https://img.shields.io/badge/Pattern-Repository_%26_UoW-green.svg)]()

LogisticsHub is an Enterprise-grade E-Commerce & Logistics Management Web API engineered to handle multi-vendor catalogs, complex order lifecycles, and resilient payment processing. Built on **.NET 8** following **Clean Architecture** and SOLID design principles, the platform acts as a centralized backend engine to ensure high system maintainability, database independence, and secure transactional integrity for high-volume trade applications.

---

## 🚀 Project Overview

### ❌ The Problem
E-commerce businesses and modern logistics systems face major scalability issues due to tightly-coupled architectures. Common failures include stock race conditions (overselling concurrent inventory), untracked order states, insecure payment flows, and messy database dependencies. When the business logic is tied to database engines or third-party gateways, scaling up or migrating infrastructure becomes highly expensive and risky.

### ✔️ The Solution
**LogisticsHub** solves these challenges by implementing a highly decoupled backend engine. By isolating the system’s core business rules inside a pure, independent Domain Layer, the application remains completely shielded from external database shifts or payment SDK updates. Real-time inventory tracking, unified transactional units, and robust external integrations are integrated seamlessly, providing a solid foundation for robust enterprise operations.

---

## ✨ System Features

* **Role-Specific API Management:** Granular controller designs separating operational capabilities into distinct functional endpoints:
  * **Admin Panel:** Global oversight of users, service providers, product listings, general configurations, and system-wide audit logs.
  * **Service Provider / Vendor Portal:** Comprehensive tools for inventory tracking, category assignments, and physical stock management.
  * **Customer Portal:** Dynamic product filtering, secure shopping cart interactions, checkout execution, and personalized order history.
* **Resilient Order Lifecycle Engine:** Complete state-machine implementation managing orders smoothly through their entire journey (Pending -> Paid -> Shipped -> Delivered) with strict validation checks at every stage.
* **Transactional Payment Integrity:** Deep integration with the **Stripe API** and **Stripe Webhooks** to safely process customer credit cards and automatically trigger post-payment workflows like stock deduction and receipt delivery.
* **Unified Response & Error Protocols:** Replaces standard exception throwing with the **Result Pattern** (`Result<T>`) for flow control, combined with a **Global Exception Middleware** to log raw diagnostics while serving clean, structured JSON errors to clients.
* **High-Speed Read Paths:** Integrated **In-Memory Caching** strategy for read-heavy, low-frequency mutation data (such as categories and dynamic store catalogs) to maintain sub-millisecond response times under peak load.
* **Enterprise Security & Session Management:** Stateless claims-based authentication utilizing cryptographically secure **JWT (Access/Refresh Tokens)** alongside ASP.NET Core Identity to guarantee strict separation of duties and secure user sessions.

---

## 🛠️ Tech Stack & Tooling

### Core Frameworks
* **Backend Runtime:** .NET 8 (ASP.NET Core Web API / C# 12)
* **Database & ORM:** Microsoft SQL Server | Entity Framework Core (Code-First approach)
* **API Standardization:** Swagger (OpenAPI) with fully customized JWT Authorization headers configuration

### Security & Architecture Components
* **Authentication:** ASP.NET Core Identity Framework
* **Token Protocol:** JSON Web Tokens (JWT) with secure Refresh Token Rotation mechanisms
* **Response Management:** Custom Result Pattern implementations representing functional success/failure states

### Utilities & Infrastructure Services
* **Payment Gateway:** Stripe.net SDK with Webhook handler integrations
* **Background Notifications:** MailKit for asynchronous transactional email routing and confirmations
* **Object Mapping:** AutoMapper for clean DTO-to-Domain conversions and decoupled contracts

---

## 🏗️ Backend Architecture

The backend enforces a strict **Separation of Concerns (SoC)** by strictly adhering to **Clean Architecture** patterns. This ensures the application core remains pure, highly testable, and isolated from UI frameworks or database changes.

### Architectural Layers Mapping

| Layer Name | Project Namespace | Primary Responsibilities & Components |
| :--- | :--- | :--- |
| **Presentation** | `LogisticsHub.Presentation` | Controllers, Custom Middlewares, API Route Configurations, Program.cs |
| **Infrastructure** | `LogisticsHub.Infrastructure` | DbContext, Migrations, Repositories, Stripe Client, MailKit SMTP Services |
| **Application** | `LogisticsHub.Application` | Use-case Services, DTOs, Mapping Profiles, Result Pattern Definitions, Validation Rules |
| **Domain** | `LogisticsHub.Domain` | Core Entities (User, Order, Product), Domain Enums, Value Objects |

### Key Design Patterns & Practices Implemented

1. **Result Pattern**: Eliminates throwing expensive .NET exceptions for validation or business failures. Functions return a structured `Result<T>` object containing the payload or readable error codes, resulting in clean, predictable flow control.
   
2. **Repository & Unit of Work Patterns**: Abstracts the persistence layer entirely. The Application Layer interacts only with interfaces, while the `UnitOfWork` ensures multi-entity modifications (like creating an order and reducing stock) are completed inside a single, secure database transaction.
   
3. **Dependency Inversion Principle (DIP)**: Core business rules inside `Domain` and `Application` have zero dependencies on external packages, SQL drivers, or Stripe SDKs. Outer layers depend entirely on the interfaces defined at the core.

---

## 🔒 Authentication & Security Architecture

* **Stateless Token Exchange:** Authentication is managed via cryptographic **JWT** tokens passed through HTTP Authorization headers.
* **Refresh Token Rotation:** Mitigates replay attacks. When a JWT expires, the frontend presents a single-use Refresh Token. The backend validates it, rotates the keys, and issues a new pair to keep sessions secure.
* **Role-Based Authorization:** Endpoints are guarded with granular attributes (e.g., `[Authorize(Roles = "Admin, Vendor")]`), shielding critical inventory configurations from baseline customer accounts.

---

## 🔌 API Architecture Overview

All endpoints follow predictable RESTful structures utilizing correct HTTP verb semantics and standardized JSON response envelopes.

| HTTP Method | Endpoint Path | Primary Purpose | Role Authorization |
| :--- | :--- | :--- | :--- |
| **POST** | `/api/Auth/register` | Registers new customer or service provider | Public |
| **POST** | `/api/Auth/login` | Validates credentials; returns JWT & Refresh Token | Public |
| **POST** | `/api/Auth/refresh` | Rotates expired JWT via active refresh token | Public |
| **GET** | `/api/Products` | Fetches a paginated, cached list of catalog items | Public |
| **POST** | `/api/Products` | Inserts new product entry into the system | Vendor / Admin |
| **POST** | `/api/Cart/items` | Adds designated product to customer's shopping cart | Customer |
| **POST** | `/api/Orders` | Processes order checkout and starts Stripe checkout | Customer |
| **POST** | `/api/Payments/webhook`| Verifies Stripe payment success state in real-time | Public / Stripe |

---

## 📂 Project Folder Structure

* **LogisticsHub.Domain/**: Core Business Models, Aggregate Roots, Custom Domain Exceptions, Enums
* **LogisticsHub.Application/**: Use-Case Services, DTOs, Repository Interfaces, AutoMapper Profiles
* **LogisticsHub.Infrastructure/**: EF Core DbContext, Migrations, Repository Implementations, Stripe & MailKit integrations
* **LogisticsHub.Presentation/**: API Controllers, Global Exception Handling Middleware, Swagger Configurations, AppSettings

---

## 💻 Local Execution & Setup

### Prerequisites
* .NET 8.0 SDK
* SQL Server (Express or LocalDB instance)

### Setup Steps

1. **Clone Project Core:**
   Run the following command in your terminal to clone the repository:
   `git clone https://github.com/shahdayman315315/LogisticsHub.git`

2. **Navigate to Project Directory:**
   `cd LogisticsHub`

3. **Configure Database Connection:**
   Navigate to `LogisticsHub.Presentation/appsettings.json` and adjust the `ConnectionStrings:DefaultConnection` and `JwtSettings:Secret` to match your local configurations.

4. **Execute Database Updates:**
   Run the EF Core command to construct your tables and execute database schemas:
   `dotnet ef database update --project LogisticsHub.Infrastructure --startup-project LogisticsHub.Presentation`

5. **Launch Backend Service:**
   `dotnet run --project LogisticsHub.Presentation`
   
*After running, open `https://localhost:7193/swagger` inside your web browser to explore and interact with the endpoints.*

---

## ☁️ Continuous Integration & Deployment (CI/CD)

* **Docker Ready:** Multistage deployment `Dockerfile` is hosted inside root directories.
* **Pipeline Automation:** Built to easily integrate with GitHub Actions workflows. Every Push or Pull Request targeting the `main` branch can initialize automatic compilation, static analysis, and unit-tests validation.

---

## 📈 Future System Roadmap

* **CQRS Separation via MediatR:** Transitioning the existing services to MediatR commands and queries to isolate Reads and Writes completely.
* **Real-Time Order Tracking:** Integrating **SignalR Hubs** to broadcast immediate order state changes and delivery progress updates directly to the client.
* **Distributed State Caching:** Moving from In-Memory cache to **Redis** for high-load horizontally scaled cloud servers.

---
*Developed by Shahd Ayman – Backend Software Engineer*
