namespace Ecoporto.CRM.Business.Models
{
    public class LayoutSubTituloMargem : Entidade<LayoutSubTituloMargem>
    {
        public Cabecalho Cabecalho { get; set; }

        public LayoutSubTituloMargem()
        {

        }

        public LayoutSubTituloMargem(Cabecalho cabecalho)
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
