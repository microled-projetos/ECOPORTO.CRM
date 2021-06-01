using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface ICidadeRepositorio
    {        
        IEnumerable<Cidade> ObterCidades();        
        IEnumerable<Cidade> ObterCidadesPorEstado(Estado estado);
        Cidade ObterCidadePorId(int id);
    }
}
