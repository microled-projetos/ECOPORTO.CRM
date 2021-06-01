namespace Ecoporto.CRM.Business.Models
{
    public class LayoutTituloMaster : Entidade<LayoutTituloMaster>
    {
        public Cabecalho Cabecalho { get; set; }

        public LayoutTituloMaster()
        {

        }

        public LayoutTituloMaster(Cabecalho cabecalho)
        {
            Cabecalho = cabecalho;            
        }

        public override void Validar()
        {
            foreach (var erro in Cabecalho.ValidationResult.Errors)
                ValidationResult.Errors.Add(erro);
        }
    }
}
