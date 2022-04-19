//Etapas 
// - Criar projeto ASP.NET Core Web API - CatalogoApi *
// - Instalar as dependências do EF CORE e provedor Npgsql *
// - Criar entidades: Categoria e Produto *
// - Criar o arquivo de contexto: AppdbContext *
// - Definir string de conexão em appsettings.json *
// - Registrar o contexto na classe Program *
// - Aplicar o Migrations usando a ferramenta: EF Core Tools *

using CatalogoApi.ApiEndpoints;
using CatalogoApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddSwagger();
builder.AddPersistence();
builder.Services.AddCors();
builder.AddAutenticationJwt();

var app = builder.Build();

app.MapAutenticacaoEndpoints();
app.MapCategoriasEndpoints();
app.MapProdutosEndpoints();

var environment = app.Environment;

app.UseExceptionHandling(environment).UseSwaggerEndpoints().UseAppCors();

app.UseAuthentication();
app.UseAuthorization();

app.Run();


// -----------------------------------------------
// Autenticação JWT (Json Web Tokens) - Roteiro

// 1 - Geração do token JWT
// - Criar classe UserModel (username e password) *
// - Criar seção JWT no arquivo appsettings.json definindo a chave secreta *
// - Incluir o pacote Microsoft.AspNetCore.Authentication.JwtBearer *
// - Criar o serviço para gerar o token para o usuário usando a chave simétrica
//   - Definir as claims *
//   - Criar a chave simétrica usando chave secreta *
//   - Criar as credenciais usando um algoritmo (HS256) *
//   - Gerar o token *
// - Registrar o serviço no contêiner DI * 

// 2 - Validação do token JWT
// - Definir o middleware de autenticação Jwt na aplicação
// - Configurar a validaçao do token usando a chave simétrica
// - Definir os serviços de autenticação e autorização na aplicação

// 3 - Criação do Endpoint para Login
// - Validar credenciais do usuário (nome e senha)
// - Gerar o token Jwt para o usuário autenticado

// 4 - Proteger os endpoints

// -----------------------------------------------

// Sugestões para organizar o código 
// - Usar Regions (#region/#endregion)
// - Usar funções locais (após app.Run())
// - Usar métodos estáticos em classes separadas 
// - Usar métodos de extensão 
// - Realizar a separação de classes com injeção no construtor  

// 1. Criar pasta ApiEndpoints onde vamos organizar os endpoints da API
// - AutenticacaoEndpoints
// - CategoriasEndpoints
// - ProdutosEndopints

// 2. Criar pasta AppServicesExtensions onde vamos organizar os serviços usados na API
// - ApplicationBuilderExtensions
//    - UserExceptionHandling
//    - UserAppCors(*)
//    - UseSwaggerEndpoints

// - ServiceCollectionExtensions
//    - AddSwagger 
//    - AddAutenticationJwt
//    - AddPersistence