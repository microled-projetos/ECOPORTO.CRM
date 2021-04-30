using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Business.Models.Oportunidades;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Servicos
{
    public interface IConcomitanciaTabelaService
    {
        IEnumerable<OportunidadeTabelaConcomitante> ObtemPropostasDuplicadasCRM(Oportunidade oportunidade);
        IEnumerable<OportunidadeTabelaConcomitante> ObtemTabelasDuplicadasChronos(int oportunidadeId);
    }
}
