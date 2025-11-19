using Calculo.Core.Calculo;
using Calculo.Core.Models.Titulo;
using Calculo.Repository.v1.Repositories.Interfaces;
using Calculo.Service.v1.Interfaces;

namespace Calculo.Service.v1;

public class TituloService : ITituloService
{
  private readonly IDevedorRepository _devedorRepository;

  public TituloService(IDevedorRepository devedorRepository)
  {
    _devedorRepository = devedorRepository;
  }

  public async Task<TituloResponse> ObterTitulosAsync(string codDevedor)
  {
    var result = await _devedorRepository.ObterTitulosPorDevedorAsync(codDevedor);
    TituloResponse titulo = new TituloResponse();
    titulo.Titulos = result.ToList();
    return titulo;
  }

  public Task<AcordoVM> SimularV3(AcordoVM acordoVM)
  {
    acordoVM.PercentualJuros += acordoVM.PercentualJurosRepasse;
    acordoVM.PercentualMulta += acordoVM.PercentualMultaRepasse;
    acordoVM.PercentualHonorario += acordoVM.PercentualHonorarioRepasse;
    acordoVM.PercentualComissao += acordoVM.PercentualComissaoRepasse;

    Acordo acordo = new Acordo(acordoVM.DataAcordo,
                acordoVM.PercentualJuros,
                acordoVM.PercentualHonorario,
                acordoVM.PercentualMulta,
                acordoVM.PercentualDesconto,
                acordoVM.NumeroParcelas,
                acordoVM.ParcelasIguais,
                acordoVM.NumeroRecorrencia);

    foreach (var titulo in acordoVM.Titulos)
    {
      acordo.AddTitulo(titulo.TituloId, titulo.ValorPrincipal, titulo.ValorCustas,
                      titulo.DataVencimento, titulo.ValorRecebidoPrincipal,
                      titulo.ValorRecebidoCustas, titulo.ValorRecebidoMulta);

      var newTitulo = acordo.Titulos.FirstOrDefault(x => x.TituloId == titulo.TituloId);
      titulo.ValorDesconto = newTitulo.ValorDesconto;
      titulo.ValorMulta = newTitulo.ValorMulta;
    }

    foreach (var item in acordoVM.ParcelasAcordo.Where(p => p.DataFixa || p.ValorFixo))
    {
      acordo.AddParcelaAcordo(item.ParcelaAcordoId, item.Data, item.Valor, item.DataFixa, item.ValorFixo);
    }

    CalcularParcelas(acordo);

    acordoVM.ParcelasAcordo.Clear();
    foreach (var item in acordo.ParcelasAcordo.OrderBy(pa => pa.ParcelaAcordoId))
    {
      var parcelaAcordoVM = new ParcelaAcordoVM()
      {
        ParcelaAcordoId = item.ParcelaAcordoId,
        Data = item.Data,
        DataFixa = item.DataFixa,
        Valor = item.Valor,
        ValorFixo = item.ValorFixo,
        Feriado = item.Feriado
      };

      acordoVM.ParcelasAcordo.Add(parcelaAcordoVM);
    }

    string mensagemValidacao = "";
    if (!acordo.IsValid())
    {
      mensagemValidacao = String.Join(", ", acordo.ValidationErrors.ToArray());
    }

    acordoVM.Mensagem = mensagemValidacao;

    foreach (var titulo in acordoVM.Titulos)
    {
      var newTitulo = acordo.Titulos.FirstOrDefault(x => x.TituloId == titulo.TituloId);
      titulo.ValorDesconto = newTitulo.ValorDesconto;
      titulo.ValorMulta = newTitulo.ValorMulta;
    }

    var parcelas = acordo.getParcelasPagamento();
    var parcelasPagamento = parcelas;

    acordoVM.ParcelasPagamento.Clear();
    foreach (var parcela in parcelas.OrderBy(p => p.ParcelaPagamentoId))
    {
      var jurosRetido = RetidoCalculadora(parcela.ValorJuros, acordoVM.PercentualJuros,
        acordoVM.PercentualJurosRepasse);

      var honorariosRetido = RetidoCalculadora(
        parcela.ValorHonorario,
        acordoVM.PercentualHonorario,
        acordoVM.PercentualHonorarioRepasse
      );

      var honorariosRepasse = parcela.ValorHonorario - honorariosRetido;

      var multaRetido = RetidoCalculadora(parcela.ValorMulta, acordoVM.PercentualMulta,
        acordoVM.PercentualMultaRepasse);

      var baseComissao = parcela.ValorPrincipal + parcela.ValorJuros + parcela.ValorMulta;

      var valorComissaoRetido = baseComissao * ((acordoVM.PercentualComissao - acordoVM.PercentualComissaoRepasse) / 100m);
      var valorComissaoRepasse = baseComissao * (acordoVM.PercentualComissaoRepasse / 100m);

      var parcelaVM = new ParcelaPagamentoVM
      {
        ParcelaPagamentoId = parcela.ParcelaPagamentoId,
        DataPagamento = parcela.DataPagamento,
        NumeroParcela = parcela.NumeroParcela,
        ValorParcela = parcela.ValorParcela,
        ValorPrincipal = parcela.ValorPrincipal,
        ValorDesconto = (parcela.ValorPrincipal / (1 - (acordoVM.PercentualDesconto / 100))) - parcela.ValorPrincipal,
        ValorCustas = parcela.ValorCustas,
        ValorMulta = parcela.ValorMulta,
        SaldoTitulo = parcela.SaldoTitulo,
        ValorJuros = parcela.ValorJuros,
        ValorJurosRepasse = parcela.ValorJuros - jurosRetido,
        ValorJurosRetido = jurosRetido,
        ValorHonorariosRepasse = honorariosRepasse,
        ValorHonorariosRetido = honorariosRetido,
        ValorComissaoRetido = valorComissaoRetido,
        ValorComissaoRepasse = valorComissaoRepasse,
        ValorComissao = valorComissaoRetido + valorComissaoRepasse,
        ValorMultaRepasse = parcela.ValorMulta - multaRetido,
        ValorMultaRetido = multaRetido,
        ValorHonorario = parcela.ValorHonorario,
        ValorEncargos = parcela.ValorEncargos,
        DiasAtraso = parcela.DiasAtraso,
        Titulo = new TituloVM
        {
          TituloId = parcela.Titulo.TituloId,
          ValorPrincipal = parcela.Titulo.ValorPrincipal,
          ValorMulta = parcela.Titulo.ValorMulta,
          ValorDesconto = parcela.Titulo.ValorDesconto,
          DataVencimento = parcela.Titulo.DataVencimento
        }
      };
      acordoVM.ParcelasPagamento.Add(parcelaVM);
    }

    return Task.FromResult(acordoVM);
  }

  private decimal RetidoCalculadora(decimal valor, decimal taxaInformada, decimal taxaMinimoRepasse)
  {
    // Caso especial: repasse ZERO = tudo retido
    if (taxaMinimoRepasse == 0)
      return valor;

    if (taxaInformada > 0 && taxaInformada >= taxaMinimoRepasse)
    {
      decimal valorRepasse = valor * (taxaMinimoRepasse / taxaInformada);
      return valor - valorRepasse;
    }

    return valor;
  }

  private static void CalcularParcelas(Acordo acordo)
  {
    var valorParcelaLiquidaPrestacaoSugerida = Decimal.Round(acordo.SaldoTitulos / acordo.NumeroParcelas + new Decimal(0.004), 2);

    if (acordo.ParcelasAcordo.Count > acordo.NumeroParcelas)
    {
      var parcelasExcedentes = acordo.ParcelasAcordo
        .Where(p => p.ParcelaAcordoId > acordo.NumeroParcelas)
        .ToList();

      foreach (var parcela in parcelasExcedentes)
      {
        acordo.ParcelasAcordo.Remove(parcela);
      }
    }

    for (int i = 0; i < acordo.NumeroParcelas; i++)
    {
      decimal valorParcelaLiquidaPrestacao = 0;

      if (i == acordo.NumeroParcelas - 1)
      {
        valorParcelaLiquidaPrestacaoSugerida = acordo.SaldoTitulos;
      }

      var parcelaAcordo = acordo.ParcelasAcordo.FirstOrDefault(p => p.ParcelaAcordoId == i + 1);
      if (parcelaAcordo == null)
      {
        var primeiraParcela = acordo.ParcelasAcordo.FirstOrDefault(p => p.ParcelaAcordoId == 1);

        if (acordo.NumeroRecorrencia == 30)
        {
          if (primeiraParcela != null)
          {
            parcelaAcordo = new ParcelaAcordo(i + 1, primeiraParcela.Data.AddMonths(i), 0, acordo);
          }
          else
          {
            parcelaAcordo = new ParcelaAcordo(i + 1, acordo.Data.AddMonths(i * acordo.NumeroRecorrencia), 0, acordo);
          }
        }
        else if (acordo.NumeroRecorrencia is 15 or 7)
        {
          parcelaAcordo = primeiraParcela != null ? new ParcelaAcordo(i + 1, primeiraParcela.Data.AddDays(i * acordo.NumeroRecorrencia), 0, acordo) : new ParcelaAcordo(i + 1, acordo.Data.AddMonths(i * acordo.NumeroRecorrencia), 0, acordo);
        }
        else
        {
          parcelaAcordo = new ParcelaAcordo(i + 1, acordo.Data.AddDays(i * acordo.NumeroRecorrencia), 0, acordo);
        }

        acordo.AddParcelaAcordo(parcelaAcordo);
        valorParcelaLiquidaPrestacao = valorParcelaLiquidaPrestacaoSugerida;
      }
      else
      {
        if (parcelaAcordo.ValorFixo)
        {
          valorParcelaLiquidaPrestacao = parcelaAcordo.Valor;
        }
        else
        {
          valorParcelaLiquidaPrestacao = valorParcelaLiquidaPrestacaoSugerida;
          parcelaAcordo.Valor = 0;
        }

        if (!parcelaAcordo.DataFixa)
        {
          var primeiraParcela = acordo.ParcelasAcordo.FirstOrDefault(p => p.ParcelaAcordoId == 1);

          if (acordo.NumeroRecorrencia == 30)
          {
            if (primeiraParcela != null)
            {
              parcelaAcordo.Data = primeiraParcela.Data.AddMonths(i);
            }
            else
            {
              parcelaAcordo.Data = acordo.Data.AddMonths(i);
            }
          }
          else if (acordo.NumeroRecorrencia is 15 or 7)
          {
            parcelaAcordo.Data = primeiraParcela != null ? primeiraParcela.Data.AddDays(i * acordo.NumeroRecorrencia) : acordo.Data.AddDays(i * acordo.NumeroRecorrencia);
          }
          else
          {
            parcelaAcordo.Data = acordo.Data.AddDays(i * acordo.NumeroRecorrencia);
          }

        }
      }

      foreach (var titulo in acordo.Titulos.OrderBy(x => x.DataVencimento))
      {
        if (!parcelaAcordo.ValorFixo)
        {
          while (titulo.SaldoTotal > 0 && parcelaAcordo.SomaPricipais < valorParcelaLiquidaPrestacao)
          {
            var parcelaPagamento = new ParcelaPagamento(acordo.getParcelasPagamento().Count + 1, parcelaAcordo, titulo);

            parcelaPagamento.CalcularValorParcela();

            if (parcelaPagamento.SomaPrincipais > (valorParcelaLiquidaPrestacao - parcelaAcordo.SomaPricipais))
            {
              parcelaPagamento.CalcularValorParcela(valorParcelaLiquidaPrestacao - parcelaAcordo.SomaPricipais);
            }

            parcelaAcordo.Valor += parcelaPagamento.ValorParcela;

            parcelaPagamento.NumeroParcela = parcelaPagamento.ParcelaAcordo.ParcelaAcordoId;

            titulo.AddParcelaPagamento(parcelaPagamento);
            parcelaAcordo.AddParcelaPagamento(parcelaPagamento);
          }
        }
        else
        {
          while (titulo.SaldoTotal > 0 && parcelaAcordo.ParcelasPagamento.Sum(p => p.ValorParcela) < valorParcelaLiquidaPrestacao)
          {
            var parcelaPagamento = new ParcelaPagamento(acordo.getParcelasPagamento().Count + 1, parcelaAcordo, titulo);

            parcelaPagamento.CalcularValorParcela();
            var valorDisponivelParcelaAcordo = (valorParcelaLiquidaPrestacao - parcelaAcordo.ParcelasPagamento.Sum(p => p.ValorParcela));
            if (parcelaPagamento.ValorParcela > valorDisponivelParcelaAcordo)
            {
              parcelaPagamento.CalcularValorPrincipal(valorDisponivelParcelaAcordo);
            }

            parcelaPagamento.NumeroParcela = parcelaPagamento.ParcelaAcordo.ParcelaAcordoId;

            titulo.AddParcelaPagamento(parcelaPagamento);
            parcelaAcordo.AddParcelaPagamento(parcelaPagamento);
          }
        }
      }
    }

    if (acordo.ParcelasIguais)
    {
      decimal somaEncargos = 0;
      for (var i = 0; i < 15; i++)
      {
        CalcularParcelasIguais(acordo, i, somaEncargos > 0 ? somaEncargos : 0);
        ProcessarAcordo(acordo);
        somaEncargos = acordo.ParcelasAcordo.Sum(pa => pa.ParcelasPagamento.Sum(pp => pp.ValorEncargos));
        if (acordo.SaldoTitulos == 0 && somaEncargos is > 0 and < 1)
        {
          break;
        }
      }
    }
    else
    {
      ProcessarAcordo(acordo);
    }

    var idsParcelasAtuais = new HashSet<int>(acordo.ParcelasAcordo.Select(p => p.ParcelaAcordoId));

    foreach (var titulo in acordo.Titulos)
    {
      var parcelasParaManter = titulo.ParcelasPagamento
        .Where(p => idsParcelasAtuais.Contains(p.ParcelaAcordo.ParcelaAcordoId))
        .ToList();

      titulo.ParcelasPagamento.Clear();
      foreach (var parcela in parcelasParaManter)
      {
        titulo.ParcelasPagamento.Add(parcela);
      }
    }

  }

  private static void CalcularParcelasIguais(Acordo acordo, int iteracao, decimal somaEncargos)
  {
    var somaValorParcelasNaoFixas = acordo.ParcelasAcordo.Where(p => p.ValorFixo == false).Sum(p => p.Valor) - somaEncargos;
    var numeroParcelasNaoFixas = acordo.ParcelasAcordo.Count(p => p.ValorFixo == false);
    if (numeroParcelasNaoFixas > 0)
    {
      var valorMedioParcelas = Decimal.Round(
                                    (somaValorParcelasNaoFixas + acordo.SaldoTitulos) /
                                    numeroParcelasNaoFixas + new Decimal(0.0025) * iteracao, 2);

      foreach (var item in acordo.ParcelasAcordo)
      {
        if (!item.ValorFixo)
        {
          item.Valor = valorMedioParcelas;
        }
      }
    }

    foreach (var item in acordo.ParcelasAcordo)
    {
      item.ParcelasPagamento.Clear();
    }

    foreach (var item in acordo.Titulos)
    {
      item.ParcelasPagamento.Clear();
    }
  }

  private static void ProcessarAcordo(Acordo acordo)
  {
    foreach (var titulo in acordo.Titulos.OrderBy(x => x.DataVencimento))
    {
      while (titulo.SaldoTotal > 0 && acordo.SaldoParcelas > 0
             || acordo is { SaldoTitulos: 0, SaldoParcelas: > 0 })
      {
        var parcelaAcordo = acordo.getParcelaAcordoComSaldo();
        var parcelaPagamento = new ParcelaPagamento(acordo.getParcelasPagamento().Count + 1, parcelaAcordo, titulo);

        parcelaPagamento.CalcularValorParcela();

        if (parcelaAcordo.Saldo < parcelaPagamento.ValorParcela)
        {
          parcelaPagamento.CalcularValorPrincipal(parcelaAcordo.Saldo);
        }

        parcelaPagamento.NumeroParcela = parcelaPagamento.ParcelaAcordo.ParcelaAcordoId;

        titulo.AddParcelaPagamento(parcelaPagamento);
        parcelaAcordo.AddParcelaPagamento(parcelaPagamento);

        if (acordo.SaldoTitulos == 0 && parcelaAcordo.Saldo > 0)
        {
          parcelaPagamento.AddValorResidual(parcelaAcordo.Saldo);
        }

        if (titulo.SaldoTotal > 0 && acordo.SaldoParcelas == 0)
        {
          parcelaPagamento.AddValorResidual(parcelaAcordo.Saldo * -1);
        }
      }
    }
  }
}
