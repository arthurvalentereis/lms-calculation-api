using System.Collections.ObjectModel;

namespace Calculo.Core.Calculo;

public class AcordoVM
{
  public AcordoVM()
  {
    Titulos = new Collection<TituloVM>();
    ParcelasPagamento = new Collection<ParcelaPagamentoVM>();
    ParcelasAcordo = new Collection<ParcelaAcordoVM>();
  }

  public int AcordoId { get; set; }
  public DateTime DataAcordo { get; set; }
  public decimal PercentualJuros { get; set; }
  public decimal PercentualJurosRepasse { get; set; }
  public decimal PercentualMultaRepasse { get; set; }
  public decimal PercentualHonorario { get; set; }
  public decimal PercentualComissao { get; set; }
  public decimal PercentualHonorarioRepasse { get; set; }
  public decimal PercentualComissaoRepasse { get; set; }
  public int NumeroParcelas { get; set; }
  public int NumeroRecorrencia { get; set; }
  public bool ParcelasIguais { get; set; }
  public decimal ValorParcela { get; set; }
  public decimal PercentualMulta { get; set; }
  public decimal PercentualDesconto { get; set; }
  public TituloVM? NewTitulo { get; set; }
  public ParcelaAcordoVM? NewParcelaAcordo { get; set; }
  public ICollection<TituloVM> Titulos { get; set; }
  public ICollection<ParcelaPagamentoVM> ParcelasPagamento { get; set; }
  public ICollection<ParcelaAcordoVM> ParcelasAcordo { get; set; }
  public string? Mensagem { get; set; }
}

public class TituloVM
{
  public string TituloId { get; set; }
  public decimal ValorPrincipal { get; set; }
  public decimal ValorCustas { get; set; }
  public decimal PercentualMulta { get; set; }
  public decimal PercentualDesconto { get; set; }
  public decimal ValorRecebidoPrincipal { get; set; }
  public decimal ValorRecebidoCustas { get; set; }
  public decimal ValorRecebidoMulta { get; set; }
  public decimal ValorMulta { get; set; }
  public decimal ValorDesconto { get; set; }
  public DateTime DataVencimento { get; set; }
}

public class ParcelaPagamentoVM
{
  public int ParcelaPagamentoId { get; set; }
  public TituloVM Titulo { get; set; }
  public DateTime DataPagamento { get; set; }
  public int NumeroParcela { get; set; }
  public decimal ValorParcela { get; set; }
  public decimal ValorPrincipal { get; set; }
  public decimal ValorDesconto { get; set; }
  public decimal ValorCustas { get; set; }
  public decimal ValorMulta { get; set; }
  public decimal ValorMultaRepasse { get; set; }
  public decimal ValorMultaRetido { get; set; }
  public decimal SaldoTitulo { get; set; }
  public decimal ValorJuros { get; set; }
  public decimal ValorJurosRepasse { get; set; }
  public decimal ValorJurosRetido { get; set; }
  public decimal ValorHonorariosRetido { get; set; }
  public decimal ValorHonorariosRepasse { get; set; }
  public decimal ValorHonorario { get; set; }
  public decimal ValorComissaoRetido { get; set; }
  public decimal ValorComissaoRepasse { get; set; }
  public decimal ValorComissao { get; set; }
  public decimal ValorEncargos { get; set; }
  public long DiasAtraso { get; set; }
}

public class ParcelaAcordoVM
{
  public int ParcelaAcordoId { get; set; }
  public DateTime Data { get; set; }
  public bool DataFixa { get; set; }
  public decimal Valor { get; set; }
  public bool ValorFixo { get; set; }
  public bool Feriado { get; set; }
}