using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface ICondicaoPagamentoFaturaRepositorio
    {
        IEnumerable<CondicaoPagamentoFatura> ObterCondicoesPagamento();
        CondicaoPagamentoFatura ObterCondicoPagamentoPorId(string id);
    }
}
