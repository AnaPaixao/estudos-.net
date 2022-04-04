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

//definir endpoints
app.MapPost("/categorias", async (Categoria categoria, AppDbContext db) => {
  db.categorias.Add(categoria);
  await db.SaveChangesAsync();

  return Results.Created($"/categorias/{categoria.CategoriaId}", categoria);
});

// Configure the HTTP request pipeline. - Configure
if(app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI();
}


app.Run();
