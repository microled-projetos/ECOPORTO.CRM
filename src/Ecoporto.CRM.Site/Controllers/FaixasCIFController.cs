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
    public class FaixasCIFController : BaseController
    {
        private readonly IFaixasCIFRepositorio _faixasCIFRepositorio;
        private readonly ILayoutRepositorio _layoutRepositorio;

        public FaixasCIFController(
            IFaixasCIFRepositorio faixasCIFRepositorio, ILayoutRepositorio layoutRepositorio, ILogger logger) : base(logger)
        {
            _faixasCIFRepositorio = faixasCIFRepositorio;
            _layoutRepositorio = layoutRepositorio;
        }

        [HttpGet]
        public PartialViewResult Consultar(int layoutId)
            => PartialView("_Consulta", ObterFaixasCIF(layoutId));

        [HttpPost]
        public ActionResult Cadastrar(FaixasCIFViewModel viewModel)
        {
            var faixaCIF = new FaixaCIF
            {
                LayoutId = viewModel.FaixaCIFLayoutId,
                Minimo = viewModel.FaixasCIFMinimo,
                Maximo = viewModel.FaixasCIFMaximo,
                Valor20 = viewModel.FaixasCIFValor20,
                Valor40 = viewModel.FaixasCIFValor40,
                Margem = viewModel.FaixasCIFMargem,
                Descricao = viewModel.FaixasCIFDescricao
            };

            if (!Validar(faixaCIF))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest,
                    faixaCIF.ValidationResult
                        .Errors
                        .First()
                        .ToString());
            }

            _faixasCIFRepositorio.Cadastrar(faixaCIF);

            var faixas = ObterFaixasCIF(viewModel.FaixaCIFLayoutId);

            return PartialView("_Consulta", faixas);
        }

        [HttpPost]
        public ActionResult Excluir(int id)
        {
            try
            {
                var faixaCIFBusca = _faixasCIFRepositorio.ObterPorId(id);

                if (faixaCIFBusca == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Registro não encontrado ou já excluído");

                _faixasCIFRepositorio.Excluir(id);

                var faixas = ObterFaixasCIF(faixaCIFBusca.LayoutId);

                return PartialView("_Consulta", faixas);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        private IEnumerable<FaixaCIF> ObterFaixasCIF(int layoutId)
            => _faixasCIFRepositorio.ObterFaixasCIF(layoutId);
    }
}