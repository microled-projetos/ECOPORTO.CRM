using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.ValueObjects;

namespace Ecoporto.CRM.Business.Models
{
    public class LayoutMinimoGeral : Entidade<LayoutMinimoGeral>
    {
        public Cabecalho Cabecalho { get; set; }

        public BaseCalculo BaseCalculo { get; set; }

        public ValorCargaMinimo ValorCarga { get; set; }

        public int LinhaReferencia { get; set; }

        public string DescricaoValor { get; set; }

        public LayoutMinimoGeral()
        {

        }

        public LayoutMinimoGeral(
            Cabecalho cabecalho,
            BaseCalculo baseCalculo,
            ValorCargaMinimo valorCarga,
            int linhaReferencia,
            string descricaoValor)
        {
            Cabecalho = cabecalho;
            BaseCalculo = baseCalculo;
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
