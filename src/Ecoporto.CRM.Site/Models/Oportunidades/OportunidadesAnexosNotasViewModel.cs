namespace Ecoporto.CRM.Site.Models
{
    public class OportunidadesAnexosNotasViewModel
    {
        public OportunidadesAnexosNotasViewModel()
        {
            OportunidadesAnexosViewModel = new OportunidadesAnexosViewModel();
            OportunidadesNotasViewModel = new OportunidadesNotasViewModel();
        }

        public OportunidadesAnexosViewModel OportunidadesAnexosViewModel { get; set; }

        public OportunidadesNotasViewModel OportunidadesNotasViewModel { get; set; }
    }
}