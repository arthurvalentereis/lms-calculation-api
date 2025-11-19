using Calculo.Repository.v1.Repositories.Interfaces;
using System.Data;
using Calculo.Core.Models.Acordo;
using Dapper;
using Serilog;

namespace Calculo.Repository.v1.Repositories;

public class AcordoRepository : IAcordoRepository
{
  private readonly IDbConnection _dbConnection;
  private readonly ILogger _logger;

  public AcordoRepository(IDbConnection dbConnection, ILogger logger)
  {
    _dbConnection = dbConnection;
    _logger = logger;
  }

  public async Task<int> GetIdCalculo()
  {
    string query = "SELECT NEXT VALUE FOR seq_calculo_pc;";

    return await _dbConnection.QueryFirstOrDefaultAsync<int>(query);
  }

  public async Task<bool> GravaPcTitulo(AcordoPcTituloRequest item)
  {
    const string query = @"
        INSERT INTO PC_TITULO_TITULO
            (COD_CALCULO, NUMERO_TITULO, DT_VENCIMENTO_TITULO, VALOR_PRINCIPAL, 
             VALOR_PROTESTO, VALOR_JUROS, VALOR_HONORARIOS_RETIDO, VALOR_HONORARIOS_REPASSE, VALOR_JUROS_RETIDO, VALOR_JUROS_REPASSE, 
             VALOR_MULTA_RETIDO, VALOR_MULTA_REPASSE, VALOR_TOTAL, VALOR_DESCONTO, 
             IND_SITUACAO, VALOR_PRINCIPAL_ORIGINAL)
        VALUES
            (@COD_CALCULO, @NUMERO_TITULO, @DT_VENCIMENTO_TITULO, @VALOR_PRINCIPAL, 
             @VALOR_PROTESTO, @VALOR_JUROS, @VALOR_HONORARIOS_RETIDO, @VALOR_HONORARIOS_REPASSE, @VALOR_JUROS_RETIDO, @VALOR_JUROS_REPASSE, 
             @VALOR_MULTA_RETIDO, @VALOR_MULTA_REPASSE, @VALOR_TOTAL, @VALOR_DESCONTO, 
             @IND_SITUACAO, @VALOR_PRINCIPAL_ORIGINAL)";

    try
    {
      var parameters = new
      {
        item.COD_CALCULO,
        item.NUMERO_TITULO,
        DT_VENCIMENTO_TITULO = item.DT_VENCIMENTO_TITULO.ToString("yyyy-MM-dd HH:mm:ss"),
        item.VALOR_PRINCIPAL,
        item.VALOR_PROTESTO,
        item.VALOR_JUROS,
        item.VALOR_HONORARIOS_RETIDO,
        item.VALOR_HONORARIOS_REPASSE,
        item.VALOR_JUROS_RETIDO,
        item.VALOR_JUROS_REPASSE,
        item.VALOR_MULTA_RETIDO,
        item.VALOR_MULTA_REPASSE,
        item.VALOR_TOTAL,
        item.VALOR_DESCONTO,
        item.IND_SITUACAO,
        item.VALOR_PRINCIPAL_ORIGINAL
      };

      return await _dbConnection.ExecuteAsync(query, parameters) > 0;
    }
    catch (Exception ex)
    {
      _logger.Error(ex, "Erro ao gravar PC_TITULO_TITULO");
      return false;
    }
  }

  public async Task<bool> GravaPcTituloParcela(AcordoPcTituloParcelaRequest item)
  {
    const string query = @"
        INSERT INTO [dbo].[PC_TITULO_PARCELA]
            ([COD_CALCULO], [COD_PARCELA], [DT_VENCIMENTO], [VALOR_PARCELA], 
             [VALOR_HONORARIOS], [VALOR_COMISSAO], [VALOR_TOTAL], 
             [VALOR_PRINCIPAL_TOTAL], [COD_CLIENTE_DEVEDOR])
        VALUES
            (@COD_CALCULO, @COD_PARCELA, @DT_VENCIMENTO, @VALOR_PARCELA, 
             @VALOR_HONORARIOS, @VALOR_COMISSAO, @VALOR_TOTAL, 
             @VALOR_PRINCIPAL_TOTAL, @COD_CLIENTE_DEVEDOR)";

    try
    {
      var result = await _dbConnection.ExecuteAsync(query, new
      {
        item.COD_CALCULO,
        item.COD_PARCELA,
        item.DT_VENCIMENTO,
        item.VALOR_PARCELA,
        item.VALOR_HONORARIOS,
        item.VALOR_COMISSAO,
        item.VALOR_TOTAL,
        item.VALOR_PRINCIPAL_TOTAL,
        item.COD_CLIENTE_DEVEDOR
      });

      return result > 0;
    }
    catch (Exception ex)
    {
      _logger.Error(ex, "Erro ao gravar PC_TITULO_PARCELA para {COD_CLIENTE_DEVEDOR}", item.COD_CLIENTE_DEVEDOR);
      return false;
    }
  }

  public async Task<bool> GravaPcTituloCalculo(AcordoPcTituloCalculoRequest item)
  {
    const string query = @"
        INSERT INTO [dbo].[PC_TITULO_CALCULO]
            ([COD_CALCULO], [IND_PARCELA], [NUMERO_TITULO], [VALOR_PRINCIPAL], 
             [VALOR_PRINCIPAL_RECEBIDO], [DT_VENCIMENTO], [DT_RECEBIMENTO], 
             [DIAS_ATRASO], [VALOR_JUROS], VALOR_HONORARIOS_RETIDO, VALOR_HONORARIOS_REPASSE, [VALOR_JUROS_RECEBIDO], [VALOR_JUROS_RETIDO], 
             [VALOR_JUROS_REPASSE], [VALOR_MULTA_RETIDO], [VALOR_MULTA_REPASSE], 
             [VALOR_CUSTAS], [VALOR_CUSTAS_RECEBIDO], [VALOR_SUB_TOTAL], [VALOR_RECEBIDO], 
             [VALOR_SALDO], [VALOR_COMISSAO], VALOR_COMISSAO_REPASSE, VALOR_COMISSAO_RETIDO, [VALOR_HONORARIOS], [VALOR_PRESTACAO_CONTAS], 
             [VALOR_MULTA], [VALOR_DESCONTO], [VALOR_MULTA_BASE])
        VALUES
            (@COD_CALCULO, @IND_PARCELA, @NUMERO_TITULO, @VALOR_PRINCIPAL, 
             @VALOR_PRINCIPAL_RECEBIDO, @DT_VENCIMENTO, @DT_RECEBIMENTO, 
             @DIAS_ATRASO, @VALOR_JUROS, @VALOR_HONORARIOS_RETIDO, @VALOR_HONORARIOS_REPASSE, @VALOR_JUROS_RECEBIDO, @VALOR_JUROS_RETIDO, 
             @VALOR_JUROS_REPASSE, @VALOR_MULTA_RETIDO, @VALOR_MULTA_REPASSE, 
             @VALOR_CUSTAS, @VALOR_CUSTAS_RECEBIDO, @VALOR_SUB_TOTAL, @VALOR_RECEBIDO, 
             @VALOR_SALDO, @VALOR_COMISSAO, @VALOR_COMISSAO_REPASSE,@VALOR_COMISSAO_RETIDO, @VALOR_HONORARIOS, @VALOR_PRESTACAO_CONTAS, 
             @VALOR_MULTA, @VALOR_DESCONTO, @VALOR_MULTA_BASE)";

    try
    {
      var result = await _dbConnection.ExecuteAsync(query, new
      {
        item.COD_CALCULO,
        item.IND_PARCELA,
        item.NUMERO_TITULO,
        item.VALOR_PRINCIPAL,
        item.VALOR_PRINCIPAL_RECEBIDO,
        item.DT_VENCIMENTO,
        item.DT_RECEBIMENTO,
        item.DIAS_ATRASO,
        item.VALOR_JUROS,
        item.VALOR_HONORARIOS_RETIDO,
        item.VALOR_HONORARIOS_REPASSE,
        item.VALOR_JUROS_RECEBIDO,
        item.VALOR_JUROS_RETIDO,
        item.VALOR_JUROS_REPASSE,
        item.VALOR_MULTA_RETIDO,
        item.VALOR_MULTA_REPASSE,
        item.VALOR_CUSTAS,
        item.VALOR_CUSTAS_RECEBIDO,
        item.VALOR_SUB_TOTAL,
        item.VALOR_RECEBIDO,
        item.VALOR_SALDO,
        item.VALOR_COMISSAO,
        item.VALOR_COMISSAO_RETIDO,
        item.VALOR_COMISSAO_REPASSE,
        item.VALOR_HONORARIOS,
        item.VALOR_PRESTACAO_CONTAS,
        item.VALOR_MULTA,
        item.VALOR_DESCONTO,
        item.VALOR_MULTA_BASE
      });

      return result > 0;
    }
    catch (Exception ex)
    {
      _logger.Error(ex, "Erro ao gravar PC_TITULO_CALCULO para título {NUMERO_TITULO}", item.NUMERO_TITULO);
      return false;
    }
  }

  public async Task<bool> GravaAcordo(AcordoTituloRequest acordo)
  {
    const string query = @"
        INSERT INTO [dbo].[ACORDO_TITULO]
            ([COD_CLIENTE_CREDOR], [NUMERO_BORDERO], [COD_CLIENTE_DEVEDOR], 
             [DT_CADASTRO_BORDERO], [DT_ACORDO], [DT_PRIMEIRO_VENCIMENTO], [DT_FECHAMENTO_CALCULO], 
             [IND_TIPO_CALCULO], [IND_TIPO_DOCUMENTO], [IND_SITUACAO_ACORDO], 
             [TAXA_COMISSAO_ACORDO], [TAXA_JUROS_ACORDO], [TAXA_MULTA_REPASSE], [TAXA_MULTA_RETIDO], 
             [VALOR_MULTA_RETIDO], [VALOR_MULTA_REPASSE],  [TAXA_HONORARIOS_REPASSE], [TAXA_HONORARIOS_RETIDO],[TAXA_COMISSAO_REPASSE], [TAXA_COMISSAO_RETIDO],  [TAXA_JUROS_REPASSE], [TAXA_JUROS_RETIDO], 
             [TAXA_HONORARIOS_ACORDO], [TAXA_CUSTAS_ACORDO], [VALOR_PRINCIPAL], [VALOR_JUROS], 
             [VALOR_CUSTAS], [VALOR_HONORARIOS], [VALOR_TOTAL], [VALOR_COMISSAO], VALOR_COMISSAO_REPASSE, VALOR_COMISSAO_RETIDO, 
             [QUANT_PARCELA_ACORDO], [VALOR_PARCELA_ACORDO], [OBS_ACORDO], [VALOR_JUROS_TOTAL], 
             [VALOR_JUROS_ANT], [TAXA_MULTA], [VALOR_MULTA], [COD_CALCULO], [IND_PAGAMENTO], 
             [TAXA_CAMPANHA], [VALOR_DESCONTO], VALOR_HONORARIOS_RETIDO, VALOR_HONORARIOS_REPASSE, [VALOR_JUROS_RETIDO], [VALOR_JUROS_REPASSE], 
             [TIPO_CPMF], [TAXA_CPMF], [VALOR_CPMF], [COD_FUNCIONARIO_ACORDO], [COD_FUNCIONARIO])
        VALUES
            (@COD_CLIENTE_CREDOR, @NUMERO_BORDERO, @COD_CLIENTE_DEVEDOR, 
             @DT_CADASTRO_BORDERO, @DT_ACORDO, @DT_PRIMEIRO_VENCIMENTO, @DT_FECHAMENTO_CALCULO, 
             @IND_TIPO_CALCULO, @IND_TIPO_DOCUMENTO, @IND_SITUACAO_ACORDO, 
             @TAXA_COMISSAO_ACORDO, @TAXA_JUROS_ACORDO,  @TAXA_HONORARIOS_REPASSE, @TAXA_HONORARIOS_RETIDO, @TAXA_COMISSAO_REPASSE, @TAXA_COMISSAO_RETIDO,  @TAXA_MULTA_REPASSE, @TAXA_MULTA_RETIDO, 
             @VALOR_MULTA_RETIDO, @VALOR_MULTA_REPASSE, @TAXA_JUROS_REPASSE, @TAXA_JUROS_RETIDO, 
             @TAXA_HONORARIOS_ACORDO, @TAXA_CUSTAS_ACORDO, @VALOR_PRINCIPAL, @VALOR_JUROS, 
             @VALOR_CUSTAS, @VALOR_HONORARIOS, @VALOR_TOTAL, @VALOR_COMISSAO, @VALOR_COMISSAO_REPASSE, @VALOR_COMISSAO_RETIDO, 
             @QUANT_PARCELA_ACORDO, @VALOR_PARCELA_ACORDO, @OBS_ACORDO, @VALOR_JUROS_TOTAL, 
             @VALOR_JUROS_ANT, @TAXA_MULTA, @VALOR_MULTA, @COD_CALCULO, 0, 
             @TAXA_CAMPANHA, @VALOR_DESCONTO, @VALOR_HONORARIOS_RETIDO, @VALOR_HONORARIOS_REPASSE, @VALOR_JUROS_RETIDO, @VALOR_JUROS_REPASSE, 
             @TIPO_CPMF, @TAXA_CPMF, @VALOR_CPMF, @COD_FUNCIONARIO, @COD_FUNCIONARIO)";

    try
    {
      var result = await _dbConnection.ExecuteAsync(query, new
      {
        acordo.COD_CLIENTE_CREDOR,
        acordo.NUMERO_BORDERO,
        acordo.COD_CLIENTE_DEVEDOR,
        acordo.DT_CADASTRO_BORDERO,
        acordo.DT_ACORDO,
        acordo.DT_PRIMEIRO_VENCIMENTO,
        acordo.DT_FECHAMENTO_CALCULO,
        acordo.IND_TIPO_CALCULO,
        acordo.IND_TIPO_DOCUMENTO,
        acordo.IND_SITUACAO_ACORDO,
        acordo.TAXA_COMISSAO_ACORDO,
        acordo.TAXA_JUROS_ACORDO,
        acordo.TAXA_HONORARIOS_REPASSE,
        acordo.TAXA_HONORARIOS_RETIDO,
        acordo.TAXA_COMISSAO_REPASSE,
        acordo.TAXA_COMISSAO_RETIDO,
        acordo.TAXA_MULTA_REPASSE,
        acordo.TAXA_MULTA_RETIDO,
        acordo.VALOR_MULTA_RETIDO,
        acordo.VALOR_MULTA_REPASSE,
        acordo.TAXA_JUROS_REPASSE,
        acordo.TAXA_JUROS_RETIDO,
        acordo.TAXA_HONORARIOS_ACORDO,
        acordo.TAXA_CUSTAS_ACORDO,
        acordo.VALOR_PRINCIPAL,
        acordo.VALOR_JUROS,
        acordo.VALOR_CUSTAS,
        acordo.VALOR_HONORARIOS,
        acordo.VALOR_TOTAL,
        acordo.VALOR_COMISSAO,
        acordo.VALOR_COMISSAO_REPASSE,
        acordo.VALOR_COMISSAO_RETIDO,
        acordo.QUANT_PARCELA_ACORDO,
        acordo.VALOR_PARCELA_ACORDO,
        acordo.OBS_ACORDO,
        acordo.VALOR_JUROS_TOTAL,
        acordo.VALOR_JUROS_ANT,
        acordo.TAXA_MULTA,
        acordo.VALOR_MULTA,
        acordo.COD_CALCULO,
        acordo.TAXA_CAMPANHA,
        acordo.VALOR_DESCONTO,
        acordo.VALOR_HONORARIOS_RETIDO,
        acordo.VALOR_HONORARIOS_REPASSE,
        acordo.VALOR_JUROS_RETIDO,
        acordo.VALOR_JUROS_REPASSE,
        acordo.TIPO_CPMF,
        acordo.TAXA_CPMF,
        acordo.VALOR_CPMF,
        acordo.COD_FUNCIONARIO
      });

      return result > 0;
    }
    catch (Exception ex)
    {
      _logger.Error(ex, "Erro ao gravar acordo para cliente {COD_CLIENTE_CREDOR}", acordo.COD_CLIENTE_CREDOR);
      return false;
    }
  }

  public async Task<int> GetIdAcordo()
  {
    const string query = "SELECT ISNULL(MAX(NUMERO_ACORDO), 0) FROM ACORDO_TITULO";

    try
    {
      return await _dbConnection.QueryFirstOrDefaultAsync<int>(query);
    }
    catch (Exception ex)
    {
      _logger.Error(ex, "Erro ao obter o último ID do acordo.");
      return 0;
    }
  }

  public async Task<bool> GravaPcTituloParam(AcordoPcTituloParamRequest pcTituloParam)
  {
    const string query = @"
        INSERT INTO [dbo].[PC_TITULO_PARAM] 
            (COD_CALCULO, COD_CLIENTE_CREDOR, NUMERO_BORDERO, COD_CLIENTE_DEVEDOR, NUMERO_ACORDO, 
             TAXA_COMISSAO, TAXA_JUROS, TAXA_HONORARIOS, TAXA_MULTA, IND_DIFERENCA, 
             VALOR_DIFERENCA, TIPO_CALCULO)
        VALUES
            (@COD_CALCULO, @COD_CLIENTE_CREDOR, @NUMERO_BORDERO, @COD_CLIENTE_DEVEDOR, @NUMERO_ACORDO, 
             @TAXA_COMISSAO, @TAXA_JUROS, @TAXA_HONORARIOS, @TAXA_MULTA, @IND_DIFERENCA, 
             @VALOR_DIFERENCA, @TIPO_CALCULO)";

    try
    {
      var result = await _dbConnection.ExecuteAsync(query, pcTituloParam);
      return result > 0;
    }
    catch (Exception ex)
    {
      _logger.Error(ex, "Erro ao gravar PC_TITULO_PARAM. Dados: {@pcTituloParam}", pcTituloParam);
      return false;
    }
  }

  public async Task<bool> GravaOcorrencia(AcordoOcorrenciaRequest acordoOcorrencia)
  {
    const string query = @"
        INSERT INTO [dbo].[OCORRENCIA_COBRANCA]
            (COD_CLIENTE_DEVEDOR, NUMERO_BORDERO, COD_CLIENTE_CREDOR, 
             DT_OCORRENCIA, DT_CADASTRO_BORDERO, DESCRICAO_OCORRENCIA, 
             IND_SITUACAO_OCORRENCIA)
        VALUES
            (@COD_CLIENTE_DEVEDOR, @NUMERO_BORDERO, @COD_CLIENTE_CREDOR, 
             @DT_OCORRENCIA, @DT_CADASTRO_BORDERO, @DESCRICAO_OCORRENCIA, 
             @IND_SITUACAO_OCORRENCIA)";

    try
    {
      var result = await _dbConnection.ExecuteAsync(query, acordoOcorrencia);
      return result > 0;
    }
    catch (Exception ex)
    {
      _logger.Error(ex, "Erro ao gravar OCORRENCIA_COBRANCA. Dados: {@acordoOcorrencia}", acordoOcorrencia);
      return false;
    }
  }

  public async Task<bool> GravaParcelas(AcordoParcelasRequest item)
  {
    const string query = @"
        INSERT INTO [dbo].[PARCELA_ACORDO_TITULO]
            (COD_CLIENTE_CREDOR, NUMERO_BORDERO, COD_CLIENTE_DEVEDOR,
             NUMERO_ACORDO, NUMERO_PARCELA, DT_CADASTRO_BORDERO, 
             DT_ACORDO, DT_VENCIMENTO_PARCELA, VALOR_PARCELA, 
             IND_CHEQUE_PARCELA, VALOR_COMISSAO, VALOR_COMISSAO_REPASSE, VALOR_COMISSAO_RETIDO, VALOR_PRINCIPAL, 
             VALOR_HONORARIOS, VALOR_PRESTACAO_CONTAS, VALOR_JUROS,
              VALOR_JUROS_RETIDO, VALOR_HONORARIOS_REPASSE, VALOR_HONORARIOS_RETIDO, VALOR_JUROS_REPASSE, VALOR_MULTA, 
             VALOR_MULTA_RETIDO, VALOR_MULTA_REPASSE, OBS_PARCELA, 
             IND_SITUACAO_PARCELA, COD_FUNCIONARIO, COD_FUNCIONARIO_ACORDO)
        VALUES
            (@COD_CLIENTE_CREDOR, @NUMERO_BORDERO, @COD_CLIENTE_DEVEDOR,
             @NUMERO_ACORDO, @NUMERO_PARCELA, @DT_CADASTRO_BORDERO, 
             @DT_ACORDO, @DT_VENCIMENTO_PARCELA, @VALOR_PARCELA, 
             @IND_CHEQUE_PARCELA, @VALOR_COMISSAO,@VALOR_COMISSAO_REPASSE,@VALOR_COMISSAO_RETIDO, @VALOR_PRINCIPAL, 
             @VALOR_HONORARIOS, @VALOR_PRESTACAO_CONTAS, @VALOR_JUROS,
             @VALOR_JUROS_RETIDO, @VALOR_HONORARIOS_REPASSE, @VALOR_HONORARIOS_RETIDO, @VALOR_JUROS_REPASSE, @VALOR_MULTA, 
             @VALOR_MULTA_RETIDO, @VALOR_MULTA_REPASSE, @OBS_PARCELA, 
             @IND_SITUACAO_PARCELA, @COD_FUNCIONARIO, @COD_FUNCIONARIO)";

    try
    {
      var result = await _dbConnection.ExecuteAsync(query, item);
      return result > 0;
    }
    catch (Exception ex)
    {
      _logger.Error(ex, "Erro ao gravar parcela do acordo. Dados: {@item}", item);
      return false;
    }
  }

  public async Task<bool> GravaPcTituloTabela(PrestacaoContasTituloRequest pcTituloTabela)
  {
    const string query = @"
        INSERT INTO PRESTACAO_CONTAS_TITULO (
            COD_CLIENTE_CREDOR, NUMERO_BORDERO, COD_CLIENTE_DEVEDOR, 
            NUMERO_ACORDO, NUMERO_PARCELA, NUMERO_TITULO, 
            VALOR_PRINCIPAL, DT_VENCIMENTO, DT_RECEBIMENTO, 
            ATRASO, VALOR_JUROS, VALOR_JUROS_RETIDO,VALOR_JUROS_REPASSE,VALOR_HONORARIOS_RETIDO, VALOR_HONORARIOS_REPASSE, VALOR_DESCONTO, VALOR_CUSTAS, 
            VALOR_SUB_TOTAL, VALOR_RECEBIDO, VALOR_SALDO, 
            VALOR_COMISSAO,VALOR_COMISSAO_REPASSE,VALOR_COMISSAO_RETIDO, VALOR_PRESTACAO_CONTAS, 
            VALOR_CPMF, VALOR_MULTA)
        SELECT  
            @COD_CLIENTE_CREDOR, @NUMERO_BORDERO, @COD_CLIENTE_DEVEDOR, 
            @NUMERO_ACORDO, IND_PARCELA, NUMERO_TITULO, 
            VALOR_PRINCIPAL_RECEBIDO, DT_VENCIMENTO, DT_RECEBIMENTO, 
            CAST(DIAS_ATRASO AS VARCHAR(10)), VALOR_JUROS_RECEBIDO, 
            VALOR_JUROS_RETIDO, VALOR_JUROS_REPASSE, 
            VALOR_HONORARIOS_RETIDO, VALOR_HONORARIOS_REPASSE, 
            VALOR_DESCONTO, VALOR_CUSTAS_RECEBIDO, VALOR_SUB_TOTAL,  
            VALOR_RECEBIDO, VALOR_SALDO, VALOR_COMISSAO, VALOR_COMISSAO_REPASSE, VALOR_COMISSAO_RETIDO, 
            VALOR_PRESTACAO_CONTAS, 0, VALOR_MULTA 
        FROM PC_TITULO_CALCULO 
        WHERE COD_CALCULO = @NUMERO_CALCULO
        ORDER BY IND_PARCELA";

    try
    {
      var parameters = new
      {
        pcTituloTabela.COD_CLIENTE_CREDOR,
        pcTituloTabela.NUMERO_BORDERO,
        pcTituloTabela.COD_CLIENTE_DEVEDOR,
        pcTituloTabela.NUMERO_ACORDO,
        pcTituloTabela.NUMERO_CALCULO
      };

      var result = await _dbConnection.ExecuteAsync(query, parameters);
      return result > 0;
    }
    catch (Exception ex)
    {
      _logger.Error(ex, "Erro ao gravar na tabela PRESTACAO_CONTAS_TITULO. Dados: {@pcTituloTabela}", pcTituloTabela);
      return false;
    }
  }
}