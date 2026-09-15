# Gestion-Fonds — Fonds Social

Backend REST API (ASP.NET Core / .NET 8) et interface web (Angular 18) pour la gestion du
Fonds Social d'une entreprise : prêts aux employés, de la demande jusqu'au remboursement.

Le système gère :
- les sociétés et leurs agents (employés) ;
- les types de prêt et leurs règles d'éligibilité (plafond, franchise, durée) ;
- les demandes de prêt, avec une machine à états complète (dépôt → étude → décision →
  contrat → échéances) ;
- les pièces justificatives (upload sécurisé, vérification) ;
- les séances de comité et leurs décisions ;
- les contrats, garanties et échéanciers de remboursement ;
- les retenues mensuelles sur salaire ;
- le budget annuel du fonds.

## Architecture

Le backend suit une architecture en couches :

```
FondsSocial.API             → Contrôleurs REST, middleware, Swagger, Serilog, CORS
FondsSocial.Application      → DTOs, services métier, validation (FluentValidation), AutoMapper
FondsSocial.Domain            → Entités et enums, sans dépendance externe
FondsSocial.Infrastructure    → DbContext EF Core, migrations, repositories, unit of work
FondsSocial.Application.Tests → Tests unitaires (xUnit + Moq)
fonds-social-ui/              → Frontend Angular 18 (standalone components, signals)
```

## Stack technique

**Backend** : .NET 8, ASP.NET Core Web API, Entity Framework Core 8 (Code First), SQL
Server, AutoMapper, FluentValidation, Serilog, Swagger/Swashbuckle, xUnit + Moq.

**Frontend** : Angular 18 (standalone components, signals), TypeScript, Karma/Jasmine.

## Prérequis

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) (LTS) et npm
- SQL Server accessible (LocalDB, SQL Express, ou une instance complète)

## Lancer le backend

1. Configurer la chaîne de connexion (voir [Configuration](#configuration) ci-dessous).

2. Restaurer et compiler :
   ```bash
   dotnet build
   ```

3. Appliquer les migrations à la base de données :
   ```bash
   dotnet ef database update --project FondsSocial.Infrastructure --startup-project FondsSocial.API
   ```
   (Le projet API ne référence pas le package EF Design ; `--startup-project` doit pointer
   vers `FondsSocial.Infrastructure` pour que l'outil `dotnet ef` fonctionne.)

4. Lancer l'API :
   ```bash
   dotnet run --project FondsSocial.API
   ```

5. Ouvrir Swagger :
   - HTTP : http://localhost:5201/swagger
   - HTTPS : https://localhost:7201/swagger

## Lancer le frontend

Dans un second terminal :

```bash
cd fonds-social-ui
npm install
npm start
```

L'application est servie sur http://localhost:4200 et proxifie automatiquement les appels
`/api/*` vers le backend sur `http://localhost:5201` (voir `proxy.conf.json`) — le backend
doit donc être démarré au préalable.

## Tests

**Backend** (xUnit + Moq) :
```bash
dotnet test
```

**Frontend** (Karma/Jasmine, headless Chrome) :
```bash
cd fonds-social-ui
npm test -- --watch=false --browsers=ChromeHeadless
```

## Configuration

- La chaîne de connexion par défaut se trouve dans `FondsSocial.API/appsettings.json` :
  ```
  Server=.;Database=FondsSocialDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True
  ```
  Adaptez `Server=.` à votre instance SQL Server (ex. `(localdb)\mssqllocaldb` ou
  `VOTRE_MACHINE\SQLEXPRESS`).
- Les outils de migration (`FondsSocial.Infrastructure/Data/FondsSocialDbContextFactory.cs`)
  acceptent une surcharge via la variable d'environnement
  `ConnectionStrings__DefaultConnection`, avec un repli sur `(localdb)\mssqllocaldb` si rien
  n'est défini.
- CORS est restreint par une liste d'origines autorisées (`Cors:AllowedOrigins` dans
  `appsettings.json`), vide par défaut — donc fermé par défaut (fail-safe). En développement,
  le frontend passe par le proxy Angular (`proxy.conf.json`) et n'a pas besoin de CORS ; pour
  appeler l'API directement depuis un autre client en dev, ajoutez son origine à cette liste.
- **Ne committez jamais de secrets/identifiants** dans `appsettings.*.json` (ces fichiers
  d'environnement sont d'ailleurs exclus par `.gitignore`, à l'exception du fichier de base).

## Structure du dépôt

- `PROJECT_DOCUMENTATION.txt` — documentation détaillée : schéma relationnel complet, liste
  des entités/enums, endpoints API, règles métier et scénario de bout en bout.
- `Gestion-Fonds.slnx` — solution .NET (nouveau format XML).

## Licence

À préciser.
