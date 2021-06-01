namespace Ecoporto.CRM.Business.Models
{
    public class SubMenu
    {
        public int Id { get; set; }

        public int MenuId { get; set; }

        public string Descricao { get; set; }

        public string Url { get; set; }

        public string UrlExterna { get; set; }

        public int ValidaUser { get; set; }

        public string ObjetoId { get; set; }
    }
}
