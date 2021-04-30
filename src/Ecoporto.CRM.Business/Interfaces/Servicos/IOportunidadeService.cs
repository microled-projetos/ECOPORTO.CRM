using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Enums;

namespace Ecoporto.CRM.Business.Interfaces.Servicos
{
    public interface IOportunidadeService
    {
        void ImportarLayoutNaOportunidade(int id, int modeloId);
        int ClonarOportunidade(ClonarOportunidadeDTO clone, int contaId);
        void ClonarProposta(int oportunidadeOrigemId, int oportunidadeDestinoId, FormaPagamento formaPagamento);
    }
}
