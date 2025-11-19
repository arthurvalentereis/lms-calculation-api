using System.Text.Json.Serialization;

namespace Calculo.Core.Models.Titulo;

public class TituloResponse
{
  [JsonPropertyName("titulos")]
  public IList<TituloResult> Titulos { get; set; }
}