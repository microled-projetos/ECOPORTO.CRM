using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Models
{
    public class Menu
    {
        public int Id { get; set; }

        public string Descricao { get; set; }

        public string Url { get; set; }

        public int Inativo { get; set; }

        public List<SubMenu> SubMenus { get; set; }
    }
}
