using Ecoporto.CRM.Site.Filtros;
using System.Web.Mvc;

namespace Ecoporto.CRM.Site
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new ValidarEquipeConta());
        }
    }
}
