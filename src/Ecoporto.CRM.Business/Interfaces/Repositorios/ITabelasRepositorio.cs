using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Models.Oportunidades;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface ITabelasRepositorio
    {
        OportunidadeTabelaConcomitante ObterTabelaChronosPorId(int id);
        bool ExisteParceiroNoGrupo(int tabelaId, int parceiroId, string tipo);
        bool ExisteParceiroNaProposta(int oportunidadeId, SegmentoSubCliente segmentoSubCliente);
    }
}
