using Calculo.Core.Models;
using Calculo.Core.Models.FaixaComissao;

namespace Calculo.Repository.v1.Repositories.Interfaces;

public interface IFaixaComissaoRepository
{
  Task<bool> VerificaFaixaComissaoBordero(string cliente, string numeroBordero);
  Task<bool> VerificaFaixaComissaoCadastro(string cliente);
  Task<bool> VerificaFaixaComissaoBorderoPD(string cliente, string numeroBordero);
  Task<bool> VerificaFaixaComissaoCadastroPD(string cliente);
  Task<bool> VerificaFaixaComissaoBorderoPDA(string cliente, string numeroBordero);
  Task<bool> VerificaFaixaComissaoCadastroPDA(string cliente);
  Task<int> FaixaVencimentoTitulo(string cliente, string bordero, string devedor);
  Task<CarregaTaxasMaximasResult?> CarregaMaximasBorderoNormal(string numeroBordero);
  Task<int> TipoFaixaComissaoBordero(string cliente, string numeroBordero);
  Task<FaixaComissaoResult?> BuscarTaxasBorderoNormal(string cliente, string numeroBordero);
  Task<int> FaixaVencimentoTituloDuplicata(string cliente, string bordero, string devedor);
  Task<int> FaixaVencimentoTituloCadastro(string cliente, string bordero, string devedor);
  Task<int> FaixaVencimentoTituloPeriodoVencido(string cliente, string bordero, string devedor);
  Task<FaixaComissaoResult?> BuscarFaixaComissaoBorderoNormal(string credor, string devedor, TipoFaixaComissao tipoFaixa, string numeroBordero, int diasAtraso);
  Task<int> TipoFaixaComissaoCadastro(string credor);
  Task<CarregaTaxasMaximasResult?> CarregaMaximasNormal(string credor);
  Task<FaixaComissaoResult?> BuscarFaixaComissaoNormal(string credor, string devedor, TipoFaixaComissao tipoFaixa, int diasAtraso);
  Task<int> TipoFaixaComissaoBorderoPD(string cliente, string numeroBordero);
  Task<FaixaComissaoResult?> BuscarTaxasBorderoPD(string credor, string numeroBordero);
  Task<FaixaComissaoResult?> BuscarFaixaComissaoBorderoPD(string credor, string devedor, TipoFaixaComissao tipoFaixa, string numeroBordero, int diasAtraso);
  Task<int> TipoFaixaComissaoCadastroPD(string credor);
  Task<FaixaComissaoResult?> BuscarFaixaComissaoPD(string credor, string devedor, TipoFaixaComissao tipoFaixa, int diasAtraso);
  Task<int> TipoFaixaComissaoBorderoPDA(string cliente, string numeroBordero);
  Task<FaixaComissaoResult?> BuscarTaxasBorderoPDA(string cliente, string numeroBordero);
  Task<FaixaComissaoResult?> BuscarFaixaComissaoBorderoPDA(string credor, string devedor, TipoFaixaComissao tipoFaixa, string numeroBordero, int diasAtraso);
  Task<int> TipoFaixaComissaoCadastroPDA(string credor);
  Task<FaixaComissaoResult?> BuscarFaixaComissaoPDA(string credor, string devedor, TipoFaixaComissao tipoFaixa, int diasAtraso);
}