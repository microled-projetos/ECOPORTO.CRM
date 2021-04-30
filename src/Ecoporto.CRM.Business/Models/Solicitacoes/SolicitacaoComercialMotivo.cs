using Ecoporto.CRM.Business.Enums;

namespace Ecoporto.CRM.Business.Models
{
    public class SolicitacaoComercialMotivo : Entidade<SolicitacaoComercialMotivo>
    {
        public SolicitacaoComercialMotivo()
        {

        }

        public SolicitacaoComercialMotivo(string descricao, bool cancelamentoNF, bool desconto, bool restituicao, bool prorrogacaoBoleto, bool outros, Status status)
        {
            Descricao = descricao;
            CancelamentoNF = cancelamentoNF;
            Desconto = desconto;
            Restituicao = restituicao;
            ProrrogacaoBoleto = prorrogacaoBoleto;
            Outros = outros;
            Status = status;
        }

        public string Descricao { get; set; }

        public bool CancelamentoNF { get; set; }

        public bool Desconto { get; set; }

        public bool Restituicao { get; set; }

        public bool ProrrogacaoBoleto { get; set; }

        public bool Outros { get; set; }

        public Status Status { get; set; }

        public void Alterar(SolicitacaoComercialMotivo solicitacaoComercialMotivo)
        {
            Descricao = solicitacaoComercialMotivo.Descricao;
            CancelamentoNF = solicitacaoComercialMotivo.CancelamentoNF;
            Desconto = solicitacaoComercialMotivo.Desconto;
            Restituicao = solicitacaoComercialMotivo.Restituicao;
            ProrrogacaoBoleto = solicitacaoComercialMotivo.ProrrogacaoBoleto;
            Outros = solicitacaoComercialMotivo.Outros;
            Status = solicitacaoComercialMotivo.Status;
        }

        public override void Validar()
        {            
            ValidationResult = Validate(this);
        }
    }  
}
