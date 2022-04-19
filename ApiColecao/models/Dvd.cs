namespace ApiColecao.Models;

public class Dvd {
  public int DvdId {get; set; } 

  public string? Nome {get; set; }
    
  public string? Descricao { get; set; }

  public int Quantidade {get; set; }

    public int CategoriaId { get; set; }

  public Categoria? Categoria {get; set;}
}
