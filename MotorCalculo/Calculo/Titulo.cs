using Calculo.Core.Models.Taxa;

namespace Calculo.Core.Calculo;

public class Titulo
{
  public Titulo(int id, decimal valor, int diasAtraso, string nomeTitulo, DateTime DtVencimentoTitulo, decimal valorCustas, Taxas taxa)
  {
    Id = id;
    this.taxa = taxa;
    DiasAtraso = diasAtraso;
    this.DtVencimentoTitulo = DtVencimentoTitulo;

    Valor = ObterValorDesconto(valor);
    valorRestante = ValorTotal();
    this.nomeTitulo = nomeTitulo;
    this.valorCustas = valorCustas;
    valorCustasRestante = valorCustas;

  }

  private decimal ObterValorDesconto(decimal valorTotal)
  {
    var valorDesconto = taxa.valores.First(p => p.Key == Taxa.desconto).Value;
    decimal valor = 0;
    if (valorDesconto > 0)
    {
      valor = Math.Round(valorTotal * valorDesconto / 100, 2);
      this.valorDesconto = valor;
    }

    return valorTotal - valor;
  }

  public int Id { get; set; }
  public string nomeTitulo { get; set; }
  private decimal Valor { get; set; }
  public int DiasAtraso { get; set; }
  public DateTime DtVencimentoTitulo { get; set; }
  public decimal valorRestante { get; set; }
  public decimal valorJurosRestante { get; set; }
  public decimal valorCustasRestante { get; set; }
  public decimal valorMultaRestante { get; set; }
  public decimal valorHonorarioRestante { get; set; }
  public decimal valorComissaoRestante { get; set; }
  public decimal valorDesconto { get; set; }

  private decimal valorCustas;
  private Taxas taxa;

  public virtual decimal ValorTotal()
  {
    return Valor;
  }

  public decimal ObterValorHonorarioAtual()
  {
    var valorSubtotal = Valor + valorJurosRestante + valorMultaRestante + valorCustasRestante;
    var honorarios = taxa.GetValue(Taxa.honorario);
    var valorHonorarios = Math.Round(valorSubtotal * honorarios / 100, 2);
    return valorHonorarios;
  }
  public decimal ValorTotalDeJuros(DateTime dtVencimentoParcela, Taxas taxas)
  {
    var totalDiasAtraso = TotalDiasAtrasos(dtVencimentoParcela);
    var valorJuros = taxas.valores.First(p => p.Key == Taxa.juros).Value;
    var valorJurosRetido = taxas.valores.FirstOrDefault(p => p.Key == Taxa.jurosRetido).Value;
    return Math.Round(totalDiasAtraso * ((valorJuros + valorJurosRetido) / 30) * Valor / 100, 2);
  }

  public int TotalDiasAtrasos(DateTime dtVencimentoParcela)
  {
    DateTime dtTitulo = DateTime.Now.AddDays(-(DiasAtraso + 1));
    var totalDiasAtraso = dtVencimentoParcela.Subtract(dtTitulo).Days;
    return totalDiasAtraso;
  }

  private DateTime _dataParcelaFinal;

  public void SetDataParcelaFinal(DateTime dtVencimentoParcela)
  {
    _dataParcelaFinal = dtVencimentoParcela;
  }
  private List<ValorTaxa> valorTaxa;
  public int parcelaFinal;

  public List<ValorTaxa> ObterValorDasTaxas()
  {
    if (valorTaxa == null)
    {
      valorTaxa = new List<ValorTaxa>();

      var valorJuros = ValorTotalDeJuros(_dataParcelaFinal, taxa);
      valorTaxa.Add(new ValorTaxa() { taxa = Taxa.juros, valor = valorJuros });

      var multa = taxa.GetValue(Taxa.multa);
      var valorMulta = Valor * (DiasAtraso * multa / 30) / 100;
      valorTaxa.Add(new ValorTaxa() { taxa = Taxa.multa, valor = valorMulta });


      var valorSubtotal = Valor + valorJuros + valorMulta + valorCustas;

      var comissao = taxa.GetValue(Taxa.comissao);
      var valorComissao = Math.Round(valorSubtotal * comissao / 100, 2);
      valorTaxa.Add(new ValorTaxa() { taxa = Taxa.comissao, valor = valorComissao });

      var honorarios = taxa.GetValue(Taxa.honorario);
      var valorHonorarios = Math.Round(valorSubtotal * honorarios / 100, 2);
      valorTaxa.Add(new ValorTaxa() { taxa = Taxa.honorario, valor = valorHonorarios });

      valorJurosRestante = valorJuros;
      valorMultaRestante = valorMulta;
      valorHonorarioRestante = valorHonorarios;
      valorComissaoRestante = valorComissao;

      return valorTaxa;

    }
    else
    {
      return valorTaxa;
    }
  }
}