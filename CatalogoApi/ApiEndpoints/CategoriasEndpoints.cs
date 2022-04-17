using catalogoApi.Models;
using CatalogoApi.Context;
using Microsoft.EntityFrameworkCore;

namespace CatalogoApi.ApiEndpoints;

public static class CategoriasEndpoints {
    public static void MapCategoriasEndpoints(this WebApplication app) {
        app.MapGet("/categoriasprodutos", async (AppDbContext db) => await db.Categorias.Include(c => c.Produtos).ToListAsync()
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
    }
}