namespace Calculo.Core.Models.Taxa
{
  public class TaxaModel
  {
    public decimal honorario { get; set; }

    public decimal juros { get; set; }
    public decimal comissao { get; set; }
    public decimal multa { get; set; }
    public decimal multaRetida { get; set; }
    public decimal custas { get; set; }
    public decimal desconto { get; set; }
    public decimal jurosRetido { get; set; }
  }
}
