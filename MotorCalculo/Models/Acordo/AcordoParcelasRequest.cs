namespace Calculo.Core.Models.Acordo;

public class AcordoParcelasRequest
{
  public string COD_CLIENTE_CREDOR { get; set; }
  public string COD_FUNCIONARIO { get; set; }
  public string NUMERO_BORDERO { get; set; }
  public string COD_CLIENTE_DEVEDOR { get; set; }
  public string NUMERO_ACORDO { get; set; }
  public string NUMERO_PARCELA { get; set; }
  public DateTime DT_CADASTRO_BORDERO { get; set; }
  public DateTime DT_ACORDO { get; set; }
  public DateTime DT_VENCIMENTO_PARCELA { get; set; }
  public double VALOR_PARCELA { get; set; }
  public string IND_CHEQUE_PARCELA { get; set; }
  public double VALOR_COMISSAO { get; set; }
  public double VALOR_COMISSAO_RETIDO { get; set; }
  public double VALOR_COMISSAO_REPASSE { get; set; }
  public double VALOR_HONORARIOS { get; set; }
  public double VALOR_PRESTACAO_CONTAS { get; set; }
  public double VALOR_JUROS { get; set; }
  public double VALOR_JUROS_RETIDO { get; set; }
  public double VALOR_JUROS_REPASSE { get; set; }
  public double VALOR_HONORARIOS_RETIDO { get; set; }
  public double VALOR_HONORARIOS_REPASSE { get; set; }
  public double VALOR_PRINCIPAL { get; set; }
  public double VALOR_MULTA { get; set; }
  public double VALOR_MULTA_RETIDO { get; set; }
  public double VALOR_MULTA_REPASSE { get; set; }
  public string OBS_PARCELA { get; set; }
  public int IND_SITUACAO_PARCELA { get; set; }
}