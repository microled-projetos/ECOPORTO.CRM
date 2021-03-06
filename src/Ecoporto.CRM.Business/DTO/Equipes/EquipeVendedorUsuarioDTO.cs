using Ecoporto.CRM.Business.Enums;

namespace Ecoporto.CRM.Business.DTO
{
    public class EquipeVendedorUsuarioDTO
    {
        public int Id { get; set; }

        public string Login { get; set; }

        public string Nome { get; set; }

        public int AcessoConta { get; set; }

        public int AcessoOportunidade { get; set; }

        public PapelEquipe PapelEquipe { get; set; }
    }
}
