namespace Calculo.Core.Models.Titulo;

public class TituloResult
{
  public int id { get; set; }
  public string nomeTitulo { get; set; }
  public DateTime dataTituloVencimento { get; set; }
  public DateTime dataTituloCadastro { get; set; }
  public DateTime dataTituloEmissao { get; set; }
  public int atrasoDuplicata { get; set; }
  public int atrasoContrato { get; set; }
  public decimal valorPrincipal { get; set; }
  public decimal valorPrincipalOriginal { get; set; }
  public string observacao { get; set; }
  public decimal valorCustas { get; set; }
  public string situacao { get; set; }
}