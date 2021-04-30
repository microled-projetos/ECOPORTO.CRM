using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Site.Extensions;
using Ecoporto.CRM.Site.Models;
using Ecoporto.CRM.Workflow.Enums;
using Ecoporto.CRM.Workflow.Models;
using Ecoporto.CRM.Workflow.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI;

namespace Ecoporto.CRM.Site.Controllers
{
    [Authorize]
    public class AnaliseCreditoController : Controller
    {
        private readonly IContaRepositorio _contaRepositorio;
        private readonly IAnaliseCreditoRepositorio _analiseCreditoRepositorio;
        private readonly IParametrosRepositorio _parametrosRepositorio;
        private readonly ICondicaoPagamentoFaturaRepositorio _condicaoPagamentoFaturaRepositorio;
        private readonly IWorkflowRepositorio _workflowRepositorio;

        private readonly Parametros _parametros;
        private readonly WsSPC.Ws _wsSPC;

        int ContaId = 0;

        public AnaliseCreditoController(
            IContaRepositorio contaRepositorio,
            IAnaliseCreditoRepositorio analiseCreditoRepositorio,
            IParametrosRepositorio parametrosRepositorio,
            ICondicaoPagamentoFaturaRepositorio condicaoPagamentoFaturaRepositorio,
            IWorkflowRepositorio workflowRepositorio)
        {
            _contaRepositorio = contaRepositorio;
            _analiseCreditoRepositorio = analiseCreditoRepositorio;
            _parametrosRepositorio = parametrosRepositorio;
            _condicaoPagamentoFaturaRepositorio = condicaoPagamentoFaturaRepositorio;
            _workflowRepositorio = workflowRepositorio;

            _parametros = _parametrosRepositorio.ObterParametros();
            _wsSPC = new WsSPC.Ws();
        }

        [HttpGet]
        public ActionResult Index(int id = 0)
        {
            var condicoesPagamento = _condicaoPagamentoFaturaRepositorio.ObterCondicoesPagamento();

            var viewModel = new AnaliseCreditoViewModel
            {
                ContasPesquisa = new List<Conta>(),
                CondicoesPagamento = condicoesPagamento,
                PendenciasFinanceiras = new List<PendenciaFinanceiraDTO>(),
                ResultadoSPC = new WsSPC.ConsultaSpcResponse(),
                SolicitacoesLimiteCredito = new List<LimiteCreditoSpcDTO>()
            };

            if (id != 0)
            {
                var contaSessao = _contaRepositorio.ObterContaPorId(id);
                Session["ContaId"] = contaSessao.Id;
                Session["RazaoSocial"] = contaSessao.Descricao;
                Session["FontePagadoraId"] = contaSessao.Id;
                Session["Cnpj"] = contaSessao.Documento;

            }
            

            return View(viewModel);
        }   

        [HttpGet]
        public PartialViewResult ConsultarContasPorDescricao(string descricao)
        {
            var resultado = _contaRepositorio.ObterContasPorDescricao(descricao, null);

            return PartialView("_PesquisarContasConsulta", resultado);
        }

        [HttpGet]
        public PartialViewResult ConsultarPendenciasFinanceiras(string documento)
        {
            var resultado = _analiseCreditoRepositorio
                .ObterPendenciasFinanceiras(documento.RemoverCaracteresEspeciaisDocumento());

            return PartialView("_PendenciasFinanceiras", resultado);
        }

        [HttpGet]
        public PartialViewResult ObterSolicitacoesLimiteCredito(int contaId)
        {
            var resultado = _analiseCreditoRepositorio
                .ObterSolicitacoesLimiteDeCredito(contaId);

            return PartialView("_SolicitacoesLimiteCredito", resultado);
        }

        [HttpGet]
        public ActionResult ConsultarClienteSpc(int? id)
        {
            try
            {
                var condicoesPagamento = _condicaoPagamentoFaturaRepositorio.ObterCondicoesPagamento();

                ViewBag.Controle = true;

                var viewModel = new AnaliseCreditoViewModel
                {
                    ContasPesquisa = new List<Conta>(),
                    CondicoesPagamento = condicoesPagamento,
                    PendenciasFinanceiras = new List<PendenciaFinanceiraDTO>(),
                    ResultadoSPC = new WsSPC.ConsultaSpcResponse(),
                    SolicitacoesLimiteCredito = new List<LimiteCreditoSpcDTO>()
                };

                viewModel.ContaPesquisaId = id.Value;

                ActionResult consulta = ConsultarSpc(viewModel);

                return RedirectToAction("Index", "AnaliseCredito", new { id = id });
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public ActionResult ConsultarSpc([Bind(Include = "ContaPesquisaId, Reprocessar, CondicaoPagamentoId, LimiteCredito, Observacao")] AnaliseCreditoViewModel viewModel)
        {
            WsSPC.ConsultaSpcResponse consultaSpc = new WsSPC.ConsultaSpcResponse();

           // _analiseCreditoRepositorio.GravarBlackList();

            var contaBusca = _contaRepositorio.ObterContaPorIdAnalise(viewModel.ContaPesquisaId);
            ContaId = viewModel.ContaPesquisaId;
            var consultaspcnew=0;

            if (contaBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Cliente não encontrado");

            if (contaBusca.Blacklist == true)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Cliente não tem permissão para consultar SPC");

            string tipoPessoa = contaBusca.ClassificacaoFiscal == ClassificacaoFiscal.PF
                   ? "F"
                   : "J";

            string hash = "d41d8cd98f00b204e9800998ecf8427e";

            var analiseCreditoBusca = _analiseCreditoRepositorio.ObterConsultaSpc(contaBusca.Id);

            if (viewModel.Reprocessar == false)
            {
                if (analiseCreditoBusca == null)
                {
                    // pesquisa no web service
                    consultaspcnew = 1;
                    consultaSpc = _wsSPC.ConsultarSPC(
                        tipoPessoa,
                        contaBusca.Documento.RemoverCaracteresEspeciaisDocumento(),
                        User.ObterId().ToString(),
                        hash);
                }
                else
                {
                    if (!ConsultaVigente(contaBusca, analiseCreditoBusca))
                    {
                        // pesquisa no web service
                        consultaspcnew = 1;
                        consultaSpc = _wsSPC.ConsultarSPC(
                            tipoPessoa, 
                            contaBusca.Documento.RemoverCaracteresEspeciaisDocumento(), 
                            User.ObterId().ToString(),
                            hash);
                    }
                    else
                    {
                        // pesquisa no banco
                        consultaSpc = _wsSPC.ConsultarBancoDeDados(contaBusca.Id.ToString(), hash);
                    }
                }
            }
            else
            {
                // sempre pesquisa no web service
                consultaspcnew = 1;
                consultaSpc = _wsSPC.ConsultarSPC(
                    tipoPessoa,
                    contaBusca.Documento.RemoverCaracteresEspeciaisDocumento(),
                    User.ObterId().ToString(),
                    hash);
            }

            viewModel.ResultadoSPC = consultaSpc;
           if (consultaspcnew == 1)
            {
                var resultadofat = _analiseCreditoRepositorio.GravarPendenciasFinanceiras(contaBusca.Documento.RemoverCaracteresEspeciaisDocumento());

            }

            return PartialView("_ResultadoSpc", viewModel);
        }
       
        private bool ConsultaVigente(Conta conta, ConsultaSpcDTO consultaSpc)
        {
            var consultaSpcAnterior = _analiseCreditoRepositorio.ObterConsultaSpc(conta.Id);

            Session["FontePagadoraId"] = conta.Id;

            if (consultaSpcAnterior == null || DateTime.Now <= consultaSpc.Validade)
            {
                return true;
            }

            return false;
        }

        private string  EnviarParaAprovacaocond(int processoId)
        {
            try
            {
                dynamic campos = null;
                var analiseCreditoBusca = _analiseCreditoRepositorio.ObterConsultaSpc(processoId);

                    var limiteCreditoCondPgtoBusca = _analiseCreditoRepositorio.ObterLimiteDeCreditoPorId(processoId);

                    if (limiteCreditoCondPgtoBusca == null)
                        return  "Conta não localizada!";
                    if (limiteCreditoCondPgtoBusca.StatusLimiteCredito == StatusLimiteCredito.APROVADO)
                        return  "Não precisa de Aprovação !";

                    var contaBusca = _contaRepositorio.ObterContaPorIdAnalise(limiteCreditoCondPgtoBusca.ContaId);

                    if (contaBusca == null)
                        return  "Conta não localizada!";
                    var aprovacoes = _workflowRepositorio.ObterAprovacoesLimiteDeCredito(processoId);

                    if (aprovacoes.Any() && limiteCreditoCondPgtoBusca.StatusLimiteCredito == StatusLimiteCredito.EM_APROVACAO)
                        return  "Já existe uma aprovação pendente para esta Oportunidade";

                    campos = new
                    {
                        CondicaoPagamentoFaturamentoId = limiteCreditoCondPgtoBusca.CondicaoPagamentoDescricao,
                        ContaId = contaBusca.Descricao + " - " + contaBusca.Documento,
                        InadimplenteEcoporto = limiteCreditoCondPgtoBusca.InadimplenteEcoporto.ToSimOuNao().Replace("Ã", "A").Replace("ã", "a").Replace("ç", "c").Replace("Ç", "C"),
                        DividaEcoporto = limiteCreditoCondPgtoBusca.TotalDividaEcoporto,
                        StatusCondPagto = StatusLimiteCredito.EM_APROVACAO.ToName().Replace("Ã", "A").Replace("ã", "a").Replace("ç", "c").Replace("Ç", "C"),
                        limiteCreditoCondPgtoBusca.LimiteCredito,
                        Observacao = limiteCreditoCondPgtoBusca.Observacoes
                    };
              

                var token = Autenticador.Autenticar();

                if (token == null)
                    return  "Não foi possível se autenticar no serviço de Workflow";

                var workflow = new WorkflowService(token);

                var retornoWorkflow = workflow.EnviarParaAprovacao(
                    new CadastroWorkflow(
                        Processo.ANALISE_DE_CREDITO_COND_PGTO,
                        1,
                        processoId,
                        User.ObterLogin(),
                        User.ObterNome(),
                        User.ObterEmail(),
                        JsonConvert.SerializeObject(campos)));

                if (retornoWorkflow == null)
                    return  "Nenhuma resposta do serviço de Workflow";

                if (retornoWorkflow.sucesso == false)
                    return  retornoWorkflow.mensagem;
               _analiseCreditoRepositorio.AtualizarlimiteDeCredito(processoId);

                var workFlowId = _workflowRepositorio.IncluirEnvioAprovacao(
                    new EnvioWorkflow(
                        processoId,
                        Processo.ANALISE_DE_CREDITO_COND_PGTO,
                        retornoWorkflow.protocolo,
                        retornoWorkflow.mensagem,
                        User.ObterId()));

                return "0";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [HttpPost]
        public ActionResult EnviarParaAprovacao(int processoId, Processo processo, [Bind(Include = "ContaPesquisaId")] AnaliseCreditoViewModel viewModel)
        {
            try
            {
                var contaSessao = _contaRepositorio.ObterContaPorId(viewModel.ContaPesquisaId);
                //string cnpj = razaoSocial.Substring(tamanho - 19, 18).Replace(")", "");
                ////achei
                //
                Session["ContaId"] = contaSessao.Id;
                Session["RazaoSocial"] = contaSessao.Descricao;
                Session["FontePagadoraId"] = contaSessao.Id;
                Session["Cnpj"] = contaSessao.Documento;
                dynamic campos = null;

                var contaEspecifica = _contaRepositorio.ObterContaPorId(viewModel.ContaPesquisaId);

                var analiseCreditoBusca = _analiseCreditoRepositorio.ObterConsultaSpc(processoId);

                if (processo == Processo.ANALISE_DE_CREDITO)
                {

                    if (analiseCreditoBusca == null)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Consulta SPC não localizada!");
                    if (analiseCreditoBusca.StatusAnaliseDeCredito == Ecoporto.CRM.Business.Enums.StatusAnaliseDeCredito.APROVADO)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Análise de Crédito dentro do prazo de Validade e está Aprovado!");
                    if (analiseCreditoBusca.StatusAnaliseDeCredito == Ecoporto.CRM.Business.Enums.StatusAnaliseDeCredito.REJEITADO)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Análise de Crédito dentro do prazo de Validade e está Rejeitada!");
                    if ((analiseCreditoBusca.InadimplenteSpc == false) && (analiseCreditoBusca.InadimplenteEcoporto == false))
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Conta não precisa de Aprovação !");
                    if (string.IsNullOrEmpty(analiseCreditoBusca.CondicaoPagamento))
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Conta sem condição de Pagamento !");
               
                    var contaBusca = _contaRepositorio.ObterContaPorIdAnalise(analiseCreditoBusca.ContaId);

                    if (contaBusca == null)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Conta não localizada!");

                    var aprovacoes = _workflowRepositorio.ObterAprovacoesAnaliseDeCredito(analiseCreditoBusca.ContaId);

                    if (aprovacoes.Any() && analiseCreditoBusca.StatusAnaliseDeCredito == StatusAnaliseDeCredito.EM_APROVACAO)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Já existe uma aprovação pendente para esta Oportunidade");

                    
                        var limiteCreditoCondPgto = _analiseCreditoRepositorio.ObterLimiteDeCreditoPorId(analiseCreditoBusca.ContaId);

                        if (limiteCreditoCondPgto != null)
                        {
                            if (limiteCreditoCondPgto.StatusLimiteCredito == StatusLimiteCredito.PENDENTE)
                            {
                            _analiseCreditoRepositorio.AtualizarlimiteDeCredito(limiteCreditoCondPgto.Id);
                                //EnviarParaAprovacaocond(limiteCreditoCondPgto.Id);
                            }
                        }
                        campos = new
                        {
                            ContaId = contaBusca.Id,
                            ContaCliente = contaBusca.Descricao + " - " + contaBusca.Documento,
                            StatusAnaliseCreditoPEFIN = StatusAnaliseDeCredito.EM_APROVACAO.ToName().Replace("Ã","A").Replace("ã", "a").Replace("ç", "c").Replace("Ç", "C"),
                            InadimplenteEcoporto=analiseCreditoBusca.InadimplenteEcoporto.ToSimOuNao().Replace("Ã", "A").Replace("ã", "a").Replace("ç", "c").Replace("Ç", "C"),
                            InadimplenteSPC = analiseCreditoBusca.InadimplenteSpc.ToSimOuNao().Replace("Ã", "A").Replace("ã", "a").Replace("ç", "c").Replace("Ç", "C"),
                            DividaSPC = analiseCreditoBusca.TotalDividaSpc,
                            DividaEcoporto=analiseCreditoBusca.TotalDividaEcoporto,
                            CondicaoPagamentoFaturamentoId=analiseCreditoBusca.CondicaoPagamentoDescricao ,
                            analiseCreditoBusca.LimiteCredito,
                            Observacao=analiseCreditoBusca.Observacoes,
                            Descricao = "A - " + analiseCreditoBusca.Id

                        };
                        }
                
                
                if (processo == Processo.ANALISE_DE_CREDITO_COND_PGTO)
                {
                    var limiteCreditoCondPgtoBusca = _analiseCreditoRepositorio.VerificarLimiteDeCreditoPorId(processoId, viewModel.ContaPesquisaId);

                    if (limiteCreditoCondPgtoBusca == null)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Conta não localizada!");
                    if (limiteCreditoCondPgtoBusca.StatusLimiteCredito== StatusLimiteCredito.APROVADO)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Não precisa de Aprovação !");

                    var contaBusca = _contaRepositorio.ObterContaPorIdAnalise(limiteCreditoCondPgtoBusca.ContaId);

                    if (contaBusca == null)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Conta não localizada!");
                    var aprovacoes = _workflowRepositorio.ObterAprovacoesLimiteDeCredito(processoId);

                    if (aprovacoes.Any() && limiteCreditoCondPgtoBusca.StatusLimiteCredito == StatusLimiteCredito.EM_APROVACAO)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Já existe uma aprovação pendente para esta Oportunidade");

                    campos = new
                {
                    CondicaoPagamentoFaturamentoId = limiteCreditoCondPgtoBusca.CondicaoPagamentoDescricao,
                    ContaId = contaBusca.Id,
                    ContaCliente = contaBusca.Descricao + " - " + contaBusca.Documento,
                    InadimplenteEcoporto = limiteCreditoCondPgtoBusca.InadimplenteEcoporto.ToSimOuNao().Replace("Ã", "A").Replace("ã", "a").Replace("ç", "c").Replace("Ç", "C"),
                    DividaEcoporto = limiteCreditoCondPgtoBusca.TotalDividaEcoporto,
                    StatusCondPagto = StatusLimiteCredito.EM_APROVACAO.ToName().Replace("Ã", "A").Replace("ã", "a").Replace("ç", "c").Replace("Ç", "C"),
                    limiteCreditoCondPgtoBusca.LimiteCredito,
                    Observacao=limiteCreditoCondPgtoBusca.Observacoes,
                    Descricao = "P - " + limiteCreditoCondPgtoBusca.Id
                    
                    };
                }

                var token = Autenticador.Autenticar();

                if (token == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Não foi possível se autenticar no serviço de Workflow");

                var workflow = new WorkflowService(token);

                var retornoWorkflow = workflow.EnviarParaAprovacao(
                    new CadastroWorkflow(
                        processo,
                        1,
                        processoId,
                        User.ObterLogin(),
                        User.ObterNome(),
                        User.ObterEmail(),
                        JsonConvert.SerializeObject(campos)));

                if (retornoWorkflow == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nenhuma resposta do serviço de Workflow");

                if (retornoWorkflow.sucesso == false)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, retornoWorkflow.mensagem);
                if (processo == Processo.ANALISE_DE_CREDITO)
                {
                    _analiseCreditoRepositorio.AtualizarSPC(processoId);
                }

                if (processo == Processo.ANALISE_DE_CREDITO_COND_PGTO)
                {
                    _analiseCreditoRepositorio.AtualizarlimiteDeCredito(processoId);
                }
                var workFlowId = _workflowRepositorio.IncluirEnvioAprovacao(
                    new EnvioWorkflow(
                        processoId,
                        processo,
                        retornoWorkflow.protocolo,
                        retornoWorkflow.mensagem,
                        User.ObterId()));

                var resultado = _analiseCreditoRepositorio
                .ObterSolicitacoesLimiteDeCredito(viewModel.ContaPesquisaId);

                return PartialView("_SolicitacoesLimiteCredito", resultado);
                //return new HttpStatusCodeResult(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [HttpPost]
        public ActionResult CadastrarLimiteCredito([Bind(Include = "ContaPesquisaId, Reprocessar, CondicaoPagamentoId, LimiteCredito, Observacao")] AnaliseCreditoViewModel viewModel)
        {
            if (viewModel.ContaPesquisaId == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nenhuma Conta Selecionada");

            if (string.IsNullOrEmpty(viewModel.CondicaoPagamentoId))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Condição Pagamento não selecionada");

            if (viewModel.LimiteCredito == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Limite de Crédito inválido");
            var resultadocond = _analiseCreditoRepositorio
            .ObterSolicitacoesLimiteDeCreditoCond(viewModel.ContaPesquisaId, viewModel.CondicaoPagamentoId);
            if (resultadocond> 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Condição Pagamento já foi registrada");

            _analiseCreditoRepositorio.SolicitarLimiteDeCredito(
                new LimiteCreditoSpcDTO
                {
                    ContaId = viewModel.ContaPesquisaId,
                    CondicaoPagamentoId = viewModel.CondicaoPagamentoId,
                    LimiteCredito = viewModel.LimiteCredito,
                    Observacoes = viewModel.Observacao,
                    StatusLimiteCredito = StatusLimiteCredito.PENDENTE

                });

            var resultado = _analiseCreditoRepositorio
                .ObterSolicitacoesLimiteDeCredito(viewModel.ContaPesquisaId);

            return PartialView("_SolicitacoesLimiteCredito", resultado);
        }

        [HttpGet]
        public ActionResult ObterDetalhesLimiteCredito(int id)
        {
            if (id == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nenhum Registro Selecionado");

            var solicitacaoBusca = _analiseCreditoRepositorio.ObterLimiteDeCreditoPorIdUnico(id);

            if (solicitacaoBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Registro não encontrado");

            return Json(solicitacaoBusca, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ExcluirLimiteCredito(int id, [Bind(Include = "ContaPesquisaId")] AnaliseCreditoViewModel viewModel)
        {
            if (id == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Nenhum Registro Selecionado");

            var solicitacaoBusca = _analiseCreditoRepositorio.ObterLimiteDeCreditoPorIdUnico(id);

            if (solicitacaoBusca == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Registro não encontrado");

            _analiseCreditoRepositorio.ExcluirLimiteDeCredito(id);

            return Json(solicitacaoBusca, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ObterHistoricoWorkflow(int contaId)
        {
            var token = Autenticador.Autenticar();

            int ProcessoId = 0; 

            if (token == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Não foi possível se autenticar no serviço de Workflow");
            
            var analiseCreditoBusca = _analiseCreditoRepositorio.ObterConsultaSpc(contaId);

            var listaProcessoId = _analiseCreditoRepositorio.ObterListaProcessoId(contaId);


            if (analiseCreditoBusca != null)
            {
                var workflow = new RetornoHistoricoWorkflow();

                //int[] processo = new int[2] { 13, 14 };
                var workflow_ = new WorkflowService(token)
                    .ObterHistoricoWorkflow(analiseCreditoBusca.ContaId, 13, 1);
                if (workflow_ != null)
                    workflow.list.AddRange(workflow_.list);

                workflow_ = null;
                foreach (int processoId in listaProcessoId)
                {
                     workflow_ = new WorkflowService(token)
                        .ObterHistoricoWorkflow(processoId, 14, 1);
                    if (workflow_ != null)
                        workflow.list.AddRange(workflow_.list);

                    workflow_ = null;
                }

                if (workflow != null)
                {
                    var WorkFlowss = workflow
                            .list.SelectMany(c => c.workFlows).ToList();


                    return PartialView("_ConsultaHistoricoWorkflow", new HistoricoWorkflowViewModel
                    {
                        WorkFlows = workflow
                            .list.SelectMany(c => c.workFlows).ToList()
                    });
                }
            }               

            return null;
        }
    }
}