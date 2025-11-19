namespace Calculo.Core.Models.Acordo;

public class AcordoPcTituloCalculoRequest
{
  public int COD_CALCULO { get; set; }
  public string IND_PARCELA { get; set; }
  public string NUMERO_TITULO { get; set; }
  public double VALOR_PRINCIPAL { get; set; }
  public double VALOR_PRINCIPAL_RECEBIDO { get; set; }
  public DateTime DT_VENCIMENTO { get; set; }
  public DateTime DT_RECEBIMENTO { get; set; }
  public int DIAS_ATRASO { get; set; }
  public double VALOR_JUROS { get; set; }
  public double VALOR_JUROS_RETIDO { get; set; }
  public double VALOR_JUROS_REPASSE { get; set; }
  public double VALOR_JUROS_RECEBIDO { get; set; }
  public double VALOR_HONORARIOS_REPASSE { get; set; }
  public double VALOR_HONORARIOS_RETIDO { get; set; }
  public double VALOR_CUSTAS { get; set; }
  public double VALOR_CUSTAS_RECEBIDO { get; set; }
  public double VALOR_SUB_TOTAL { get; set; }
  public double VALOR_RECEBIDO { get; set; }
  public double VALOR_SALDO { get; set; }
  public double VALOR_COMISSAO { get; set; }
  public double VALOR_COMISSAO_RETIDO { get; set; }
  public double VALOR_COMISSAO_REPASSE { get; set; }
  public double VALOR_HONORARIOS { get; set; }
  public double VALOR_PRESTACAO_CONTAS { get; set; }
  public double VALOR_MULTA { get; set; }
  public double VALOR_MULTA_RETIDO { get; set; }
  public double VALOR_MULTA_REPASSE { get; set; }
  public double VALOR_DESCONTO { get; set; }
  public double VALOR_MULTA_BASE { get; set; }
}