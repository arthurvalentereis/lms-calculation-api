using Calculo.Core.Models.Taxa;

namespace Calculo.Core.Calculo;

public class ResultadoCalc
{
  public ResultadoCalc()
  {
    resTitulos = new List<ParcelaResultado>();
  }

  public int Periodicidade { get; set; }
  public int quantidadeParcela { get; set; }
  public List<ParcelaResultado> resTitulos { get; set; }

  public decimal AdicionarTituloAParcela(Titulo titulo, Parcela parcela, Taxas taxa)
  {
    var par = resTitulos.Find(p => p.Parcela != null && p.Parcela.numeroParcela == parcela.numeroParcela);

    var d = titulo.valorRestante - par.valorRestante;

    TituloResultado tituloResultado = new TituloResultado(titulo, par.valorRestante);

    par.titulos.Add(tituloResultado);
    return par.AbaterValores(par.valorRestante, null, titulo);


  }

  public decimal AbaterTituloDaParcela(Parcela parcela, Titulo titulo, Taxas taxa)
  {
    var par = resTitulos.Find(p => p.Parcela != null && p.Parcela.numeroParcela == parcela.numeroParcela);

    var d = titulo.valorRestante;



    TituloResultado tituloResultado = new TituloResultado(titulo, d);
    tituloResultado.parcelaFinal = parcela.numeroParcela;
    titulo.parcelaFinal = parcela.numeroParcela;

    par.titulos.Add(tituloResultado);

    return par.AbaterValores(d, null, titulo);



  }

  public decimal ObterValorRealParcela(Parcela parcela, Taxas taxa)
  {
    var par = resTitulos.Find(p => p.Parcela != null && p.Parcela.numeroParcela == parcela.numeroParcela);

    return Math.Round(par.valorRestante, 2);

  }


  public decimal AbaterCustasDaParcela(Parcela parcela, Titulo titulo, Taxas taxa)
  {
    var parc = resTitulos.Find(p => p.Parcela.numeroParcela == parcela.numeroParcela);
    var totalCustas = titulo.valorCustasRestante;
    var x = parc.titulos.Find(p => p.titulo.nomeTitulo == titulo.nomeTitulo);
    if (x == null)
    {
      TituloResultado tituloResultado = new TituloResultado(titulo, titulo.valorRestante);
      parc.titulos.Add(tituloResultado);
    }

    if (parc.valorRestante > 0 && totalCustas > 0)
    {

      if (totalCustas > parc.valorRestante)
      {
        parc.AbaterValores(parc.valorRestante, Taxa.custas, titulo);


      }
      else
      {
        parc.AbaterValores(totalCustas, Taxa.custas, titulo);


      }
    }
    return parc.valorRestante;

  }


  public decimal AbaterMultaDaParcela(Parcela parcela, Titulo titulo, Taxas taxa)
  {
    var parc = resTitulos.Find(p => p.Parcela.numeroParcela == parcela.numeroParcela);
    var totalMulta = titulo.valorMultaRestante;
    var x = parc.titulos.Find(p => p.titulo.nomeTitulo == titulo.nomeTitulo);
    if (x == null)
    {
      TituloResultado tituloResultado = new TituloResultado(titulo, titulo.valorRestante);
      parc.titulos.Add(tituloResultado);
    }


    if (parc.valorRestante > 0)
    {

      if (totalMulta > parc.valorRestante)
      {
        parc.AbaterValores(parc.valorRestante, Taxa.multa, titulo);

      }
      else
      {
        parc.AbaterValores(totalMulta, Taxa.multa, titulo);

      }
    }
    return parc.valorRestante;
  }

  public decimal AbaterJurosDaParcela(Parcela parcela, Titulo titulo, Taxas taxa)
  {

    var parc = resTitulos.Find(p => p.Parcela.numeroParcela == parcela.numeroParcela);

    var totalJuros = titulo.valorJurosRestante;
    var x = parc.titulos.Find(p => p.titulo.nomeTitulo == titulo.nomeTitulo);
    if (x == null)
    {
      TituloResultado tituloResultado = new TituloResultado(titulo, titulo.valorRestante);
      parc.titulos.Add(tituloResultado);

    }


    if (totalJuros > 0)
    {

      if (totalJuros > parc.valorRestante)
      {
        parc.AbaterValores(parc.valorRestante, Taxa.juros, titulo);
      }
      else
      {
        parc.AbaterValores(totalJuros, Taxa.juros, titulo);
      }

    }

    return parc.valorRestante;
  }

  public bool ExisteParcelaNegativa()
  {
    return resTitulos.Exists(p => p.valorRestante < 0);
  }

  public bool ExisteParcelaPositiva()
  {
    return resTitulos.Exists(p => p.valorRestante > 0);
  }

}