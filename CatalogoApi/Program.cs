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
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. - ConfigureServices
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

var app = builder.Build();

//definir endpoints - Categortia 

// endpoint para criar um novo recurso (Categoria)
// - Url padrão para o endpoint - "/categorias"
// - Delegate assíncrono para tratar o request Post
// - Results - Factory de IResult usada para produzir response HTTP comuns: OK, NotFound(), BadRequest, Created, etc.
app.MapPost("/categorias", async (Categoria categoria, AppDbContext db) => {
  db.Categorias.Add(categoria);
  await db.SaveChangesAsync();

  return Results.Created($"/categorias/{categoria.CategoriaId}", categoria);
});

// endpoint para receber uma lista de categorias 
// - verbo HTTP usado - GET
// - corpo da resposta - Um array de representação JSON de objetos Categoria
// - código de status - 200 (OK)
app.MapGet("/categorias", async (AppDbContext db) => await db.Categorias.ToListAsync());

// endpoint para obter uma Categoria pelo seu ID
app.MapGet("/categorias/{id:int}", async (int id, AppDbContext db) => {
  return await db.Categorias.FindAsync(id) is Categoria categoria ? Results.Ok(categoria) : Results.NotFound();
});

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
});

// endpoint para deletar uma Categoria pelo seu ID
app.MapDelete("/categorias/{id:int}", async (int id, AppDbContext db) => {
  var categoria = await db.Categorias.FindAsync(id);

  if(categoria is null) return Results.NotFound();

  db.Categorias.Remove(categoria);
  await db.SaveChangesAsync();

  return Results.NoContent();
});



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

  if(produtoDB == null) return Results.NotFound();

  produtoDB.Nome = produtoNome;

  await db.SaveChangesAsync();
  return Results.Ok(produtoDB);

})
.Produces<Produto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.WithName("AtualizaNomeProduto")
.WithTags("Produtos");



// Configure the HTTP request pipeline. - Configure
if(app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI();
}


app.Run();
