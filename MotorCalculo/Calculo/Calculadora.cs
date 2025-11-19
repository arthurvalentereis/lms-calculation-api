using System.Diagnostics;
using Calculo.Core.Models.Taxa;

namespace Calculo.Core.Calculo;

public class Calculadora
{

  public Parcela[] GerarParcelas(Titulo[] titulos, Taxas taxa, DateTime dataPrimeiroVencimento, int PeriodicidadeEmDias, int quantidadeParcela)
  {
    List<Parcela> parcelas = new List<Parcela>();

    decimal valorJuros = 0;
    decimal valorPrincipal = 0;
    decimal valorHonorarios = 0;
    decimal valorMulta = 0;
    decimal valorCustas = 0;
    foreach (var item in titulos)
    {

      item.SetDataParcelaFinal(DateTime.Now.AddDays(PeriodicidadeEmDias * (quantidadeParcela - 1)).Date);

      item.ObterValorDasTaxas();

      Debug.WriteLine("##########################");
      Debug.WriteLine(item.valorJurosRestante);
      Debug.WriteLine(item.valorMultaRestante);
      Debug.WriteLine(item.valorHonorarioRestante);
      Debug.WriteLine(item.valorCustasRestante);

      valorJuros = Math.Round(valorJuros + item.valorJurosRestante, 2);
      valorPrincipal = Math.Round(valorPrincipal + item.ValorTotal(), 2);
      valorHonorarios = Math.Round(valorHonorarios + item.valorHonorarioRestante, 2);
      valorMulta = Math.Round(valorMulta + item.valorMultaRestante, 2);
      valorCustas = Math.Round(valorCustas + item.valorCustasRestante, 2);

    }
    // valorHonorarios
    var valorTotalCalculado = Math.Round(valorPrincipal + valorMulta + valorJuros + valorCustas + valorHonorarios, 2);

    var valorUnitarioParcela = Math.Round(valorTotalCalculado / quantidadeParcela, 2);

    for (int i = 0; i < quantidadeParcela; i++)
    {
      var parcela = new Parcela();
      parcela.dtVencimento = dataPrimeiroVencimento.AddDays(PeriodicidadeEmDias * i);
      parcela.numeroParcela = i;
      parcela.valor = valorUnitarioParcela;
      parcela.valorRestante = valorUnitarioParcela;
      parcelas.Add(parcela);
    }

    return parcelas.OrderBy(p => p.numeroParcela).ToArray();

  }

  public Parcela[] GerarAtualizacaoParcelas(Titulo[] titulos, Taxas taxa, DateTime dataPrimeiroVencimento, List<Parcela> parcelasGeradas)
  {
    List<Parcela> parcelas = new List<Parcela>();

    decimal valorJuros = 0;
    decimal valorPrincipal = 0;
    decimal valorHonorarios = 0;
    decimal valorMulta = 0;
    decimal valorCustas = 0;
    foreach (var item in titulos)
    {

      item.SetDataParcelaFinal(parcelasGeradas[parcelasGeradas.Count - 1].dtVencimento);

      item.ObterValorDasTaxas();

      valorJuros = Math.Round(valorJuros + item.valorJurosRestante, 2);
      valorPrincipal = Math.Round(valorPrincipal + item.ValorTotal(), 2);
      valorHonorarios = Math.Round(valorHonorarios + item.valorHonorarioRestante, 2);
      valorMulta = Math.Round(valorMulta + item.valorMultaRestante, 2);
      valorCustas = Math.Round(valorCustas + item.valorCustasRestante, 2);

    }
    // valorHonorarios
    var valorTotalCalculado = Math.Round(valorPrincipal + valorMulta + valorJuros + valorCustas + valorHonorarios, 2);

    var valorUnitarioParcela = Math.Round(valorTotalCalculado / parcelasGeradas.Count, 2);

    int i = 0;
    foreach (var items in parcelasGeradas)
    {
      var parcela = new Parcela();
      parcela.dtVencimento = items.dtVencimento;
      parcela.numeroParcela = i;
      parcela.valor = valorUnitarioParcela;
      parcela.valorRestante = valorUnitarioParcela;
      parcelas.Add(parcela);
      i++;
    }

    return parcelas.OrderBy(p => p.numeroParcela).ToArray();

  }

  public ResultadoCalc CalcParcela(Parcela[] parcelas, Titulo[] titulos, int i, int j, ResultadoCalc res, Taxas taxa)
  {

    if (i < 0)
    {
      return res;
    }

    if (res.resTitulos.Count == 0)
    {
      CriarResTitulosParcela(res, parcelas, taxa);
    }
    var parcela = parcelas[i];


    var titulo = titulos[j];
    if (titulo.valorRestante == 0 && j < titulos.Length - 1 && titulo.valorJurosRestante == 0 && titulo.valorCustasRestante == 0 && titulo.valorMultaRestante == 0)
    {
      return CalcParcela(parcelas, titulos, i, j + 1, res, taxa);
    }

    var real = res.ObterValorRealParcela(parcela, taxa);
    decimal newRestante = real;

    if (titulo.valorRestante != 0)
    {
      if (real >= titulo.valorRestante)
      {
        newRestante = res.AbaterTituloDaParcela(parcela, titulo, taxa);
      }
      else
      {
        newRestante = res.AdicionarTituloAParcela(titulo, parcela, taxa);
      }
    }

    if (titulo.valorRestante == 0 && newRestante > 0 && titulo.valorJurosRestante > 0)
    {
      newRestante = res.AbaterJurosDaParcela(parcela, titulo, taxa);
    }

    if (titulo.valorRestante == 0 && newRestante > 0 && titulo.valorCustasRestante > 0)
    {
      newRestante = res.AbaterCustasDaParcela(parcela, titulo, taxa);
    }

    if (titulo.valorRestante == 0 && newRestante > 0 && titulo.valorMultaRestante > 0)
    {
      newRestante = res.AbaterMultaDaParcela(parcela, titulo, taxa);
    }

    if (newRestante <= 0)
    {

      return CalcParcela(parcelas, titulos, i - 1, j, res, taxa);

    }
    else if (j < titulos.Length - 1)
    {
      return CalcParcela(parcelas, titulos, i, j + 1, res, taxa);
    }
    else
    {
      return CalcParcela(parcelas, titulos, i - 1, 0, res, taxa);
    }

  }

  private void CriarResTitulosParcela(ResultadoCalc res, Parcela[] parcelas, Taxas taxa)
  {
    foreach (var parcela in parcelas)
    {
      var par = new ParcelaResultado(parcela, taxa);
      res.resTitulos.Add(par);
    }
  }
}