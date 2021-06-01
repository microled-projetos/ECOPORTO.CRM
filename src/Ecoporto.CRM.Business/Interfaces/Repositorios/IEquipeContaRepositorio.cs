using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface IEquipeContaRepositorio
    {      
        IEnumerable<EquipeContaUsuarioDTO> ObterUsuariosVinculados(int contaId);
        void Vincular(EquipeConta equipe);
        void Atualizar(EquipeConta equipe);
        void Excluir(int id);
        EquipeConta ObterVinculoPorId(int id);
        bool VinculoJaExistente(EquipeConta equipe);
        EquipeConta ObterPermissoesContaPorVendedor(int contaId, string login);
        EquipeConta ObterPermissoesContaPorConta(int contaId, string login);
        EquipeConta ObterPermissoesOportunidadePorVendedor(int vendedorId, string login);
        EquipeOportunidade ObterPermissoesPorOportunidade(int oportunidadeId, string login);
    }
}
