using Calculo.Repository.v1.Repositories.Interfaces;
using Calculo.Core.Models;
using System.Data;
using Calculo.Core.Models.Titulo;
using Dapper;

namespace Calculo.Repository.v1.Repositories;

public class DevedorRepository : IDevedorRepository
{
  private readonly IDbConnection _dbConnection;

  public DevedorRepository(IDbConnection dbConnection)
  {
    _dbConnection = dbConnection;
  }

  public async Task<DevedorResult?> ObterDevedorAsync(string codDevedor)
  {
    const string query = @"
            SELECT 
                D.VALOR_PRINCIPAL AS VlPrincipal, 
                C.RAZAO_SOCIAL_CREDOR AS RazaoSocialCredor, 
                D.NOME_CLIENTE_DEVEDOR AS RazaoSocialDevedor,
                D.COD_CLIENTE_CREDOR AS CodigoCredor, 
                D.COD_CLIENTE_DEVEDOR AS CodigoDevedor,
                D.COD_FUNCIONARIO AS CodigoRecuperador, 
                F.NOME_FUNCIONARIO AS NomeRecuperador,
                D.NUMERO_BORDERO AS NumeroBordero, 
                B.TAXA_COMISSAO_BORDERO AS TxComissao,
                B.TAXA_JUROS_BORDERO AS TxJuros, 
                B.TAXA_HONORARIOS_BORDERO AS TxHonorarios,
                B.TAXA_CUSTAS_BORDERO AS TxCustas, 
                B.TAXA_JUROS2_BORDERO AS TxJurosMin,
                B.TAXA_JUROS_BORDERO AS TxJurosMax, 
                B.TAXA_MULTA_BORDERO AS TxMulta,
                D.DT_CADASTRO_BORDERO AS DataBordero,
                D.LIBERAR_PARCELAS AS LiberaParcelas,
                C.OBS_REPASSE AS ObsRepasse
            FROM CLIENTE_CREDOR C
            INNER JOIN CLIENTE_DEVEDOR D ON C.COD_CLIENTE_CREDOR = D.COD_CLIENTE_CREDOR
            INNER JOIN FUNCIONARIO_DEJURIS F ON D.COD_FUNCIONARIO = F.COD_FUNCIONARIO
            INNER JOIN BORDERO_COBRANCA B ON D.NUMERO_BORDERO = B.NUMERO_BORDERO
            WHERE D.COD_CLIENTE_DEVEDOR = @codDevedor";

    return await _dbConnection.QuerySingleOrDefaultAsync<DevedorResult>(query, new { codDevedor });
  }

  public async Task<IEnumerable<TituloResult>> ObterTitulosPorDevedorAsync(string codDevedor)
  {
    const string query = @"
            SELECT 
                NUMERO_TITULO AS NomeTitulo,
                DT_VENCIMENTO_TITULO AS DataTituloVencimento,
                DT_CADASTRO_BORDERO AS DataTituloCadastro,
                DT_EMISSAO_TITULO AS DataTituloEmissao,
                DATEDIFF(DAY, DT_EMISSAO_TITULO, GETDATE()) AS AtrasoDuplicata,
                DATEDIFF(DAY, DT_VENCIMENTO_TITULO, DT_CADASTRO_BORDERO) AS AtrasoContrato,
                VALOR_PRINCIPAL AS ValorPrincipal,
                VALOR_PRINCIPAL_ORIGINAL AS ValorPrincipalOriginal,
                VALOR_PROTESTO AS ValorCustas,
                OBS_TITULO AS Observacao,
                IND_SITUACAO_TITULO AS Situacao
            FROM TITULO_COBRANCA 
            WHERE COD_CLIENTE_DEVEDOR = @codDevedor
            ORDER BY AtrasoContrato DESC";

    return await _dbConnection.QueryAsync<TituloResult>(query, new { codDevedor });
  }
}