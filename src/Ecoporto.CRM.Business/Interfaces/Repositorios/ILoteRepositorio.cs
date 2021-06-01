using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Models;
using System;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface ILoteRepositorio
    {
        Lote ObterLotePorId(int lote);
        Lote ExisteAverbacao(int oportunidadeId, DateTime? dataCancelamento);
        IEnumerable<LotesMasterDTO> ObterLotesMaster(int lote);
    }
}
