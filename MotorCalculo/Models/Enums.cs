namespace Calculo.Core.Models;

public enum TipoCalculo
{
  Normal = 0,
  PD = 1,
  PDA = 2
}

public enum OrigemTaxas
{
  Cadastro,
  FaixaComissaoCadastro,
  Bordero,
  FaixaComissaoBordero
}

public enum TipoFaixaComissao
{
  SemFaixa = 0,
  Valor = 1,
  Dias = 2,
  ValorEDias = 3,
  DiasDeDuplicata = 4,
  DiasDePagamento = 5,
  PeriodoVencimento = 6,
  AtrasoCadastro = 7
}