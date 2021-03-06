using Ecoporto.CRM.Business.ValueObjects;

namespace Ecoporto.CRM.Business.Models
{
    public class LayoutMinimoMecanicaManual : Entidade<LayoutMinimoMecanicaManual>
    {
        public Cabecalho Cabecalho { get; set; }
        
        public ValorCargaMinimo ValorCarga { get; set; }

        public int LinhaReferencia { get; set; }

        public string DescricaoValor { get; set; }

        public LayoutMinimoMecanicaManual()
        {

        }

        public LayoutMinimoMecanicaManual(
            Cabecalho cabecalho,
            ValorCargaMinimo valorCarga, 
            int linhaReferencia, 
            string descricaoValor)
        {
            Cabecalho = cabecalho;
            ValorCarga = valorCarga;
            LinhaReferencia = linhaReferencia;
            DescricaoValor = descricaoValor;           
        }

        public override void Validar()
        {
            foreach (var erro in Cabecalho.ValidationResult.Errors)
                ValidationResult.Errors.Add(erro);

            foreach (var erro in ValorCarga.ValidationResult.Errors)
                ValidationResult.Errors.Add(erro);
        }
    }
}
