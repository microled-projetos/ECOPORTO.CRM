using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Site.Models;
using NLog;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Ecoporto.CRM.Site.Controllers
{
    [Authorize]
    public class ImpostosExcessaoController : BaseController
    {
        private readonly IImpostosExcecaoRepositorio _impostosExcecaoRepositorio;
        private readonly IOportunidadeRepositorio _oportunidadeRepositorio;

        public ImpostosExcessaoController(
            IImpostosExcecaoRepositorio impostosExcecaoRepositorio, IOportunidadeRepositorio oportunidadeRepositorio, ILogger logger) : base(logger)
        {
            _impostosExcecaoRepositorio = impostosExcecaoRepositorio;
            _oportunidadeRepositorio = oportunidadeRepositorio;
        }

        [HttpGet]
        public ActionResult Index(int modeloId, int oportunidadeId)
        {
            if (modeloId == 0)
                return RedirectToAction("Index", "Home");

            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(oportunidadeId);

            if (oportunidadeBusca == null)
                return RedirectToAction("Index", "Home");

            var servicos = _impostosExcecaoRepositorio.ObterServicos(modeloId, oportunidadeId);

            return View(new ImpostosExcecaoViewModel
            {
                ModeloId = modeloId,
                OportunidadeId = oportunidadeId,
                Tipo = TiposExcecoesImpostos.ImpostoAIsentar,
                StatusOportunidade = oportunidadeBusca.StatusOportunidade,
                Servicos = servicos
            });
        }

        private void ObterServicos(ImpostosExcecaoViewModel viewModel)
        {
            viewModel.Servicos = _impostosExcecaoRepositorio.ObterServicos(viewModel.ModeloId, viewModel.OportunidadeId);
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = "Tipo, ServicosSelecionados, ISS, PIS, COFINS, ValorISS, ValorPIS, ValorCOFINS, ModeloId, OportunidadeId")] ImpostosExcecaoViewModel viewModel)
        {
            var servicosSelecionados = (viewModel?.ServicosSelecionados is int[] servicosArr && servicosArr.Length > 0);

            if (!servicosSelecionados)
            {
                ModelState.Clear();

                ModelState.AddModelError(string.Empty, "Nenhum serviço foi selecionado");

                ObterServicos(viewModel);

                return View(viewModel);
            }

            if (viewModel.Tipo == TiposExcecoesImpostos.ImpostoDiferenciado)
            {
                if (viewModel.ValorISS.ToDecimal() == 0 && viewModel.ValorPIS.ToDecimal() == 0 && viewModel.ValorCOFINS.ToDecimal() == 0)
                {
                    ModelState.Clear();

                    ModelState.AddModelError(string.Empty, "Pelo menos um valor de imposto deverá ser informado");

                    ObterServicos(viewModel);

                    foreach (var servico in viewModel.Servicos)
                        servico.Selecionado = viewModel.ServicosSelecionados.Any(c => c == servico.ServicoId);

                    return View(viewModel);
                }
            }
            else
            {
                if (viewModel.ISS == false && viewModel.PIS == false && viewModel.COFINS == false)
                {
                    ModelState.Clear();

                    ModelState.AddModelError(string.Empty, "Pelo menos um valor de imposto deverá ser marcado");

                    ObterServicos(viewModel);

                    foreach (var servico in viewModel.Servicos)
                        servico.Selecionado = viewModel.ServicosSelecionados.Any(c => c == servico.ServicoId);

                    return View(viewModel);
                }
            }

            if (ModelState.IsValid)
            {
                var model = new ImpostosExcecaoDTO
                {
                    ModeloId = viewModel.ModeloId,
                    OportunidadeId = viewModel.OportunidadeId,
                    Tipo = viewModel.Tipo,
                    ISS = viewModel.ISS,
                    PIS = viewModel.PIS,
                    COFINS = viewModel.COFINS,
                    ValorISS = viewModel.ValorISS.ToDecimal(),
                    ValorPIS = viewModel.ValorPIS.ToDecimal(),
                    ValorCOFINS = viewModel.ValorCOFINS.ToDecimal(),
                    ServicosSelecionados = viewModel.ServicosSelecionados
                };

                _impostosExcecaoRepositorio.GravarServicos(model);

                return RedirectToAction(nameof(Index), new {  viewModel.ModeloId, viewModel.OportunidadeId });
            }

            ObterServicos(viewModel);

            return View(viewModel);
        }

        public ActionResult ObterDetalhes(int id)
        {
            var detalhes = _impostosExcecaoRepositorio.ObterPorId(id);

            return Json(new
            {
                detalhes.ServicoId,
                ISS = detalhes.ISS.ToInt(),
                PIS = detalhes.PIS.ToInt(),
                COFINS = detalhes.COFINS.ToInt(),
                ValorISS = detalhes.ValorISS.ToString("n2"),
                ValorPIS = detalhes.ValorPIS.ToString("n2"),
                ValorCOFINS = detalhes.ValorCOFINS.ToString("n2"),
                Tipo = detalhes.Tipo.ToValue()
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Excluir(int id)
        {
            var impostoBusca = _impostosExcecaoRepositorio.ObterPorId(id);

            try
            {
                _impostosExcecaoRepositorio.Excluir(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao excluir os valores de impostos");
            }

            var servicos = _impostosExcecaoRepositorio.ObterServicos(impostoBusca.ModeloId, impostoBusca.OportunidadeId);

            return PartialView("_ConsultaServicos", servicos);
        }
    }
}