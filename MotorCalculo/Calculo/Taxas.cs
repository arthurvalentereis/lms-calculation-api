using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculo.Core.Models.Taxa
{
  public class Taxas
  {
    public Taxas()
    {
      this.valores = new Dictionary<Taxa, decimal>();
    }
    public Dictionary<Taxa, decimal> valores { get; set; }

    public decimal GetValue(Taxa taxa)
    {
      return this.valores.FirstOrDefault(p => p.Key == taxa).Value;
    }
  }

  public enum Taxa
  {
    honorario,
    juros,
    comissao,
    multa,
    custas,
    desconto,
    jurosRetido,
    multaRetido,
    honorariosRetido

  }

  public class ValorTaxa
  {
    public Taxa taxa { get; set; }
    public decimal valor { get; set; }
  }
}
