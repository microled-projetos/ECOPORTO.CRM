using Ecoporto.CRM.Service.Enums;

namespace Ecoporto.CRM.Service.Models
{
    public class OportunidadeAdendo
    {
        public int Id { get; set; }

        public int TipoAdendo { get; set; }

        public int OportunidadeId { get; set; }

        public int StatusAdendo { get; set; }
    }
}
