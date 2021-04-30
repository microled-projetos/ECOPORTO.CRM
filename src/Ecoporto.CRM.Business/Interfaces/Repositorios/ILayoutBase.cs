using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface ILayoutBase
    {
        void CadastrarLayoutTitulo(LayoutTitulo layout);
        void AtualizarLayoutTitulo(LayoutTitulo layout);
        void CadastrarLayoutTituloMaster(LayoutTituloMaster layout);
        void AtualizarLayoutTituloMaster(LayoutTituloMaster layout);
        void CadastrarLayoutSubTitulo(LayoutSubTitulo layout);
        void AtualizarLayoutSubTitulo(LayoutSubTitulo layout);
        void CadastrarLayoutSubTituloMargem(LayoutSubTituloMargem layout);
        void CadastrarLayoutSubTituloAllIn(LayoutSubTituloAllIn layout);
        void AtualizarLayoutSubTituloAllIn(LayoutSubTituloAllIn layout);
        void AtualizarLayoutSubTituloMargem(LayoutSubTituloMargem layout);
        void CadastrarLayoutArmazenagem(LayoutArmazenagem layout);
        void AtualizarLayoutArmazenagem(LayoutArmazenagem layout);
        void CadastrarLayoutArmazenagemCIF(LayoutArmazenagemCIF layout);
        void AtualizarLayoutArmazenagemCIF(LayoutArmazenagemCIF layout);
        int CadastrarLayoutArmazenagemMinimoCIF(LayoutArmazenagemMinimoCIF layout);
        void AtualizarLayoutArmazenagemMinimoCIF(LayoutArmazenagemMinimoCIF layout);
        void CadastrarLayoutPeriodoPadrao(LayoutPeriodoPadrao layout);
        void AtualizarLayoutPeriodoPadrao(LayoutPeriodoPadrao layout);
        int CadastrarLayoutArmazenagemMinimo(LayoutArmazenagemMinimo layout);
        void AtualizarLayoutArmazenagemMinimo(LayoutArmazenagemMinimo layout);
        int CadastrarLayoutArmazenagemAllIn(LayoutArmazenagemAllIn layout);
        void AtualizarLayoutArmazenagemAllIn(LayoutArmazenagemAllIn layout);
        int CadastrarLayoutServicoParaMargem(LayoutServicoParaMargem layout);
        void AtualizarLayoutServicoParaMargem(LayoutServicoParaMargem layout);
        void CadastrarLayoutMinimoParaMargem(LayoutMinimoParaMargem layout);
        void AtualizarLayoutMinimoParaMargem(LayoutMinimoParaMargem layout);
        void CadastrarLayoutServicoMecanicaManual(LayoutServicoMecanicaManual layout);
        void AtualizarLayoutServicoMecanicaManual(LayoutServicoMecanicaManual layout);
        void CadastrarLayoutMinimoMecanicaManual(LayoutMinimoMecanicaManual layout);
        void AtualizarLayoutMinimoMecanicaManual(LayoutMinimoMecanicaManual layout);
        void CadastrarLayoutServicoLiberacao(LayoutServicoLiberacao layout);
        void AtualizarLayoutServicoLiberacao(LayoutServicoLiberacao layout);
        void CadastrarLayoutServicoHubPort(LayoutServicoHubPort layout);
        void AtualizarLayoutServicoHubPort(LayoutServicoHubPort layout);
        void CadastrarLayoutServicosGerais(LayoutServicosGerais layout);
        void AtualizarLayoutServicosGerais(LayoutServicosGerais layout);
        void CadastrarLayoutMinimoGeral(LayoutMinimoGeral layout);
        void AtualizarLayoutMinimoGeral(LayoutMinimoGeral layout);
        void CadastrarLayoutCondicoesGerais(LayoutCondicoesGerais layout);
        void AtualizarLayoutCondicoesGerais(LayoutCondicoesGerais layout);
        void CadastrarLayoutCondicoesIniciais(LayoutCondicoesIniciais layout);
        void AtualizarLayoutCondicoesIniciais(LayoutCondicoesIniciais layout);
        bool LinhaJaCadastrada(Cabecalho cabecalho);
        bool ExistemLinhasPosteriores(Cabecalho cabecalho);
        void AtualizarLinhasPosteriores(Cabecalho cabecalho);
        IEnumerable<LayoutDTO> ObterLayouts(int modeloId, bool ocultar);
        LayoutDTO ObterLayoutPorId(int id);
        IEnumerable<EdicaoValoresPropostaDTO> ObterLayoutEdicaoProposta(int oportunidadeId);
        LayoutDTO ObterLayoutPorServico(int servicoId);
        LayoutDTO ObterLayoutPorModelo(int modeloId);
        int ObterUltimaLinha(int modeloId);
        void ExcluirLinha(int linha, int modeloId);
        void Excluir(int id);
        string ObterValorPorLinha(int linha, int modeloId, string campo, int oportunidadeId = 0);
        string ObterValorSemLinha(int modeloId, string campo, int oportunidadeId = 0);
        void ImportarLayout(int modeloNovo, int modeloAntigo);
        string ObterTipoRegistroPorLinha(int linha, int modeloId);
        IEnumerable<LayoutDTO> ObterProximasLinhas(int linha, int modeloId, int oportunidadeId);
        bool LayoutProposta();
    }
}
