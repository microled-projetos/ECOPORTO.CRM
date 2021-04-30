using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface IGRRepositorio
    {
        IEnumerable<GR> ObterGRsPorLote(int lote);
        IEnumerable<GR> ObterGRsPorBL(string bl);
        IEnumerable<GR> ObterGRsFaturadasPorLote(int lote);
        IEnumerable<GR> ObterGRsRedexPorReserva(string reserva);
        GR ObterDetalhesGR(int seqGR);
        GR ObterDetalhesGRRedex(long booking, int seqGR, int clienteId);
        GR ObterDetalhesGRRedexPorReserva(long booking);
        GR ObterDetalhesGRPorLote(int lote, int seq_gr);
        GR ObterDetalhesGRPorBL(string bl, string seq_gr);
        GR ObterDetalhesPreCalculoLote(int lote);
        GR ObterDetalhesPreCalculoPorBL(string numero);
    }
}
