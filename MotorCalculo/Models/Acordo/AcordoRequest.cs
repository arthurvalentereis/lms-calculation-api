namespace Calculo.Core.Models.Acordo;

public class PARCELA
{
  public DateTime dataVencimento { get; set; }
  public double valorParcela { get; set; }
  public bool first { get; set; }
  public int numeroParcela { get; set; }
  public bool alterado { get; set; }
}

public class TITULO
{
  public int numeroParcela { get; set; }
  public string numeroTitulo { get; set; }
  public string nomeTitulo { get; set; }
  public double valorPrincipal { get; set; }
  public DateTime dataVencimento { get; set; }
  public DateTime dataVencimentoTitulo { get; set; }
  public DateTime dataRecebimento { get; set; }
  public double valorParcela { get; set; }
  public double honorarios { get; set; }
  public double comissao { get; set; }
  public double comissaoRetido { get; set; }
  public double comissaoRepasse { get; set; }
  public double juros { get; set; }
  public int atrasos { get; set; }
  public double valorMulta { get; set; }
  public double multaRetido { get; set; }
  public double multaRepasse { get; set; }
  public double valorCustas { get; set; }
  public double valorEncargos { get; set; }
  public double valorRestante { get; set; }
  public double valorDesconto { get; set; }
  public double jurosRetido { get; set; }
  public double jurosRepasse { get; set; }
  public double honorariosRepasse { get; set; }
  public double honorariosRetido { get; set; }
  public bool alterado { get; set; }
  public bool first { get; set; }
  public bool? last { get; set; }
}

public class ACORDO
{
  public string COD_CLIENTE_CREDOR { get; set; }
  public string NUMERO_BORDERO { get; set; }

  public string DT_CADASTRO_BORDERO { get; set; }
  public string COD_CLIENTE_DEVEDOR { get; set; }
  public string FUNCIONARIO { get; set; }
  public string COD_FUNCIONARIO { get; set; }
  public int IND_TIPO_CALCULO { get; set; }
  public int TIPO_DOCUMENTO { get; set; }
  public double TAXA_COMISSAO_ACORDO { get; set; }
  public double TAXA_COMISSAO_REPASSE { get; set; }
  public double TAXA_COMISSAO_RETIDO { get; set; }
  public double TAXA_JUROS_ACORDO { get; set; }
  public double TAXA_JUROS_REPASSE { get; set; }
  public double TAXA_JUROS_RETIDO { get; set; }
  public double TAXA_HONORARIOS_REPASSE { get; set; }
  public double TAXA_HONORARIOS_RETIDO { get; set; }
  public double TAXA_HONORARIOS_ACORDO { get; set; }
  public double VALOR_PRINCIPAL { get; set; }
  public double VALOR_JUROS_RETIDO { get; set; }
  public double VALOR_JUROS_REPASSE { get; set; }
  public double VALOR_CUSTAS { get; set; }
  public double VALOR_HONORARIOS { get; set; }
  public double VALOR_TOTAL { get; set; }
  public double VALOR_COMISSAO { get; set; }
  public double VALOR_COMISSAO_RETIDO { get; set; }
  public double VALOR_COMISSAO_REPASSE { get; set; }
  public int QUANT_PARCELA_ACORDO { get; set; }
  public double VALOR_JUROS_TOTAL { get; set; }
  public double TAXA_MULTA { get; set; }
  public double TAXA_MULTA_REPASSE { get; set; }
  public double TAXA_MULTA_RETIDO { get; set; }
  public double VALOR_MULTA { get; set; }
  public double VALOR_MULTA_RETIDO { get; set; }
  public double VALOR_MULTA_REPASSE { get; set; }
  public double TAXA_CAMPANHA { get; set; }
  public double TAXA_CAMPANHA_DE { get; set; }
  public double TAXA_CAMPANHA_ATE { get; set; }
  public double VALOR_DESCONTO { get; set; }
  public List<PARCELA> PARCELAS { get; set; }
  public string TIPO_CALCULO { get; set; }
  public List<TITULO> TITULOS { get; set; }
}

public class AcordoRequest
{
  public ACORDO ACORDO { get; set; }
}