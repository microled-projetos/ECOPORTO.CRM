namespace Ecoporto.CRM.Business.Models
{
    public class ServicoVinculo : Entidade<ServicoVinculo>
    {
        public ServicoVinculo()
        {

        }

        public ServicoVinculo(int servicoFaturamentoId, int servicoId)
        {
            ServicoFaturamentoId = servicoFaturamentoId;
            ServicoId = servicoId;
        }

        public int ServicoFaturamentoId { get; set; }

        public ServicoFaturamento ServicoFaturamento { get; set; }

        public Servico Servico { get; set; }

        public int ServicoId { get; set; }

        public override void Validar()
        {
            ValidationResult = Validate(this);
        }
    }
}
