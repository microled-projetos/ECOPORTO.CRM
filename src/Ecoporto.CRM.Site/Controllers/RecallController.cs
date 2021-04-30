using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Workflow.Services;
using System;
using System.Net;
using System.Web.Mvc;
using Ecoporto.CRM.Workflow.Models;
using Ecoporto.CRM.Workflow.Enums;
using NLog;
using Ecoporto.CRM.Site.Services;
using Ecoporto.CRM.Site.Extensions;

namespace Ecoporto.CRM.Site.Controllers
{
    [Authorize]
    public class RecallController : BaseController
    {
        private readonly IOportunidadeRepositorio _oportunidadeRepositorio;
        private readonly ISolicitacoesRepositorio _solicitacoesRepositorio;
        private readonly IPremioParceriaRepositorio _premioParceriaRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IWorkflowRepositorio _workflowRepositorio;
        private readonly IEquipesService _equipesService;
        private readonly IAnaliseCreditoRepositorio _analiseCreditoRepositorio;
        private readonly IContaRepositorio _contaRepositorio;

        public RecallController(
            IOportunidadeRepositorio oportunidadeRepositorio,
            ISolicitacoesRepositorio solicitacoesRepositorio,
            IPremioParceriaRepositorio premioParceriaRepositorio,
            IUsuarioRepositorio usuarioRepositorio,
            IWorkflowRepositorio workflowRepositorio,
            IEquipesService equipesService,
            IAnaliseCreditoRepositorio analiseCreditoRepositorio,
            IContaRepositorio contaRepositorio,
            ILogger logger) : base(logger)
        {
            _oportunidadeRepositorio = oportunidadeRepositorio;
            _solicitacoesRepositorio = solicitacoesRepositorio;
            _premioParceriaRepositorio = premioParceriaRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
            _workflowRepositorio = workflowRepositorio;
            _equipesService = equipesService;
            _analiseCreditoRepositorio = analiseCreditoRepositorio;
            _contaRepositorio = contaRepositorio;
        }

        [HttpPost]
        public ActionResult RecallOportunidade(int recallOportunidadeId, string motivoRecallOportunidade)
        {
            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(recallOportunidadeId);

            if (oportunidadeBusca == null)
                throw new Exception("Oportunidade não encontrada ou já excluída");

            var token = Autenticador.Autenticar();

            if (token == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Não foi possível se autenticar no serviço de Recall");

            var ultimoProtocolo = _workflowRepositorio.UltimoProtocolo(oportunidadeBusca.Id, Processo.OPORTUNIDADE);

            var workflow = new RecallService(token);

            var retorno = workflow.Recall(new CadastroRecall(ultimoProtocolo, User.ObterLogin(), User.ObterNome(), User.ObterEmail(), motivoRecallOportunidade));

            if (retorno == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nenhuma resposta do serviço de Recall");

            if (retorno.sucesso)
            {
                if (oportunidadeBusca.Cancelado)
                {
                    //_oportunidadeRepositorio.AtualizarStatusOportunidade(StatusOportunidade.ATIVA, oportunidadeBusca.Id);
                    //_oportunidadeRepositorio.AtualizarCancelamentoOportunidade(false, oportunidadeBusca.Id);
                    //_oportunidadeRepositorio.AtualizarDataCancelamentoOportunidade(null, oportunidadeBusca.Id);
                    _oportunidadeRepositorio.PermiteAlterarDataCancelamento(oportunidadeBusca.Id, true);
                }

                if (oportunidadeBusca.StatusOportunidade == StatusOportunidade.ENVIADO_PARA_APROVACAO)
                {
                    _oportunidadeRepositorio.AtualizarStatusOportunidade(null, oportunidadeBusca.Id);
                }

                var fichasFaturamento = _oportunidadeRepositorio.ObterFichasFaturamento(oportunidadeBusca.Id);

                foreach (var ficha in fichasFaturamento)
                {
                    if (ficha.StatusFichaFaturamento == StatusFichaFaturamento.EM_APROVACAO)
                    {
                        _oportunidadeRepositorio.AtualizarStatusFichaFaturamento(StatusFichaFaturamento.EM_ANDAMENTO, ficha.Id);
                    }
                }

                return Json(new
                {
                    Processo = Processo.OPORTUNIDADE,
                    RedirectUrl = $"/Oportunidades/Atualizar/{oportunidadeBusca.Id}"
                }, JsonRequestBehavior.AllowGet);
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, retorno.mensagem);
        }

        //recall solicitacao
        [HttpPost]
        public ActionResult RecallLimiteCredito(int recallLimiteId, string motivoRecallLimite)
        {
            var solicitacaoBusca = _analiseCreditoRepositorio.ObterLimiteDeCreditoPorIdUnico(recallLimiteId) ;


            if (solicitacaoBusca == null)
                throw new Exception("Solicitação não encontrada ou já excluída");

            if (!User.IsInRole("OportunidadesFichas:RecallFichaFaturamento"))
            {
                if (!_equipesService.ValidarEquipeOportunidade(solicitacaoBusca.Id))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui permissão para edição da Solicitacao (Equipes)");
                }
            }

            var token = Autenticador.Autenticar();

            if (token == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Não foi possível se autenticar no serviço de Recall");

            var ultimoProtocolo = _workflowRepositorio.UltimoProtocolo(solicitacaoBusca.Id, Processo.ANALISE_DE_CREDITO_COND_PGTO);
           

            var workflow = new RecallService(token);

            var retorno = workflow.Recall(new CadastroRecall(ultimoProtocolo, User.ObterLogin(), User.ObterNome(), User.ObterEmail(), motivoRecallLimite));

            if (retorno == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nenhuma resposta do serviço de Recall");

            if (retorno.sucesso)
            {
                if (solicitacaoBusca.StatusLimiteCredito == StatusLimiteCredito.EM_APROVACAO)
                {
                    _analiseCreditoRepositorio.AtualizarlimiteDeCreditoPendente(recallLimiteId);
                }

                //var resultado = _analiseCreditoRepositorio
                //.ObterSolicitacoesLimiteDeCredito(solicitacaoBusca.ContaId);

                //return PartialView("_SolicitacoesLimiteCredito", resultado);
                return Json(new
                {
                    Processo = Processo.ANALISE_DE_CREDITO_COND_PGTO,
                    RedirectUrl = $"/AnaliseCredito/Index/{solicitacaoBusca.Id}"
                }, JsonRequestBehavior.AllowGet);
            }
            
           return new HttpStatusCodeResult(HttpStatusCode.BadRequest, retorno.mensagem);
        }
        //recall solicitacao
        [HttpPost]
        public ActionResult RecallAnaliseCredito(int recallAnaliseId, string motivoRecallAnalise)
        {
            var contaSessao = _contaRepositorio.ObterContaPorId(recallAnaliseId);

            Session["ContaId"] = contaSessao.Id;
            Session["RazaoSocial"] = contaSessao.Descricao;
            Session["FontePagadoraId"] = contaSessao.Id;
            Session["Cnpj"] = contaSessao.Documento;

            var analiseCreditoBusca = _analiseCreditoRepositorio.ObterConsultaSpc(recallAnaliseId);

            if (analiseCreditoBusca == null)
                throw new Exception("Solicitação não encontrada ou já excluída");

            if (!User.IsInRole("OportunidadesFichas:RecallFichaFaturamento"))
            {
                if (!_equipesService.ValidarEquipeOportunidade(analiseCreditoBusca.Id))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui permissão para edição da Solicitacao (Equipes)");
                }
            }

            var token = Autenticador.Autenticar();

            if (token == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Não foi possível se autenticar no serviço de Recall");

            var ultimoProtocolo = _workflowRepositorio.UltimoProtocolo(analiseCreditoBusca.ContaId, Processo.ANALISE_DE_CREDITO);


            var workflow = new RecallService(token);

            var retorno = workflow.Recall(new CadastroRecall(ultimoProtocolo, User.ObterLogin(), User.ObterNome(), User.ObterEmail(), motivoRecallAnalise));

            if (retorno == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nenhuma resposta do serviço de Recall");

            if (retorno.sucesso)
            {
                if (analiseCreditoBusca.StatusAnaliseDeCredito == StatusAnaliseDeCredito.EM_APROVACAO)
                {
                    _analiseCreditoRepositorio.AtualizarSPC1(recallAnaliseId);
                }

                //var resultado = _analiseCreditoRepositorio
                //.ObterSolicitacoesLimiteDeCredito(solicitacaoBusca.ContaId);

                //return PartialView("_SolicitacoesLimiteCredito", resultado);
                return Json(new
                {
                    Processo = Processo.ANALISE_DE_CREDITO,
                    RedirectUrl = $"/AnaliseCredito/{recallAnaliseId}",
                }, JsonRequestBehavior.AllowGet);
                //return RedirectToAction("Index", "AnaliseCredito", new { id = recallAnaliseId });
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, retorno.mensagem);
        }
        [HttpPost]
        //recallfichafaturamento
        public ActionResult RecallFichaFaturamento(int recallFichasOportunidadeId, int recallFichasId, string motivoRecallFichas)
        {
            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(recallFichasOportunidadeId);

            if (oportunidadeBusca == null)
                throw new Exception("Oportunidade não encontrada ou já excluída");

            var fichaBusca = _oportunidadeRepositorio.ObterFichaFaturamentoPorId(recallFichasId);

            if (fichaBusca == null)
                throw new Exception("Ficha Faturamento não encontrada ou já excluída");

            if (!User.IsInRole("OportunidadesFichas:RecallFichaFaturamento"))
            {
                if (!_equipesService.ValidarEquipeOportunidade(oportunidadeBusca.Id))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuário não possui permissão para edição da Oportunidade (Equipes)");
                }
            }

            var token = Autenticador.Autenticar();

            if (token == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Não foi possível se autenticar no serviço de Recall");

            var ultimoProtocolo = _workflowRepositorio.UltimoProtocolo(oportunidadeBusca.Id, Processo.FICHA_FATURAMENTO, fichaBusca.Id);

            var workflow = new RecallService(token);

            var retorno = workflow.Recall(new CadastroRecall(ultimoProtocolo, User.ObterLogin(), User.ObterNome(), User.ObterEmail(), motivoRecallFichas));

            if (retorno == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nenhuma resposta do serviço de Recall");

            if (retorno.sucesso)
            {
                if (fichaBusca.StatusFichaFaturamento == StatusFichaFaturamento.EM_APROVACAO)
                {
                    _oportunidadeRepositorio.AtualizarStatusFichaFaturamento(StatusFichaFaturamento.EM_ANDAMENTO, fichaBusca.Id);
                }

                return Json(new
                {
                    Processo = Processo.FICHA_FATURAMENTO,
                    RedirectUrl = $"/Oportunidades/Atualizar/{oportunidadeBusca.Id}"
                }, JsonRequestBehavior.AllowGet);
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, retorno.mensagem);
        }

        [HttpPost]
        public ActionResult RecallPremioParceria(int recallPremiosOportunidadeId, int recallPremioId, string motivoRecallPremios)
        {
            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(recallPremiosOportunidadeId);

            if (oportunidadeBusca == null)
                throw new Exception("Oportunidade não encontrada ou já excluída");

            var premioBusca = _premioParceriaRepositorio.ObterPremioParceriaPorId(recallPremioId);

            if (premioBusca == null)
                throw new Exception("Prêmio Parceria não encontrado ou já excluído");

            var token = Autenticador.Autenticar();

            if (token == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Não foi possível se autenticar no serviço de Recall");

            var ultimoProtocolo = _workflowRepositorio.UltimoProtocolo(oportunidadeBusca.Id, Processo.PREMIO_PARCERIA);

            var workflow = new RecallService(token);

            var retorno = workflow.Recall(new CadastroRecall(ultimoProtocolo, User.ObterLogin(), User.ObterNome(), User.ObterEmail(), motivoRecallPremios));

            if (retorno == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nenhuma resposta do serviço de Recall");

            if (retorno.sucesso)
            {
                if (premioBusca.Cancelado)
                {
                    _premioParceriaRepositorio.AtualizarStatusPremioParceria(StatusPremioParceria.CADASTRADO, oportunidadeBusca.Id);
                    _premioParceriaRepositorio.AtualizarCancelamento(false);
                }

                if (premioBusca.StatusPremioParceria == StatusPremioParceria.EM_APROVACAO)
                {
                    _premioParceriaRepositorio.AtualizarStatusPremioParceria(StatusPremioParceria.EM_ANDAMENTO, premioBusca.Id);
                }

                return Json(new
                {
                    Processo = Processo.PREMIO_PARCERIA,
                    RedirectUrl = $"/Oportunidades/Atualizar/{oportunidadeBusca.Id}"
                }, JsonRequestBehavior.AllowGet);
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, retorno.mensagem);
        }

        [HttpPost]
        public ActionResult RecallAdendos(int recallAdendosOportunidadeId, int recallAdendoId, string motivoRecallAdendos)
        {
            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(recallAdendosOportunidadeId);

            if (oportunidadeBusca == null)
                throw new Exception("Oportunidade não encontrada ou já excluída");

            var adendoBusca = _oportunidadeRepositorio.ObterAdendoPorId(recallAdendoId);

            if (adendoBusca == null)
                throw new Exception("Adendo não encontrado ou já excluído");

            var token = Autenticador.Autenticar();

            if (token == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Não foi possível se autenticar no serviço de Recall");

            var ultimoProtocolo = _workflowRepositorio.UltimoProtocolo(oportunidadeBusca.Id, Processo.ADENDO, adendoBusca.Id);

            var workflow = new RecallService(token);

            var retorno = workflow.Recall(new CadastroRecall(ultimoProtocolo, User.ObterLogin(), User.ObterNome(), User.ObterEmail(), motivoRecallAdendos));

            if (retorno == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nenhuma resposta do serviço de Recall");

            if (retorno.sucesso)
            {
                if (adendoBusca.StatusAdendo == StatusAdendo.ENVIADO)
                {
                    _oportunidadeRepositorio.AtualizarStatusAdendo(StatusAdendo.ABERTO, adendoBusca.Id);

                    if (adendoBusca.TipoAdendo == TipoAdendo.FORMA_PAGAMENTO || adendoBusca.TipoAdendo == TipoAdendo.INCLUSAO_SUB_CLIENTE)
                    {
                        var fichasFaturamento = _oportunidadeRepositorio.ObterFichasFaturamento(oportunidadeBusca.Id);

                        foreach (var ficha in fichasFaturamento)
                        {
                            if (ficha.StatusFichaFaturamento == StatusFichaFaturamento.EM_APROVACAO)
                            {
                                _oportunidadeRepositorio.AtualizarStatusFichaFaturamento(StatusFichaFaturamento.EM_ANDAMENTO, ficha.Id);
                            }
                        }
                    }
                }

                return Json(new
                {
                    Processo = Processo.ADENDO,
                    RedirectUrl = $"/Oportunidades/Atualizar/{oportunidadeBusca.Id}"
                }, JsonRequestBehavior.AllowGet);
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, retorno.mensagem);
        }

        [HttpPost]
        public ActionResult RecallSolicitacaoComercial(int recallSolicitacaoId, string motivoRecall)
        {
            var solicitacaoBusca = _solicitacoesRepositorio.ObterSolicitacaoPorId(recallSolicitacaoId);

            if (solicitacaoBusca == null)
                throw new Exception("Solicitação Comercial não encontrada ou já excluída");

            var token = Autenticador.Autenticar();

            if (token == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Não foi possível se autenticar no serviço de Recall");

            Processo processo = Processo.SOLICITACAO_CANCELAMENTO;

            switch (solicitacaoBusca.TipoSolicitacao)
            {
                case TipoSolicitacao.CANCELAMENTO_NF:
                    processo = Processo.SOLICITACAO_CANCELAMENTO;
                    break;
                case TipoSolicitacao.DESCONTO:
                    processo = Processo.SOLICITACAO_DESCONTO;
                    break;
                case TipoSolicitacao.PRORROGACAO_BOLETO:
                    processo = Processo.SOLICITACAO_PRORROGACAO;
                    break;
                case TipoSolicitacao.RESTITUICAO:
                    processo = Processo.SOLICITACAO_RESTITUICAO;
                    break;
                case TipoSolicitacao.OUTROS:
                    processo = Processo.SOLICITACAO_OUTROS;
                    break;
            }

            var ultimoProtocolo = _workflowRepositorio.UltimoProtocolo(solicitacaoBusca.Id, processo);

            var workflow = new RecallService(token);

            var retorno = workflow.Recall(new CadastroRecall(ultimoProtocolo, User.ObterLogin(), User.ObterNome(), User.ObterEmail(), motivoRecall));

            if (retorno == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nenhuma resposta do serviço de Recall");

            if (retorno.sucesso)
            {
                if (solicitacaoBusca.StatusSolicitacao == StatusSolicitacao.EM_APROVAVAO)
                {
                    _solicitacoesRepositorio.AtualizarStatusSolicitacao(StatusSolicitacao.NOVO, solicitacaoBusca.Id);
                }

                return Json(new
                {
                    Processo = Processo.OPORTUNIDADE,
                    RedirectUrl = $"/Solicitacoes/Atualizar/{solicitacaoBusca.Id}"
                }, JsonRequestBehavior.AllowGet);
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, retorno.mensagem);
        }
    }
}