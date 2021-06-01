using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface IMargemRepositorio
    {
        IEnumerable<string> ObterMargens();
    }
}
