using System;

namespace Ecoporto.CRM.IntegraChronosAPI.Responses
{
    [Serializable]
    public class CadastrarResponse
    {
        public bool Sucesso { get; set; }

        public string Mensagem { get; set; }

        public string[] Erros { get; set; }

        public string Protocolo { get; set; }
    }
}