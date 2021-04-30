using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface IEquipeVendedorRepositorio
    {
        IEnumerable<EquipeVendedorUsuarioDTO> ObterUsuariosVinculados(int vendedorId);
        void Vincular(EquipeVendedor equipe);
        void Atualizar(EquipeVendedor equipe);
        void Excluir(int id);
        EquipeVendedor ObterVinculoPorId(int id);
        bool VinculoJaExistente(EquipeVendedor equipe);
    }
}
