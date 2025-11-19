namespace Calculo.Core.Models.Acordo;

public class AcordoOcorrenciaRequest
{
  public string COD_CLIENTE_DEVEDOR { get; set; }
  public string NUMERO_BORDERO { get; set; }
  public string COD_CLIENTE_CREDOR { get; set; }
  public DateTime DT_OCORRENCIA { get; set; }
  public DateTime DT_CADASTRO_BORDERO { get; set; }
  public string DESCRICAO_OCORRENCIA { get; set; }
  public string IND_SITUACAO_OCORRENCIA { get; set; }
  public string TIPO_OCORRENCIA { get; set; }
}