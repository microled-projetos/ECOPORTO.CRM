using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface IAnexoRepositorio
    {
        int IncluirAnexo(Anexo anexo);
        void ExcluirAnexo(int id);
        void ExcluirAnexosOportunidadePorTipo(int processoId, TipoAnexo tipoAnexo);
        Anexo ObterAnexoPorId(int id);
        AnexosDTO ObterDetalhesAnexo(int id);
        IEnumerable<AnexosDTO> ObterAnexosPorOportunidade(int oportunidadeId);
    }
}
