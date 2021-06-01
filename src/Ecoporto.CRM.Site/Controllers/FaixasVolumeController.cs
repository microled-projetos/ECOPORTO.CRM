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
    public class FaixasVolumeController : BaseController
    {
        private readonly IFaixasVolumeRepositorio _faixasVolumeRepositorio;
        private readonly ILayoutRepositorio _layoutRepositorio;

        public FaixasVolumeController(
            IFaixasVolumeRepositorio faixasVolumeRepositorio, ILayoutRepositorio layoutRepositorio, ILogger logger) : base(logger)
        {
            _faixasVolumeRepositorio = faixasVolumeRepositorio;
            _layoutRepositorio = layoutRepositorio;
        }

        [HttpGet]
        public PartialViewResult Consultar(int layoutId)
            => PartialView("_Consulta", ObterFaixasVolume(layoutId));

        [HttpPost]
        public ActionResult Cadastrar(FaixasVolumeViewModel viewModel)
        {
            var faixaVolume = new FaixaVolume
            {
                LayoutId = viewModel.FaixaVolumeLayoutId,
                ValorInicial = viewModel.FaixasVolumeValorInicial,
                ValorFinal = viewModel.FaixasVolumeValorFinal,
                Preco = viewModel.FaixasVolumePreco
            };

            if (!Validar(faixaVolume))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest,
                    faixaVolume.ValidationResult
                        .Errors
                        .First()
                        .ToString());
            }

            _faixasVolumeRepositorio.Cadastrar(faixaVolume);

            var faixas = ObterFaixasVolume(viewModel.FaixaVolumeLayoutId);

            return PartialView("_Consulta", faixas);
        }

        [HttpPost]
        public ActionResult Excluir(int id)
        {
            try
            {
                var faixaVolumeBusca = _faixasVolumeRepositorio.ObterPorId(id);

                if (faixaVolumeBusca == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Registro não encontrado ou já excluído");

                _faixasVolumeRepositorio.Excluir(id);

                var faixas = ObterFaixasVolume(faixaVolumeBusca.LayoutId);

                return PartialView("_Consulta", faixas);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        private IEnumerable<FaixaVolume> ObterFaixasVolume(int layoutId)
            => _faixasVolumeRepositorio.ObterFaixasVolume(layoutId);
    }
}