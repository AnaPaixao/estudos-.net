namespace ApiColecao.Models;
  public class Categoria {

    public int CategoriaId { get; set; }
    public string? Nome { get; set; }

    public ICollection<Livro>? Livros {get; set;}
    public ICollection<Dvd>? Dvds {get; set;}
    public ICollection<Cd>? Cds {get; set;}
  }

