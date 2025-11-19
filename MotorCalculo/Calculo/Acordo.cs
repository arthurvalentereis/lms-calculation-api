using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculo.Core.Calculo
{
  public class Acordo
  {
    public int AcordoId { get; }
    public decimal PercentualHonorario { get; set; }
    public decimal PercentualComissao { get; set; }
    public decimal PercentualJuros { get; set; }
    public decimal PercentualMulta { get; set; }
    public decimal PercentualDesconto { get; set; }
    public decimal PercentualJurosReceita { get; set; }
    public decimal PercentualJurosMinimo { get; set; }
    public int NumeroParcelas { get; set; }
    public int NumeroRecorrencia { get; set; }
    public DateTime Data { get; set; }
    public bool ParcelasIguais { get; set; }
    public ICollection<ParcelaAcordo> ParcelasAcordo { get; set; }
    public ICollection<TituloCalculo> Titulos { get; set; }
    public List<string> ValidationErrors { get; }

    public decimal SaldoParcelas
    {
      get
      {
        return ParcelasAcordo.Sum(x => x.Saldo);
      }
    }

    public decimal TaxaJurosDiaria
    {
      get
      {
        return Decimal.Round((PercentualJuros / 30 / 100), 9);
      }
    }

    public decimal TaxaHonorario
    {
      get
      {
        return (PercentualHonorario / 100);
      }
    }
    public decimal TaxaComissao
    {
      get
      {
        return (PercentualComissao / 100);
      }
    }

    public decimal SaldoTitulos
    {
      get
      {
        return Titulos.Sum(x => x.SaldoTotal);
      }
    }

    public Acordo(DateTime DataAcordo, decimal PercentualJuros, decimal PercentualHonorario, decimal PercentualMulta, decimal PercentualDesconto, int numeroParcelas, bool parcelasIguais, int numeroRecorrencia = 30)
    {
      this.Data = DataAcordo;
      this.PercentualJuros = PercentualJuros;
      this.PercentualHonorario = PercentualHonorario;
      this.PercentualMulta = PercentualMulta;
      this.PercentualDesconto = PercentualDesconto;
      this.NumeroParcelas = numeroParcelas;
      this.NumeroRecorrencia = numeroRecorrencia;
      this.ParcelasIguais = parcelasIguais;
      ParcelasAcordo = new Collection<ParcelaAcordo>();
      Titulos = new Collection<TituloCalculo>();
      ValidationErrors = new List<string>();
    }

    public void AddParcelaAcordo(int numParcela, DateTime Data, decimal Valor, bool DataFixa = false, bool ValorFixo = false)
    {
      //int numParcela = ParcelasAcordo.Count() + 1;
      ParcelaAcordo parcela = new ParcelaAcordo(numParcela, Data, Valor, this, DataFixa, ValorFixo);
      ParcelasAcordo.Add(parcela);
    }

    public void AddParcelaAcordo(ParcelaAcordo parcelaAcordo)
    {
      ParcelasAcordo.Add(parcelaAcordo);
    }

    public void AddTitulo(TituloCalculo titulo)
    {
      this.Titulos.Add(titulo);
    }

    public void AddTitulo(string TituloId, decimal ValorPrincipal, decimal ValorCustas, DateTime DataVencimento)
    {
      TituloCalculo titulo = new TituloCalculo(TituloId, ValorPrincipal, ValorCustas, this.PercentualMulta, DataVencimento, this.PercentualDesconto);
      this.Titulos.Add(titulo);
    }

    public void AddTitulo(string TituloId, decimal ValorPrincipal, decimal ValorCustas, DateTime DataVencimento, decimal ValorRecebidoPrincipal, decimal ValorRecebidoCustas, decimal ValorRecebidoMulta)
    {
      TituloCalculo titulo = new TituloCalculo(TituloId, ValorPrincipal, ValorCustas, this.PercentualMulta, DataVencimento, this.PercentualDesconto, ValorRecebidoPrincipal, ValorRecebidoCustas, ValorRecebidoMulta);
      this.Titulos.Add(titulo);
    }

    public ParcelaAcordo getParcelaAcordoComSaldo()
    {
      return ParcelasAcordo.Where(x => x.Saldo > 0).OrderBy(x => x.ParcelaAcordoId).FirstOrDefault();
    }

    public List<ParcelaPagamento> getParcelasPagamento()
    {
      var parcelasPagamento = new Collection<ParcelaPagamento>();

      foreach (var parcelaAcordo in ParcelasAcordo)
      {
        foreach (var parcelaPagamento in parcelaAcordo.ParcelasPagamento)
        {
          parcelasPagamento.Add(parcelaPagamento);
        }
      }

      return parcelasPagamento.ToList();
    }

    public bool IsValid()
    {
      this.Validate();
      return (ValidationErrors.Count == 0);
    }

    private void Validate()
    {
      ValidationErrors.Clear();

      if (this.SaldoTitulos > (decimal)0.10)
      {
        ValidationErrors.Add("As parcelas informadas não são insuficientes para quitar os títulos do acordo.");
      }

      if (ParcelasAcordo.Sum(x => x.SomaEncargos) > 1)
      {
        ValidationErrors.Add("As parcelas informadas são superiores ao total dos títulos e taxas do acordo, cosidere diminuir o valor das parcelas para que caibam na negociação.");
      }
    }
  }
}
