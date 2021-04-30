namespace Ecoporto.CRM.Business.Models
{
    public class ServicoFaturamento : Entidade<ServicoFaturamento>
    {
        public ServicoFaturamento()
        {

        }

        public ServicoFaturamento(int id)
        {
            Id = id;
        }

        public string Descricao { get; set; }

        public int Servico { get; set; }

        public string ClienteDescricao { get; set; }

        public decimal Valor { get; set; }

        public override void Validar()
        {
            ValidationResult = Validate(this);
        }
    }
}
