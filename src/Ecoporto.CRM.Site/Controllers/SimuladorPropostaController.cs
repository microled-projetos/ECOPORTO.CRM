using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Site.Extensions;
using Ecoporto.CRM.Site.Models;
using NLog;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Ecoporto.CRM.Site.Controllers
{
    [Authorize]
    public class SimuladorPropostaController : BaseController
    {
        private readonly ILogger _logger;
        private readonly ISimuladorPropostaRepositorio _simuladorRepositorio;
        private readonly IDocumentoRepositorio _documentoRepositorio;
        private readonly IModeloSimuladorRepositorio _modeloSimuladorRepositorio;
        private readonly ILocalAtracacaoRepositorio _localAtracacaoRepositorio;
        private readonly IGrupoAtracacaoRepositorio _grupoAtracacaoRepositorio;
        private readonly IParceiroRepositorio _parceiroRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IOportunidadeRepositorio _oportunidadeRepositorio;
        private readonly IAnexoRepositorio _anexoRepositorio;
        private readonly IMargemRepositorio _margemRepositorio;

        public SimuladorPropostaController(
            ISimuladorPropostaRepositorio simuladorRepositorio,
            IDocumentoRepositorio documentoRepositorio,
            IModeloSimuladorRepositorio modeloSimuladorRepositorio,
            ILocalAtracacaoRepositorio localAtracacaoRepositorio,
            IGrupoAtracacaoRepositorio grupoAtracacaoRepositorio,
            IParceiroRepositorio parceiroRepositorio,
            IUsuarioRepositorio usuarioRepositorio,
            IOportunidadeRepositorio oportunidadeRepositorio,
            IAnexoRepositorio anexoRepositorio,
            IMargemRepositorio margemRepositorio,
            ILogger logger) : base(logger)
        {
            _simuladorRepositorio = simuladorRepositorio;
            _documentoRepositorio = documentoRepositorio;
            _modeloSimuladorRepositorio = modeloSimuladorRepositorio;
            _localAtracacaoRepositorio = localAtracacaoRepositorio;
            _grupoAtracacaoRepositorio = grupoAtracacaoRepositorio;
            _parceiroRepositorio = parceiroRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
            _oportunidadeRepositorio = oportunidadeRepositorio;
            _anexoRepositorio = anexoRepositorio;
            _margemRepositorio = margemRepositorio;
            _logger = logger;
        }

        [HttpPost]
        public ActionResult CadastrarParametrosSimulador([Bind(Include = "SimuladorPropostaId, SimuladorPropostaModeloId, SimuladorPropostaOportunidadeId, SimuladorPropostaRegime, SimuladorPropostaPeriodos, SimuladorPropostaGrupoAtracacaoId, SimuladorPropostaTipoDocumentoId, SimuladorPropostaNumeroLotes, SimuladorPropostaMargem, SimuladorPropostaPeso, SimuladorPropostaVolumeM3, SimuladorPropostaArmadorId, SimuladorPropostaQtde20, SimuladorPropostaQtde40, SimuladorPropostaCif, Observacoes")] SimuladorPropostaViewModel viewModel)
        {
            var oportunidadeBusca = _oportunidadeRepositorio
                .ObterOportunidadePorId(viewModel.SimuladorPropostaOportunidadeId);

            if (oportunidadeBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Oportunidade não encontrada");

            var parametro = new SimuladorPropostaParametros(
                viewModel.SimuladorPropostaOportunidadeId,
                viewModel.SimuladorPropostaModeloId,
                oportunidadeBusca.TipoServico.ToName(),
                viewModel.SimuladorPropostaMargem,
                viewModel.SimuladorPropostaGrupoAtracacaoId,
                viewModel.SimuladorPropostaVolumeM3,
                viewModel.SimuladorPropostaPeso,
                viewModel.SimuladorPropostaPeriodos,
                viewModel.SimuladorPropostaTipoDocumentoId,
                viewModel.SimuladorPropostaQtde20,
                viewModel.SimuladorPropostaQtde40,
                viewModel.Observacoes,
                viewModel.SimuladorPropostaCif,
                User.ObterId());

            if (Enum.IsDefined(typeof(StatusOportunidade), oportunidadeBusca.StatusOportunidade))
            {
                if (oportunidadeBusca.StatusOportunidade != StatusOportunidade.RECUSADO)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Não permitido para Oportunidades com Status {oportunidadeBusca.StatusOportunidade.ToName()}");
            }

            if (viewModel.SimuladorPropostaModeloId == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, $"Informe o Modelo do Simulador");

            var existe = _simuladorRepositorio
                .ObterParametrosSimulador(viewModel.SimuladorPropostaOportunidadeId)
                .FirstOrDefault(c => c.ModeloId == viewModel.SimuladorPropostaModeloId);

            if (existe != null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Modelo de Simulador já cadastrado");

            _simuladorRepositorio.CadastrarParametrosSimulador(parametro);

            var simuladores = _simuladorRepositorio.ObterParametrosSimulador(viewModel.SimuladorPropostaOportunidadeId).ToList();

            return PartialView("_ConsultaParametros", simuladores);
        }

        [HttpPost]
        public ActionResult SimularOportunidade(int simuladorOportunidadeId, int simuladorParametroId, int modeloSimuladorId)
        {
            using (var wsSimulador = new WsSimulador.SimuladorCalculo())
            {
                wsSimulador.Timeout = 900000;

                var response = wsSimulador.SimuladorOportunidade(simuladorOportunidadeId, simuladorParametroId, modeloSimuladorId, User.ObterId());

                if (response == null)
                    return PartialView("_ErroSimulador", new SimuladorDownloadRelatorioViewModel("Nenhum retorno do serviço do Simulador"));

                if (response.Sucesso == false)
                    return PartialView("_ErroSimulador", new SimuladorDownloadRelatorioViewModel(response.Mensagem));
                else
                {
                    AnexosDTO anexoBusca = null;

                    if (response.ArquivoId > 0)
                    {
                        anexoBusca = _anexoRepositorio.ObterDetalhesAnexo(response.ArquivoId);

                        if (anexoBusca == null)
                            return PartialView("_ErroSimulador", new SimuladorDownloadRelatorioViewModel("Anexo não encontrado"));
                    }

                    return PartialView("_SucessoSimulador", new SimuladorDownloadRelatorioViewModel
                    {
                        NomeArquivo = anexoBusca?.Anexo ?? response.NomeArquivo,
                        ArquivoId = anexoBusca?.IdFile ?? response.Hash, 
                        Hash = response.Hash,
                        TamanhoArquivo = string.Format("{0} kb", response.TamanhoArquivo / 1024)
                    });
                }
            }
        }

        [HttpPost]
        public ActionResult ExcluirParametroSimulador(int id)
        {
            try
            {
                var parametroSimulador = _simuladorRepositorio.ObterParametroSimuladorPorId(id);

                if (parametroSimulador == null)
                    RegistroNaoEncontrado();

                _simuladorRepositorio.ExcluirParametroSimulador(parametroSimulador.Id);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }
    }
}