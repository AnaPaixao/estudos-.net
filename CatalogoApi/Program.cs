//Etapas 
// - Criar projeto ASP.NET Core Web API - CatalogoApi *
// - Instalar as dependências do EF CORE e provedor Npgsql *
// - Criar entidades: Categoria e Produto *
// - Criar o arquivo de contexto: AppdbContext *
// - Definir string de conexão em appsettings.json *
// - Registrar o contexto na classe Program *
// - Aplicar o Migrations usando a ferramenta: EF Core Tools *

using catalogoApi.Models;
using CatalogoApi.Context;
using CatalogoApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CatalogoApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. - ConfigureServices
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
  c.SwaggerDoc("v1", new OpenApiInfo {Title = "CatalogoApi", Version = "v1"});

  c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme() {
    Name = "Authorization",
    Type = SecuritySchemeType.ApiKey,
    Scheme = "Bearer",
    BearerFormat = "JWT",
    In = ParameterLocation.Header,
    Description = @"JWT Authorization header using the Bearer scheme.
                  Enter 'Bearer'[space].Example: \'Bearer 12345abcdef\'",

  });
  c.AddSecurityRequirement(new OpenApiSecurityRequirement {
    {
      new OpenApiSecurityScheme {
        Reference = new OpenApiReference {  
          Type = ReferenceType.SecurityScheme,
          Id = "Bearer"
        }
      },
      new string[] {}
    }   
  });
});


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddSingleton<ITokenService>(new TokenService());

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
  options.TokenValidationParameters = new TokenValidationParameters {
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,

    ValidIssuer = builder.Configuration["Jwt:Issuer"],
    ValidAudience = builder.Configuration["Jwt:Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
  };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// definir endpoints - Login
app.MapPost("/login", [AllowAnonymous] (UserModel UserModel, ITokenService tokenService) => {
  if(UserModel == null) {
    return Results.BadRequest("Login Inválido");
  }

  if(UserModel.UserName == "ana" && UserModel.Password == "qwert") {
    var tokenString = tokenService.GerarToken(app.Configuration["Jwt:Key"],
        app.Configuration["Jwt:Issuer"],
        app.Configuration["Jwt:Audience"],
        UserModel);
        return Results.Ok(new {token = tokenString});
    
  } else {
    return Results.BadRequest("Login Inválido");
  }

}).Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status200OK)
.WithName("Login")
.WithTags("Autenticacao");

//definir endpoints - Categoria 
app.MapGet("/categoriaprodutos", async (AppDbContext db) => await db.Categorias.Include(c => c.Produtos).ToListAsync()
)
.Produces<List<Categoria>>(StatusCodes.Status200OK)
.WithTags("Categorias");

// endpoint para criar um novo recurso (Categoria)
// - Url padrão para o endpoint - "/categorias"
// - Delegate assíncrono para tratar o request Post
// - Results - Factory de IResult usada para produzir response HTTP comuns: OK, NotFound(), BadRequest, Created, etc.
app.MapPost("/categorias", async (Categoria categoria, AppDbContext db) => {
  db.Categorias.Add(categoria);
  await db.SaveChangesAsync();

  return Results.Created($"/categorias/{categoria.CategoriaId}", categoria);
})
.WithTags("Categorias");

// endpoint para receber uma lista de categorias 
// - verbo HTTP usado - GET
// - corpo da resposta - Um array de representação JSON de objetos Categoria
// - código de status - 200 (OK)
app.MapGet("/categorias", async (AppDbContext db) => await db.Categorias.ToListAsync())
.WithTags("Categorias")
.RequireAuthorization();

// endpoint para obter uma Categoria pelo seu ID
app.MapGet("/categorias/{id:int}", async (int id, AppDbContext db) => {
  return await db.Categorias.FindAsync(id) is Categoria categoria ? Results.Ok(categoria) : Results.NotFound();
})
.WithTags("Categorias");

// Endpoint para atualizar uma Categoria pelo ID
app.MapPut("/categorias/{id:int}", async (int id, Categoria categoria, AppDbContext db) => {
  if(categoria.CategoriaId != id) {
    return Results.BadRequest();
  }

  var categoriaDB = await db.Categorias.FindAsync(id);

  if(categoriaDB is null) return Results.NotFound();

  categoriaDB.Nome = categoria.Nome;
  categoriaDB.Descricao = categoria.Descricao;

  await db.SaveChangesAsync();
  return Results.Ok(categoriaDB);
})
.WithTags("Categorias");

// endpoint para deletar uma Categoria pelo seu ID
app.MapDelete("/categorias/{id:int}", async (int id, AppDbContext db) => {
  var categoria = await db.Categorias.FindAsync(id);

  if(categoria is null) return Results.NotFound();

  db.Categorias.Remove(categoria);
  await db.SaveChangesAsync();

  return Results.NoContent();
})
.WithTags("Categorias");



// Definir endpoints - Produto

// Post
// Produces -> Define o tipo de retorno e o código de status esperado (Documentação)
// WithName -> Identifica o endpoint de forma única
// WithTags -> Agrupa os endpoints
app.MapPost("/produtos", async (Produto produto, AppDbContext db) => {
  db.Produtos.Add(produto);
  await db.SaveChangesAsync();

  return Results.Created($"/produtos/{produto.ProdutoId}", produto);
})
.Produces<Produto>(StatusCodes.Status201Created)
.WithName("CriarNovoProduto")
.WithTags("Produtos");

// Get -> Obter todos os produtos 
app.MapGet("/produtos", async(AppDbContext db) => {
  await db.Produtos.ToListAsync();
})
.Produces<List<Produto>>(StatusCodes.Status200OK)
.WithTags("Produtos");

// Get -> Obter produtos pelo id
app.MapGet("/produtos/{id:int}", async (int id, AppDbContext db) => {
  return await db.Produtos.FindAsync(id) is Produto produto ?  Results.Ok(produto) : Results.NotFound("Produto não encontrado");
})
.Produces<Produto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.WithTags("Produtos");

// Put -> Para atualizar apenas uma propriedade do Produto
app.MapPut("/produtos", async (int produtoId, string produtoNome, AppDbContext db) => {
  var produtoDB = db.Produtos.SingleOrDefault(s => s.ProdutoId == produtoId);

  if(produtoDB == null) return Results.NotFound("Produto não encontrado");

  produtoDB.Nome = produtoNome;

  await db.SaveChangesAsync();
  return Results.Ok(produtoDB);

})
.Produces<Produto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.WithName("AtualizaNomeProduto")
.WithTags("Produtos");

app.MapPut("/produtos/{id:int}", async (int id, Produto produto, AppDbContext db) => {
  if(produto.ProdutoId != id) {
    return Results.BadRequest("Ids não conferem");
  }

  var produtoDB = await db.Produtos.FindAsync(id);

  if(produtoDB is null) return Results.NotFound("Produto não encontrado");

  produtoDB.Nome = produto.Nome;
  produtoDB.Descricao = produto.Descricao;
  produtoDB.Preco = produto.Preco;
  produtoDB.DataCompra = produto.DataCompra;
  produtoDB.Estoque = produto.Estoque;
  produtoDB.ImagemUrl = produto.ImagemUrl;
  produtoDB.CategoriaId = produto.CategoriaId;

  await db.SaveChangesAsync();
  return Results.Ok(produtoDB);
})
.Produces<Produto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status404NotFound)
.WithName("AtualizaProduto")
.WithTags("Produtos");

app.MapDelete("/produtos/{id:int}", async(int id, AppDbContext db) => {
  var produtoDB = await db.Produtos.FindAsync(id);

  if(produtoDB is null) {
    return Results.NotFound("Produto não encontrado");
  }
  db.Produtos.Remove(produtoDB);
  await db.SaveChangesAsync();
  return Results.Ok(produtoDB);
})
.Produces<Produto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.WithName("DeletaProduto")
.WithTags("Produtos");


// endpoint para localizar um Produto por nome usando um critério
app.MapGet("/produtos/nome/{criterio}", (string criterio, AppDbContext db) => {
  
  var produtosSelecionados = db.Produtos.Where(x => x.Nome.ToLower().Contains(criterio.ToLower())).ToList();

  return produtosSelecionados.Count > 0 ? Results.Ok(produtosSelecionados) : Results.NotFound(Array.Empty<Produto>());  
  
})
.Produces<List<Produto>>(StatusCodes.Status200OK)
.WithName("FiltrarPorNome")
.WithTags("Produtos");

// endpoint para obter os produtos com paginação
app.MapGet("/produtosporpagina", async (int numeroPagina, int tamanhoPagina, AppDbContext db) => {
  await db.Produtos.Skip((numeroPagina - 1) * tamanhoPagina).Take(tamanhoPagina).ToListAsync();
})
.Produces<List<Produto>>(StatusCodes.Status200OK)
.WithName("ProdutosPorPagina")
.WithTags("Produtos");

// endpoint para obter categorias com seus produtos 
app.MapGet("/categoriaprodutos", async (AppDbContext db) => {
  await db.Categorias.Include(c => c.Produtos).ToListAsync();
})
.Produces<List<Categoria>>(StatusCodes.Status200OK)
.WithTags("Categorias");


// Configure the HTTP request pipeline. - Configure
if(app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.Run();

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