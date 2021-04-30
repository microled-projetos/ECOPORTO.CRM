namespace Ecoporto.CRM.Business.Models
{
    public class LayoutSubTitulo : Entidade<LayoutSubTitulo>
    {
        public Cabecalho Cabecalho { get; set; }
        
        public LayoutSubTitulo()
        {

        }

        public LayoutSubTitulo(Cabecalho cabecalho)
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
