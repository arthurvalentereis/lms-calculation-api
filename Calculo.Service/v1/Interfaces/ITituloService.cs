using Calculo.Core.Calculo;
using Calculo.Core.Models.Titulo;

namespace Calculo.Service.v1.Interfaces;

public interface ITituloService
{
  Task<TituloResponse> ObterTitulosAsync(string codDevedor);
  Task<AcordoVM> SimularV3(AcordoVM request);
}