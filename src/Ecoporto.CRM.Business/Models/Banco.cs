namespace Ecoporto.CRM.Business.Models
{
    public class Banco : Entidade<Banco>
    {
        public Banco()
        {

        }
        
        public string Descricao { get; set; }        

        public override void Validar()
        {            
            ValidationResult = Validate(this);
        }
    }
}