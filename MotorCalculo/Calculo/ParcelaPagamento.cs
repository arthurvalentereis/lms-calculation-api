namespace Calculo.Core.Calculo;

public class ParcelaPagamento
{
  public int ParcelaPagamentoId { get; set; }
  public ParcelaAcordo ParcelaAcordo { get; private set; }
  public TituloCalculo Titulo { get; private set; }
  public DateTime DataPagamento { get; private set; }
  public int NumeroParcela { get; set; }
  public decimal ValorParcela { get; private set; }
  public decimal ValorPrincipal { get; private set; }
  public decimal ValorCustas { get; private set; }
  public decimal ValorComissao { get; private set; }
  public decimal ValorRepasse { get; private set; }
  public decimal ValorMulta { get; private set; }
  public decimal SaldoTitulo { get; private set; }

  /// <summary>
  /// Retorna o valor dos juros da parcela de pagamento do título.
  /// </summary>
  /// <remarks>
  /// No cálculo, os juros incidem somente sobre o principal.
  /// </remarks>
  public decimal ValorJuros
  {
    get
    {
      decimal MontanteBase;
      MontanteBase = ValorPrincipal; // + ValorCustas;

      return getValorDosJuros(MontanteBase);
    }
  }

  /// <summary>
  /// Retorna o valor dos honorários da parcela de pagamento do título.
  /// </summary>
  public decimal ValorHonorario
  {
    get
    {
      decimal MontanteBase;
      MontanteBase = ValorPrincipal + ValorCustas + ValorJuros + ValorMulta;

      //return CalculoService.RoundBasedOnThirdDecimal((MontanteBase * ParcelaAcordo.Acordo.TaxaHonorario));

      return Decimal.Round(((MontanteBase * ParcelaAcordo.Acordo.TaxaHonorario) - new Decimal(0.004)), 2, MidpointRounding.AwayFromZero);
    }
  }

  /// <summary>
  /// Retorna o valor dos encargos da parcela de pagamento do título.
  /// </summary>
  public decimal ValorEncargos
  {
    get
    {
      return (ValorParcela - (ValorPrincipal + ValorJuros + ValorCustas + ValorMulta + ValorHonorario));
    }
  }

  /// <summary>
  /// Retorna o número de dias de atrado da parcela de pagamento com relação do vencimento do título.
  /// </summary>
  public long DiasAtraso
  {
    get
    {
      long Dias = (DataPagamento - Titulo.DataVencimento).Days;
      return (Dias > 0) ? Dias : 0;
    }
  }

  public decimal SomaPrincipais
  {
    get
    {
      return (ValorPrincipal + ValorCustas + ValorMulta);
    }
  }

  public ParcelaPagamento(int parcelaPagamentoId, ParcelaAcordo parcelaAcordo, TituloCalculo titulo)
  {
    this.ParcelaPagamentoId = parcelaPagamentoId;
    this.ParcelaAcordo = parcelaAcordo;
    this.Titulo = titulo;
    this.DataPagamento = this.ParcelaAcordo.Data;
    this.NumeroParcela = this.Titulo.ParcelasPagamento.Count() + 1;
  }

  /// <summary>
  /// Realiza o cálculo do pagamento com base em um valor de parcela informado.
  /// </summary>
  /// <param name="valorParcela"></param>
  public void CalcularValorPrincipal(decimal valorParcela)
  {
    this.ValorParcela = valorParcela;
    DistribuirParcela();
  }

  /// <summary>
  /// Realiza o cálculo do valor de parcela de forma a quitar o saldo em débito título.
  /// </summary>
  public void CalcularValorParcela()
  {
    ValorPrincipal = Titulo.SaldoPrincipal;
    ValorCustas = Titulo.SaldoCustas;
    ValorMulta = Titulo.SaldoMulta;

    ProcessarValorParcela();
  }

  /// <summary>
  /// Realiza o cálculo do valor de parcela de forma a quitar um valorPrincipal informado.
  /// </summary>
  /// <param name="valorPrincipalPagamento"></param>
  public void CalcularValorParcela(decimal valorPrincipalPagamento)
  {

    var saldoPrincipal = valorPrincipalPagamento;
    ValorPrincipal = (Titulo.SaldoPrincipal > saldoPrincipal ? saldoPrincipal : Titulo.SaldoPrincipal);
    saldoPrincipal -= ValorPrincipal;

    ValorCustas = (Titulo.SaldoCustas > saldoPrincipal ? saldoPrincipal : Titulo.SaldoCustas);
    saldoPrincipal -= ValorCustas;

    ValorMulta = (Titulo.SaldoMulta > saldoPrincipal ? saldoPrincipal : Titulo.SaldoMulta);
    saldoPrincipal -= ValorMulta;


    ProcessarValorParcela();
  }

  private void ProcessarValorParcela()
  {
    //var ValorJurosTotal = getValorDosJuros(ValorPrincipal);
    //var MontanteBase = ValorPrincipal + ValorCustas + ValorMulta + ValorJurosTotal;
    //var ValorHonorarioMontanteBase = getValorDeHonorarios(MontanteBase);

    //ValorParcela = CalculoService.RoundBasedOnThirdDecimal(MontanteBase + ValorHonorarioMontanteBase);

    var ValorJurosTotal = getValorDosJuros(ValorPrincipal);
    var MontanteBase = ValorPrincipal + ValorCustas + ValorMulta + ValorJurosTotal;
    var ValorHonorarioMontanteBase = getValorDeHonorarios(MontanteBase);

    ValorParcela = Decimal.Round(MontanteBase + ValorHonorarioMontanteBase, 2);
  }

  public void AddValorResidual(decimal valorResidual)
  {
    //Define valor residual que será considerado nos encargos
    this.ValorParcela += valorResidual;
  }

  /// <summary>
  /// Distribui o valor da parcela entre principal, custas e multa, nesta ordem.
  /// </summary>
  private void DistribuirParcela()
  {
    var ValorDisponivelParaPrincipal = new Decimal(0.0);
    var ValorDisponivelParaCustas = new Decimal(0.0);
    var ValorDisponivelParaMulta = new Decimal(0.0);

    //Define valor disponível para abatimento da dívida
    var SaldoParaDistribuir = getValorSemHonorario(this.ValorParcela);

    if (SaldoParaDistribuir > 0)
    {
      var valorDisponivelSemPrincipal = SaldoParaDistribuir - getValorComJuros(Titulo.SaldoPrincipal);

      //Define o valor disponível para abatimento do principal
      if (valorDisponivelSemPrincipal > 0)
      {
        ValorDisponivelParaPrincipal = Titulo.SaldoPrincipal;

        SaldoParaDistribuir -= (ValorDisponivelParaPrincipal + getValorDosJuros(ValorDisponivelParaPrincipal));

        if (SaldoParaDistribuir > 0)
        {
          var ValorDisponivelSemCustas = SaldoParaDistribuir - Titulo.SaldoCustas;

          //Define o valor disponível para abatimento das custas
          if (ValorDisponivelSemCustas > 0)
          {
            ValorDisponivelParaCustas = Titulo.SaldoCustas;

            SaldoParaDistribuir -= ValorDisponivelParaCustas;

            //Define o valor disponível para abatimento da multa
            if (SaldoParaDistribuir > 0)
            {
              ValorDisponivelParaMulta = (SaldoParaDistribuir > Titulo.SaldoMulta ? Titulo.SaldoMulta : SaldoParaDistribuir);
            }
          }
          else
          {
            ValorDisponivelParaCustas = SaldoParaDistribuir;
          }
        }
      }
      else
      {
        ValorDisponivelParaPrincipal = getValorSemJuros(SaldoParaDistribuir);
      }
    }

    ValorPrincipal = ValorDisponivelParaPrincipal;
    ValorCustas = ValorDisponivelParaCustas;
    ValorMulta = ValorDisponivelParaMulta;
  }

  private decimal getValorSemHonorario(decimal valor)
  {
    //Retorna um valor livre de honorário
    //return CalculoService.RoundBasedOnThirdDecimal(valor / (1 + ParcelaAcordo.Acordo.TaxaHonorario));

    return (Decimal.Round(valor / (1 + ParcelaAcordo.Acordo.TaxaHonorario), 2));
  }

  private decimal getValorSemJuros(decimal valor)
  {
    //Retorna um valor livre de juros
    //return CalculoService.RoundBasedOnThirdDecimal(valor / (1 + ParcelaAcordo.Acordo.TaxaJurosDiaria * DiasAtraso));

    return (Decimal.Round(valor / (1 + ParcelaAcordo.Acordo.TaxaJurosDiaria * DiasAtraso) - new Decimal(0.000), 2, MidpointRounding.ToEven));
  }

  private decimal getValorComJuros(decimal valor)
  {
    //Retorna um valor com os juros
    //return CalculoService.RoundBasedOnThirdDecimal(valor * (1 + ParcelaAcordo.Acordo.TaxaJurosDiaria * DiasAtraso));

    return (Decimal.Round(valor * (1 + ParcelaAcordo.Acordo.TaxaJurosDiaria * DiasAtraso), 2));
  }

  private decimal getValorDosJuros(decimal valor)
  {
    //decimal resultado = valor * (ParcelaAcordo.Acordo.TaxaJurosDiaria * DiasAtraso);

    //return CalculoService.RoundBasedOnThirdDecimal(resultado);

    return (Decimal.Round(valor * (ParcelaAcordo.Acordo.TaxaJurosDiaria * DiasAtraso) - new Decimal(0.004), 2, MidpointRounding.AwayFromZero));
  }

  private decimal getValorDeHonorarios(decimal valor)
  {
    //decimal resultado = ((valor * ParcelaAcordo.Acordo.TaxaHonorario));

    //return CalculoService.RoundBasedOnThirdDecimal(resultado);

    return Decimal.Round(((valor * ParcelaAcordo.Acordo.TaxaHonorario) - new Decimal(0.004)), 2, MidpointRounding.AwayFromZero);
  }
}