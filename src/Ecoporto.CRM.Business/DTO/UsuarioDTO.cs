namespace Ecoporto.CRM.Business.DTO
{
    public class UsuarioDTO
    {
        public int Id { get; set; }

        public string Login { get; set; }

        public string LoginExterno { get; set; }

        public string Nome { get; set; }

        public string Email { get; set; }

        public string DescricaoCargo { get; set; }

        public bool Administrador { get; set; }

        public bool Externo { get; set; }

        public bool Ativo { get; set; }
    }
}
