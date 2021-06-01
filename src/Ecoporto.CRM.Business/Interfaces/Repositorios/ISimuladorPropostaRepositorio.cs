using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface ISimuladorPropostaRepositorio
    {
        int CadastrarParametrosSimulador(SimuladorPropostaParametros simulador);
        void AtualizarParametrosSimulador(SimuladorPropostaParametros simulador);
        IEnumerable<OportunidadeParametrosSimuladorDTO> ObterParametrosSimulador(int oportunidadeId);
        OportunidadeParametrosSimuladorDTO ExisteParametrosSimulador(int oportunidadeId, int modeloSimuladorId);
        OportunidadeParametrosSimuladorDTO ObterParametroSimuladorPorId(int id);
        void ExcluirParametroSimulador(int id);
    }
}
