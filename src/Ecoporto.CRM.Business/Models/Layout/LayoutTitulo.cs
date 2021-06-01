namespace Ecoporto.CRM.Business.Models
{
    public class LayoutTitulo : Entidade<LayoutTitulo>
    {
        public Cabecalho Cabecalho { get; set; }

        public LayoutTitulo()
        {

        }

        public LayoutTitulo(Cabecalho cabecalho)
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
