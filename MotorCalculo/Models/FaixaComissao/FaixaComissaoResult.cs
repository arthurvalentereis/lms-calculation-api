namespace Calculo.Core.Models.FaixaComissao;

public class FaixaComissaoResult
{
  public double valorDe { get; set; }
  public double valorAte { get; set; }
  public string atrasoDe { get; set; }
  public string atrasoAte { get; set; }
  public double honorarioMin { get; set; }
  public double honorarioMax { get; set; }
  public double jurosMin { get; set; }
  public double jurosMax { get; set; }
  public double honorariosRepasse { get; set; }
  public double jurosRepasse { get; set; }
  public double comissaoMin { get; set; }
  public double comissaoMax { get; set; }
  public double multa { get; set; }
  public double multaRepasse { get; set; }
  public int limiteParcelamento { get; set; }
  public double desconto { get; set; }
  public double descontoMin { get; set; }
  public double campanhaDe { get; set; }
  public double campanhaAte { get; set; }
  public string obsComissao { get; set; }
  public bool carregaMaximas { get; set; }
  public OrigemTaxas origemTaxas { get; set; }
  public TipoFaixaComissao tipoFaixaComissao { get; set; }
  public TipoCalculo tipoCalculo { get; set; }


}

public class CarregaTaxasMaximasResult
{
  public string maximaNormal { get; set; }
  public string maximaPD { get; set; }
  public string maximaPDA { get; set; }
}