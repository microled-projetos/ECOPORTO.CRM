using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface IServicoFaturamentoRepositorio
    {
        IEnumerable<ServicoFaturamento> ObterServicos();
        ServicoFaturamento ObterServicoFaturamentoPorId(int id);
        ServicoFaturamento ObterServicoFaturamentoRedexPorId(int id);
        IEnumerable<ServicoFaturamento> ObterServicos(int[] ids);
        IEnumerable<ServicoFaturamento> ObterServicosPorGR(int seqGr);
        IEnumerable<ServicoFaturamento> ObterServicosRedex(long booking, long clienteId, int? seqGr);
        IEnumerable<ServicoFaturamento> ObterServicosPorBL(int bl, int? seqGr);
        IEnumerable<ServicoFaturamento> ObterServicosPreCalculoPorBL(int bl);
        decimal ObterValorImposto(int lote, string[] grs);
        decimal ObterValorImpostoRedex(int booking, string[] grs);
    }
}
