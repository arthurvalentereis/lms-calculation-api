namespace Calculo.Core.Models.Acordo;

public class AcordoPcTituloParamRequest
{
  public int COD_CALCULO { get; set; }
  public string COD_CLIENTE_CREDOR { get; set; }
  public string NUMERO_BORDERO { get; set; }
  public string COD_CLIENTE_DEVEDOR { get; set; }
  public string NUMERO_ACORDO { get; set; }
  public double TAXA_COMISSAO { get; set; }
  public double TAXA_JUROS { get; set; }
  public double TAXA_HONORARIOS { get; set; }
  public double TAXA_MULTA { get; set; }
  public string IND_DIFERENCA { get; set; }
  public double VALOR_DIFERENCA { get; set; }
  public string TIPO_CALCULO { get; set; }
}