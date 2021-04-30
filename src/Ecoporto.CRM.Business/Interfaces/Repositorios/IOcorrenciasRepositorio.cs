using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface IOcorrenciasRepositorio
    {
        void CadastrarOcorrencia(SolicitacaoComercialOcorrencia solicitacaoOcorrencia);
        void AtualizarOcorrencia(SolicitacaoComercialOcorrencia solicitacaoOcorrencia);
        void ExcluirOcorrencia(int ocorrenciaSolicitacaoId);
        IEnumerable<SolicitacaoComercialOcorrencia> ObterSolicitacoesOcorrencia();
        SolicitacaoComercialOcorrencia ObterSolicitacaoOcorrenciaPorId(int id);
    }
}
