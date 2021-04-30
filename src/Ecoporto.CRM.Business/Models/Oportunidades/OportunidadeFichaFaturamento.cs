using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Utils;
using FluentValidation;

namespace Ecoporto.CRM.Business.Models
{
    public class OportunidadeFichaFaturamento : Entidade<OportunidadeFichaFaturamento>
    {
        public StatusFichaFaturamento StatusFichaFaturamento { get; set; }

        public int OportunidadeId { get; set; }

        public int ContaId { get; set; }

        public int? FaturadoContraId { get; set; }

        public string FaturadoContraDescricao { get; set; }

        public string DiasSemana { get; set; }

        public string DiasFaturamento { get; set; }

        public int? DataCorte { get; set; }

        public string CondicaoPagamentoFaturamentoId { get; set; }

        public string EmailFaturamento { get; set; }

        public string ObservacoesFaturamento { get; set; }

        public int AnexoFaturamentoId { get; set; }

        public bool UltimoDiaDoMes { get; set; }

        public bool DiaUtil { get; set; }

        public int FontePagadoraId { get; set; }

        public string FontePagadoraDescricao { get; set; }

        public string CondicaoPagamentoPorDia { get; set; }

        public string CondicaoPagamentoPorDiaSemana { get; set; }

        public bool EntregaEletronica { get; set; }

        public bool EntregaManual { get; set; }

        public bool EntregaManualSedex { get; set; }

        public bool CorreioComum { get; set; }

        public bool CorreioSedex { get; set; }

        public string AnexoFaturamento { get; set; }

        public int RevisaoId { get; set; }

        public int[] ContasSelecionadas { get; set; }

        public override void Validar()
        {
            RuleFor(c => c.FontePagadoraId)
               .GreaterThan(0)
               .WithMessage("O campo Fonte Pagadora é obrigatório");

            RuleFor(c => c.StatusFichaFaturamento)
               .IsInEnum()
               .WithMessage("O Status é obrigatório");          

            ValidationResult = Validate(this);

            if (!string.IsNullOrEmpty(EmailFaturamento))
            {
                var retorno = Validacoes.ValidarListaDeEmails(EmailFaturamento);

                foreach (var erro in retorno)
                    ValidationResult.Errors.Add(erro);
            }
        }
    }
}
