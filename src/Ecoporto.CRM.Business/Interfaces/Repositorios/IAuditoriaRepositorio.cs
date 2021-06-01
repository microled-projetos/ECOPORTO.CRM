using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Filtros;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface IAuditoriaRepositorio
    {
        IEnumerable<AuditoriaDTO> ObterLogsOportunidade(int oportunidadeId);
        IEnumerable<AuditoriaDTO> ObterHistorico(string controller, int chave, int chavePai = 0);
        IEnumerable<AuditoriaDTO> ObterLogsFichasFaturamento(int oportunidadeId);
        IEnumerable<AuditoriaDTO> ObterLogsPremiosParceria(int oportunidadeId);
        IEnumerable<AuditoriaDTO> ObterLogsAdendos(int oportunidadeId);
        IEnumerable<AuditoriaDTO> ObterLogsProposta(int oportunidadeId);
        IEnumerable<AuditoriaDTO> ObterLogsAnexos(int oportunidadeId);
        IEnumerable<AuditoriaAcessosDTO> ObterLogsAcesso(int pagina, int registrosPorPagina, AuditoriaAcessoFiltro filtro, string orderBy, out int totalFiltro);
        int ObterTotalLogsAcessos();
    }
}
