namespace Ecoporto.CRM.Business.Models
{
    public class Minuta : Entidade<Minuta>
    {
        public string Status { get; set; }

        public decimal Valor { get; set; }

        public int ClienteId { get; set; }

        public string ClienteDescricao { get; set; }

        public override void Validar()
        {
            ValidationResult = Validate(this);
        }
    }
}
