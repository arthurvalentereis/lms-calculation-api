using Microsoft.AspNetCore.Mvc;
using Calculo.Service.v1.Interfaces;
using Calculo.Core.Models;
using Calculo.Core.Models.FaixaComissao;
using Calculo.Core.Models.Acordo;

namespace Calculo.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AcordoController : ControllerBase
{
  private readonly IAcordoService _acordoService;

  public AcordoController(IAcordoService acordoService)
  {
    _acordoService = acordoService;
  }

  [HttpGet, Route("devedor/{devedor}")]
  public async Task<DevedorResult> GetDevedor(string devedor)
  {
    return await _acordoService.ObterDevedorAsync(devedor);
  }

  [HttpPost, Route("faixa-comissao")]
  public async Task<FaixaComissaoResult?> ObterFaixasDeComissao([FromBody] DevedorFaixaRequest devedorFaixaRequest)
  {
    return await _acordoService.ObterTaxas(devedorFaixaRequest);
  }

  [HttpPost, Route("salvar")]
  public async Task<AcordoResult> GravaAcordo([FromBody] AcordoRequest acordo)
  {
    return await _acordoService.GravaAcordo(acordo);
  }
}