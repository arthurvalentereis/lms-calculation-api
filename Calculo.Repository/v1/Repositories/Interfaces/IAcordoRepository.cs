using Calculo.Core.Models;
using Calculo.Core.Models.Acordo;

namespace Calculo.Repository.v1.Repositories.Interfaces
{
  public interface IAcordoRepository
  {
    Task<int> GetIdCalculo();
    Task<bool> GravaPcTitulo(AcordoPcTituloRequest pcTitulo);
    Task<bool> GravaPcTituloParcela(AcordoPcTituloParcelaRequest pcTituloParcela);
    Task<bool> GravaPcTituloCalculo(AcordoPcTituloCalculoRequest pcTituloCalculo);
    Task<bool> GravaAcordo(AcordoTituloRequest acordo);
    Task<int> GetIdAcordo();
    Task<bool> GravaPcTituloParam(AcordoPcTituloParamRequest pcTituloParam);
    Task<bool> GravaOcorrencia(AcordoOcorrenciaRequest acordoOcorrencia);
    Task<bool> GravaParcelas(AcordoParcelasRequest pcTituloCalculo);
    Task<bool> GravaPcTituloTabela(PrestacaoContasTituloRequest pcTituloTabela);
  }
}
