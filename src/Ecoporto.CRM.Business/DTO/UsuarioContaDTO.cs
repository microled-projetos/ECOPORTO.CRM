namespace Ecoporto.CRM.Business.DTO
{
    public class UsuarioContaDTO
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        public int ContaId { get; set; }

        public string ContaDescricao { get; set; }

        public string ContaDocumento { get; set; }
    }
}
