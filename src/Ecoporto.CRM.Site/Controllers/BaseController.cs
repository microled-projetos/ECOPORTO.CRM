using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Models;
using NLog;
using System.Web;
using System.Web.Mvc;

namespace Ecoporto.CRM.Site.Controllers
{
    public class BaseController : Controller
    {
        private readonly ILogger _logger;

        public BaseController(ILogger logger)
        {
            _logger = logger;
        }

        public bool Validar<T>(Entidade<T> entidade)
        {
            ModelState.Clear();
            
            entidade.Validar();

            foreach (var erro in entidade.ValidationResult.Errors)
                ModelState.AddModelError(erro.PropertyName, erro.ErrorMessage);
                      
            return entidade.Valido;
        }

        public ActionResult RegistroNaoEncontrado() =>
            throw new HttpException(404, "Registro não encontrado");

        public void GravarLogAuditoria(TipoLogAuditoria tipoLog, dynamic objeto, int chavePai = 0, string mensagem = "")
        {
            if (objeto != null)
            {   
                if (string.IsNullOrEmpty(mensagem))
                {
                    if (tipoLog == TipoLogAuditoria.INSERT)
                    {
                        mensagem = "Registro incluído";
                    }

                    if (tipoLog == TipoLogAuditoria.UPDATE)
                    {
                        mensagem = "Registro alterado";
                    }

                    if (tipoLog == TipoLogAuditoria.DELETE)
                    {
                        mensagem = "Registro excluído";
                    }
                }                

                GlobalDiagnosticsContext.Set("ticket", " ");
                GlobalDiagnosticsContext.Set("acao", tipoLog.ToValue());
                GlobalDiagnosticsContext.Set("objeto", ((object)objeto).ToJson());
                GlobalDiagnosticsContext.Set("chave", objeto.Id);
                GlobalDiagnosticsContext.Set("chavePai", chavePai);

                _logger.Info(mensagem);
            }
        }
    }
}