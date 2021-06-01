using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Site.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Ecoporto.CRM.Site.Controllers
{
    [Authorize]
    public class FaixasBLController : BaseController
    {
        private readonly IFaixasBLRepositorio _faixasBLRepositorio;
        private readonly ILayoutRepositorio _layoutRepositorio;

        public FaixasBLController(
            IFaixasBLRepositorio faixasBLRepositorio, ILayoutRepositorio layoutRepositorio, ILogger logger) : base(logger)
        {
            _faixasBLRepositorio = faixasBLRepositorio;
            _layoutRepositorio = layoutRepositorio;
        }

        [HttpGet]
        public PartialViewResult Consultar(int layoutId)
            => PartialView("_Consulta", ObterFaixasBL(layoutId));

        [HttpPost]
        public ActionResult Cadastrar(FaixasBLViewModel viewModel)
        {
            var faixaBL = new FaixaBL
            {
                LayoutId = viewModel.FaixaBLLayoutId,
                BLMinimo = viewModel.FaixasBLMinimo,
                BLMaximo = viewModel.FaixasBLMaximo,
                ValorMinimo = viewModel.FaixasBLValorMinimo
            };

            if (!Validar(faixaBL))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest,
                    faixaBL.ValidationResult
                        .Errors
                        .First()
                        .ToString());
            }

            _faixasBLRepositorio.Cadastrar(faixaBL);

            var faixas = ObterFaixasBL(viewModel.FaixaBLLayoutId);

            return PartialView("_Consulta", faixas);
        }

        [HttpPost]
        public ActionResult Excluir(int id)
        {
            try
            {
                var faixaBLFBusca = _faixasBLRepositorio.ObterPorId(id);

                if (faixaBLFBusca == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Registro não encontrado ou já excluído");

                _faixasBLRepositorio.Excluir(id);

                var faixas = ObterFaixasBL(faixaBLFBusca.LayoutId);

                return PartialView("_Consulta", faixas);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        private IEnumerable<FaixaBL> ObterFaixasBL(int layoutId)
            => _faixasBLRepositorio.ObterFaixasBL(layoutId);
    }
}