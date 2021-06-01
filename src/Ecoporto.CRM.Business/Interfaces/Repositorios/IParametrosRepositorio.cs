using Ecoporto.CRM.Business.Models;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface IParametrosRepositorio
    {
        Parametros ObterParametros();
        ParametrosFatura ObterParametrosFatura(int empresaId);
    }
}
