# 📁 Project Structure Overview


```
.
└── Kaizen.Server/
    ├── API/
    │   └── Controllers
    ├── Application/
    │   ├── Commands
    │   ├── Dtos
    │   ├── Interfaces
    │   ├── Queries
    │   └── Services
    └── Infrastructure/
        ├── Configuration
        ├── Helpers
        ├── Repositories
        └── Services
```

---

## 📦 API/

**Purpose:**  
Contains HTTP controllers that expose endpoints to external clients.

**Files:**  
- ASP.NET Core Controllers  
- Action methods for handling incoming HTTP requests  
- Request routing and basic validation logic

---

## 📦 Application/

**Purpose:**  
Houses the core application logic. This layer is independent of external concerns like databases or UI frameworks.

#### 📂 Commands/
- Contains CQRS Command classes (write operations)
- Each command typically has a corresponding `Handler`

#### 📂 Queries/
- Contains CQRS Query classes (read operations)
- Each query also has a corresponding `Handler`

#### 📂 Dtos/
- Data Transfer Objects used to communicate between layers
- Shape of the data exposed to the outside world

#### 📂 Interfaces/
- Abstractions for dependencies like repositories or services
- Enables dependency inversion and easier testing

#### 📂 Services/
- Application-level business logic (e.g., domain calculators)
- These are pure services that do not depend on infrastructure

---

## 📦 Infrastructure/

**Purpose:**  
Provides concrete implementations for interfaces defined in `Application`. This includes file I/O, database access, and other external dependencies.

#### 📂 Configuration/
- Infrastructure-level app configuration (e.g., EF Core setup, appsettings bindings)

#### 📂 Helpers/
- Utility classes specific to infrastructure needs (e.g., file readers, formatters)

#### 📂 Repositories/
- Implementations of data access interfaces
- Interact with databases or other persistence mechanisms

#### 📂 Services/
- Infrastructure-level services like file readers, email senders, etc.
- Implement `Application` interfaces using external resources

---

## ✅ Summary

- **Application** = Business logic (independent and testable)
- **Infrastructure** = Implementation details (file system, DB, etc.)
- **API** = Entry point (controllers and HTTP endpoints)
