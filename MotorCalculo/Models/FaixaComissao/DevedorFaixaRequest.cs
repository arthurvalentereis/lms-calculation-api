namespace Calculo.Core.Models.FaixaComissao
{
  public class DevedorFaixaRequest
  {
    public string codClienteDevedor { get; set; }
    public string codClienteCredor { get; set; }
    public string numeroBordero { get; set; }
    public TipoCalculo TipoCalculo { get; set; }
  }
}
