using System.Collections.ObjectModel;

namespace Calculo.Core.Calculo
{
  public class TituloCalculo
  {
    public string TituloId { get; }
    public decimal ValorPrincipal { get; }
    public decimal ValorCustas { get; }
    public decimal PercentualMulta { get; }
    public decimal PercentualDesconto { get; }
    public decimal ValorRecebidoPrincipal { get; }
    public decimal ValorRecebidoCustas { get; }
    public decimal ValorRecebidoMulta { get; }
    public DateTime DataVencimento { get; }
    public ICollection<ParcelaPagamento> ParcelasPagamento { get; set; }

    public decimal ValorMulta
    {
      get
      {
        return Decimal.Round((ValorPrincipal - ValorDesconto) * PercentualMulta / 100, 2);
      }
    }

    public decimal ValorDesconto
    {
      get
      {
        return Decimal.Round(ValorPrincipal * PercentualDesconto / 100, 2); ;
      }
    }

    public decimal SaldoPrincipal
    {
      get
      {
        return (ValorPrincipal - ValorDesconto - ValorRecebidoPrincipal - ParcelasPagamento.Sum(x => x.ValorPrincipal));
      }
    }
    public decimal SaldoCustas
    {
      get
      {
        return (ValorCustas - ValorRecebidoCustas - ParcelasPagamento.Sum(x => x.ValorCustas));
      }
    }
    public decimal SaldoMulta
    {
      get
      {
        return (ValorMulta - ValorRecebidoMulta - ParcelasPagamento.Sum(x => x.ValorMulta));
      }
    }
    public decimal SaldoTotal
    {
      get
      {
        return SaldoPrincipal + SaldoCustas + SaldoMulta;
      }
    }

    public TituloCalculo(string TituloId, decimal ValorPrincipal, decimal ValorCustas, decimal PercentualMulta, DateTime DataVencimento, decimal PercentualDesconto)
    {
      this.TituloId = TituloId;
      this.DataVencimento = DataVencimento;
      this.ValorPrincipal = ValorPrincipal;
      this.ValorCustas = ValorCustas;
      this.PercentualMulta = PercentualMulta;
      this.PercentualDesconto = PercentualDesconto;
      ParcelasPagamento = new Collection<ParcelaPagamento>();
    }

    public TituloCalculo(string TituloId, decimal ValorPrincipal, decimal ValorCustas, decimal PercentualMulta, DateTime DataVencimento, decimal PercentualDesconto, decimal ValorRecebidoPrincipal, decimal ValorRecebidoCustas, decimal ValorRecebidoMulta)
        : this(TituloId, ValorPrincipal, ValorCustas, PercentualMulta, DataVencimento, PercentualDesconto)
    {
      this.ValorRecebidoPrincipal = ValorRecebidoPrincipal;
      this.ValorRecebidoCustas = ValorRecebidoCustas;
      this.ValorRecebidoMulta = ValorRecebidoMulta;
    }

    public void AddParcelaPagamento(ParcelaPagamento parcelaPagemnto)
    {
      this.ParcelasPagamento.Add(parcelaPagemnto);
    }
  }
}
