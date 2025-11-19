namespace Calculo.Core.Calculo;

public class TituloResultado
{
  internal int parcelaFinal;

  public TituloResultado(Titulo titulo, decimal valor)
  {
    this.titulo = titulo;
    this.valor = valor;
  }

  public Titulo titulo { get; set; }
  public decimal valor { get; set; }
  public decimal valorJurosDoTitulo { get; set; }
  public decimal valorCustasDoTitulo { get; set; }
  public decimal valorMultaDoTitulo { get; set; }
  public decimal valorHonorarioDoTitulo { get; internal set; }
  public decimal valorComisssaoDoTitulo { get; internal set; }

}