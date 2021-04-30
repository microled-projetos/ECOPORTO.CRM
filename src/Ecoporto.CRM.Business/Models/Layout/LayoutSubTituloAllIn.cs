namespace Ecoporto.CRM.Business.Models
{
    public class LayoutSubTituloAllIn : Entidade<LayoutSubTituloAllIn>
    {
        public Cabecalho Cabecalho { get; set; }

        public LayoutSubTituloAllIn()
        {

        }

        public LayoutSubTituloAllIn(Cabecalho cabecalho)
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
