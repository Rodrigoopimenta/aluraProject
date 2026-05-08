# aluraProject

Web API em ASP.NET Core 8 com arquitetura DDD (Domain, Application, Infrastructure e API), autenticacao JWT e controle de acesso por papeis.

## Estrutura (DDD)
- `aluraProject.Domain`: entidades e regras de negocio
- `aluraProject.Application`: casos de uso, DTOs, contratos e servicos de aplicacao
- `aluraProject.Infrastructure`: EF Core, Identity, JWT, repositorios e seed/migrations
- `aluraProject.Api`: controllers, middleware, Swagger e composicao da aplicacao

## Requisitos
- .NET SDK 8.0+
- Git

## Configuracao de ambiente
1. Copie `.env.example` para `.env` (ou configure via User Secrets/variaveis do sistema).
2. Defina obrigatoriamente:
   - `ConnectionStrings__DefaultConnection`
   - `Jwt__Key` (minimo 16 caracteres)

### User Secrets (recomendado em dev)
```bash
cd aluraProject.Api
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "sua-chave-forte-com-16-ou-mais"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Data Source=aluraProject.db"
dotnet user-secrets set "Seed:Admin:Email" "admin@aluraproject.local"
dotnet user-secrets set "Seed:Admin:Password" "SenhaForte123"
```

## Como rodar localmente
```bash
dotnet restore
dotnet build
dotnet run --project aluraProject.Api --launch-profile https
```

Swagger:
- https://localhost:7243/swagger

## Migrations e seed
- A API aplica migrations automaticamente na inicializacao.
- Seed idempotente:
  - papeis `Admin`, `Instructor`, `Student`
  - usuario admin, se `Seed:Admin:Email` e `Seed:Admin:Password` estiverem configurados.

## Convencoes de trabalho
- Branch estavel: `main`
- Features: `feature/*`
- Commits: Conventional Commits
- Template de PR: `.github/PULL_REQUEST_TEMPLATE.md`
- Backlog: GitHub Issues + Projects + Milestones

## Modelagem
- Veja [docs/data-model.md](docs/data-model.md) para entidades, relacionamentos, indices e regras.

