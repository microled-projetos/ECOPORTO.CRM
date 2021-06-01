using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Filtros;
using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface ISolicitacoesRepositorio
    {
        IEnumerable<SolicitacaoDTO> ObterSolicitacoes(int pagina, int registrosPorPagina, SolicitacoesFiltro filtro, string orderBy, out int totalFiltro);
        int ObterTotalSolicitacoes();
        IEnumerable<SolicitacaoDTO> ObterSolicitacoesAcessoExterno(int pagina, int registrosPorPagina, SolicitacoesFiltro filtro, string orderBy, out int totalFiltro, SolicitacoesUsuarioExternoFiltro usuarioExternoFiltro);
        int ObterTotalSolicitacoesAcessoExterno();
        int Cadastrar(SolicitacaoComercial solicitacao);
        void Atualizar(SolicitacaoComercial solicitacao);
        void AtualizarResumoRestituicao(SolicitacaoComercial solicitacao);
        void AtualizarStatusSolicitacao(StatusSolicitacao status, int id);
        SolicitacaoComercial ObterSolicitacaoPorId(int id);
        void Excluir(int id, TipoSolicitacao tipoSolicitacao);
        SolicitacaoDTO ObterDetalhesSolicitacao(int id);
        void CadastrarCancelamentoNF(SolicitacaoCancelamentoNF solicitacaoDadosFinanceiros);
        void AtualizarCancelamentoNF(SolicitacaoCancelamentoNF solicitacaoDadosFinanceiros);
        void ExcluirCancelamentoNF(int id);
        IEnumerable<SolicitacaoCancelamentoNFDTO> ObterCancelamentosNF(int solicitacaoId);
        SolicitacaoCancelamentoNF ObterCancelamentoNFPorId(int id);
        void CadastrarProrrogacao(SolicitacaoProrrogacao solicitacao);
        void AtualizarProrrogacao(SolicitacaoProrrogacao solicitacao);
        SolicitacaoProrrogacao ObterProrrogacaoPorId(int id);
        void ExcluirProrrogacao(int id);
        IEnumerable<SolicitacaoProrrogacaoDTO> ObterProrrogacoes(int solicitacaoId);
        void CadastrarRestituicao(SolicitacaoRestituicao solicitacao);
        void AtualizarRestituicao(SolicitacaoRestituicao solicitacao);
        SolicitacaoRestituicao ObterRestituicaoPorId(int id);
        SolicitacaoRestituicao ObterRestituicaoPorNotaFiscal(int notaFiscalId, int solicitacaoId);
        SolicitacaoRestituicaoDTO ObterDetalhesRestituicao(int id);
        void ExcluirRestituicao(int id);
        IEnumerable<SolicitacaoRestituicaoDTO> ObterRestituicoes(int solicitacaoId);
        void CadastrarDesconto(SolicitacaoDesconto solicitacao);
        void AtualizarDesconto(SolicitacaoDesconto solicitacao);
        SolicitacaoDesconto ObterDescontoPorId(int id);
        SolicitacaoDescontoDTO ObterDetalhesDesconto(int id);
        SolicitacaoDescontoDTO ObterDetalhesDescontoRedex(int id);
        void ExcluirDesconto(int id);
        IEnumerable<SolicitacaoDescontoDTO> ObterDescontos(int solicitacaoid);
        IEnumerable<SolicitacaoDescontoDTO> ObterDescontosRedex(int solicitacaoid);
        IEnumerable<NotaFiscal> ObterNotasFiscaisPorTipoPesquisa(TipoPesquisa tipoPesquisa, TipoOperacao tipoOperacao, string termoPesquisa, SolicitacoesUsuarioExternoFiltro usuarioExternoFiltro);
        IEnumerable<AnexosDTO> ObterAnexosDaSolicitacao(int solicitacaoId);
        IEnumerable<UsuarioDTO> ObterUsuariosSolicitacoes();
        void CadastrarAlteracaoFormaPgto(SolicitacaoAlteraFormaPagamento solicitacao);
        void AtualizarAlteracaoFormaPgto(SolicitacaoAlteraFormaPagamento solicitacao);
        SolicitacaoAlteraFormaPagamento ObterAlteracaoFormaPgtoPorId(int id);
        IEnumerable<SolicitacaoFormaPagamentoDTO> ObterAlteracoesFormaPagamento(int solicitacaoId);
        void ExcluirAlteracoesFormaPagamento(int id);
        IEnumerable<SolicitacaoUnidade> ObterUnidadesSolicitacao();
        IEnumerable<SolicitacaoTipoOperacao> ObterTiposOperacaoSolicitacao();
    }
}
