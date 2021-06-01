using Ecoporto.CRM.Business.DTO;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface IImpostosExcecaoRepositorio
    {
        IEnumerable<ImpostosExcecaoDTO> ObterServicos(int modeloId, int oportunidadeId);
        void GravarServicos(ImpostosExcecaoDTO model);
        void Excluir(int id);
        ImpostosExcecaoDTO ObterPorId(int id);
        void ExcluirTodosDaOportunidade(int oportunidadeId);
    }
}
