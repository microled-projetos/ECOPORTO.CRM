using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface IModeloRepositorio
    {
        int Cadastrar(Modelo modelo);
        void Atualizar(Modelo modelo);
        IEnumerable<Modelo> ObterModelos();
        IEnumerable<Modelo> ObterModelosAtivos();
        Modelo ObterModeloPorId(int id);
        void Excluir(int id);
        string ObterValorPorCampo(string campo, int id);
        Modelo ObterModeloPorDescricao(string descricao, int? id = 0);
        IEnumerable<Modelo> ObterModelosPorDescricao(string descricao);
        IEnumerable<Modelo> ObterModelosPorTipoOperacao(TipoOperacao tipo);
        IEnumerable<VinculoModeloSimuladoDTO> ObterModelosSimuladorVinculados(int id);
        void CadastrarModeloSimulador(ModeloSimulador modeloSimulador);
        void ExcluirModeloSimulador(int id);
    }
}
