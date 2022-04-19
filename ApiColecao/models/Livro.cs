namespace ApiColecao.Models;

public class Livro {

  public int LivroId {get; set; }

  public string Nome {get; set; }

  public string? Descricao {get; set; }

  public int Quantidade {get; set; }

  public string? Autor {get; set; }

  public int CategoriaId {get; set; }

  public Categoria? Categoria {get; set;}
}
