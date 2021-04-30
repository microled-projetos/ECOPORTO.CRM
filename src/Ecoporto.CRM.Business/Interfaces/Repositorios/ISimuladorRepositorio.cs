using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface ISimuladorRepositorio
    {
        int CadastrarSimulador(Simulador simulador);
        void AtualizarSimulador(Simulador simulador);
        void ExcluirSimulador(int id);
        void IncluirCargaConteiner(SimuladorCargaConteiner simuladorCargaConteiner);
        void IncluirCargaSolta(SimuladorCargaSolta simuladorCargaSolta);
        void ExcluirCargaConteiner(int id);
        void ExcluirCargaSolta(int id);
        SimuladorCargaConteiner ObterCargaConteinerPorId(int id);
        SimuladorCargaSolta ObterCargaSoltaPorId(int id);
        IEnumerable<SimuladorDTO> ObterSimuladores();
        IEnumerable<SimuladorDTO> ObterSimuladoresPorUsuario(int usuarioId);
        IEnumerable<SimuladorTabelasDTO> ObterTabelasSimulador(string cnpj);
        SimuladorDTO ObterDetalhesSimuladorPorId(int id);
        IEnumerable<SimuladorCargaConteiner> ObterCargaConteiner(int simuladorId);
        IEnumerable<SimuladorCargaSolta> ObterCargaSolta(int simuladorId);
        void LimparServicosSimulador(string proposta);
        SimuladorDTO ObterClientesSimulador(string proposta);
        void CadastrarTabela(SimuladorDTO simulador);
        void ImportarArmazenagem(string proposta, int oportunidadeId);
        void ImportarServicoMargem(string proposta, int oportunidadeId);
        void ImportarServicoMecanicaManual(string proposta, int oportunidadeId);
        void ImportarServicoLiberacao(string proposta, int oportunidadeId);
        void ImportarServicoHubPort(string proposta, int oportunidadeId);
        void ImportarServicosGerais(string proposta, int oportunidadeId);
    }
}
