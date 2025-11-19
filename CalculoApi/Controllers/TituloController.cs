using Calculo.Core.Calculo;
using Calculo.Core.Models.Titulo;
using Calculo.Service.v1.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Calculo.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TituloController : ControllerBase
{
  private readonly ITituloService _tituloService;

  public TituloController(ITituloService tituloService)
  {
    _tituloService = tituloService;
  }

  [HttpGet, Route("devedor/{devedor}")]
  public async Task<TituloResponse> ObterTitulos(string devedor)
  {
    return await _tituloService.ObterTitulosAsync(devedor);
  }

  [HttpPost, Route("calculov3")]
  public async Task<AcordoVM> SimularV3(AcordoVM acordoVM)
  {
    return await _tituloService.SimularV3(acordoVM);
  }
}