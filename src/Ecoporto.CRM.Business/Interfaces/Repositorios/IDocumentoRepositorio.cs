using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface IDocumentoRepositorio
    {
        IEnumerable<Documento> ObterTiposDocumentos();
    }
}
