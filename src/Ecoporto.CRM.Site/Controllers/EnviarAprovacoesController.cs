using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Interfaces.Servicos;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Site.Services;
using Ecoporto.CRM.Workflow.Services;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using Ecoporto.CRM.Workflow.Models;
using Ecoporto.CRM.Workflow.Enums;
using NLog;
using System;
using Ecoporto.CRM.Site.Extensions;
using System.Net;

namespace Ecoporto.CRM.Site.Controllers
{
    [Authorize]
    public class EnviarAprovacoesController : BaseController
    {
        private readonly IOportunidadeRepositorio _oportunidadeRepositorio;
        private readonly IContaRepositorio _contaRepositorio;
        private readonly IMercadoriaRepositorio _mercadoriaRepositorio;
        private readonly IContatoRepositorio _contatoRepositorio;
        private readonly IImpostoRepositorio _impostoRepositorio;
        private readonly IModeloRepositorio _modeloRepositorio;
        private readonly IVendedorRepositorio _vendedorRepositorio;
        private readonly ILayoutPropostaRepositorio _layoutPropostaRepositorio;
        private readonly ICondicaoPagamentoFaturaRepositorio _condicaoPagamentoFaturaRepositorio;
        private readonly ICidadeRepositorio _cidadeRepositorio;
        private readonly IPremioParceriaRepositorio _premioParceriaRepositorio;
        private readonly IOportunidadeService _oportunidadeService;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IWorkflowRepositorio _workflowRepositorio;
        private readonly IAnexoRepositorio _anexoRepositorio;
        private readonly IEquipeContaRepositorio _equipeContaRepositorio;
        private readonly ILogger _logger;
        private readonly IEquipesService _equipesService;

        public EnviarAprovacoesController(
            IOportunidadeRepositorio oportunidadeRepositorio,
            IContaRepositorio contaRepositorio,
            IMercadoriaRepositorio mercadoriaRepositorio,
            IContatoRepositorio contatoRepositorio,
            IImpostoRepositorio impostoRepositorio,
            IModeloRepositorio modeloRepositorio,
            IVendedorRepositorio vendedorRepositorio,
            ILayoutPropostaRepositorio layoutPropostaRepositorio,
            ICondicaoPagamentoFaturaRepositorio condicaoPagamentoFaturaRepositorio,
            ICidadeRepositorio cidadeRepositorio,
            IPremioParceriaRepositorio premioParceriaRepositorio,
            IOportunidadeService oportunidadeService,
            IUsuarioRepositorio usuarioRepositorio,
            IWorkflowRepositorio workflowRepositorio,
            IAnexoRepositorio anexoRepositorio,
            IEquipeContaRepositorio equipeContaRepositorio,
            IEquipesService equipesService,
            ILogger logger) : base(logger)
        {
            _oportunidadeRepositorio = oportunidadeRepositorio;
            _contaRepositorio = contaRepositorio;
            _mercadoriaRepositorio = mercadoriaRepositorio;
            _contatoRepositorio = contatoRepositorio;
            _impostoRepositorio = impostoRepositorio;
            _modeloRepositorio = modeloRepositorio;
            _vendedorRepositorio = vendedorRepositorio;
            _layoutPropostaRepositorio = layoutPropostaRepositorio;
            _condicaoPagamentoFaturaRepositorio = condicaoPagamentoFaturaRepositorio;
            _cidadeRepositorio = cidadeRepositorio;
            _premioParceriaRepositorio = premioParceriaRepositorio;
            _oportunidadeService = oportunidadeService;
            _usuarioRepositorio = usuarioRepositorio;
            _workflowRepositorio = workflowRepositorio;
            _anexoRepositorio = anexoRepositorio;
            _equipeContaRepositorio = equipeContaRepositorio;
            _equipesService = equipesService;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Processar()
        {

            _logger.Info($"{DateTime.Now.DataHoraFormatada()} - Início do processamento");

            var oportunidades = _oportunidadeRepositorio.ObterOportunidadesPorStatus(StatusOportunidade.ENVIADO_PARA_APROVACAO);

            foreach (var oportunidade in oportunidades)
            {
                if (oportunidade.StatusOportunidade == StatusOportunidade.ENVIADO_PARA_APROVACAO)
                {
                    _logger.Info($"{DateTime.Now.DataHoraFormatada()} - Enviando oportunidade {oportunidade.Id} para aprovação");
                    EnviarOportunidadeParaAprovacao(oportunidade.Id, oportunidade.CriadoPor);
                    _logger.Info($"{DateTime.Now.DataHoraFormatada()} - Finalizado envio da oportunidade {oportunidade.Id} para aprovação");
                }
            }

            _logger.Info($"{DateTime.Now.DataHoraFormatada()} - Término do processamento");

            TempData["Sucesso"] = true;

            return RedirectToAction(nameof(Index));
        }

        public void EnviarOportunidadeParaAprovacao(int id, string criadoPor)
        {
            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(id);

            if (oportunidadeBusca == null)
                _logger.Info($"Oportunidade {oportunidadeBusca.Id} não encontrada");

            var token = Autenticador.Autenticar();

            if (token == null)
                _logger.Info("Não foi possível se autenticar no serviço de Workflow");

            var workflow = new WorkflowService(token);

            var oportunidadeDetalhes = _oportunidadeRepositorio.ObterDetalhesOportunidade(id);

            var usuario = _usuarioRepositorio.ObterUsuarioPorId(criadoPor.ToInt());

            var campos = new
            {
                oportunidadeId = oportunidadeDetalhes.Id,
                oportunidadeDetalhes.Descricao,
                oportunidadeDetalhes.Identificacao,
                ContaId = oportunidadeDetalhes.Conta,
                ContatoId = oportunidadeDetalhes.Contato,
                MercadoriaId = oportunidadeDetalhes.Mercadoria,
                oportunidadeDetalhes.Aprovada,
                oportunidadeDetalhes.DataFechamento,
                oportunidadeDetalhes.TabelaId,
                oportunidadeDetalhes.Probabilidade,
                oportunidadeDetalhes.SucessoNegociacao,
                oportunidadeDetalhes.ClassificacaoCliente,
                oportunidadeDetalhes.Segmento,
                oportunidadeDetalhes.EstagioNegociacao,
                oportunidadeDetalhes.StatusOportunidade,
                oportunidadeDetalhes.MotivoPerda,
                oportunidadeDetalhes.TipoDeProposta,
                oportunidadeDetalhes.TipoServico,
                oportunidadeDetalhes.TipoNegocio,
                oportunidadeDetalhes.TipoOperacaoOportunidade,
                oportunidadeDetalhes.RevisaoId,
                oportunidadeDetalhes.Observacao,
                oportunidadeDetalhes.FaturamentoMensalLCL,
                oportunidadeDetalhes.FaturamentoMensalFCL,
                oportunidadeDetalhes.VolumeMensal,
                oportunidadeDetalhes.CIFMedio,
                oportunidadeDetalhes.PremioParceria,
                CriadoPor = usuario.Login,
                oportunidadeDetalhes.TipoOperacao,
                oportunidadeDetalhes.Vendedor,
                oportunidadeDetalhes.FormaPagamento,
                DataInicio = oportunidadeDetalhes.DataInicio.DataFormatada(),
                DataTermino = oportunidadeDetalhes.DataTermino.DataFormatada()
            };           

            var retorno = workflow.EnviarParaAprovacao(
                new CadastroWorkflow(Processo.OPORTUNIDADE, 1, oportunidadeBusca.Id, usuario.Login, usuario.Nome, usuario.Email, JsonConvert.SerializeObject(campos)));

            if (retorno == null)
                throw new Exception("Nenhuma resposta do serviço de Workflow");

            if (retorno.sucesso == false)
                _logger.Info(retorno.mensagem);

            var oportunidadeWorkflow = new EnvioWorkflow(oportunidadeBusca.Id, Processo.OPORTUNIDADE, retorno.protocolo, retorno.mensagem, usuario.Id);
            _workflowRepositorio.IncluirEnvioAprovacao(oportunidadeWorkflow);

            if (oportunidadeBusca.StatusOportunidade != StatusOportunidade.CANCELADA)
                oportunidadeBusca.StatusOportunidade = StatusOportunidade.ENVIADO_PARA_APROVACAO;

            _oportunidadeRepositorio.AtualizarStatusOportunidade(oportunidadeBusca.StatusOportunidade, oportunidadeBusca.Id);
        }

        public void EnviarFichaFaturamentoParaAprovacao(int id)
        {
            var fichaFaturamentoBusca = _oportunidadeRepositorio.ObterFichaFaturamentoPorId(id);

            if (fichaFaturamentoBusca == null)
                _logger.Info($"Ficha Faturamento {fichaFaturamentoBusca.Id} não encontrada");

            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(fichaFaturamentoBusca.OportunidadeId);

            if (oportunidadeBusca == null)
                _logger.Info($"Oportunidade {oportunidadeBusca.Id} não encontrada");

            var aprovacoes = _workflowRepositorio.ObterAprovacoesPorOportunidade(oportunidadeBusca.Id, Processo.FICHA_FATURAMENTO);

            if (aprovacoes.Any() && fichaFaturamentoBusca.StatusFichaFaturamento == StatusFichaFaturamento.EM_APROVACAO)
                _logger.Info($"Já existe uma aprovação pendente para esta Ficha de Faturamento {fichaFaturamentoBusca.Id}");

            var token = Autenticador.Autenticar();

            if (token == null)
                _logger.Info("Não foi possível se autenticar no serviço de Workflow");

            var workflow = new WorkflowService(token);

            var conta = _contaRepositorio.ObterContaPorId(oportunidadeBusca.ContaId);

            CondicaoPagamentoFatura condicaoPagamentoFatura = null;

            if (fichaFaturamentoBusca.CondicaoPagamentoFaturamentoId != null)
                condicaoPagamentoFatura = _condicaoPagamentoFaturaRepositorio.ObterCondicoPagamentoPorId(fichaFaturamentoBusca.CondicaoPagamentoFaturamentoId);

            var oportunidadeDetalhes = _oportunidadeRepositorio.ObterDetalhesOportunidade(oportunidadeBusca.Id);

            var campos = new
            {
                OportunidadeId = oportunidadeBusca.Id,
                Descricao = $"F-{fichaFaturamentoBusca.Id}",
                DescricaoOportunidade = oportunidadeBusca.Descricao,
                StatusFichaFaturamento = fichaFaturamentoBusca.StatusFichaFaturamento.ToName(),
                CondicaoPagamentoFaturamentoId = condicaoPagamentoFatura?.Descricao,
                ContaId = oportunidadeDetalhes.Conta,
                DataInicio = oportunidadeDetalhes.DataInicio.DataFormatada(),
                DataTermino = oportunidadeDetalhes.DataTermino.DataFormatada(),
                oportunidadeDetalhes.TabelaId,
                fichaFaturamentoBusca.FaturadoContraId,
                fichaFaturamentoBusca.DiasSemana,
                fichaFaturamentoBusca.DiasFaturamento,
                fichaFaturamentoBusca.DataCorte,
                fichaFaturamentoBusca.EmailFaturamento,
                fichaFaturamentoBusca.ObservacoesFaturamento,
                fichaFaturamentoBusca.AnexoFaturamento,
                oportunidadeDetalhes.TipoOperacao
            };
            
            var retorno = workflow.EnviarParaAprovacao(
                new CadastroWorkflow(Processo.FICHA_FATURAMENTO, 1, fichaFaturamentoBusca.Id, User.ObterLogin(), User.ObterNome(), User.ObterEmail(), JsonConvert.SerializeObject(campos)));

            if (retorno == null)
                throw new Exception("Nenhuma resposta do serviço de Workflow");

            if (retorno.sucesso == false)
                _logger.Info(retorno.mensagem);

            _workflowRepositorio.IncluirEnvioAprovacao(new EnvioWorkflow(oportunidadeBusca.Id, Processo.FICHA_FATURAMENTO, retorno.protocolo, retorno.mensagem, User.ObterId()));

            _oportunidadeRepositorio.AtualizarStatusFichaFaturamento(StatusFichaFaturamento.EM_APROVACAO, fichaFaturamentoBusca.Id);

            _logger.Info($"Ficha de Faturamento {fichaFaturamentoBusca.Id} enviada para Aprovação");
        }

        public void EnviarPremioParceriaParaAprovacao(int id)
        {
            var premioParceriaBusca = _premioParceriaRepositorio.ObterPremioParceriaPorId(id);

            if (premioParceriaBusca == null)
                _logger.Info($"Prêmio Parceria {premioParceriaBusca.Id} não encontrado");

            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(premioParceriaBusca.OportunidadeId);

            if (oportunidadeBusca == null)
                _logger.Info($"Oportunidade {oportunidadeBusca.Id} não encontrada");

            var aprovacoes = _workflowRepositorio.ObterAprovacoesPorOportunidade(oportunidadeBusca.Id, Processo.PREMIO_PARCERIA);

            if (aprovacoes.Any() && premioParceriaBusca.StatusPremioParceria == StatusPremioParceria.EM_APROVACAO)
                _logger.Info($"Já existe uma aprovação pendente para este Prêmio Parceria {premioParceriaBusca.Id}");

            var token = Autenticador.Autenticar();

            if (token == null)
                _logger.Info("Não foi possível se autenticar no serviço de Workflow");

            var workflow = new WorkflowService(token);

            var premioDetalhes = _premioParceriaRepositorio.ObterDetalhesPremioParceria(premioParceriaBusca.Id);

            var campos = new
            {
                Descricao = $"P-{premioDetalhes.Id}",
                DescricaoOportunidade = oportunidadeBusca.Descricao,
                ContaDescricao = oportunidadeBusca.Conta.Descricao,
                premioDetalhes.OportunidadeId,
                ContatoId = premioDetalhes.DescricaoContato,
                StatusPremioParceria = premioDetalhes.DescricaoStatusPremioParceria,
                Instrucao = premioDetalhes.DescricaoInstrucao,
                TipoServicoPremioParceria = premioDetalhes.DescricaoServicoPremioParceria,
                Favorecido1 = premioDetalhes.DescricaoFavorecido1,
                Favorecido2 = premioDetalhes.DescricaoFavorecido2,
                Favorecido3 = premioDetalhes.DescricaoFavorecido3,
                PremioReferenciaId = $"P-{premioDetalhes.PremioReferenciaId}",
                premioDetalhes.Observacoes,
                premioDetalhes.Anexo,
                premioDetalhes.UrlPremio,
                DataUrlPremio = premioDetalhes.DataUrlPremio.DataFormatada(),
                premioDetalhes.EmailFavorecido1,
                premioDetalhes.EmailFavorecido2,
                premioDetalhes.EmailFavorecido3,
                premioDetalhes.CriadoPor,
                TipoOperacao = premioDetalhes.DescricaoTipoOperacao
            };

            var retorno = workflow.EnviarParaAprovacao(
                new CadastroWorkflow(Processo.PREMIO_PARCERIA, 1, premioParceriaBusca.Id, User.ObterLogin(), User.ObterNome(), User.ObterEmail(), JsonConvert.SerializeObject(campos)));

            if (retorno == null)
                throw new Exception("Nenhuma resposta do serviço de Workflow");

            if (retorno.sucesso == false)
                _logger.Info(retorno.mensagem);

            _workflowRepositorio.IncluirEnvioAprovacao(new EnvioWorkflow(oportunidadeBusca.Id, Processo.PREMIO_PARCERIA, retorno.protocolo, retorno.mensagem, User.ObterId()));

            _premioParceriaRepositorio.AtualizarStatusPremioParceria(StatusPremioParceria.EM_APROVACAO, premioParceriaBusca.Id);

            _logger.Info($"Prêmio Parceria {premioParceriaBusca.Id} enviado para Aprovação");
        }

        public void EnviarAdendoParaAprovacao(int id)
        {
            var adendoBusca = _oportunidadeRepositorio.ObterAdendoPorId(id);

            if (adendoBusca == null)
                _logger.Info($"Adendo {adendoBusca.Id} não encontrado");

            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(adendoBusca.OportunidadeId);

            if (oportunidadeBusca == null)
                _logger.Info($"Oportunidade {oportunidadeBusca.Id} não encontrada");

            if (oportunidadeBusca.OportunidadeProposta == null || oportunidadeBusca.OportunidadeProposta.ModeloId == 0)
                _logger.Info($"A Oportunidade {oportunidadeBusca.Id} não possui nenhum Modelo Vinculado");

            var modelo = _modeloRepositorio.ObterModeloPorId(oportunidadeBusca.OportunidadeProposta.ModeloId);

            var aprovacoes = _workflowRepositorio.ObterAprovacoesPorOportunidade(oportunidadeBusca.Id, Processo.ADENDO);

            if (aprovacoes.Any() && adendoBusca.StatusAdendo == StatusAdendo.ENVIADO)
                _logger.Info($"Já existe uma aprovação pendente para este Adendo {adendoBusca.Id}");

            var token = Autenticador.Autenticar();

            if (token == null)
                _logger.Info("Não foi possível se autenticar no serviço de Workflow");

            var workflow = new WorkflowService(token);

            IEnumerable<dynamic> clientes = null;

            if (adendoBusca.TipoAdendo == TipoAdendo.INCLUSAO_SUB_CLIENTE)
            {
                clientes = _oportunidadeRepositorio
                    .ObterAdendoSubClientesInclusao(adendoBusca.Id)
                    .Select(c => $"{c.Descricao} ({ c.Documento})");
            }

            if (adendoBusca.TipoAdendo == TipoAdendo.INCLUSAO_GRUPO_CNPJ)
            {
                clientes = _oportunidadeRepositorio
                    .ObterAdendoGruposCNPJ(adendoBusca.Id, AdendoAcao.INCLUSAO)
                    .Select(c => $"{c.Descricao} ({ c.Documento})");
            }

            if (adendoBusca.TipoAdendo == TipoAdendo.EXCLUSAO_SUB_CLIENTE)
            {
                clientes = _oportunidadeRepositorio
                    .ObterAdendoSubClientesExclusao(adendoBusca.Id)
                    .Select(c => $"{c.Descricao} ({ c.Documento})");
            }

            if (adendoBusca.TipoAdendo == TipoAdendo.EXCLUSAO_GRUPO_CNPJ)
            {
                clientes = _oportunidadeRepositorio
                    .ObterAdendoGruposCNPJ(adendoBusca.Id, AdendoAcao.EXCLUSAO)
                    .Select(c => $"{c.Descricao} ({ c.Documento})");
            }

            if (adendoBusca.TipoAdendo == TipoAdendo.FORMA_PAGAMENTO)
            {
                var adendoFormaPagamento = _oportunidadeRepositorio
                    .ObterAdendoFormaPagamento(adendoBusca.Id)
                    .FormaPagamento
                    .ToName();

                var lst = new List<string>
                {
                    adendoFormaPagamento
                };

                clientes = lst;
            }

            if (adendoBusca.TipoAdendo == TipoAdendo.ALTERACAO_VENDEDOR)
            {
                var adendoVendedor = _oportunidadeRepositorio
                    .ObterAdendoVendedor(adendoBusca.Id).VendedorId;

                var vendedor = _vendedorRepositorio.ObterVendedorPorId(adendoVendedor);

                var lst = new List<string>
                {
                    vendedor.Nome
                };

                clientes = lst;
            }

            var oportunidadeDetalhes = _oportunidadeRepositorio.ObterDetalhesOportunidade(oportunidadeBusca.Id);

            var campos = new
            {
                OportunidadeId = oportunidadeBusca.Id,
                Descricao = $"A-{adendoBusca.Id}",
                DescricaoOportunidade = oportunidadeBusca.Descricao,
                oportunidadeDetalhes.Conta,
                oportunidadeDetalhes.TabelaId,
                TipoAdendo = adendoBusca.TipoAdendo.ToName(),
                StatusAdendo = adendoBusca.StatusAdendo.ToName(),
                adendoBusca.DataCadastro,
                adendoBusca.CriadoPor,
                TipoOperacao = modelo.TipoOperacao.ToString(),
                Clientes = clientes != null ? string.Join(",", clientes) : string.Empty
            };

            var retorno = workflow.EnviarParaAprovacao(
                new CadastroWorkflow(Processo.ADENDO, 1, adendoBusca.Id, User.ObterLogin(), User.ObterNome(), User.ObterEmail(), JsonConvert.SerializeObject(campos)));

            if (retorno == null)
                throw new Exception( "Nenhuma resposta do serviço de Workflow");

            if (retorno.sucesso == false)
                _logger.Info(retorno.mensagem);

            _workflowRepositorio.IncluirEnvioAprovacao(new EnvioWorkflow(oportunidadeBusca.Id, Processo.ADENDO, retorno.protocolo, retorno.mensagem, User.ObterId()));

            _oportunidadeRepositorio.AtualizarStatusAdendo(StatusAdendo.ENVIADO, adendoBusca.Id);

            _logger.Info($"Adendo {adendoBusca.Id} enviado para Aprovação");
        }
    }
}