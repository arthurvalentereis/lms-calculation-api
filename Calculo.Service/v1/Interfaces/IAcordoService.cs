using Calculo.Core.Models;
using Calculo.Core.Models.Acordo;
using Calculo.Core.Models.FaixaComissao;

namespace Calculo.Service.v1.Interfaces;

public interface IAcordoService
{
  Task<DevedorResult> ObterDevedorAsync(string codDevedor);
  Task<FaixaComissaoResult?> ObterTaxas(DevedorFaixaRequest devedorFaixaRequest);
  Task<AcordoResult> GravaAcordo(AcordoRequest acordoRequest);
}