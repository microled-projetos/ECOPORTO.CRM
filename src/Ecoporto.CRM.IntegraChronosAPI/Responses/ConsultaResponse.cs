namespace Ecoporto.CRM.IntegraChronosAPI.Responses
{
    public class ConsultaResponse
    {
        public bool Sucesso { get; set; }
        
        public string Protocolo { get; set; }

        public int Status { get; set; }

        public string StatusDescricao { get; set; }

        public string Motivo { get; set; }

        public string Mensagem { get; set; }
    }
}