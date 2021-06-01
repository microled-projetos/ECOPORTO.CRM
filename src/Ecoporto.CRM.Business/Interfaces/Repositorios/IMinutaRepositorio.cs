using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface IMinutaRepositorio
    {
        Minuta ObterMinuta(int minuta);
        IEnumerable<ServicoFaturamento> ObterServicosPorMinuta(int minuta);
        ServicoFaturamento ObterServicoFaturamentoPorId(int id);
    }
}
