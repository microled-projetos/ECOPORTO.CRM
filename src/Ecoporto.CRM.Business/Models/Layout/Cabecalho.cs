using Ecoporto.CRM.Business.Enums;
using FluentValidation;

namespace Ecoporto.CRM.Business.Models
{
    public class Cabecalho: Entidade<Cabecalho>
    {
        public Cabecalho(int modeloId, int linha, string descricao, TipoRegistro tipoRegistro, bool ocultar)
        {
            ModeloId = modeloId;
            Linha = linha;
            Descricao = descricao;
            TipoRegistro = tipoRegistro;
            Ocultar = ocultar;

            Validar();
        }

        public int ModeloId { get; set; }

        public int Linha { get; set; }

        public string Descricao { get; set; }

        public TipoRegistro TipoRegistro { get; set; }

        public bool Ocultar { get; set; }

        public override void Validar()
        {
            RuleFor(c => c.ModeloId)
                .GreaterThan(0)
                .WithMessage("Modelo não especificado");

            RuleFor(c => c.Linha)
                .GreaterThan(0)
                .WithMessage("O número da linha é obrigatório");            

            RuleFor(c => c.TipoRegistro)
                .IsInEnum()
                .WithMessage("Escolha o tipo de registro");

            RuleFor(c => c.Descricao)
                .NotEmpty()
                .WithMessage("A descrição é obrigatória");

            ValidationResult = Validate(this);
        }
    }
}
