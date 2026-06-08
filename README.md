# Туристична агенція

Web-застосунок для туристичної агенції на платформі **.NET 8** з багатошаровою архітектурою.  
Курсова робота, **варіант 2** — предметна область «Туристична агенція».

## Можливості

- **Керування турами** — створення, редагування та деактивація (менеджер / адміністратор)
- **Каталог турів** — перегляд з пошуком і фільтрами (гарячі тури, тип, країна, ціна)
- **Бронювання турів** — для зареєстрованих користувачів
- **Бронювання квитків** — транспорт (літак, поїзд, автобус, корабель)
- **Бронювання номерів** — готелі з перевіркою доступності
- **Авторизація** — JWT, ролі: Guest, Registered, Manager, Administrator

## Архітектура

```
UI/   CourseWork.Api              → Presentation Layer (ASP.NET WebAPI + Bootstrap UI)
BLL/  CourseWork.Business         → Business Logic Layer (сервіси, DTO, винятки)
DAL/  CourseWork.Data             → Data Access Layer (EF Core, Repository, Unit of Work)
Tests/ CourseWork.Business.Tests  → модульні тести (xUnit, Moq)
```

Детальніший опис — у файлі [ARCHITECTURE.md](./ARCHITECTURE.md).

## Технології

| Категорія | Стек |
|-----------|------|
| Платформа | .NET 8, C# |
| API | ASP.NET Core WebAPI |
| БД | MS SQL Server (LocalDB) |
| ORM | Entity Framework Core 8 (Code First) |
| Мапінг | AutoMapper |
| Авторизація | JWT Bearer |
| UI | Bootstrap 5, JavaScript |
| Тести | xUnit, Moq, FluentAssertions |

## Вимоги

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server LocalDB (зазвичай входить до Visual Studio) або MS SQL Server
- Visual Studio 2022 / VS Code / Rider (опційно)

## Швидкий старт

```bash
git clone <repository-url>
cd CourseWork
dotnet restore
dotnet run --project UI/CourseWork.Api
```

Після запуску:

| Ресурс | URL |
|--------|-----|
| Web-інтерфейс | http://localhost:5134 |
| Swagger | http://localhost:5134/swagger |
| HTTPS | https://localhost:7046 |

Міграції БД та початкові дані застосовуються **автоматично** при першому запуску.

## Налаштування БД

Рядок підключення у `CourseWork.Api/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CourseWorkAgencyDb;Trusted_Connection=True;TrustServerCertificate=True"
}
```

Для повноцінного SQL Server змініть `Server=` на свій інстанс.

Ручне застосування міграцій:

```bash
dotnet ef database update --project DAL/CourseWork.Data --startup-project UI/CourseWork.Api
```

## Тестові облікові записи

| Email | Пароль | Роль |
|-------|--------|------|
| `admin@agency.ua` | `Admin123!` | Administrator |
| `manager@agency.ua` | `Manager123!` | Manager |
| `user@agency.ua` | `User123!` | Registered |

Незареєстрований користувач може переглядати тури без входу.

## API (основні endpoints)

| Метод | Endpoint | Доступ |
|-------|----------|--------|
| `GET` | `/api/tours` | Усі |
| `GET` | `/api/tours/{id}` | Усі |
| `POST` | `/api/tours` | Manager, Administrator |
| `PUT` | `/api/tours/{id}` | Manager, Administrator |
| `DELETE` | `/api/tours/{id}` | Manager, Administrator |
| `POST` | `/api/auth/register` | Усі |
| `POST` | `/api/auth/login` | Усі |
| `POST` | `/api/bookings/tours` | Авторизовані |
| `GET` | `/api/bookings/tours` | Авторизовані |
| `POST` | `/api/ticketbookings/transport` | Авторизовані |
| `POST` | `/api/ticketbookings/hotel` | Авторизовані |
| `GET` | `/api/reference/countries` | Усі |
| `GET` | `/api/reference/transports` | Усі |
| `GET` | `/api/reference/hotel-rooms` | Усі |

Для захищених запитів передайте заголовок:

```
Authorization: Bearer <token>
```

## Запуск тестів

```bash
dotnet test Tests/CourseWork.Business.Tests
```

Тести перевіряють лише рівень бізнес-логіки з mock-об'єктами `IUnitOfWork` (патерн AAA, без доступу до БД).

## Структура проєкту

```
CourseWork/
├── CourseWork.sln
├── README.md
├── ARCHITECTURE.md
├── DAL/
│   └── CourseWork.Data/       # Data Access Layer
│       ├── Entities/
│       ├── Repositories/
│       ├── UnitOfWork/
│       ├── Context/
│       └── Migrations/
├── BLL/
│   └── CourseWork.Business/   # Business Logic Layer
│       ├── Services/
│       ├── DTOs/
│       ├── Exceptions/
│       └── Mapping/
├── UI/
│   └── CourseWork.Api/        # Presentation Layer + Web UI
│       ├── Controllers/
│       ├── Models/
│       ├── Mapping/
│       └── wwwroot/
└── Tests/
    └── CourseWork.Business.Tests/
```

## Ролі та права

| Роль | Перегляд | Бронювання | Керування турами |
|------|:--------:|:----------:|:----------------:|
| Guest | ✓ | — | — |
| Registered | ✓ | ✓ | — |
| Manager | ✓ | ✓ | ✓ |
| Administrator | ✓ | ✓ | ✓ |

## Ліцензія

Навчальний проєкт. Вільне використання в межах курсової роботи.
