namespace Ecoporto.CRM.Business.Models
{
    public class OportunidadeAlteracaoLinhaProposta
    {
        public int OportunidadeId { get; set; }

        public int UsuarioId { get; set; }

        public int Linha { get; set; }

        public string Propriedade { get; set; }
    }
}
