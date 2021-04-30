namespace Ecoporto.CRM.Business.Models
{
    public class Empresa : Entidade<Empresa>
    {
        public Empresa()
        {

        }

        public Empresa(string descricao)
        {
            Descricao = descricao;
        }

        public string Descricao { get; set; }

        public override void Validar()
        {          
            ValidationResult = Validate(this);
        }
    }
}
