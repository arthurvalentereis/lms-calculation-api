using Calculo.Core.Models.Taxa;

namespace Calculo.Core.Calculo;

public class ParcelaResultado
{
  public ParcelaResultado(Parcela parcela, Taxas taxa)
  {
    Parcela = parcela;
    titulos = new List<TituloResultado>();
    valorRestante = parcela.ValorReal(taxa);
    this.taxa = taxa;
  }
  public List<TituloResultado> titulos { get; set; }
  public decimal ValorJuros { get; set; }
  public decimal ValorJurosRetido { get; set; }
  public decimal ValorJurosRepasse { get; set; }
  public decimal ValorHonorariosRetido { get; set; }
  public decimal ValorHonorariosRepasse { get; set; }
  public decimal ValorMulta { get; set; }
  public decimal ValorCustas { get; set; }
  public Parcela Parcela { get; set; }

  public decimal valorRestante { get; set; }
  private Taxas taxa;

  public decimal Comissao
  {
    get
    {
      return (titulos.Sum(p => p.valor) + titulos.Sum(p => p.valorJurosDoTitulo) + titulos.Sum(p => p.valorCustasDoTitulo) + titulos.Sum(p => p.valorMultaDoTitulo) + titulos.Sum(p => p.valorHonorarioDoTitulo)) * taxa.GetValue(Taxa.comissao) / 100;
    }
  }



  public decimal AbaterValores(decimal valorCheio, Taxa? taxaEnum, Titulo titulo)
  {

    if (valorCheio > valorRestante)
    {
      throw new Exception("Valor não pode ser maior que o restante");

    }
    decimal vRes = valorCheio;
    var tit = titulos.Find(p => p.titulo.nomeTitulo == titulo.nomeTitulo);
    if (titulo.valorHonorarioRestante > 0)
    {
      var hon = taxa.GetValue(Taxa.honorario);
      var valorHonorario = Math.Round(valorCheio * hon / 100, 2);
      valorHonorario = valorHonorario > titulo.valorHonorarioRestante ? titulo.valorHonorarioRestante : valorHonorario;
      if (valorRestante - (valorCheio + valorHonorario) >= 0)
      {
        vRes = Math.Round(valorCheio + valorHonorario, 2);
      }
      else
      {
        var honRestante = Math.Round(valorRestante * hon / 100, 2);
        if (honRestante >= titulo.valorHonorarioRestante)
        {
          valorCheio = Math.Round(valorRestante - titulo.valorHonorarioRestante, 2);
          vRes = valorRestante;
          valorHonorario = titulo.valorHonorarioRestante;
        }
        else
        {
          valorCheio = Math.Round(valorRestante - honRestante, 2);
          vRes = valorRestante;
          valorHonorario = honRestante;
        }
      }

      tit.valorHonorarioDoTitulo = Math.Round(tit.valorHonorarioDoTitulo + valorHonorario, 2);
      titulo.valorHonorarioRestante = Math.Round(titulo.valorHonorarioRestante - valorHonorario, 2);
    }

    if (titulo.valorComissaoRestante > 0)
    {
      var com = taxa.GetValue(Taxa.comissao);
      var valorComissao = Math.Round(valorCheio * com / 100, 2);
      valorComissao = valorComissao > titulo.valorComissaoRestante ? titulo.valorComissaoRestante : valorComissao;

      tit.valorComisssaoDoTitulo = Math.Round(tit.valorComisssaoDoTitulo + valorComissao, 2);
      titulo.valorComissaoRestante = Math.Round(titulo.valorComissaoRestante - valorComissao, 2);

    }
    valorRestante = Math.Round(valorRestante - vRes, 2);
    switch (taxaEnum)
    {
      case Taxa.juros:
        {
          tit.valorJurosDoTitulo = Math.Round(tit.valorJurosDoTitulo + valorCheio, 2);
          titulo.valorJurosRestante = Math.Round(titulo.valorJurosRestante - valorCheio, 2);
        }
        break;
      case Taxa.multa:
        {
          tit.valorMultaDoTitulo = Math.Round(tit.valorMultaDoTitulo + valorCheio, 2);
          titulo.valorMultaRestante = Math.Round(titulo.valorMultaRestante - valorCheio, 2);
        }
        break;
      case Taxa.custas:
        {
          tit.valorCustasDoTitulo = Math.Round(tit.valorCustasDoTitulo + valorCheio, 2);
          titulo.valorCustasRestante = Math.Round(titulo.valorCustasRestante - valorCheio, 2);
        }
        break;
      default:
        {
          tit.valor = Math.Round(valorCheio, 2);
          titulo.valorRestante = Math.Round(titulo.valorRestante - valorCheio, 2);
        }
        break;
    }

    return valorRestante;

  }

}