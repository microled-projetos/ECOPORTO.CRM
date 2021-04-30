using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface IMotivosRepositorio
    {
        void CadastrarMotivo(SolicitacaoComercialMotivo solicitacaoMotivo);
        void AtualizarMotivo(SolicitacaoComercialMotivo solicitacaoMotivo);
        void ExcluirMotivo(int motivoSolicitacaoId);
        SolicitacaoComercialMotivo ObterSolicitacaoMotivoPorId(int id);
        IEnumerable<SolicitacaoComercialMotivo> ObterSolicitacoesMotivo();
    }
}
