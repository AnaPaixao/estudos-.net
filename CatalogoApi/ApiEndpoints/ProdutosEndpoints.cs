using catalogoApi.Models;
using CatalogoApi.Context;
using Microsoft.EntityFrameworkCore;

namespace CatalogoApi.ApiEndpoints;

public static class ProdutosEndopints {
    public static void MapProdutosEndpoints(this WebApplication app) {
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
    }
}