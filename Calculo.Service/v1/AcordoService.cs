using Calculo.Core.Models;
using Calculo.Core.Models.Acordo;
using Calculo.Core.Models.FaixaComissao;
using Calculo.Repository.v1.Repositories.Interfaces;
using Calculo.Service.v1.Interfaces;

namespace Calculo.Service.v1;

public class AcordoService : IAcordoService
{
  private readonly IDevedorRepository _devedorRepository;
  private readonly IFaixaComissaoRepository _faixaComissaoRepository;
  private readonly IAcordoRepository _acordoRepository;

  public AcordoService(IDevedorRepository devedorRepository, IFaixaComissaoRepository faixaComissaoRepository, IAcordoRepository acordoRepository)
  {
    _devedorRepository = devedorRepository;
    _faixaComissaoRepository = faixaComissaoRepository;
    _acordoRepository = acordoRepository;
  }

  public async Task<DevedorResult> ObterDevedorAsync(string codDevedor)
  {
    var devedor = await _devedorRepository.ObterDevedorAsync(codDevedor);

    if (devedor == null)
      throw new Exception("Devedor não encontrado");

    return devedor;
  }

  public async Task<FaixaComissaoResult?> ObterTaxas(DevedorFaixaRequest devedorFaixaRequest)
  {
    var taxas = new FaixaComissaoResult { tipoCalculo = devedorFaixaRequest.TipoCalculo };

    var origemTaxas = await IdentificadorOrigemTaxas(devedorFaixaRequest);
    var tipoFaixaComissao = origemTaxas switch
    {
      OrigemTaxas.FaixaComissaoBordero =>
        (TipoFaixaComissao)await _faixaComissaoRepository.TipoFaixaComissaoBordero(devedorFaixaRequest.codClienteCredor, devedorFaixaRequest.numeroBordero),

      OrigemTaxas.FaixaComissaoCadastro =>
        (TipoFaixaComissao)await _faixaComissaoRepository.TipoFaixaComissaoCadastro(devedorFaixaRequest.codClienteCredor),

      _ => TipoFaixaComissao.SemFaixa
    };

    var diaAtrasoContrato = await _faixaComissaoRepository.FaixaVencimentoTitulo(
      devedorFaixaRequest.codClienteCredor, devedorFaixaRequest.numeroBordero, devedorFaixaRequest.codClienteDevedor);

    if (tipoFaixaComissao != TipoFaixaComissao.SemFaixa)
    {
      diaAtrasoContrato = tipoFaixaComissao switch
      {
        TipoFaixaComissao.DiasDeDuplicata => await _faixaComissaoRepository.FaixaVencimentoTituloDuplicata(
          devedorFaixaRequest.codClienteCredor, devedorFaixaRequest.numeroBordero, devedorFaixaRequest.codClienteDevedor),

        TipoFaixaComissao.AtrasoCadastro => await _faixaComissaoRepository.FaixaVencimentoTituloCadastro(
          devedorFaixaRequest.codClienteCredor, devedorFaixaRequest.numeroBordero, devedorFaixaRequest.codClienteDevedor),

        TipoFaixaComissao.PeriodoVencimento => await _faixaComissaoRepository.FaixaVencimentoTituloPeriodoVencido(
          devedorFaixaRequest.codClienteCredor, devedorFaixaRequest.numeroBordero, devedorFaixaRequest.codClienteDevedor),

        _ => diaAtrasoContrato
      };
    }

    taxas = origemTaxas switch
    {
      OrigemTaxas.FaixaComissaoBordero => await BuscarTaxasBordero(devedorFaixaRequest, tipoFaixaComissao, diaAtrasoContrato),
      OrigemTaxas.FaixaComissaoCadastro => await BuscarTaxasCadastro(devedorFaixaRequest, tipoFaixaComissao, diaAtrasoContrato),
      OrigemTaxas.Bordero => await BuscarTaxasPadrao(devedorFaixaRequest),
      _ => taxas
    };
    if (taxas != null)
    {
      taxas.tipoFaixaComissao = tipoFaixaComissao;
      taxas.origemTaxas = origemTaxas;
    }


    return taxas;
  }

  public async Task<AcordoResult> GravaAcordo(AcordoRequest acordoRequest)
  {
    var status = new Dictionary<string, bool>
    {
        { "ParcelasStatus", false },
        { "OcorrenciaStatus", false },
        { "PcTituloParamStatus", false },
        { "PcTituloCalculoStatus", false },
        { "AcordoStatus", false },
        { "PcTituloParcelaStatus", false },
        { "PcTituloStatus", false }
    };

    int idCalculo = await _acordoRepository.GetIdCalculo();

    var acordoPcTitulo = acordoRequest.ACORDO.TITULOS
        .Select(item => new AcordoPcTituloRequest
        {
          DT_VENCIMENTO_TITULO = item.dataVencimento,
          IND_SITUACAO = "A",
          NUMERO_TITULO = item.nomeTitulo,
          VALOR_JUROS = item.juros,
          VALOR_JUROS_RETIDO = item.jurosRetido,
          VALOR_JUROS_REPASSE = item.jurosRepasse,
          VALOR_HONORARIOS_RETIDO = item.honorariosRetido,
          VALOR_HONORARIOS_REPASSE = item.honorariosRepasse,
          VALOR_MULTA_RETIDO = item.multaRetido,
          VALOR_MULTA_REPASSE = item.multaRepasse,
          VALOR_PRINCIPAL = item.valorPrincipal,
          VALOR_PRINCIPAL_ORIGINAL = item.valorPrincipal,
          VALOR_PROTESTO = item.valorCustas,
          VALOR_DESCONTO = item.valorDesconto,
          COD_CALCULO = idCalculo
        }).ToList();

    foreach (var item in acordoPcTitulo)
    {
      status["PcTituloStatus"] = await _acordoRepository.GravaPcTitulo(item);
    }

    if (!status["PcTituloStatus"]) return RetornaErro(status);

    var parcelasValores = acordoRequest.ACORDO.TITULOS
        .GroupBy(item => item.numeroParcela)
        .Select(grupo => new AcordoValoresParcelasRequest
        {
          PARCELA = grupo.Key,
          VALOR_HONORARIOS = grupo.Sum(x => x.honorarios),
          VALOR_MULTA = grupo.Sum(x => x.valorMulta),
          VALOR_JUROS = grupo.Sum(x => x.juros),
          VALOR_PARCELA = grupo.Sum(x => x.valorParcela),
          VALOR_COMISSAO = grupo.Sum(x => x.comissao),
          VALOR_COMISSAO_REPASSE = grupo.Sum(x => x.comissaoRepasse),
          VALOR_COMISSAO_RETIDO = grupo.Sum(x => x.comissaoRetido),
          VALOR_JUROS_RETIDO = grupo.Sum(x => x.jurosRetido),
          VALOR_JUROS_REPASSE = grupo.Sum(x => x.jurosRepasse),
          VALOR_HONORARIOS_RETIDO = grupo.Sum(x => x.honorariosRetido),
          VALOR_HONORARIOS_REPASSE = grupo.Sum(x => x.honorariosRepasse),
          VALOR_MULTA_REPASSE = grupo.Sum(x => x.multaRepasse),
          VALOR_MULTA_RETIDO = grupo.Sum(x => x.multaRetido),
          VALOR_PRINCIPAL = grupo.Sum(x => x.valorPrincipal),
          VALOR_PRESTACAO_CONTAS = grupo.Sum(x => x.multaRepasse + x.jurosRepasse + x.honorariosRepasse + x.valorPrincipal + x.valorCustas + x.valorEncargos)
        }).ToList();

    var acordoPcTituloParcela = acordoRequest.ACORDO.PARCELAS
        .Select(item => new AcordoPcTituloParcelaRequest
        {
          COD_CLIENTE_DEVEDOR = acordoRequest.ACORDO.COD_CLIENTE_DEVEDOR,
          COD_PARCELA = (item.numeroParcela + 1).ToString(),
          DT_VENCIMENTO = item.dataVencimento,
          VALOR_COMISSAO = parcelasValores.FirstOrDefault(x => x.PARCELA == item.numeroParcela)?.VALOR_COMISSAO ?? 0,
          VALOR_PARCELA = item.valorParcela,
          COD_CALCULO = idCalculo,
          VALOR_HONORARIOS = parcelasValores.FirstOrDefault(x => x.PARCELA == item.numeroParcela)?.VALOR_HONORARIOS ?? 0
        }).ToList();

    foreach (var item in acordoPcTituloParcela)
    {
      status["PcTituloParcelaStatus"] = await _acordoRepository.GravaPcTituloParcela(item);
    }

    if (!status["PcTituloParcelaStatus"]) return RetornaErro(status);

    var acordoPcTituloCalculo = acordoRequest.ACORDO.TITULOS
        .Select((item, index) => new AcordoPcTituloCalculoRequest
        {
          DIAS_ATRASO = item.atrasos,
          DT_RECEBIMENTO = item.dataRecebimento,
          IND_PARCELA = $"{FormataParcela2(item.numeroParcela.ToString())}/{FormataParcela2((index + 1).ToString())}",
          DT_VENCIMENTO = item.dataVencimentoTitulo,
          NUMERO_TITULO = item.nomeTitulo,
          VALOR_COMISSAO = item.comissao,
          VALOR_COMISSAO_RETIDO = item.comissaoRetido,
          VALOR_COMISSAO_REPASSE = item.comissaoRepasse,
          COD_CALCULO = idCalculo,
          VALOR_CUSTAS = item.valorCustas,
          VALOR_HONORARIOS = item.honorarios,
          VALOR_JUROS = item.juros,
          VALOR_JUROS_RETIDO = item.jurosRetido,
          VALOR_JUROS_REPASSE = item.jurosRepasse,
          VALOR_HONORARIOS_RETIDO = item.honorariosRetido,
          VALOR_HONORARIOS_REPASSE = item.honorariosRepasse,
          VALOR_MULTA = item.valorMulta,
          VALOR_MULTA_RETIDO = item.multaRetido,
          VALOR_MULTA_REPASSE = item.multaRepasse,
          VALOR_DESCONTO = item.valorDesconto,
          VALOR_CUSTAS_RECEBIDO = item.valorCustas,
          VALOR_JUROS_RECEBIDO = item.juros,
          VALOR_MULTA_BASE = item.valorMulta,
          VALOR_PRESTACAO_CONTAS = (item.valorPrincipal + item.valorEncargos + item.juros + item.valorMulta + item.valorCustas + item.honorarios) - item.comissao - item.jurosRetido - item.multaRetido - item.honorariosRetido,
          VALOR_PRINCIPAL = item.valorPrincipal,
          VALOR_PRINCIPAL_RECEBIDO = item.valorPrincipal,
          VALOR_RECEBIDO = item.juros + item.valorCustas + item.valorPrincipal + item.valorMulta + item.honorariosRepasse,
          VALOR_SALDO = 0,
          VALOR_SUB_TOTAL = item.juros + item.valorCustas + item.valorPrincipal + item.valorMulta + item.honorariosRepasse
        }).ToList();

    foreach (var item in acordoPcTituloCalculo)
    {
      status["PcTituloCalculoStatus"] = await _acordoRepository.GravaPcTituloCalculo(item);
    }

    if (!status["PcTituloCalculoStatus"]) return RetornaErro(status);

    var acordoTitulo = CriarAcordoTitulo(acordoRequest, idCalculo);
    status["AcordoStatus"] = await _acordoRepository.GravaAcordo(acordoTitulo);
    if (!status["AcordoStatus"]) return RetornaErro(status);

    int idAcordo = await _acordoRepository.GetIdAcordo();

    var acordoPcTituloParam = CriarAcordoPcTituloParam(acordoRequest, idAcordo, idCalculo);
    status["PcTituloParamStatus"] = await _acordoRepository.GravaPcTituloParam(acordoPcTituloParam);
    if (!status["PcTituloParamStatus"]) return RetornaErro(status);

    var acordoOcorrencia = CriarAcordoOcorrencia(acordoRequest);
    status["OcorrenciaStatus"] = await _acordoRepository.GravaOcorrencia(acordoOcorrencia);
    if (!status["OcorrenciaStatus"]) return RetornaErro(status);

    var acordoParcelas = CriarParcelas(acordoRequest, idAcordo, parcelasValores);

    foreach (var parcela in acordoParcelas)
    {
      status["ParcelasStatus"] = await _acordoRepository.GravaParcelas(parcela);
    }

    if (!status["ParcelasStatus"]) return RetornaErro(status);

    await _acordoRepository.GravaPcTituloTabela(new PrestacaoContasTituloRequest
    {
      NUMERO_ACORDO = idAcordo,
      COD_CLIENTE_CREDOR = acordoRequest.ACORDO.COD_CLIENTE_CREDOR.Trim(),
      COD_CLIENTE_DEVEDOR = acordoRequest.ACORDO.COD_CLIENTE_DEVEDOR.Trim(),
      NUMERO_BORDERO = acordoRequest.ACORDO.NUMERO_BORDERO.Trim(),
      NUMERO_CALCULO = idCalculo
    });

    return new AcordoResult { codigo = "0", mensagem = "OK" };
  }

  private AcordoResult RetornaErro(Dictionary<string, bool> status) =>
      new()
      {
        codigo = "1",
        mensagem = string.Join(" - ", status.Select(s => $"{s.Key}: {s.Value}"))
      };


  private async Task<FaixaComissaoResult?> BuscarTaxasBordero(DevedorFaixaRequest request, TipoFaixaComissao tipoFaixa, int diasAtraso)
  {
    var maximas = await _faixaComissaoRepository.CarregaMaximasBorderoNormal(request.numeroBordero);
    var resultMaximas = maximas.maximaNormal == "on";

    if (tipoFaixa == TipoFaixaComissao.SemFaixa)
      return await _faixaComissaoRepository.BuscarTaxasBorderoNormal(request.codClienteCredor, request.numeroBordero);

    var taxas = await _faixaComissaoRepository.BuscarFaixaComissaoBorderoNormal(
      request.codClienteCredor, request.codClienteDevedor, tipoFaixa, request.numeroBordero, diasAtraso);
    if (taxas != null)
      taxas.carregaMaximas = resultMaximas;
    return taxas;
  }

  private async Task<FaixaComissaoResult?> BuscarTaxasCadastro(DevedorFaixaRequest request, TipoFaixaComissao tipoFaixa, int diasAtraso)
  {
    var maximasCredor = await _faixaComissaoRepository.CarregaMaximasNormal(request.codClienteCredor);
    var resultMaximas = maximasCredor?.maximaNormal == "on";

    if (tipoFaixa == TipoFaixaComissao.SemFaixa)
      return await _faixaComissaoRepository.BuscarTaxasBorderoNormal(request.codClienteCredor, request.numeroBordero);

    var taxas = await _faixaComissaoRepository.BuscarFaixaComissaoNormal(
      request.codClienteCredor, request.codClienteDevedor, tipoFaixa, diasAtraso);

    if (taxas != null)
      taxas.carregaMaximas = resultMaximas;
    return taxas;
  }

  private async Task<FaixaComissaoResult?> BuscarTaxasPadrao(DevedorFaixaRequest request)
  {
    var maximasCredor = await _faixaComissaoRepository.CarregaMaximasBorderoNormal(request.codClienteCredor);
    var resultMaximas = maximasCredor?.maximaNormal == "on";

    var taxas = await _faixaComissaoRepository.BuscarTaxasBorderoNormal(request.codClienteCredor, request.numeroBordero);
    taxas.carregaMaximas = resultMaximas;
    taxas.tipoFaixaComissao = TipoFaixaComissao.SemFaixa;

    return taxas;
  }

  private async Task<OrigemTaxas> IdentificadorOrigemTaxas(DevedorFaixaRequest request)
  {
    var origem = request.TipoCalculo switch
    {
      TipoCalculo.Normal => await _faixaComissaoRepository.VerificaFaixaComissaoBordero(request.codClienteCredor, request.numeroBordero),
      TipoCalculo.PD => await _faixaComissaoRepository.VerificaFaixaComissaoBorderoPD(request.codClienteCredor, request.numeroBordero),
      TipoCalculo.PDA => await _faixaComissaoRepository.VerificaFaixaComissaoBorderoPDA(request.codClienteCredor, request.numeroBordero),
      _ => false
    };

    if (origem) return OrigemTaxas.FaixaComissaoBordero;

    var origemCadastro = request.TipoCalculo switch
    {
      TipoCalculo.Normal => await _faixaComissaoRepository.VerificaFaixaComissaoCadastro(request.codClienteCredor),
      TipoCalculo.PD => await _faixaComissaoRepository.VerificaFaixaComissaoCadastroPD(request.codClienteCredor),
      TipoCalculo.PDA => await _faixaComissaoRepository.VerificaFaixaComissaoCadastroPDA(request.codClienteCredor),
      _ => false
    };

    return origemCadastro ? OrigemTaxas.FaixaComissaoCadastro : OrigemTaxas.Bordero;
  }

  private string FormataParcela2(string numero)
  {
    return numero.PadLeft(2, '0');
  }

  private string FormataParcela(string numero)
  {
    return numero.PadLeft(3, '0');
  }

  private AcordoTituloRequest CriarAcordoTitulo(AcordoRequest acordoRequest, int idCalculo)
  {
    return new AcordoTituloRequest
    {
      COD_CALCULO = idCalculo,
      COD_FUNCIONARIO = acordoRequest.ACORDO.COD_FUNCIONARIO,
      COD_CLIENTE_CREDOR = acordoRequest.ACORDO.COD_CLIENTE_CREDOR,
      COD_CLIENTE_DEVEDOR = acordoRequest.ACORDO.COD_CLIENTE_DEVEDOR,
      DT_ACORDO = DateTime.Now,
      DT_FECHAMENTO_CALCULO = DateTime.Now,
      DT_PRIMEIRO_VENCIMENTO = acordoRequest.ACORDO.TITULOS.First().dataVencimento,
      IND_PAGAMENTO = 0,
      IND_SITUACAO_ACORDO = "1",
      IND_TIPO_CALCULO = 3,
      IND_TIPO_DOCUMENTO = acordoRequest.ACORDO.TIPO_DOCUMENTO,
      NUMERO_BORDERO = acordoRequest.ACORDO.NUMERO_BORDERO,
      OBS_ACORDO = "",
      QUANT_PARCELA_ACORDO = FormataParcela(acordoRequest.ACORDO.QUANT_PARCELA_ACORDO.ToString()),
      TAXA_CAMPANHA = acordoRequest.ACORDO.TAXA_CAMPANHA,
      TAXA_COMISSAO_ACORDO = acordoRequest.ACORDO.TAXA_COMISSAO_ACORDO,
      TAXA_COMISSAO_REPASSE = acordoRequest.ACORDO.TAXA_COMISSAO_REPASSE,
      TAXA_COMISSAO_RETIDO = acordoRequest.ACORDO.TAXA_COMISSAO_RETIDO,
      TAXA_CUSTAS_ACORDO = acordoRequest.ACORDO.TITULOS.Sum(x => x.valorCustas),
      TAXA_HONORARIOS_ACORDO = acordoRequest.ACORDO.TAXA_HONORARIOS_ACORDO,
      TAXA_JUROS_ACORDO = acordoRequest.ACORDO.TAXA_JUROS_ACORDO,
      TAXA_JUROS_REPASSE = acordoRequest.ACORDO.TAXA_JUROS_REPASSE,
      TAXA_JUROS_RETIDO = acordoRequest.ACORDO.TAXA_JUROS_RETIDO,
      TAXA_HONORARIOS_REPASSE = acordoRequest.ACORDO.TAXA_HONORARIOS_REPASSE,
      TAXA_HONORARIOS_RETIDO = acordoRequest.ACORDO.TAXA_HONORARIOS_RETIDO,
      TAXA_MULTA = acordoRequest.ACORDO.TAXA_MULTA,
      TAXA_MULTA_REPASSE = acordoRequest.ACORDO.TAXA_MULTA_REPASSE,
      TAXA_MULTA_RETIDO = acordoRequest.ACORDO.TAXA_MULTA_RETIDO,
      VALOR_COMISSAO = acordoRequest.ACORDO.VALOR_COMISSAO,
      VALOR_COMISSAO_RETIDO = acordoRequest.ACORDO.VALOR_COMISSAO_RETIDO,
      VALOR_COMISSAO_REPASSE = acordoRequest.ACORDO.VALOR_COMISSAO_REPASSE,
      VALOR_CUSTAS = acordoRequest.ACORDO.TITULOS.Sum(x => x.valorCustas),
      VALOR_DESCONTO = acordoRequest.ACORDO.VALOR_DESCONTO,
      VALOR_HONORARIOS = acordoRequest.ACORDO.VALOR_HONORARIOS,
      VALOR_JUROS = acordoRequest.ACORDO.VALOR_JUROS_TOTAL,
      VALOR_JUROS_RETIDO = acordoRequest.ACORDO.VALOR_JUROS_RETIDO,
      VALOR_JUROS_REPASSE = acordoRequest.ACORDO.VALOR_JUROS_REPASSE,
      VALOR_JUROS_ANT = 0,
      VALOR_JUROS_TOTAL = acordoRequest.ACORDO.VALOR_JUROS_TOTAL,
      VALOR_MULTA = acordoRequest.ACORDO.VALOR_MULTA,
      VALOR_PARCELA_ACORDO = acordoRequest.ACORDO.VALOR_TOTAL,
      VALOR_PRINCIPAL = acordoRequest.ACORDO.VALOR_PRINCIPAL,
      VALOR_TOTAL = acordoRequest.ACORDO.VALOR_TOTAL,
      DT_CADASTRO_BORDERO = Convert.ToDateTime(acordoRequest.ACORDO.DT_CADASTRO_BORDERO),
      NOVO_MODULO = "S",
      TIPO_CPMF = "0",
      TAXA_CPMF = 0.38m,
      VALOR_CPMF = 0
    };
  }

  private AcordoPcTituloParamRequest CriarAcordoPcTituloParam(AcordoRequest acordoRequest, int idAcordo, int idCalculo)
  {
    return new AcordoPcTituloParamRequest
    {
      NUMERO_BORDERO = acordoRequest.ACORDO.NUMERO_BORDERO,
      NUMERO_ACORDO = idAcordo.ToString(),
      TAXA_COMISSAO = acordoRequest.ACORDO.TAXA_COMISSAO_ACORDO,
      TAXA_HONORARIOS = acordoRequest.ACORDO.TAXA_HONORARIOS_ACORDO,
      TAXA_JUROS = acordoRequest.ACORDO.TAXA_JUROS_ACORDO,
      TAXA_MULTA = acordoRequest.ACORDO.TAXA_MULTA,
      TIPO_CALCULO = acordoRequest.ACORDO.TIPO_CALCULO,
      COD_CALCULO = idCalculo,
      COD_CLIENTE_CREDOR = acordoRequest.ACORDO.COD_CLIENTE_CREDOR,
      COD_CLIENTE_DEVEDOR = acordoRequest.ACORDO.COD_CLIENTE_DEVEDOR,
      IND_DIFERENCA = string.Empty,
      VALOR_DIFERENCA = 0
    };
  }

  private AcordoOcorrenciaRequest CriarAcordoOcorrencia(AcordoRequest acordoRequest)
  {
    string textoOcorrencia = acordoRequest.ACORDO.FUNCIONARIO + ": Acordo fechado em " +
                             acordoRequest.ACORDO.PARCELAS.Count + " parcela(s), com vencimentos " +
                             string.Join(", ", acordoRequest.ACORDO.PARCELAS.Select(p => p.dataVencimento.ToString("dd/MM/yyyy")));

    return new AcordoOcorrenciaRequest
    {
      COD_CLIENTE_CREDOR = acordoRequest.ACORDO.COD_CLIENTE_CREDOR,
      COD_CLIENTE_DEVEDOR = acordoRequest.ACORDO.COD_CLIENTE_DEVEDOR,
      DESCRICAO_OCORRENCIA = textoOcorrencia,
      DT_OCORRENCIA = DateTime.Now,
      DT_CADASTRO_BORDERO = Convert.ToDateTime(acordoRequest.ACORDO.DT_CADASTRO_BORDERO),
      IND_SITUACAO_OCORRENCIA = "N",
      NUMERO_BORDERO = acordoRequest.ACORDO.NUMERO_BORDERO,
      TIPO_OCORRENCIA = "05 - Acordo"
    };
  }

  private List<AcordoParcelasRequest> CriarParcelas(AcordoRequest acordoRequest, int idAcordo, List<AcordoValoresParcelasRequest> parcelasValores)
  {
    string indCheque = acordoRequest.ACORDO.TIPO_DOCUMENTO == 2 ? "S" : "N";

    return acordoRequest.ACORDO.PARCELAS
      .Select(item =>
      {
        var parcela = parcelasValores.FirstOrDefault(x => x.PARCELA == item.numeroParcela);

        return new AcordoParcelasRequest
        {
          COD_CLIENTE_DEVEDOR = acordoRequest.ACORDO.COD_CLIENTE_DEVEDOR,
          COD_CLIENTE_CREDOR = acordoRequest.ACORDO.COD_CLIENTE_CREDOR,
          COD_FUNCIONARIO = acordoRequest.ACORDO.COD_FUNCIONARIO,
          DT_ACORDO = DateTime.Now,
          DT_CADASTRO_BORDERO = Convert.ToDateTime(acordoRequest.ACORDO.DT_CADASTRO_BORDERO),
          DT_VENCIMENTO_PARCELA = item.dataVencimento,
          IND_CHEQUE_PARCELA = indCheque,
          IND_SITUACAO_PARCELA = 1,
          NUMERO_ACORDO = idAcordo.ToString(),
          NUMERO_BORDERO = acordoRequest.ACORDO.NUMERO_BORDERO,
          NUMERO_PARCELA = FormataParcela(item.numeroParcela.ToString()),
          OBS_PARCELA = "",
          VALOR_PRESTACAO_CONTAS = parcela?.VALOR_PRESTACAO_CONTAS ?? 0,
          VALOR_HONORARIOS = parcela?.VALOR_HONORARIOS ?? 0,
          VALOR_PRINCIPAL = parcela?.VALOR_PRINCIPAL ?? 0,
          VALOR_COMISSAO = parcela?.VALOR_COMISSAO ?? 0,
          VALOR_COMISSAO_REPASSE = parcela?.VALOR_COMISSAO_REPASSE ?? 0,
          VALOR_COMISSAO_RETIDO = parcela?.VALOR_COMISSAO_RETIDO ?? 0,
          VALOR_JUROS = parcela?.VALOR_JUROS ?? 0,
          VALOR_JUROS_RETIDO = parcela?.VALOR_JUROS_RETIDO ?? 0,
          VALOR_JUROS_REPASSE = parcela?.VALOR_JUROS_REPASSE ?? 0,
          VALOR_HONORARIOS_RETIDO = parcela?.VALOR_HONORARIOS_RETIDO ?? 0,
          VALOR_HONORARIOS_REPASSE = parcela?.VALOR_HONORARIOS_REPASSE ?? 0,
          VALOR_MULTA = parcela?.VALOR_MULTA ?? 0,
          VALOR_MULTA_REPASSE = parcela?.VALOR_MULTA_REPASSE ?? 0,
          VALOR_MULTA_RETIDO = parcela?.VALOR_MULTA_RETIDO ?? 0,
          VALOR_PARCELA = item.valorParcela
        };
      }).ToList();
  }

}