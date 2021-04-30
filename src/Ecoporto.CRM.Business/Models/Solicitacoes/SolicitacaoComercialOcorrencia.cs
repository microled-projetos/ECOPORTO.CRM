using Ecoporto.CRM.Business.Enums;

namespace Ecoporto.CRM.Business.Models
{
    public class SolicitacaoComercialOcorrencia : Entidade<SolicitacaoComercialOcorrencia>
    {
        public SolicitacaoComercialOcorrencia()
        {

        }
        
        public SolicitacaoComercialOcorrencia(
            string descricao, 
            bool cancelamentoNF, 
            bool desconto, 
            bool restituicao, 
            bool prorrogacaoBoleto, 
            bool outros,
            Status status)
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

        public bool Outros { get; set; }

        public bool ProrrogacaoBoleto { get; set; }

        public Status Status { get; set; }

        public void Alterar(SolicitacaoComercialOcorrencia solicitacaoComercialOcorrencia)
        {
            Descricao = solicitacaoComercialOcorrencia.Descricao;
            CancelamentoNF = solicitacaoComercialOcorrencia.CancelamentoNF;
            Desconto = solicitacaoComercialOcorrencia.Desconto;
            Restituicao = solicitacaoComercialOcorrencia.Restituicao;
            ProrrogacaoBoleto = solicitacaoComercialOcorrencia.ProrrogacaoBoleto;
            Outros = solicitacaoComercialOcorrencia.Outros;
            Status = solicitacaoComercialOcorrencia.Status;
        }

        public override void Validar()
        {            
            ValidationResult = Validate(this);
        }
    }  
}
