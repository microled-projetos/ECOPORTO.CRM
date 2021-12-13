using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface IAnaliseCreditoRepositorio
    {
        IEnumerable<PendenciaFinanceiraDTO> ObterPendenciasFinanceiras(string documento);
        IEnumerable<PendenciaFinanceiraDTO> GravarPendenciasFinanceiras(string documento);
        void GravarConsultaSpc(ConsultaSpcDTO consultaSpc, IEnumerable<Conta> contas);
        ConsultaSpcDTO ObterConsultaSpc(int contaId);
        ConsultaSpcDTO ObterExterno(int contaId);      
        IEnumerable<DetalhesSpcDTO> ObterDetalhesSpc(int consultaId);
        IEnumerable<DetalhesPendenciaFinanceiraDTO> ObterDetalhesPendenciasFinanceiras(int consultaId);
        IEnumerable<DetalhesChequeLojistaDTO> ObterDetalhesChequesLojistas(int consultaId);
        IEnumerable<ContraOrdemDocumentoDiferenteDTO> ObterDetalhesContraOrdemDocumentoDiferente(int consultaId);
        IEnumerable<ConsultaRealizadaDTO> ObterDetalhesHistoricoConsultas(int consultaId);
        IEnumerable<AlertaDocumentosDTO> ObterDetalhesAlertasDocumentos(int consultaId);
        IEnumerable<CCFDetalhesDTO> ObterDetalhesCCF(int consultaId);
        IEnumerable<int> ObterListaProcessoId(int contaId);
        void SolicitarLimiteDeCredito(LimiteCreditoSpcDTO limiteCredito);
        IEnumerable<LimiteCreditoSpcDTO> ObterSolicitacoesLimiteDeCredito(int contaId);
        int VerificarSeEstrangeiro(int contaId);
        int ObterSolicitacoesLimiteDeCreditoCond(int contaId, string condicao);
        LimiteCreditoSpcDTO ObterLimiteDeCreditoPorId(int contaId);
        LimiteCreditoSpcDTO ObterLimiteDeCreditoPorIdUnico(int id);
        LimiteCreditoSpcDTO VerificarLimiteDeCreditoPorId(int Id, int ContaId);
        void ExcluirLimiteDeCredito(int id);
        void GravarBlackList();
        void AtualizarSPC(int id);
        void AtualizarSPC1(int id);
        void AtualizarlimiteDeCredito(int id);
        void AtualizarlimiteDeCreditoPendente(int id);

        int buscaformaadendo(int id);
        

    }
}
