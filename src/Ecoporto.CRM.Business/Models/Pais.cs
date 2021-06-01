namespace Ecoporto.CRM.Business.Models
{
    public class Pais : Entidade<Pais>
    {
        public Pais()
        {

        }

        public Pais(string descricao, string sigla)
        {
            Descricao = descricao;
            Sigla = sigla;
        }

        public string Descricao { get; set; }

        public string Sigla { get; set; }

        public override void Validar()
        {
            ValidationResult = Validate(this);
        }
    }
}
