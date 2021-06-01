using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface IEquipeOportunidadeRepositorio
    {      
        IEnumerable<EquipeOportunidadeUsuarioDTO> ObterUsuariosVinculados(int oportunidadeId);
        void Vincular(EquipeOportunidade equipe);
        void Atualizar(EquipeOportunidade equipe);
        EquipeOportunidade ObterVinculoPorId(int id);
        bool VinculoJaExistente(EquipeOportunidade equipe);
        void Excluir(int id);
    }
}
