# Gestion-Fonds

Projet de gestion du Fonds Social — modélisation Code First avec Entity Framework Core 8.

Structure
- FondsSocial.Domain : entités POCO et enums
- FondsSocial.Infrastructure : DbContext, configurations Fluent API, migrations

Prérequis
- .NET 8 SDK
- SQL Server (ex. (localdb)\\MSSQLLocalDB ou HAMZA\\SQLEXPRESS)

Build & migrations
1. dotnet build
2. dotnet ef migrations add InitialCreate --project FondsSocial.Infrastructure
3. dotnet ef database update --project FondsSocial.Infrastructure

Remarques
- La chaîne de connexion utilisée par défaut pour les outils de migration se trouve dans FondsSocial.Infrastructure/Data/FondsSocialDbContextFactory.cs. Déplacez-la vers appsettings ou User Secrets en dev avant publication.
- Ne comitez pas de secrets/credentials.

Licence
- A préciser
