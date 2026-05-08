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

## Provedor de banco
- Desenvolvimento: **SQLite** (`Data Source=aluraProject.db`)
- Evolucao para ambientes maiores: SQL Server/PostgreSQL (mantendo migrations por provedor)

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
dotnet user-secrets set "Jwt:Issuer" "aluraProject"
dotnet user-secrets set "Jwt:Audience" "aluraProject.Client"
dotnet user-secrets set "Jwt:AccessTokenMinutes" "120"
dotnet user-secrets set "Seed:Admin:Email" "admin@aluraproject.local"
dotnet user-secrets set "Seed:Admin:Password" "SenhaForte123"
```

## EF Core + Identity + JWT (resumo da configuracao)
- `AppDbContext` herda de `IdentityDbContext<ApplicationUser, IdentityRole, string>`.
- DI configurado com:
  - `AddDbContext<AppDbContext>(UseSqlite(...))`
  - `AddIdentityCore<ApplicationUser>()`
  - `AddRoles<IdentityRole>()`
  - `AddEntityFrameworkStores<AppDbContext>()`
  - `AddSignInManager()` + `AddDefaultTokenProviders()`
- Politicas do Identity:
  - `RequireUniqueEmail = true`
  - senha forte (min 8, maiuscula, minuscula, numero, simbolo)
  - lockout: 5 tentativas, 15 minutos
- JWT Bearer:
  - emissor: `Jwt:Issuer`
  - audiencia: `Jwt:Audience`
  - chave de assinatura: `Jwt:Key`
  - expiracao do access token: `Jwt:AccessTokenMinutes`

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
- Comando manual de migrations (se necessario):
```bash
dotnet ef migrations add NomeDaMigration --project aluraProject.Infrastructure --startup-project aluraProject.Api
dotnet ef database update --project aluraProject.Infrastructure --startup-project aluraProject.Api
```

## Rotas de autenticacao (contrato)
### `POST /api/auth/register`
- Objetivo: registrar usuario e vincular papel (`Admin`, `Instructor`, `Student`).
- Entrada: email, senha, role.
- Saida: access token JWT + expiracao.

### `POST /api/auth/login`
- Objetivo: autenticar usuario existente.
- Entrada: email, senha.
- Saida: access token JWT + expiracao.

### `POST /api/auth/refresh-token`
- Objetivo: trocar um refresh token valido por novo access token (e rotacionar refresh token).
- Entrada esperada: refresh token (httpOnly cookie ou payload dedicado, conforme politica final).
- Saida esperada: novo access token + novo refresh token.
- Status atual: **rota definida no contrato/documentacao; implementacao de refresh token ainda pendente**.

## Convencoes de trabalho
- Branch estavel: `main`
- Features: `feature/*`
- Commits: Conventional Commits
- Template de PR: `.github/PULL_REQUEST_TEMPLATE.md`
- Backlog: GitHub Issues + Projects + Milestones

## Modelagem
- Veja [docs/data-model.md](docs/data-model.md) para entidades, relacionamentos, indices e regras.

