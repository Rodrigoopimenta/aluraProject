# aluraProject

ASP.NET Core 8 Web API com arquitetura DDD (Domain, Application, Infrastructure, API), autenticacao JWT e autorizacao por roles.

## Estrutura
- `aluraProject.Domain`: entidades e regras de dominio.
- `aluraProject.Application`: casos de uso, DTOs, contratos e servicos.
- `aluraProject.Infrastructure`: EF Core, Identity, JWT, repositorios, migrations e seed.
- `aluraProject.Api`: controllers, middleware, Swagger e composicao da aplicacao.

## Requisitos
- .NET SDK 8+
- Git

## Configuracao
Defina os valores abaixo em `appsettings.*`, variaveis de ambiente ou User Secrets:
- `ConnectionStrings__DefaultConnection`
- `Jwt__Key` (minimo 16 caracteres)
- `Jwt__Issuer`
- `Jwt__Audience`
- `Jwt__AccessTokenMinutes`

Exemplo com User Secrets:
```bash
cd aluraProject.Api
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Data Source=aluraProject.db"
dotnet user-secrets set "Jwt:Key" "sua-chave-forte-com-16-ou-mais"
dotnet user-secrets set "Jwt:Issuer" "aluraProject"
dotnet user-secrets set "Jwt:Audience" "aluraProject.Client"
dotnet user-secrets set "Jwt:AccessTokenMinutes" "120"
```

## Como rodar o projeto
Na raiz da solucao:
```bash
dotnet restore
dotnet build
dotnet run --project aluraProject.Api --launch-profile https
```

Swagger UI (Development e Staging/homolog):
- `https://localhost:7243/swagger`

## Como rodar os testes
```bash
dotnet test
```

Observacao: atualmente a solucao nao possui projetos de teste automatizado; o comando executa sem testes.

## Swagger e autenticacao JWT
1. Abra `POST /api/auth/login` ou `POST /api/auth/register` no Swagger.
2. Execute a requisicao e copie o `accessToken` retornado.
3. Clique em **Authorize** no Swagger UI.
4. Informe: `Bearer {seu_token}`.
5. Execute endpoints protegidos.

Security Scheme configurado:
- Tipo: `HTTP`
- Scheme: `bearer`
- Bearer format: `JWT`

## Organizacao de endpoints (tags)
- `Auth`
- `Courses`
- `Students`
- `Enrollments`

## Roles por endpoint (resumo)
- `POST /api/auth/register`: anonimo
- `POST /api/auth/login`: anonimo
- `GET /api/courses`: anonimo
- `GET /api/courses/{id}`: anonimo
- `POST /api/courses`: `Admin`, `Instructor`
- `PUT /api/courses/{id}`: `Admin`, `Instructor`
- `DELETE /api/courses/{id}`: `Admin`
- `POST /api/students`: `Admin`
- `GET /api/students`: `Admin`
- `GET /api/students/me` e `GET /me`: autenticado
- `GET /api/students/{id}`: `Admin` ou dono do recurso
- `PUT /api/students/{id}`: `Admin` ou dono do recurso
- `DELETE /api/students/{id}`: `Admin`
- `GET /api/students/{id}/enrollments`: `Admin` ou dono do recurso
- `POST /api/enrollments`: `Student`, `Admin`

## Status e erros padronizados
Erros retornam `ProblemDetails` com:
- `status`
- `title`
- `detail`
- `instance`
- `traceId`
- `timestampUtc`
- `errorCode`

Codigos previstos:
- `400` entrada invalida
- `401` nao autenticado
- `403` sem permissao
- `404` nao encontrado
- `409` conflito (ex.: duplicidade)
- `422` violacao de regra de negocio

## Paginacao e filtros
- `page` inicia em `1`.
- `pageSize` maximo `100`.
- `GET /api/students/{id}/enrollments`:
  - filtro `status`: `Active` ou `Cancelled`.

## Regras de validacao relevantes
- `Course.title`: minimo 3 caracteres.
- `Student.email`: formato valido e unico.
- `Enrollment`: nao permite duplicidade (`StudentId + CourseId` com indice unico).

## Arquivo de requests HTTP
Foi incluido:
- `aluraProject.Api/aluraProject.Api.http`

Esse arquivo cobre fluxo de login e chamadas protegidas com JWT para testes manuais.
