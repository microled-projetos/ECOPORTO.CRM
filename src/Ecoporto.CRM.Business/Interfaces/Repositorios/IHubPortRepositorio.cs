using System.Collections.Generic;
using Ecoporto.CRM.Business.Models;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface IHubPortRepositorio
    {
        IEnumerable<ClienteHubPort> ObterClientesHubPort();
    }
}
