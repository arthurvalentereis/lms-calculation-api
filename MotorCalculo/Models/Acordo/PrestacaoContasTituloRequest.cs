namespace Calculo.Core.Models.Acordo;

public class PrestacaoContasTituloRequest
{
  public string COD_CLIENTE_CREDOR { get; set; }
  public string NUMERO_BORDERO { get; set; }
  public string COD_CLIENTE_DEVEDOR { get; set; }
  public int NUMERO_ACORDO { get; set; }
  public int NUMERO_CALCULO { get; set; }
}