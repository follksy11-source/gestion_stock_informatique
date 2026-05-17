# StockInfo — Plateforme de gestion de stock informatique

Application web ASP.NET Core MVC sécurisée pour la gestion de stock de matériel informatique. Développée dans le cadre d'un cours sur la sécurité web : authentification, autorisation, JWT, logging et architecture en couches.

---

## Fonctionnalités

- Authentification par cookies avec ASP.NET Core Identity
- Authentification par JWT pour les endpoints API
- Gestion des rôles : Admin, Gestionnaire, Vendeur
- Autorisation par rôles et policies
- CRUD complet des produits informatiques
- Dashboard avec statistiques de stock en temps réel
- Alertes visuelles pour les produits en rupture de stock
- Gestion centralisée des exceptions
- Logging métier avec ILogger
- Configuration par environnement (Development / Production)
- Interface responsive Bootstrap 5

---

## Prérequis

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- [dotnet-ef](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) (outil Entity Framework Core)

---

## Installation

### 1. Cloner le projet

```bash
git clone https://github.com/votre-utilisateur/SecureApp.git
cd SecureApp
```

### 2. Installer l'outil Entity Framework

```bash
dotnet tool install --global dotnet-ef
```

### 3. Restaurer les packages

```bash
dotnet restore
```

### 4. Créer la base de données

```bash
dotnet ef database update
```

### 5. Lancer l'application

```bash
dotnet run
```

L'application est accessible sur `http://localhost:5164`

---

## Structure du projet

```
SecureApp/
├── Controllers/
│   ├── HomeController.cs        # Pages principales et dashboard
│   ├── AccountController.cs     # Inscription, connexion, déconnexion
│   ├── ProduitController.cs     # CRUD des produits
│   ├── AdminController.cs       # Gestion des utilisateurs et rôles
│   └── ApiController.cs         # Endpoints API REST avec JWT
│
├── Models/
│   ├── Produit.cs               # Modèle produit
│   └── AccountViewModels.cs     # ViewModels pour les formulaires
│
├── Views/
│   ├── Shared/
│   │   └── _Layout.cshtml       # Layout global (navbar, footer)
│   ├── Home/                    # Accueil, Dashboard, pages protégées
│   ├── Account/                 # Login, Register, AccessDenied
│   ├── Produit/                 # Liste, Create, Edit
│   └── Admin/                   # Gestion des utilisateurs
│
├── Services/
│   ├── IUserService.cs          # Interface du service utilisateur
│   └── UserService.cs           # Logique métier utilisateur
│
├── Data/
│   ├── AppDbContext.cs          # Contexte Entity Framework
│   └── DbInitializer.cs         # Seed des rôles et produits fictifs
│
├── Middleware/
│   └── ExceptionMiddleware.cs   # Gestion centralisée des exceptions
│
├── appsettings.json             # Configuration générale
├── appsettings.Development.json # Configuration développement
├── appsettings.Production.json  # Configuration production
└── Program.cs                   # Point d'entrée et configuration
```

---

## Architecture et rôle de chaque composant

### Controllers
Reçoivent les requêtes HTTP, appellent les services ou le contexte de données, et retournent une vue ou une réponse JSON. Ils ne contiennent pas de logique métier complexe.

### Models
Représentent les données de l'application. Le modèle `Produit` est mappé directement à une table en base de données via Entity Framework. Les `ViewModels` transportent les données des formulaires vers les controllers.

### Views
Pages HTML générées côté serveur avec Razor. Elles reçoivent les données du controller via `ViewBag` ou un modèle typé (`@model`).

### Services
Contiennent la logique métier réutilisable. On programme contre des interfaces (`IUserService`) pour faciliter les tests et la maintenance.

### Data
`AppDbContext` est le point central d'accès à la base de données SQLite. `DbInitializer` crée automatiquement les rôles et les produits fictifs au démarrage.

### Middleware
`ExceptionMiddleware` intercepte toutes les exceptions non gérées, les enregistre dans les logs, et redirige vers une page d'erreur propre.

---

## Sécurité

### Authentification par cookies
Utilisée pour les pages web. Après connexion, un cookie chiffré est stocké dans le navigateur et envoyé automatiquement à chaque requête.

```
POST /Account/Login → vérification → cookie créé → accès autorisé
```

### Authentification par JWT
Utilisée pour l'API REST. Le client obtient un token en s'authentifiant, puis l'envoie dans le header de chaque requête.

```
POST /api/Api/token  → retourne un token JWT
GET  /api/Api/secure → Authorization: Bearer <token>
```

### Rôles et permissions

| Action | Admin | Gestionnaire | Vendeur |
|---|---|---|---|
| Consulter les produits | ✅ | ✅ | ✅ |
| Ajouter un produit | ✅ | ✅ | ❌ |
| Modifier un produit | ✅ | ✅ | ❌ |
| Supprimer un produit | ✅ | ❌ | ❌ |
| Gérer les utilisateurs | ✅ | ❌ | ❌ |

### Policies
Les policies permettent de définir des règles d'autorisation centralisées et réutilisables dans `Program.cs` :

```csharp
options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
```

---

## API REST

### Obtenir un token JWT

```bash
curl -X POST http://localhost:5164/api/Api/token \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@example.com","password":"motdepasse"}'
```

Réponse :
```json
{ "token": "eyJhbGciOiJIUzI1NiIs..." }
```

### Accéder à un endpoint sécurisé

```bash
curl http://localhost:5164/api/Api/secure \
  -H "Authorization: Bearer VOTRE_TOKEN"
```

---

## Configuration

### appsettings.json
Configuration générale incluant la clé JWT :

```json
{
  "Jwt": {
    "Key": "CeciEstUneCleSecreteTresLongueEtSecurisee123!",
    "Issuer": "SecureApp",
    "Audience": "SecureAppUsers"
  }
}
```

### Environnements

```bash
# Développement (logs détaillés)
export ASPNETCORE_ENVIRONMENT=Development

# Production (logs minimaux)
export ASPNETCORE_ENVIRONMENT=Production
```

---

## Données fictives

Au premier démarrage, l'application crée automatiquement :

- 3 rôles : Admin, Gestionnaire, Vendeur
- 10 produits informatiques répartis en 4 catégories : Laptops, Smartphones, Moniteurs, Accessoires

Pour tester les rôles, créez un compte puis accédez à `/Account/MakeAdmin` pour vous promouvoir Admin.

---

## Logging

Les logs apparaissent dans le terminal pendant l'exécution :

```
info: Page d'accueil visitée
info: Liste des produits consultée par user@example.com
warn: Produit Dell XPS supprimé par admin@example.com
error: Une erreur inattendue s'est produite
```

---

## Technologies utilisées

| Technologie | Rôle |
|---|---|
| ASP.NET Core 8 MVC | Framework web |
| Entity Framework Core 8 | ORM — accès base de données |
| SQLite | Base de données |
| ASP.NET Core Identity | Gestion des utilisateurs |
| JWT Bearer | Authentification API |
| Bootstrap 5 | Interface utilisateur |
| Bootstrap Icons | Icônes |

---

## Concepts couverts

- Pattern MVC (Model, View, Controller)
- Injection de dépendances
- Middleware pipeline
- Authentification et autorisation
- Gestion des rôles et policies
- API REST sécurisée par JWT
- Entity Framework Core et migrations
- Logging avec ILogger
- Configuration par environnement
- Séparation des responsabilités

---

## Auteur

Projet réalisé dans le cadre d'un cours sur la sécurité ASP.NET Core.
