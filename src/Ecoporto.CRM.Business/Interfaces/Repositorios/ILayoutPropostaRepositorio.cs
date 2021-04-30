using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface ILayoutPropostaRepositorio : ILayoutBase
    {
        void AtualizarTitulos(int linha, string descricao, int oportunidadeId, int tipoRegistro);
        void AtualizarArmazenagem(LayoutArmazenagem layout, int linha, int oportunidadeId);
        void AtualizarArmazenagemCIF(LayoutArmazenagemCIF layout, int linha, int oportunidadeId);
        void AtualizarArmazenagemMinimo(LayoutArmazenagemMinimo layout, int linha, int oportunidadeId);
        void AtualizarArmazenagemMinimoCIF(LayoutArmazenagemMinimoCIF layout, int linha, int oportunidadeId);
        void AtualizarArmazenagemAllIn(LayoutArmazenagemAllIn layout, int linha, int oportunidadeId);
        void AtualizarServicoMargem(LayoutServicoParaMargem layout, int linha, int oportunidadeId);
        void AtualizarMinimoMargem(LayoutMinimoParaMargem layout, int linha, int oportunidadeId);
        void AtualizarServicoMecanicaManual(LayoutServicoMecanicaManual layout, int linha, int oportunidadeId);
        void AtualizarMinimoMecanicaManual(LayoutMinimoMecanicaManual layout, int linha, int oportunidadeId);
        void AtualizarServicoLiberacao(LayoutServicoLiberacao layout, int linha, int oportunidadeId);
        void AtualizarHubPort(LayoutServicoHubPort layout, int linha, int oportunidadeId);
        void AtualizarServicosGerais(LayoutServicosGerais layout, int linha, int oportunidadeId);
        void AtualizarMinimoGeral(LayoutMinimoGeral layout, int linha, int oportunidadeId);
        void AtualizarPeriodoPadrao(LayoutPeriodoPadrao layout, int linha, int oportunidadeId);
        void AtualizarCondicoesIniciais(LayoutCondicoesIniciais layout, int linha, int oportunidadeId);
        void AtualizarCondicoesGerais(LayoutCondicoesGerais layout, int linha, int oportunidadeId);
        LayoutDTO ObterLayoutEdicaoPorLinha(int oportunidadeId, int linha);
        void ExcluirLinhaOportunidadeProposta(int oportunidadeId, int linha);
        void GravarCampoPropostaAlterado(OportunidadeAlteracaoLinhaProposta alteracao);
        void LimparCamposAlterados(int oportunidadeId);
        IEnumerable<OportunidadeAlteracaoLinhaProposta> ObterAlteracoesProposta(int oportunidadeId);
        void AtualizarCamposEmComumArmazenagem(LayoutArmazenagem layout, string campo,  int oportunidadeId, int linha, int id);
        void AtualizarCamposEmComumArmazenagemCIF(LayoutArmazenagemCIF layout, string campo, int oportunidadeId, int linha, int id);
        void CadastrarFaixasBL(FaixaBL faixa);
        void ExcluirFaixaBL(int id);
        IEnumerable<FaixaBL> ObterFaixasBL(int layoutId);
        FaixaBL ObterFaixaBLPorId(int id);
        void CadastrarFaixasCIF(FaixaCIF faixa);
        void ExcluirFaixaCIF(int id);
        IEnumerable<FaixaCIF> ObterFaixasCIF(int layoutId);
        FaixaCIF ObterFaixaCIFPorId(int id);
        void CadastrarFaixasVolume(FaixaVolume faixa);
        void ExcluirFaixaVolume(int id);
        IEnumerable<FaixaVolume> ObterFaixasVolume(int layoutId);
        FaixaVolume ObterFaixasVolumePorId(int id);
        void CadastrarFaixasPeso(FaixaPeso faixa);
        void ExcluirFaixaPeso(int id);
        IEnumerable<FaixaPeso> ObterFaixasPeso(int layoutId);
        FaixaPeso ObterFaixaPesoPorId(int id);
        LayoutDTO ObterLinhaAnterior(int linha);        
    }
}
