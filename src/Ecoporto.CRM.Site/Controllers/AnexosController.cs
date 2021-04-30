using System;
using System.Web;
using System.Web.Mvc;

namespace Ecoporto.CRM.Site.Controllers
{
    public class AnexosController : Controller
    {
        // GET: Anexos
        public ActionResult Index()
        {
            return View();
        }

        public FileResult Download(string id)
        {
            var token = Sharepoint.Services.Autenticador.Autenticar();

            if (token == null)
                throw new HttpException(404, "Não foi possível se autenticar no serviço de Anexos");

            var arquivoBusca = new Sharepoint.Services.AnexosService(token)
                  .ObterArquivo(id);

            if (arquivoBusca.success == false)
                throw new Exception("API Upload: " + arquivoBusca.message);

            var contentType = MimeMapping.GetMimeMapping("a" + arquivoBusca.Arquivo.extension);

            var bytes = Convert.FromBase64String(arquivoBusca.Arquivo.dataArray);

            return File(bytes, contentType, arquivoBusca.Arquivo.name);
        }
    }
}