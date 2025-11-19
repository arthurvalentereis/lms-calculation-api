using Calculo.Core.Models;
using Calculo.Core.Models.Titulo;

namespace Calculo.Repository.v1.Repositories.Interfaces
{
  public interface IDevedorRepository
  {
    Task<DevedorResult?> ObterDevedorAsync(string codDevedor);
    Task<IEnumerable<TituloResult>> ObterTitulosPorDevedorAsync(string codDevedor);
  }
}
