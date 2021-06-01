using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface IModeloSimuladorRepositorio
    {
        int Cadastrar(ModeloSimulador modeloSimulador);
        void Atualizar(ModeloSimulador modeloSimulador);
        void Excluir(int id);
        ModeloSimulador ObterModeloSimuladorPorId(int id);
        IEnumerable<ModeloSimulador> ObterModelosSimulador();
        VinculoModeloSimuladoDTO ObterVinculoSimuladorPorId(int id);
    }
}
