using System.Collections.ObjectModel;

namespace Calculo.Core.Calculo;

public class ParcelaAcordo
{
  public int ParcelaAcordoId { get; set; }
  public DateTime Data { get; set; }
  public bool DataFixa { get; set; }
  public decimal Valor { get; set; }
  public bool ValorFixo { get; set; }
  public bool Feriado { get; set; }
  public Acordo Acordo { get; set; }
  public ICollection<ParcelaPagamento> ParcelasPagamento { get; set; }

  public decimal Saldo
  {
    get
    {
      return (Valor - ParcelasPagamento.Sum(x => x.ValorParcela));
    }
  }

  public decimal SomaPricipais
  {
    get
    {
      return (ParcelasPagamento.Sum(x => x.SomaPrincipais));
    }
  }

  public decimal SomaEncargos
  {
    get
    {
      return (ParcelasPagamento.Sum(x => x.ValorEncargos));
    }
  }

  public ParcelaAcordo(int ParcelaAcordoId, DateTime Data, decimal Valor, Acordo Acordo, bool DataFixa = false, bool ValorFixo = false)
  {
    this.ParcelaAcordoId = ParcelaAcordoId;
    this.Data = Data;
    this.Valor = Valor;
    this.Acordo = Acordo;
    this.DataFixa = DataFixa;
    this.ValorFixo = ValorFixo;
    this.Feriado = EhFimDeSemana(Data);
    ParcelasPagamento = new Collection<ParcelaPagamento>();
  }

  public bool EhFimDeSemana(DateTime data)
  {
    DayOfWeek diaDaSemana = data.DayOfWeek;

    return diaDaSemana == DayOfWeek.Saturday || diaDaSemana == DayOfWeek.Sunday;
  }

  public void AddParcelaPagamento(ParcelaPagamento parcelaPagemento)
  {
    this.ParcelasPagamento.Add(parcelaPagemento);
  }
}