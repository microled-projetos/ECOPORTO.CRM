namespace Ecoporto.CRM.Business.Models
{
    public class Vendedor : Entidade<Vendedor>
    {       
        public string Nome { get; set; }

        public string Login { get; set; }

        public override void Validar()
        {
            ValidationResult = Validate(this);
        }
    }
}
