using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface INotaFiscalRepositorio
    {
        IEnumerable<NotaFiscal> ObterNotasFiscais(int nfe);
        NotaFiscal ObterDetalhesNotaFiscal(int nfeId);
        NotaFiscal ObterDetalhesNotaFiscalRedex(int nfeId);
    }
}
