namespace Calculo.Core.Models.Acordo;

public class AcordoTituloRequest
{
  public string NOVO_MODULO { get; set; }
  public string TIPO_CPMF { get; set; }
  public decimal TAXA_CPMF { get; set; }
  public decimal VALOR_CPMF { get; set; }
  public string COD_CLIENTE_CREDOR { get; set; }
  public string NUMERO_BORDERO { get; set; }
  public string COD_CLIENTE_DEVEDOR { get; set; }
  public string NUMERO_ACORDO { get; set; }
  public DateTime DT_CADASTRO_BORDERO { get; set; }
  public DateTime DT_ACORDO { get; set; }
  public DateTime DT_PRIMEIRO_VENCIMENTO { get; set; }
  public DateTime DT_FECHAMENTO_CALCULO { get; set; }
  public int IND_TIPO_CALCULO { get; set; }
  public int IND_TIPO_DOCUMENTO { get; set; }
  public string IND_SITUACAO_ACORDO { get; set; }
  public double TAXA_COMISSAO_ACORDO { get; set; }
  public double TAXA_COMISSAO_REPASSE { get; set; }
  public double TAXA_COMISSAO_RETIDO { get; set; }
  public double TAXA_JUROS_ACORDO { get; set; }
  public double TAXA_JUROS_REPASSE { get; set; }
  public double TAXA_JUROS_RETIDO { get; set; }
  public double TAXA_HONORARIOS_REPASSE { get; set; }
  public double TAXA_HONORARIOS_RETIDO { get; set; }
  public double TAXA_HONORARIOS_ACORDO { get; set; }
  public double TAXA_CUSTAS_ACORDO { get; set; }
  public double VALOR_PRINCIPAL { get; set; }
  public double VALOR_JUROS { get; set; }
  public double VALOR_JUROS_RETIDO { get; set; }
  public double VALOR_JUROS_REPASSE { get; set; }
  public double VALOR_CUSTAS { get; set; }
  public double VALOR_HONORARIOS { get; set; }
  public double VALOR_TOTAL { get; set; }
  public double VALOR_COMISSAO { get; set; }
  public double VALOR_COMISSAO_REPASSE { get; set; }
  public double VALOR_COMISSAO_RETIDO { get; set; }
  public string QUANT_PARCELA_ACORDO { get; set; }
  public double VALOR_PARCELA_ACORDO { get; set; }
  public string OBS_ACORDO { get; set; }
  public double VALOR_JUROS_TOTAL { get; set; }
  public double VALOR_JUROS_ANT { get; set; }
  public double TAXA_MULTA { get; set; }
  public double TAXA_MULTA_REPASSE { get; set; }
  public double TAXA_MULTA_RETIDO { get; set; }
  public double VALOR_MULTA_RETIDO { get; set; }
  public double VALOR_MULTA_REPASSE { get; set; }
  public double VALOR_MULTA { get; set; }
  public int COD_CALCULO { get; set; }
  public int IND_PAGAMENTO { get; set; }
  public double TAXA_CAMPANHA { get; set; }
  public double VALOR_DESCONTO { get; set; }
  public double VALOR_HONORARIOS_RETIDO { get; set; }
  public double VALOR_HONORARIOS_REPASSE { get; set; }
  public string COD_FUNCIONARIO { get; set; }
}