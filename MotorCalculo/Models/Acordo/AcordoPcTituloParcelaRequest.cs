namespace Calculo.Core.Models.Acordo;

public class AcordoPcTituloParcelaRequest
{
  public int COD_CALCULO { get; set; }
  public string COD_PARCELA { get; set; }
  public DateTime DT_VENCIMENTO { get; set; }
  public double VALOR_PARCELA { get; set; }
  public double VALOR_HONORARIOS { get; set; }
  public double VALOR_COMISSAO { get; set; }
  public double VALOR_TOTAL { get; set; }
  public double VALOR_PRINCIPAL_TOTAL { get; set; }
  public string COD_CLIENTE_DEVEDOR { get; set; }
}