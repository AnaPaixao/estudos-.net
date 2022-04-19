namespace ApiColecao.Models;

public class Cd {
  public int CdId {get; set; }

  public string? Nome {get; set; }

  public string? Descricao { get; set; }

  public int Quantidade {get; set; }

  public int CategoriaId {get; set; }

  public Categoria? Categoria {get; set;}
}
