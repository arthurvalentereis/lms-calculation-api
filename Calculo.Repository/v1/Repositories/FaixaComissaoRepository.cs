using Calculo.Repository.v1.Repositories.Interfaces;
using Calculo.Core.Models;
using System.Data;
using Calculo.Core.Models.FaixaComissao;
using Dapper;
using Calculo.Core.Models.Titulo;

namespace Calculo.Repository.v1.Repositories;

public class FaixaComissaoRepository : IFaixaComissaoRepository
{
  private readonly IDbConnection _dbConnection;

  public FaixaComissaoRepository(IDbConnection dbConnection)
  {
    _dbConnection = dbConnection;
  }

  public async Task<bool> VerificaFaixaComissaoBordero(string cliente, string numeroBordero)
  {
    const string query = @"SELECT 1 FROM CLIENTE_CREDOR_COMISSAO_BORDERO 
                           WHERE COD_CLIENTE_CREDOR = @cliente 
                           AND NUMERO_BORDERO = @numeroBordero";

    return await _dbConnection.ExecuteScalarAsync<bool>(query, new { cliente, numeroBordero });
  }

  public async Task<bool> VerificaFaixaComissaoCadastro(string cliente)
  {
    const string query = @"SELECT 1 FROM CLIENTE_CREDOR_COMISSAO 
                           WHERE COD_CLIENTE_CREDOR = @cliente";

    return await _dbConnection.ExecuteScalarAsync<bool>(query, new { cliente });
  }

  public async Task<bool> VerificaFaixaComissaoBorderoPD(string cliente, string numeroBordero)
  {
    const string query = @"SELECT 1 FROM CLIENTE_CREDOR_COMISSAO_PD_BORDERO 
                           WHERE COD_CLIENTE_CREDOR = @cliente 
                           AND NUMERO_BORDERO = @numeroBordero";

    return await _dbConnection.ExecuteScalarAsync<bool>(query, new { cliente, numeroBordero });
  }

  public async Task<bool> VerificaFaixaComissaoCadastroPD(string cliente)
  {
    const string query = @"SELECT 1 FROM CLIENTE_CREDOR_COMISSAO_PD 
                           WHERE COD_CLIENTE_CREDOR = @cliente";

    return await _dbConnection.ExecuteScalarAsync<bool>(query, new { cliente });
  }

  public async Task<bool> VerificaFaixaComissaoBorderoPDA(string cliente, string numeroBordero)
  {
    const string query = @"SELECT 1 FROM CLIENTE_CREDOR_COMISSAO_PDA_BORDERO 
                           WHERE COD_CLIENTE_CREDOR = @cliente 
                           AND NUMERO_BORDERO = @numeroBordero";

    return await _dbConnection.ExecuteScalarAsync<bool>(query, new { cliente, numeroBordero });
  }

  public async Task<bool> VerificaFaixaComissaoCadastroPDA(string cliente)
  {
    const string query = @"SELECT 1 FROM CLIENTE_CREDOR_COMISSAO_PDA 
                           WHERE COD_CLIENTE_CREDOR = @cliente";

    return await _dbConnection.ExecuteScalarAsync<bool>(query, new { cliente });
  }

  public async Task<int> FaixaVencimentoTitulo(string cliente, string bordero, string devedor)
  {
    const string query = @"SELECT DT_CADASTRO_BORDERO AS dataTituloCadastro, 
                                  DT_VENCIMENTO_TITULO AS dataTituloVencimento 
                           FROM TITULO_COBRANCA 
                           WHERE COD_CLIENTE_CREDOR = @cliente 
                           AND NUMERO_BORDERO = @bordero 
                           AND COD_CLIENTE_DEVEDOR = @devedor 
                           ORDER BY DT_VENCIMENTO_TITULO";

    var result = await _dbConnection.QueryFirstOrDefaultAsync<TituloResult>(query, new { cliente, bordero, devedor });

    return Convert.ToInt32((result?.dataTituloCadastro - result?.dataTituloVencimento)?.TotalDays ?? 0);
  }

  public async Task<CarregaTaxasMaximasResult?> CarregaMaximasBorderoNormal(string numeroBordero)
  {
    const string query = @"SELECT 
                         CASE WHEN EXISTS (SELECT 1 
                         FROM CLIENTE_CREDOR_COMISSAO_BORDERO F 
                         WHERE F.NUMERO_BORDERO COLLATE Latin1_General_CS_AS = B.NUMERO_BORDERO)
                         THEN 'on' ELSE '' END AS maximaNormal, 
                                  LOAD_TAXAS_FAIXAS_PD AS maximaPD, 
                                  LOAD_TAXAS_FAIXAS_PDA AS maximaPDA 
                           FROM BORDERO_COBRANCA B
                           WHERE NUMERO_BORDERO = @numeroBordero";

    return await _dbConnection.QueryFirstOrDefaultAsync<CarregaTaxasMaximasResult>(query, new { numeroBordero });
  }

  public async Task<int> TipoFaixaComissaoBordero(string cliente, string numeroBordero)
  {
    const string query = @"SELECT IND_FAIXA_COMISSAO 
                           FROM BORDERO_COBRANCA 
                           WHERE NUMERO_BORDERO = @numeroBordero";

    var result = await _dbConnection.QueryFirstOrDefaultAsync<string>(query, new { numeroBordero });

    return int.TryParse(result, out int valor) ? valor : 0;
  }

  public async Task<FaixaComissaoResult?> BuscarTaxasBorderoNormal(string cliente, string numeroBordero)
  {
    const string query = @"SELECT BC.TAXA_COMISSAO_MIN_BORDERO AS comissaoMin, 
                                  BC.TAXA_COMISSAO_BORDERO AS comissaoMax, 
                                  BC.TAXA_JUROS_BORDERO AS jurosMin, 
                                  BC.TAXA_JUROS2_BORDERO AS jurosMax, 
                                  BC.TAXA_HONORARIOS_MIN_BORDERO AS honorarioMin, 
                                  BC.TAXA_HONORARIOS_BORDERO AS honorarioMax, 
                                  BC.TAXA_MULTA_BORDERO AS multa, 
                                  BC.TAXA_CAMPANHA_DE AS campanhaDe, 
                                  BC.TAXA_CAMPANHA_ATE AS campanhaAte, 
                                  TRY_CAST(NULLIF(CC.limite_parcelamento, '') AS INT) AS limiteParcelamento
                           FROM CLIENTE_CREDOR CC 
                           JOIN BORDERO_COBRANCA BC ON CC.COD_CLIENTE_CREDOR = BC.COD_CLIENTE_CREDOR
                           WHERE BC.NUMERO_BORDERO = @numeroBordero
                           AND CC.COD_CLIENTE_CREDOR = @cliente";

    return await _dbConnection.QueryFirstOrDefaultAsync<FaixaComissaoResult>(query, new { cliente, numeroBordero });
  }

  public async Task<int> FaixaVencimentoTituloDuplicata(string cliente, string bordero, string devedor)
  {
    const string query = @"SELECT DT_CADASTRO_BORDERO AS dataTituloCadastro, 
                                  DT_VENCIMENTO_TITULO AS dataTituloVencimento 
                           FROM TITULO_COBRANCA 
                           WHERE COD_CLIENTE_CREDOR = @cliente 
                           AND NUMERO_BORDERO = @bordero 
                           AND COD_CLIENTE_DEVEDOR = @devedor 
                           ORDER BY DT_VENCIMENTO_TITULO";

    var result = await _dbConnection.QueryFirstOrDefaultAsync<TituloResult>(query, new { cliente, bordero, devedor });

    return Convert.ToInt32((DateTime.Now - result?.dataTituloVencimento)?.TotalDays ?? 0);
  }

  public async Task<int> FaixaVencimentoTituloCadastro(string cliente, string bordero, string devedor)
  {
    const string query = @"SELECT DT_CADASTRO_BORDERO AS dataTituloCadastro, 
                                  DT_VENCIMENTO_TITULO AS dataTituloVencimento 
                           FROM TITULO_COBRANCA 
                           WHERE COD_CLIENTE_CREDOR = @cliente 
                           AND NUMERO_BORDERO = @bordero 
                           AND COD_CLIENTE_DEVEDOR = @devedor 
                           ORDER BY DT_VENCIMENTO_TITULO";

    var result = await _dbConnection.QueryFirstOrDefaultAsync<TituloResult>(query, new { cliente, bordero, devedor });

    return Convert.ToInt32((DateTime.Now - result?.dataTituloCadastro)?.TotalDays ?? 0);
  }

  public async Task<int> FaixaVencimentoTituloPeriodoVencido(string cliente, string bordero, string devedor)
  {
    const string query = @"SELECT DT_CADASTRO_BORDERO AS dataTituloCadastro, 
                                  DT_VENCIMENTO_TITULO AS dataTituloVencimento 
                           FROM TITULO_COBRANCA 
                           WHERE COD_CLIENTE_CREDOR = @cliente 
                           AND NUMERO_BORDERO = @bordero 
                           AND COD_CLIENTE_DEVEDOR = @devedor 
                           ORDER BY DT_VENCIMENTO_TITULO ASC";

    var result = await _dbConnection.QueryFirstOrDefaultAsync<TituloResult>(query, new { cliente, bordero, devedor });

    return Convert.ToInt32((result?.dataTituloCadastro - result?.dataTituloCadastro)?.TotalDays ?? 0);
  }

  public async Task<FaixaComissaoResult?> BuscarFaixaComissaoBorderoNormal(string credor, string devedor, TipoFaixaComissao tipoFaixa, string numeroBordero, int diasAtraso)
  {
    FaixaComissaoResult? list = null;

    if (tipoFaixa == TipoFaixaComissao.Valor)
    {
      const string queryValor = @"SELECT VALOR_PRINCIPAL 
                                    FROM CLIENTE_DEVEDOR 
                                    WHERE COD_CLIENTE_DEVEDOR = @devedor";

      double valor = await _dbConnection.ExecuteScalarAsync<double>(queryValor, new { devedor });

      const string query = @"SELECT 
                                    CCC.VALOR_DE AS valorDe, 
                                    CCC.VALOR_ATE AS valorAte, 
                                    CCC.ATRASO_DE AS atrasoDe, 
                                    CCC.ATRASO_ATE AS atrasoAte, 
                                    CCC.TAXA_JUROS_MAX_CREDOR AS jurosMax, 
                                    CCC.TAXA_JUROS_MIN_CREDOR AS jurosMin, 
                                    CCC.TAXA_MULTA_CREDOR AS multa, 
                                    CCC.TAXA_MULTA_REPASSE_CREDOR AS multaRepasse, 
                                    CCC.TAXA_JUROS_REPASSE_CREDOR AS jurosRepasse, 
                                    CCC.TAXA_HONORARIOS_REPASSE_CREDOR AS honorariosRepasse, 
                                    CCC.TAXA_COMISSAO_REPASSE_CREDOR AS comissaoRepasse, 
                                    CCC.TAXA_HONORARIOS_CREDOR AS honorarioMax, 
                                    CCC.TAXA_HONORARIOS_MIN_CREDOR AS honorarioMin, 
                                    CCC.TAXA_COMISSAO_CREDOR AS comissaoMax, 
                                    CCC.OBS_COMISSAO AS obsComissao, 
                                    CCC.TAXA_COMISSAO_MIN_CREDOR AS comissaoMin, 
                                    CCC.LIMITE_PARCELAMENTO AS limiteParcelamento, 
                                    CCC.TAXA_DESCONTO AS desconto ,
                                    CCC.TAXA_DESCONTO_MIN AS descontoMin
                                FROM CLIENTE_CREDOR_COMISSAO_BORDERO CCC
                                
                                WHERE CCC.COD_CLIENTE_CREDOR = @credor 
                                  AND @valor BETWEEN CCC.VALOR_DE AND CCC.VALOR_ATE  
                                  AND CCC.NUMERO_BORDERO = @numeroBordero 
                                ORDER BY CCC.COD_CLIENTE_CREDOR, CCC.COD_COMISSAO";

      list = await _dbConnection.QueryFirstOrDefaultAsync<FaixaComissaoResult>(query, new { credor, valor, numeroBordero });
    }
    else if (tipoFaixa == TipoFaixaComissao.Dias || tipoFaixa == TipoFaixaComissao.DiasDeDuplicata)
    {
      const string query = @"
                            SELECT 
                                CCC.VALOR_DE AS valorDe, 
                                CCC.VALOR_ATE AS valorAte, 
                                CCC.ATRASO_DE AS atrasoDe, 
                                CCC.ATRASO_ATE AS atrasoAte, 
                                CCC.TAXA_JUROS_MAX_CREDOR AS jurosMax, 
                                CCC.TAXA_JUROS_MIN_CREDOR AS jurosMin, 
                                CCC.TAXA_MULTA_CREDOR AS multa, 
                                CCC.TAXA_MULTA_REPASSE_CREDOR AS multaRepasse, 
                                CCC.TAXA_JUROS_REPASSE_CREDOR AS jurosRepasse, 
                                CCC.TAXA_HONORARIOS_REPASSE_CREDOR AS honorariosRepasse, 
                                CCC.TAXA_HONORARIOS_CREDOR AS honorarioMax, 
                                CCC.TAXA_HONORARIOS_MIN_CREDOR AS honorarioMin, 
                                CCC.TAXA_COMISSAO_CREDOR AS comissaoMax, 
                                CCC.TAXA_COMISSAO_REPASSE_CREDOR AS comissaoRepasse, 
                                CCC.OBS_COMISSAO AS obsComissao, 
                                CCC.TAXA_COMISSAO_MIN_CREDOR AS comissaoMin, 
                                CCC.LIMITE_PARCELAMENTO AS limiteParcelamento, 
                                CCC.TAXA_DESCONTO AS desconto,
                                CCC.TAXA_DESCONTO_MIN AS descontoMin
                            FROM CLIENTE_CREDOR_COMISSAO_BORDERO CCC  
                            WHERE CCC.COD_CLIENTE_CREDOR = @credor 
                              AND @diasAtraso BETWEEN CCC.ATRASO_DE AND CCC.ATRASO_ATE  
                              AND CCC.NUMERO_BORDERO = @numeroBordero 
                            ORDER BY CCC.COD_CLIENTE_CREDOR, CCC.COD_COMISSAO";

      list = await _dbConnection.QueryFirstOrDefaultAsync<FaixaComissaoResult>(query, new { credor, diasAtraso, numeroBordero });
    }
    else if (tipoFaixa == TipoFaixaComissao.ValorEDias)
    {
      const string queryValor = @"SELECT VALOR_PRINCIPAL 
                                    FROM CLIENTE_DEVEDOR 
                                    WHERE COD_CLIENTE_DEVEDOR = @devedor";

      double valor = await _dbConnection.ExecuteScalarAsync<double>(queryValor, new { devedor });

      const string query = @"SELECT 
                                    CCC.VALOR_DE AS valorDe, 
                                    CCC.VALOR_ATE AS valorAte, 
                                    CCC.ATRASO_DE AS atrasoDe, 
                                    CCC.ATRASO_ATE AS atrasoAte, 
                                    CCC.TAXA_JUROS_MAX_CREDOR AS jurosMax, 
                                    CCC.TAXA_JUROS_MIN_CREDOR AS jurosMin, 
                                    CCC.TAXA_MULTA_CREDOR AS multa, 
                                    CCC.TAXA_MULTA_REPASSE_CREDOR AS multaRepasse, 
                                    CCC.TAXA_JUROS_REPASSE_CREDOR AS jurosRepasse, 
                                    CCC.TAXA_HONORARIOS_REPASSE_CREDOR AS honorariosRepasse, 
                                    CCC.TAXA_COMISSAO_REPASSE_CREDOR AS comissaoRepasse, 
                                    CCC.TAXA_HONORARIOS_CREDOR AS honorarioMax, 
                                    CCC.TAXA_HONORARIOS_MIN_CREDOR AS honorarioMin, 
                                    CCC.TAXA_COMISSAO_CREDOR AS comissaoMax, 
                                    CCC.OBS_COMISSAO AS obsComissao, 
                                    CCC.TAXA_COMISSAO_MIN_CREDOR AS comissaoMin, 
                                    CCC.LIMITE_PARCELAMENTO AS limiteParcelamento, 
                                    CCC.TAXA_DESCONTO AS desconto ,
                                    CCC.TAXA_DESCONTO_MIN AS descontoMin
                                FROM CLIENTE_CREDOR_COMISSAO_BORDERO CCC
                                WHERE CCC.COD_CLIENTE_CREDOR = @credor 
                                  AND @diasAtraso BETWEEN CCC.ATRASO_DE AND CCC.ATRASO_ATE  
                                  AND @valor BETWEEN CCC.VALOR_DE AND CCC.VALOR_ATE 
                                  AND CCC.NUMERO_BORDERO = @numeroBordero 
                                ORDER BY CCC.COD_CLIENTE_CREDOR, CCC.COD_COMISSAO";

      list = await _dbConnection.QueryFirstOrDefaultAsync<FaixaComissaoResult>(query, new { credor, diasAtraso, valor, numeroBordero });
    }

    return list;
  }

  public async Task<int> TipoFaixaComissaoCadastro(string credor)
  {
    const string query = @"SELECT IND_FAIXA_COMISSAO
                           FROM CLIENTE_CREDOR 
                           WHERE COD_CLIENTE_CREDOR = @credor";

    return await _dbConnection.ExecuteScalarAsync<int>(query, new { credor });
  }

  public async Task<CarregaTaxasMaximasResult?> CarregaMaximasNormal(string credor)
  {
    const string query = @"SELECT LOAD_TAXAS_FAIXAS AS maximaNormal, 
                                  LOAD_TAXAS_FAIXAS_PD AS maximaPD, 
                                  LOAD_TAXAS_FAIXAS_PDA AS maximaPDA 
                           FROM CLIENTE_CREDOR 
                           WHERE COD_CLIENTE_CREDOR = @credor";

    return await _dbConnection.QueryFirstOrDefaultAsync<CarregaTaxasMaximasResult>(query, new { credor });
  }

  public async Task<int> TipoFaixaComissaoBorderoPD(string cliente, string numeroBordero)
  {
    const string query = @"SELECT IND_FAIXA_COMISSAO_PD 
                           FROM BORDERO_COBRANCA 
                           WHERE NUMERO_BORDERO = @numeroBordero";

    return await _dbConnection.ExecuteScalarAsync<int>(query, new { numeroBordero });
  }

  public async Task<FaixaComissaoResult?> BuscarTaxasBorderoPD(string credor, string numeroBordero)
  {
    const string query = @"SELECT BC.TAXA_COMISSAO_PD_BORDERO AS comissaoMax, 
                                  BC.TAXA_JUROS_PD_BORDERO AS jurosMax, 
                                  CC.limite_parcelamento AS limiteParcelamento 
                           FROM CLIENTE_CREDOR CC 
                           JOIN BORDERO_COBRANCA BC ON CC.COD_CLIENTE_CREDOR = BC.COD_CLIENTE_CREDOR 
                           WHERE BC.NUMERO_BORDERO = @numeroBordero 
                           AND CC.COD_CLIENTE_CREDOR = @credor";

    return await _dbConnection.QueryFirstOrDefaultAsync<FaixaComissaoResult>(query, new { credor, numeroBordero });
  }

  public async Task<FaixaComissaoResult?> BuscarFaixaComissaoBorderoPD(string credor, string devedor, TipoFaixaComissao tipoFaixa, string numeroBordero, int diasAtraso)
  {
    FaixaComissaoResult? list = null;

    if (tipoFaixa == TipoFaixaComissao.Valor)
    {
      const string queryValor = @"SELECT VALOR_PRINCIPAL FROM CLIENTE_DEVEDOR WHERE COD_CLIENTE_DEVEDOR = @devedor";
      double valor = await _dbConnection.QueryFirstOrDefaultAsync<double>(queryValor, new { devedor });

      const string query = @"
                                SELECT 
                                    CCC.VALOR_DE AS valorDe, 
                                    CCC.VALOR_ATE AS valorAte, 
                                    CCC.ATRASO_DE AS atrasoDe, 
                                    CCC.ATRASO_ATE AS atrasoAte, 
                                    CCC.TAXA_JUROS_MAX_CREDOR AS jurosMax, 
                                    CCC.TAXA_JUROS_MIN_CREDOR AS jurosMin, 
                                    CCC.TAXA_MULTA_CREDOR AS multa, 
                                    CCC.TAXA_MULTA_REPASSE_CREDOR AS multaRepasse, 
                                    CCC.TAXA_JUROS_REPASSE_CREDOR AS jurosRepasse, 
                                    CCC.TAXA_HONORARIOS_REPASSE_CREDOR AS honorariosRepasse, 
                                    CCC.TAXA_COMISSAO_REPASSE_CREDOR AS comissaoRepasse, 
                                    CCC.TAXA_HONORARIOS_CREDOR AS honorarioMax, 
                                    CCC.TAXA_HONORARIOS_MIN_CREDOR AS honorarioMin, 
                                    CCC.TAXA_COMISSAO_CREDOR AS comissaoMax, 
                                    CCC.OBS_COMISSAO AS obsComissao, 
                                    CCC.TAXA_COMISSAO_MIN_CREDOR AS comissaoMin, 
                                    ISNULL(NULLIF(LTRIM(RTRIM(CC.LIMITE_PARCELAMENTO)), ''), 0) AS limiteParcelamento, 
                                    CCC.TAXA_DESCONTO AS desconto 
                                FROM CLIENTE_CREDOR_COMISSAO_PD_BORDERO CCC
                                INNER JOIN CLIENTE_CREDOR CC ON CC.COD_CLIENTE_CREDOR collate Latin1_General_CI_AS = CCC.COD_CLIENTE_CREDOR
                                WHERE CCC.COD_CLIENTE_CREDOR = @credor
                                  AND @valor BETWEEN CCC.VALOR_DE AND CCC.VALOR_ATE
                                  AND CCC.NUMERO_BORDERO = @numeroBordero
                                ORDER BY CCC.COD_CLIENTE_CREDOR, CCC.COD_COMISSAO";

      list = await _dbConnection.QueryFirstOrDefaultAsync<FaixaComissaoResult>(query, new { credor, valor, numeroBordero });
    }
    else if (tipoFaixa == TipoFaixaComissao.Dias)
    {
      const string query = @"
                                SELECT 
                                    CCC.VALOR_DE AS valorDe,
                                    CCC.VALOR_ATE AS valorAte,
                                    CCC.ATRASO_DE AS atrasoDe,
                                    CCC.ATRASO_ATE AS atrasoAte,
                                    CCC.TAXA_JUROS_MAX_CREDOR AS jurosMax,
                                    CCC.TAXA_JUROS_MIN_CREDOR AS jurosMin,
                                    CCC.TAXA_MULTA_CREDOR AS multa,
                                    CCC.TAXA_MULTA_REPASSE_CREDOR AS multaRepasse,
                                    CCC.TAXA_JUROS_REPASSE_CREDOR AS jurosRepasse,
                                    CCC.TAXA_HONORARIOS_REPASSE_CREDOR AS honorariosRepasse,
                                    CCC.TAXA_COMISSAO_REPASSE_CREDOR AS comissaoRepasse, 
                                    CCC.TAXA_HONORARIOS_CREDOR AS honorarioMax,
                                    CCC.TAXA_HONORARIOS_MIN_CREDOR AS honorarioMin,
                                    CCC.TAXA_COMISSAO_CREDOR AS comissaoMax,
                                    CCC.OBS_COMISSAO AS obsComissao,
                                    CCC.TAXA_COMISSAO_MIN_CREDOR AS comissaoMin,
                                    ISNULL(NULLIF(LTRIM(RTRIM(CC.LIMITE_PARCELAMENTO)), ''), 0) AS limiteParcelamento,
                                    CCC.TAXA_DESCONTO AS desconto
                                FROM CLIENTE_CREDOR_COMISSAO_PD_BORDERO CCC
                                INNER JOIN CLIENTE_CREDOR CC ON CC.COD_CLIENTE_CREDOR collate Latin1_General_CI_AS = CCC.COD_CLIENTE_CREDOR
                                WHERE CCC.COD_CLIENTE_CREDOR = @credor
                                  AND @diasAtraso BETWEEN CONVERT(INT, CCC.ATRASO_DE) AND CONVERT(INT, CCC.ATRASO_ATE)
                                  AND CCC.NUMERO_BORDERO = @numeroBordero
                                ORDER BY CCC.COD_CLIENTE_CREDOR, CCC.COD_COMISSAO";

      list = await _dbConnection.QueryFirstOrDefaultAsync<FaixaComissaoResult>(query, new { credor, diasAtraso, numeroBordero });
    }
    else if (tipoFaixa == TipoFaixaComissao.ValorEDias)
    {
      const string queryValor = @"SELECT VALOR_PRINCIPAL FROM CLIENTE_DEVEDOR WHERE COD_CLIENTE_DEVEDOR = @devedor";
      double valor = await _dbConnection.QueryFirstOrDefaultAsync<double>(queryValor, new { devedor });

      const string query = @"
                            SELECT 
                                CCC.VALOR_DE AS valorDe,
                                CCC.VALOR_ATE AS valorAte,
                                CCC.ATRASO_DE AS atrasoDe,
                                CCC.ATRASO_ATE AS atrasoAte,
                                CCC.TAXA_JUROS_MAX_CREDOR AS jurosMax,
                                CCC.TAXA_JUROS_MIN_CREDOR AS jurosMin,
                                CCC.TAXA_MULTA_CREDOR AS multa,
                                CCC.TAXA_MULTA_REPASSE_CREDOR AS multaRepasse,
                                CCC.TAXA_JUROS_REPASSE_CREDOR AS jurosRepasse,
                                CCC.TAXA_HONORARIOS_RETIDO_CREDOR AS honorariosRetido,
                                CCC.TAXA_HONORARIOS_REPASSE_CREDOR AS honorariosRepasse,
                                CCC.TAXA_COMISSAO_REPASSE_CREDOR AS comissaoRepasse, 
                                CCC.TAXA_HONORARIOS_CREDOR AS honorarioMax,
                                CCC.TAXA_HONORARIOS_MIN_CREDOR AS honorarioMin,
                                CCC.TAXA_COMISSAO_CREDOR AS comissaoMax,
                                CCC.OBS_COMISSAO AS obsComissao,
                                CCC.TAXA_COMISSAO_MIN_CREDOR AS comissaoMin,
                                ISNULL(NULLIF(LTRIM(RTRIM(CC.LIMITE_PARCELAMENTO)), ''), 0) AS limiteParcelamento,
                                CCC.TAXA_DESCONTO AS desconto
                            FROM CLIENTE_CREDOR_COMISSAO_PD_BORDERO CCC
                            INNER JOIN CLIENTE_CREDOR CC ON CC.COD_CLIENTE_CREDOR collate Latin1_General_CI_AS = CCC.COD_CLIENTE_CREDOR
                            WHERE CCC.COD_CLIENTE_CREDOR = @credor
                              AND @diasAtraso BETWEEN CONVERT(INT, CCC.ATRASO_DE) AND CONVERT(INT, CCC.ATRASO_ATE)
                              AND @valor BETWEEN CCC.VALOR_DE AND CCC.VALOR_ATE
                              AND CCC.NUMERO_BORDERO = @numeroBordero
                            ORDER BY CCC.COD_CLIENTE_CREDOR, CCC.COD_COMISSAO";

      list = await _dbConnection.QueryFirstOrDefaultAsync<FaixaComissaoResult>(query, new { credor, diasAtraso, valor, numeroBordero });
    }

    return list;
  }

  public async Task<int> TipoFaixaComissaoCadastroPD(string credor)
  {
    const string query = @"SELECT IND_FAIXA_COMISSAO_PD FROM CLIENTE_CREDOR WHERE COD_CLIENTE_CREDOR = @credor";

    return await _dbConnection.ExecuteScalarAsync<int>(query, new { credor });
  }

  public async Task<FaixaComissaoResult?> BuscarFaixaComissaoPD(string credor, string devedor, TipoFaixaComissao tipoFaixa, int diasAtraso)
  {
    FaixaComissaoResult? list = null;

    if (tipoFaixa == TipoFaixaComissao.Valor)
    {
      const string queryValor = @"SELECT VALOR_PRINCIPAL FROM CLIENTE_DEVEDOR WHERE COD_CLIENTE_DEVEDOR = @devedor";
      double valor = await _dbConnection.QueryFirstOrDefaultAsync<double>(queryValor, new { devedor });

      const string query = @"
                                SELECT 
                                    CCC.VALOR_DE AS valorDe,
                                    CCC.VALOR_ATE AS valorAte,
                                    CCC.ATRASO_DE AS atrasoDe,
                                    CCC.ATRASO_ATE AS atrasoAte,
                                    CCC.TAXA_JUROS_MAX_CREDOR AS jurosMax,
                                    CCC.TAXA_JUROS_MIN_CREDOR AS jurosMin,
                                    CCC.TAXA_MULTA_CREDOR AS multa,
                                    CCC.TAXA_MULTA_REPASSE_CREDOR AS multaRepasse,
                                    CCC.TAXA_JUROS_REPASSE_CREDOR AS jurosRepasse,
                                    CCC.TAXA_HONORARIOS_REPASSE_CREDOR AS honorariosRepasse,
                                    CCC.TAXA_COMISSAO_REPASSE_CREDOR AS comissaoRepasse, 
                                    CCC.TAXA_HONORARIOS_CREDOR AS honorarioMax,
                                    CCC.TAXA_HONORARIOS_MIN_CREDOR AS honorarioMin,
                                    CCC.TAXA_COMISSAO_CREDOR AS comissaoMax,
                                    CCC.OBS_COMISSAO AS obsComissao,
                                    CCC.TAXA_COMISSAO_MIN_CREDOR AS comissaoMin,
                                    ISNULL(NULLIF(LTRIM(RTRIM(CC.LIMITE_PARCELAMENTO)), ''), 0) AS limiteParcelamento,
                                    CCC.TAXA_DESCONTO AS desconto
                                FROM CLIENTE_CREDOR_COMISSAO_PD CCC
                                INNER JOIN CLIENTE_CREDOR CC ON CC.COD_CLIENTE_CREDOR collate Latin1_General_CI_AS = CCC.COD_CLIENTE_CREDOR
                                WHERE CCC.COD_CLIENTE_CREDOR = @credor
                                  AND @valor BETWEEN CCC.VALOR_DE AND CCC.VALOR_ATE
                                ORDER BY CCC.COD_CLIENTE_CREDOR, CCC.COD_COMISSAO";

      list = await _dbConnection.QueryFirstOrDefaultAsync<FaixaComissaoResult>(query, new { credor, valor });
    }
    else if (tipoFaixa == TipoFaixaComissao.Dias)
    {
      const string query = @"
                            SELECT 
                                CCC.VALOR_DE AS valorDe,
                                CCC.VALOR_ATE AS valorAte,
                                CCC.ATRASO_DE AS atrasoDe,
                                CCC.ATRASO_ATE AS atrasoAte,
                                CCC.TAXA_JUROS_MAX_CREDOR AS jurosMax,
                                CCC.TAXA_JUROS_MIN_CREDOR AS jurosMin,
                                CCC.TAXA_MULTA_CREDOR AS multa,
                                CCC.TAXA_MULTA_REPASSE_CREDOR AS multaRepasse,
                                CCC.TAXA_JUROS_REPASSE_CREDOR AS jurosRepasse,
                                CCC.TAXA_HONORARIOS_REPASSE_CREDOR AS honorariosRepasse,
                                CCC.TAXA_COMISSAO_REPASSE_CREDOR AS comissaoRepasse, 
                                CCC.TAXA_HONORARIOS_CREDOR AS honorarioMax,
                                CCC.TAXA_HONORARIOS_MIN_CREDOR AS honorarioMin,
                                CCC.TAXA_COMISSAO_CREDOR AS comissaoMax,
                                CCC.OBS_COMISSAO AS obsComissao,
                                CCC.TAXA_COMISSAO_MIN_CREDOR AS comissaoMin,
                                ISNULL(NULLIF(LTRIM(RTRIM(CC.LIMITE_PARCELAMENTO)), ''), 0) AS limiteParcelamento,
                                CCC.TAXA_DESCONTO AS desconto
                            FROM CLIENTE_CREDOR_COMISSAO_PD CCC
                            INNER JOIN CLIENTE_CREDOR CC ON CC.COD_CLIENTE_CREDOR collate Latin1_General_CI_AS = CCC.COD_CLIENTE_CREDOR
                            WHERE CCC.COD_CLIENTE_CREDOR = @credor
                              AND @diasAtraso BETWEEN CONVERT(INT, CCC.ATRASO_DE) AND CONVERT(INT, CCC.ATRASO_ATE)
                            ORDER BY CCC.COD_CLIENTE_CREDOR, CCC.COD_COMISSAO";

      list = await _dbConnection.QueryFirstOrDefaultAsync<FaixaComissaoResult>(query, new { credor, diasAtraso });
    }

    return list;
  }

  public async Task<int> TipoFaixaComissaoBorderoPDA(string cliente, string numeroBordero)
  {
    const string query = @"SELECT IND_FAIXA_COMISSAO_PDA FROM BORDERO_COBRANCA WHERE NUMERO_BORDERO = @numeroBordero";

    return await _dbConnection.ExecuteScalarAsync<int>(query, new { numeroBordero });
  }

  public async Task<FaixaComissaoResult?> BuscarTaxasBorderoPDA(string cliente, string numeroBordero)
  {
    const string query = @"SELECT BC.TAXA_COMISSAO_PDA_BORDERO AS comissaoMax,
                                  BC.TAXA_JUROS_PDA_BORDERO AS jurosMax,
                                  CC.limite_parcelamento AS limiteParcelamento
                           FROM cliente_credor CC
                           INNER JOIN BORDERO_COBRANCA BC ON CC.COD_CLIENTE_CREDOR = BC.COD_CLIENTE_CREDOR
                           WHERE BC.NUMERO_BORDERO = @numeroBordero 
                           AND CC.COD_CLIENTE_CREDOR = @cliente";

    return await _dbConnection.QueryFirstOrDefaultAsync<FaixaComissaoResult>(query, new { cliente, numeroBordero });
  }

  public async Task<int> TipoFaixaComissaoCadastroPDA(string credor)
  {
    const string query = @"SELECT IND_FAIXA_COMISSAO_PDA FROM CLIENTE_CREDOR WHERE COD_CLIENTE_CREDOR = @credor";

    return await _dbConnection.ExecuteScalarAsync<int>(query, new { credor });
  }

  public async Task<FaixaComissaoResult?> BuscarFaixaComissaoBorderoPDA(string credor, string devedor, TipoFaixaComissao tipoFaixa, string numeroBordero, int diasAtraso)
  {
    FaixaComissaoResult? result = null;

    if (tipoFaixa == TipoFaixaComissao.Valor)
    {
      const string queryValor = @"SELECT VALOR_PRINCIPAL FROM CLIENTE_DEVEDOR WHERE COD_CLIENTE_DEVEDOR = @devedor";
      double valor = await _dbConnection.QueryFirstOrDefaultAsync<double>(queryValor, new { devedor });

      const string query = @"SELECT 
                                    VALOR_DE AS valorDe, 
                                    VALOR_ATE AS valorAte, 
                                    ATRASO_DE AS atrasoDe, 
                                    ATRASO_ATE AS atrasoAte,
                                    TAXA_JUROS_MAX_CREDOR AS jurosMax, 
                                    TAXA_JUROS_MIN_CREDOR AS jurosMin, 
                                    TAXA_MULTA_CREDOR AS multa, 
                                    TAXA_MULTA_REPASSE_CREDOR AS multaRepasse, 
                                    TAXA_JUROS_REPASSE_CREDOR AS jurosRepasse, 
                                    TAXA_HONORARIOS_REPASSE_CREDOR AS honorariosRepasse,
                                    
                                    CCC.TAXA_COMISSAO_REPASSE_CREDOR AS comissaoRepasse, 
                                    CCC.TAXA_HONORARIOS_CREDOR AS honorarioMax, 
                                    TAXA_HONORARIOS_MIN_CREDOR AS honorarioMin, 
                                    CCC.TAXA_COMISSAO_CREDOR AS comissaoMax, 
                                    OBS_COMISSAO AS obsComissao, 
                                    TAXA_COMISSAO_MIN_CREDOR AS comissaoMin, 
                                    ISNULL(NULLIF(LTRIM(RTRIM(CC.LIMITE_PARCELAMENTO)), ''), 0) AS limiteParcelamento, 
                                    TAXA_DESCONTO AS desconto
                                FROM CLIENTE_CREDOR_COMISSAO_PDA_BORDERO CCC
                                INNER JOIN CLIENTE_CREDOR CC ON CC.COD_CLIENTE_CREDOR collate Latin1_General_CI_AS = CCC.COD_CLIENTE_CREDOR
                                WHERE CCC.COD_CLIENTE_CREDOR = @credor
                                  AND @valor BETWEEN VALOR_DE AND VALOR_ATE
                                  AND NUMERO_BORDERO = @numeroBordero
                                ORDER BY CCC.COD_CLIENTE_CREDOR, COD_COMISSAO";

      result = await _dbConnection.QueryFirstOrDefaultAsync<FaixaComissaoResult>(query, new { credor, valor, numeroBordero });
    }
    else if (tipoFaixa == TipoFaixaComissao.Dias)
    {
      const string query = @"SELECT 
                                    VALOR_DE AS valorDe, 
                                    VALOR_ATE AS valorAte, 
                                    ATRASO_DE AS atrasoDe, 
                                    ATRASO_ATE AS atrasoAte,
                                    TAXA_JUROS_MAX_CREDOR AS jurosMax, 
                                    TAXA_JUROS_MIN_CREDOR AS jurosMin, 
                                    TAXA_MULTA_CREDOR AS multa, 
                                    TAXA_MULTA_REPASSE_CREDOR AS multaRepasse, 
                                    TAXA_JUROS_REPASSE_CREDOR AS jurosRepasse,  
                                    TAXA_HONORARIOS_REPASSE_CREDOR AS honorariosRepasse,
                                    CCC.TAXA_COMISSAO_REPASSE_CREDOR AS comissaoRepasse, 
                                    CCC.TAXA_HONORARIOS_CREDOR AS honorarioMax, 
                                    TAXA_HONORARIOS_MIN_CREDOR AS honorarioMin, 
                                    CCC.TAXA_COMISSAO_CREDOR AS comissaoMax, 
                                    OBS_COMISSAO AS obsComissao, 
                                    TAXA_COMISSAO_MIN_CREDOR AS comissaoMin, 
                                    ISNULL(NULLIF(LTRIM(RTRIM(CC.LIMITE_PARCELAMENTO)), ''), 0) AS limiteParcelamento, 
                                    TAXA_DESCONTO AS desconto
                                FROM CLIENTE_CREDOR_COMISSAO_PDA_BORDERO CCC
                                INNER JOIN CLIENTE_CREDOR CC ON CC.COD_CLIENTE_CREDOR collate Latin1_General_CI_AS = CCC.COD_CLIENTE_CREDOR
                                WHERE CCC.COD_CLIENTE_CREDOR = @credor
                                  AND @diasAtraso BETWEEN CONVERT(INT, ATRASO_DE) AND CONVERT(INT, ATRASO_ATE)
                                  AND NUMERO_BORDERO = @numeroBordero
                                ORDER BY CCC.COD_CLIENTE_CREDOR, COD_COMISSAO";

      result = await _dbConnection.QueryFirstOrDefaultAsync<FaixaComissaoResult>(query, new { credor, diasAtraso, numeroBordero });
    }
    else if (tipoFaixa == TipoFaixaComissao.ValorEDias)
    {
      const string queryValor = @"SELECT VALOR_PRINCIPAL FROM CLIENTE_DEVEDOR WHERE COD_CLIENTE_DEVEDOR = @devedor";
      double valor = await _dbConnection.QueryFirstOrDefaultAsync<double>(queryValor, new { devedor });

      const string query = @"SELECT 
                                    VALOR_DE AS valorDe, 
                                    VALOR_ATE AS valorAte, 
                                    ATRASO_DE AS atrasoDe, 
                                    ATRASO_ATE AS atrasoAte,
                                    TAXA_JUROS_MAX_CREDOR AS jurosMax, 
                                    TAXA_JUROS_MIN_CREDOR AS jurosMin, 
                                    TAXA_MULTA_CREDOR AS multa, 
                                    TAXA_MULTA_REPASSE_CREDOR AS multaRepasse, 
                                    TAXA_JUROS_REPASSE_CREDOR AS jurosRepasse,  
                                    TAXA_HONORARIOS_REPASSE_CREDOR AS honorariosRepasse,
                                    CCC.TAXA_COMISSAO_REPASSE_CREDOR AS comissaoRepasse, 
                                    CCC.TAXA_HONORARIOS_CREDOR AS honorarioMax, 
                                    TAXA_HONORARIOS_MIN_CREDOR AS honorarioMin, 
                                    CCC.TAXA_COMISSAO_CREDOR AS comissaoMax, 
                                    OBS_COMISSAO AS obsComissao, 
                                    TAXA_COMISSAO_MIN_CREDOR AS comissaoMin, 
                                    ISNULL(NULLIF(LTRIM(RTRIM(CC.LIMITE_PARCELAMENTO)), ''), 0) AS limiteParcelamento, 
                                    TAXA_DESCONTO AS desconto
                                FROM CLIENTE_CREDOR_COMISSAO_PDA_BORDERO CCC
                                INNER JOIN CLIENTE_CREDOR CC ON CC.COD_CLIENTE_CREDOR collate Latin1_General_CI_AS = CCC.COD_CLIENTE_CREDOR
                                WHERE CCC.COD_CLIENTE_CREDOR = @credor
                                  AND @diasAtraso BETWEEN CONVERT(INT, ATRASO_DE) AND CONVERT(INT, ATRASO_ATE)
                                  AND @valor BETWEEN VALOR_DE AND VALOR_ATE
                                  AND NUMERO_BORDERO = @numeroBordero
                                ORDER BY CCC.COD_CLIENTE_CREDOR, COD_COMISSAO";

      result = await _dbConnection.QueryFirstOrDefaultAsync<FaixaComissaoResult>(query, new { credor, diasAtraso, valor, numeroBordero });
    }

    return result;
  }

  public async Task<FaixaComissaoResult?> BuscarFaixaComissaoNormal(string credor, string devedor, TipoFaixaComissao tipoFaixa, int diasAtraso)
  {
    FaixaComissaoResult? resultado = null;

    if (tipoFaixa == TipoFaixaComissao.Valor)
    {
      const string queryValor = @"SELECT VALOR_PRINCIPAL FROM CLIENTE_DEVEDOR WHERE COD_CLIENTE_DEVEDOR = @devedor";
      double valor = await _dbConnection.QueryFirstOrDefaultAsync<double>(queryValor, new { devedor });

      const string query = @"
                                SELECT 
                                    CCC.VALOR_DE AS valorDe, 
                                    CCC.VALOR_ATE AS valorAte, 
                                    CCC.ATRASO_DE AS atrasoDe, 
                                    CCC.ATRASO_ATE AS atrasoAte,
                                    CCC.TAXA_JUROS_MAX_CREDOR AS jurosMax, 
                                    CCC.TAXA_JUROS_MIN_CREDOR AS jurosMin,
                                    CCC.TAXA_MULTA_CREDOR AS multa, 
                                    CCC.TAXA_MULTA_REPASSE_CREDOR AS multaRepasse, 
                                    CCC.TAXA_JUROS_REPASSE_CREDOR AS jurosRepasse, 
                                    CCC.TAXA_HONORARIOS_REPASSE_CREDOR AS honorariosRepasse,
                                    CCC.TAXA_COMISSAO_REPASSE_CREDOR AS comissaoRepasse, 
                                    CCC.TAXA_HONORARIOS_CREDOR AS honorarioMax, 
                                    CCC.TAXA_HONORARIOS_MIN_CREDOR AS honorarioMin,
                                    CCC.TAXA_COMISSAO_CREDOR AS comissaoMax, 
                                    CCC.OBS_COMISSAO AS obsComissao,
                                    CCC.TAXA_COMISSAO_MIN_CREDOR AS comissaoMin, 
                                    ISNULL(NULLIF(LTRIM(RTRIM(CC.LIMITE_PARCELAMENTO)), ''), 0) AS limiteParcelamento, 
                                    CCC.TAXA_DESCONTO AS desconto
                                FROM 
                                    CLIENTE_CREDOR_COMISSAO CCC
                                INNER JOIN 
                                    CLIENTE_CREDOR CC ON CC.COD_CLIENTE_CREDOR collate Latin1_General_CI_AS = CCC.COD_CLIENTE_CREDOR
                                WHERE 
                                    CCC.COD_CLIENTE_CREDOR = @credor
                                    AND @valor BETWEEN VALOR_DE AND VALOR_ATE
                                ORDER BY 
                                    CCC.COD_CLIENTE_CREDOR, COD_COMISSAO";

      resultado = await _dbConnection.QueryFirstOrDefaultAsync<FaixaComissaoResult>(query, new { credor, valor });
    }
    else if (tipoFaixa == TipoFaixaComissao.Dias || tipoFaixa == TipoFaixaComissao.AtrasoCadastro)
    {
      const string query = @"
                                SELECT 
                                    CCC.VALOR_DE AS valorDe, 
                                    CCC.VALOR_ATE AS valorAte, 
                                    CCC.ATRASO_DE AS atrasoDe, 
                                    CCC.ATRASO_ATE AS atrasoAte,
                                    CCC.TAXA_JUROS_MAX_CREDOR AS jurosMax, 
                                    CCC.TAXA_JUROS_MIN_CREDOR AS jurosMin,
                                    CCC.TAXA_MULTA_CREDOR AS multa, 
                                    CCC.TAXA_MULTA_REPASSE_CREDOR AS multaRepasse, 
                                    CCC.TAXA_JUROS_REPASSE_CREDOR AS jurosRepasse,
                                    CCC.TAXA_HONORARIOS_CREDOR AS honorarioMax, 
                                    CCC.TAXA_HONORARIOS_MIN_CREDOR AS honorarioMin, 
                                    CCC.TAXA_HONORARIOS_REPASSE_CREDOR AS honorariosRepasse,
                                    CCC.TAXA_COMISSAO_REPASSE_CREDOR AS comissaoRepasse, 
                                    CCC.TAXA_COMISSAO_CREDOR AS comissaoMax, 
                                    CCC.OBS_COMISSAO AS obsComissao,
                                    CCC.TAXA_COMISSAO_MIN_CREDOR AS comissaoMin, 
                                    ISNULL(NULLIF(LTRIM(RTRIM(CC.LIMITE_PARCELAMENTO)), ''), 0) AS limiteParcelamento, 
                                    CCC.TAXA_DESCONTO AS desconto
                                FROM 
                                    CLIENTE_CREDOR_COMISSAO CCC
                                INNER JOIN 
                                    CLIENTE_CREDOR CC ON CC.COD_CLIENTE_CREDOR collate Latin1_General_CI_AS = CCC.COD_CLIENTE_CREDOR
                                WHERE 
                                    CCC.COD_CLIENTE_CREDOR = @credor
                                    AND @diasAtraso BETWEEN CONVERT(INT, ATRASO_DE) AND CONVERT(INT, ATRASO_ATE)
                                ORDER BY 
                                    CCC.COD_CLIENTE_CREDOR, COD_COMISSAO";

      resultado = await _dbConnection.QueryFirstOrDefaultAsync<FaixaComissaoResult>(query, new { credor, diasAtraso });
    }
    else if (tipoFaixa == TipoFaixaComissao.ValorEDias)
    {
      const string queryValor = @"SELECT VALOR_PRINCIPAL FROM CLIENTE_DEVEDOR WHERE COD_CLIENTE_DEVEDOR = @devedor";
      double valor = await _dbConnection.QueryFirstOrDefaultAsync<double>(queryValor, new { devedor });

      const string query = @"
                                SELECT 
                                    CCC.VALOR_DE AS valorDe, 
                                    CCC.VALOR_ATE AS valorAte, 
                                    CCC.ATRASO_DE AS atrasoDe, 
                                    CCC.ATRASO_ATE AS atrasoAte,
                                    CCC.TAXA_JUROS_MAX_CREDOR AS jurosMax, 
                                    CCC.TAXA_JUROS_MIN_CREDOR AS jurosMin,
                                    CCC.TAXA_MULTA_CREDOR AS multa, 
                                    CCC.TAXA_MULTA_REPASSE_CREDOR AS multaRepasse, 
                                    CCC.TAXA_JUROS_REPASSE_CREDOR AS jurosRepasse,
                                    CCC.TAXA_HONORARIOS_CREDOR AS honorarioMax, 
                                    CCC.TAXA_HONORARIOS_MIN_CREDOR AS honorarioMin,
                                    CCC.TAXA_COMISSAO_CREDOR AS comissaoMax, 
                                    CCC.OBS_COMISSAO AS obsComissao, 
                                    CCC.TAXA_HONORARIOS_REPASSE_CREDOR AS honorariosRepasse,
                                    CCC.TAXA_COMISSAO_REPASSE_CREDOR AS comissaoRepasse, 
                                    CCC.TAXA_COMISSAO_MIN_CREDOR AS comissaoMin, 
                                    ISNULL(NULLIF(LTRIM(RTRIM(CC.LIMITE_PARCELAMENTO)), ''), 0) AS limiteParcelamento, 
                                    CCC.TAXA_DESCONTO AS desconto
                                FROM 
                                    CLIENTE_CREDOR_COMISSAO CCC
                                INNER JOIN 
                                    CLIENTE_CREDOR CC ON CC.COD_CLIENTE_CREDOR collate Latin1_General_CI_AS = CCC.COD_CLIENTE_CREDOR
                                WHERE 
                                    CCC.COD_CLIENTE_CREDOR = @credor
                                    AND @diasAtraso BETWEEN CONVERT(INT, ATRASO_DE) AND CONVERT(INT, ATRASO_ATE)
                                    AND @valor BETWEEN VALOR_DE AND VALOR_ATE
                                ORDER BY 
                                    CCC.COD_CLIENTE_CREDOR, COD_COMISSAO";

      resultado = await _dbConnection.QueryFirstOrDefaultAsync<FaixaComissaoResult>(query, new { credor, diasAtraso, valor });
    }
    else if (tipoFaixa == TipoFaixaComissao.DiasDePagamento || tipoFaixa == TipoFaixaComissao.DiasDeDuplicata || tipoFaixa == TipoFaixaComissao.PeriodoVencimento)
    {
      const string query = @"
                        SELECT 
                            A.VALOR_DE AS valorDe,
                            A.VALOR_ATE AS valorAte,
                            A.ATRASO_DE AS atrasoDe,
                            A.ATRASO_ATE AS atrasoAte,
                            A.TAXA_JUROS_MAX_CREDOR AS jurosMax,
                            A.TAXA_JUROS_MIN_CREDOR AS jurosMin,
                            A.TAXA_MULTA_CREDOR AS multa,
                            A.TAXA_MULTA_REPASSE_CREDOR AS multaRepasse,
                            A.TAXA_JUROS_REPASSE_CREDOR AS jurosRepasse,
                            A.TAXA_HONORARIOS_CREDOR AS honorarioMax,
                            A.TAXA_HONORARIOS_MIN_CREDOR AS honorarioMin,
                            A.TAXA_COMISSAO_CREDOR AS comissaoMax,
                            A.OBS_COMISSAO AS obsComissao,
                            A.TAXA_HONORARIOS_REPASSE_CREDOR AS honorariosRepasse,
                            A.TAXA_COMISSAO_REPASSE_CREDOR AS comissaoRepasse, 
                            A.TAXA_COMISSAO_MIN_CREDOR AS comissaoMin,
                            ISNULL(NULLIF(LTRIM(RTRIM(C.LIMITE_PARCELAMENTO)), ''), 0) AS limiteParcelamento,
                            A.TAXA_DESCONTO AS desconto
                        FROM CLIENTE_CREDOR_COMISSAO A
                        INNER JOIN CLIENTE_CREDOR C ON C.COD_CLIENTE_CREDOR collate Latin1_General_CI_AS = A.COD_CLIENTE_CREDOR
                        WHERE A.COD_CLIENTE_CREDOR = @credor
                        ORDER BY A.COD_CLIENTE_CREDOR, A.COD_COMISSAO";

      resultado = await _dbConnection.QueryFirstOrDefaultAsync<FaixaComissaoResult>(query, new { credor });
    }

    return resultado;
  }

  public async Task<FaixaComissaoResult?> BuscarFaixaComissaoPDA(string credor, string devedor, TipoFaixaComissao tipoFaixa, int diasAtraso)
  {
    FaixaComissaoResult? resultado = null;

    if (tipoFaixa == TipoFaixaComissao.Valor)
    {
      const string queryValor = "SELECT VALOR_PRINCIPAL FROM CLIENTE_DEVEDOR WHERE COD_CLIENTE_DEVEDOR = @devedor";
      double valor = await _dbConnection.QueryFirstOrDefaultAsync<double>(queryValor, new { devedor });

      const string query = @"
                                SELECT 
                                    CCC.VALOR_DE AS valorDe,
                                    CCC.VALOR_ATE AS valorAte,
                                    CCC.ATRASO_DE AS atrasoDe,
                                    CCC.ATRASO_ATE AS atrasoAte,
                                    CCC.TAXA_JUROS_MAX_CREDOR AS jurosMax,
                                    CCC.TAXA_JUROS_MIN_CREDOR AS jurosMin,
                                    CCC.TAXA_MULTA_CREDOR AS multa,
                                    CCC.TAXA_MULTA_REPASSE_CREDOR AS multaRepasse,
                                    CCC.TAXA_JUROS_REPASSE_CREDOR AS jurosRepasse,
                                    CCC.TAXA_HONORARIOS_CREDOR AS honorarioMax,
                                    CCC.TAXA_HONORARIOS_MIN_CREDOR AS honorarioMin,
                                    CCC.TAXA_COMISSAO_CREDOR AS comissaoMax,
                                    CCC.OBS_COMISSAO AS obsComissao,
                                    CCC.TAXA_HONORARIOS_REPASSE_CREDOR AS honorariosRepasse,
                                    CCC.TAXA_COMISSAO_REPASSE_CREDOR AS comissaoRepasse, 
                                    CCC.TAXA_COMISSAO_MIN_CREDOR AS comissaoMin,
                                    ISNULL(NULLIF(LTRIM(RTRIM(CC.LIMITE_PARCELAMENTO)), ''), 0) AS limiteParcelamento,
                                    CCC.TAXA_DESCONTO AS desconto
                                FROM CLIENTE_CREDOR_COMISSAO_PDA CCC
                                INNER JOIN CLIENTE_CREDOR CC ON CC.COD_CLIENTE_CREDOR collate Latin1_General_CI_AS = CCC.COD_CLIENTE_CREDOR
                                WHERE CCC.COD_CLIENTE_CREDOR = @credor
                                  AND @valor BETWEEN CCC.VALOR_DE AND CCC.VALOR_ATE
                                ORDER BY CCC.COD_CLIENTE_CREDOR, CCC.COD_COMISSAO";

      resultado = await _dbConnection.QueryFirstOrDefaultAsync<FaixaComissaoResult>(query, new { credor, valor });
    }
    else if (tipoFaixa == TipoFaixaComissao.Dias || tipoFaixa == TipoFaixaComissao.AtrasoCadastro)
    {
      const string query = @"
                            SELECT 
                                CCC.VALOR_DE AS valorDe,
                                CCC.VALOR_ATE AS valorAte,
                                CCC.ATRASO_DE AS atrasoDe,
                                CCC.ATRASO_ATE AS atrasoAte,
                                CCC.TAXA_JUROS_MAX_CREDOR AS jurosMax,
                                CCC.TAXA_JUROS_MIN_CREDOR AS jurosMin,
                                CCC.TAXA_MULTA_CREDOR AS multa,
                                CCC.TAXA_MULTA_REPASSE_CREDOR AS multaRepasse,
                                CCC.TAXA_JUROS_REPASSE_CREDOR AS jurosRepasse,
                                CCC.TAXA_HONORARIOS_CREDOR AS honorarioMax,
                                CCC.TAXA_HONORARIOS_MIN_CREDOR AS honorarioMin,
                                CCC.TAXA_COMISSAO_CREDOR AS comissaoMax,
                                CCC.OBS_COMISSAO AS obsComissao,
                                CCC.TAXA_HONORARIOS_REPASSE_CREDOR AS honorariosRepasse,
                                CCC.TAXA_COMISSAO_REPASSE_CREDOR AS comissaoRepasse, 
                                CCC.TAXA_COMISSAO_MIN_CREDOR AS comissaoMin,
                                ISNULL(NULLIF(LTRIM(RTRIM(CC.LIMITE_PARCELAMENTO)), ''), 0) AS limiteParcelamento,
                                CCC.TAXA_DESCONTO AS desconto
                            FROM CLIENTE_CREDOR_COMISSAO_PDA CCC
                            INNER JOIN CLIENTE_CREDOR CC ON CC.COD_CLIENTE_CREDOR collate Latin1_General_CI_AS = CCC.COD_CLIENTE_CREDOR
                            WHERE CCC.COD_CLIENTE_CREDOR = @credor
                              AND @diasAtraso BETWEEN CONVERT(INT, CCC.ATRASO_DE) AND CONVERT(INT, CCC.ATRASO_ATE)
                            ORDER BY CCC.COD_CLIENTE_CREDOR, CCC.COD_COMISSAO";

      resultado = await _dbConnection.QueryFirstOrDefaultAsync<FaixaComissaoResult>(query, new { credor, diasAtraso });
    }
    else if (tipoFaixa == TipoFaixaComissao.ValorEDias)
    {
      const string queryValor = "SELECT VALOR_PRINCIPAL FROM CLIENTE_DEVEDOR WHERE COD_CLIENTE_DEVEDOR = @devedor";
      double valor = await _dbConnection.QueryFirstOrDefaultAsync<double>(queryValor, new { devedor });

      const string query = @"
                            SELECT 
                                CCC.VALOR_DE AS valorDe,
                                CCC.VALOR_ATE AS valorAte,
                                CCC.ATRASO_DE AS atrasoDe,
                                CCC.ATRASO_ATE AS atrasoAte,
                                CCC.TAXA_JUROS_MAX_CREDOR AS jurosMax,
                                CCC.TAXA_JUROS_MIN_CREDOR AS jurosMin,
                                CCC.TAXA_MULTA_CREDOR AS multa,
                                CCC.TAXA_MULTA_REPASSE_CREDOR AS multaRepasse,
                                CCC.TAXA_JUROS_REPASSE_CREDOR AS jurosRepasse,
                                CCC.TAXA_HONORARIOS_CREDOR AS honorarioMax,
                                CCC.TAXA_HONORARIOS_MIN_CREDOR AS honorarioMin,
                                CCC.TAXA_COMISSAO_CREDOR AS comissaoMax,
                                CCC.OBS_COMISSAO AS obsComissao,
                                CCC.TAXA_HONORARIOS_REPASSE_CREDOR AS honorariosRepasse,
                                CCC.TAXA_COMISSAO_REPASSE_CREDOR AS comissaoRepasse, 
                                CCC.TAXA_COMISSAO_MIN_CREDOR AS comissaoMin,
                                ISNULL(NULLIF(LTRIM(RTRIM(CC.LIMITE_PARCELAMENTO)), ''), 0) AS limiteParcelamento,
                                CCC.TAXA_DESCONTO AS desconto
                            FROM CLIENTE_CREDOR_COMISSAO_PDA CCC
                            INNER JOIN CLIENTE_CREDOR CC ON CC.COD_CLIENTE_CREDOR collate Latin1_General_CI_AS = CCC.COD_CLIENTE_CREDOR
                            WHERE CCC.COD_CLIENTE_CREDOR = @credor
                              AND @diasAtraso BETWEEN CONVERT(INT, CCC.ATRASO_DE) AND CONVERT(INT, CCC.ATRASO_ATE)
                              AND @valor BETWEEN CCC.VALOR_DE AND CCC.VALOR_ATE
                            ORDER BY CCC.COD_CLIENTE_CREDOR, CCC.COD_COMISSAO";

      resultado = await _dbConnection.QueryFirstOrDefaultAsync<FaixaComissaoResult>(query, new { credor, diasAtraso, valor });
    }

    return resultado;
  }
}