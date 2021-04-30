using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Workflow.Enums;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface IWorkflowRepositorio
    {
        int IncluirEnvioAprovacao(EnvioWorkflow oportunidadeWorkflow);
        void CancelarEnvioAprovacao(int id);
        IEnumerable<EnvioWorkflow> ObterAprovacoesPorOportunidade(int oportunidadeId, Processo processo);
        IEnumerable<EnvioWorkflow> ObterAprovacoesAnaliseDeCredito(int processoId);
        IEnumerable<EnvioWorkflow> ObterAprovacoesLimiteDeCredito(int processoId);

        int UltimoProtocolo(int oportunidadeId, Processo processo, int processoFilhoId = 0);
        int AcessoSistemaWorkflow(Usuario usuario);
    }
}
