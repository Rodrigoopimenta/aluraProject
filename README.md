# aluraProject

API Web em ASP.NET Core para gerenciamento de cursos, estudantes e matriculas.

## Objetivo
Criar uma base de API REST em .NET 8 com Controllers, suporte a HTTPS e boas praticas de configuracao para evoluir o dominio do projeto.

## Requisitos
- .NET SDK 8.0 (ou superior com suporte a `net8.0`)
- Git

## Como rodar localmente
1. Restaurar dependencias:
   ```bash
   dotnet restore
   ```
2. (Opcional) Definir segredos via User Secrets:
   ```bash
   cd aluraProject.Api
   dotnet user-secrets init
   dotnet user-secrets set "Jwt:Key" "sua-chave-forte"
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Data Source=aluraProject.db"
   ```
3. Rodar a API em HTTPS:
   ```bash
   dotnet run --project aluraProject.Api --launch-profile https
   ```
4. Acessar Swagger:
   - https://localhost:7243/swagger

## Configuracao de variaveis de ambiente
Use as chaves do arquivo `.env.example` (nunca commitar segredos reais):

- `ConnectionStrings__DefaultConnection`
- `Jwt__Issuer`
- `Jwt__Audience`
- `Jwt__Key`

## Convencoes de trabalho
- Branch estavel: `main`
- Branches de feature: `feature/*`
- Commits: Conventional Commits (ex.: `feat: adiciona endpoint de cursos`)
- PR: usar o template em `.github/PULL_REQUEST_TEMPLATE.md`

## Backlog e planejamento
- Usar GitHub Issues para historias/tarefas
- Organizar roadmap em GitHub Projects
- Agrupar entregas por Milestones
