using Calculo.Core.Models.Taxa;

namespace Calculo.Core.Calculo;

public class Parcela
{
  public bool alterado { get; set; }

  public int numeroParcela { get; set; }
  public DateTime dtVencimento { get; set; }
  public decimal valor { get; set; }

  public decimal valorRestante { get; set; }

  public decimal valorHonorarioRestante { get; set; }
  public decimal ValorReal(Taxas taxa)
  {
    return Math.Round(valor, 2);
  }
}