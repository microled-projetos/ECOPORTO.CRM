using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Site.Models;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Ecoporto.CRM.Site.Controllers
{
    [Authorize]
    public class FaixasPesoController : BaseController
    {
        private readonly IFaixasPesoRepositorio _faixasPesoRepositorio;
        private readonly ILayoutRepositorio _layoutRepositorio;

        public FaixasPesoController(
            IFaixasPesoRepositorio faixasPesoRepositorio, ILayoutRepositorio layoutRepositorio, ILogger logger) : base(logger)
        {
            _faixasPesoRepositorio = faixasPesoRepositorio;
            _layoutRepositorio = layoutRepositorio;
        }

        [HttpGet]
        public PartialViewResult Consultar(int layoutId)
            => PartialView("_Consulta", ObterFaixasPeso(layoutId));

        [HttpPost]
        public ActionResult Cadastrar(FaixasPesoViewModel viewModel)
        {
            var faixaPeso = new FaixaPeso
            {
                LayoutId = viewModel.FaixaPesoLayoutId,
                ValorInicial = viewModel.FaixasPesoValorInicial,
                ValorFinal = viewModel.FaixasPesoValorFinal,
                Preco = viewModel.FaixasPesoPreco
            };

            if (!Validar(faixaPeso))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest,
                    faixaPeso.ValidationResult
                        .Errors
                        .First()
                        .ToString());
            }

            _faixasPesoRepositorio.Cadastrar(faixaPeso);

            var faixas = ObterFaixasPeso(viewModel.FaixaPesoLayoutId);

            return PartialView("_Consulta", faixas);
        }

        [HttpPost]
        public ActionResult Excluir(int id)
        {
            try
            {
                var faixaPesoBusca = _faixasPesoRepositorio.ObterPorId(id);

                if (faixaPesoBusca == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Registro não encontrado ou já excluído");

                _faixasPesoRepositorio.Excluir(id);

                var faixas = ObterFaixasPeso(faixaPesoBusca.LayoutId);

                return PartialView("_Consulta", faixas);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        private IEnumerable<FaixaPeso> ObterFaixasPeso(int layoutId)
            => _faixasPesoRepositorio.ObterFaixasPeso(layoutId);
    }
}