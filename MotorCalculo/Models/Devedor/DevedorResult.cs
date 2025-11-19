namespace Calculo.Core.Models;

public class DevedorResult
{
  public string RazaoSocialCredor { get; set; }
  public string RazaoSocialDevedor { get; set; }
  public string CodigoCredor { get; set; }
  public string CodigoDevedor { get; set; }
  public string CodigoRecuperador { get; set; }
  public DateTime DataBordero { get; set; }
  public string NomeRecuperador { get; set; }
  public string NumeroBordero { get; set; }
  public string LiberaParcelas { get; set; }
  public string ObsRepasse { get; set; }
  public decimal TxComissao { get; set; }
  public decimal TxJuros { get; set; }
  public decimal Txhonorarios { get; set; }
  public decimal TxCustas { get; set; }
  public decimal TxJurosMin { get; set; }
  public decimal TxJurosMax { get; set; }
  public decimal TxMulta { get; set; }
  public decimal VlPrincipal { get; set; }
}