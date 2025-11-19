namespace Calculo.Core.Models.Acordo;

public class AcordoPcTituloRequest
{
  public int COD_CALCULO { get; set; }
  public string NUMERO_TITULO { get; set; }
  public DateTime DT_VENCIMENTO_TITULO { get; set; }
  public double VALOR_PRINCIPAL { get; set; }
  public double VALOR_PROTESTO { get; set; }
  public double VALOR_JUROS { get; set; }
  public double VALOR_DESCONTO { get; set; }
  public double VALOR_JUROS_RETIDO { get; set; }
  public double VALOR_JUROS_REPASSE { get; set; }
  public double VALOR_HONORARIOS_RETIDO { get; set; }
  public double VALOR_HONORARIOS_REPASSE { get; set; }
  public double VALOR_MULTA_RETIDO { get; set; }
  public double VALOR_MULTA_REPASSE { get; set; }
  public double VALOR_TOTAL { get; set; }
  public string IND_SITUACAO { get; set; }
  public double VALOR_PRINCIPAL_ORIGINAL { get; set; }
}