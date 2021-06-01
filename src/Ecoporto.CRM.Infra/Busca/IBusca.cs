using System.Collections.Generic;

namespace Ecoporto.CRM.Infra.Busca
{
    public interface IBusca
    {
        IEnumerable<BuscaInternaResultado> Buscar(string criterio, int? usuarioId);
    }
}
