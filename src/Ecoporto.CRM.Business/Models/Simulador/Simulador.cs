using Ecoporto.CRM.Business.Enums;
using FluentValidation;

namespace Ecoporto.CRM.Business.Models
{
    public class Simulador : Entidade<Simulador>
    {
        public Simulador()
        {

        }

        public Simulador(
            string descricao, 
            Regime regime, 
            int numeroLotes, 
            int? armadorId, 
            decimal cifConteiner, 
            decimal cifCargaSolta, 
            string margem, 
            int? localAtracacaoId, 
            int? grupoAtracacaoId, 
            decimal volumeM3, 
            int periodos,              
            int? tipoDocumentoId,
            int criadoPor)
        {
            Descricao = descricao;
            Regime = regime;
            NumeroLotes = numeroLotes;
            ArmadorId = armadorId;
            CifConteiner = cifConteiner;
            CifCargaSolta = cifCargaSolta;
            Margem = margem;
            LocalAtracacaoId = localAtracacaoId;
            GrupoAtracacaoId = grupoAtracacaoId;
            VolumeM3 = volumeM3;
            Periodos = periodos;          
            TipoDocumentoId = tipoDocumentoId;
            CriadoPor = criadoPor;
        }

        public string Descricao { get; set; }

        public Regime Regime { get; set; }

        public int NumeroLotes { get; set; }

        public int? ArmadorId { get; set; }

        public decimal CifConteiner { get; set; }

        public decimal CifCargaSolta { get; set; }

        public string Margem { get; set; }

        public int? LocalAtracacaoId { get; set; }

        public int? GrupoAtracacaoId { get; set; }

        public decimal VolumeM3 { get; set; }

        public int Periodos { get; set; }

        public int ConteinerTamanho { get; set; }

        public decimal ConteinerPesoTotal { get; set; }

        public int ConteinerQuantidade { get; set; }

        public int CargaSoltaQuantidade { get; set; }
        
        public decimal CargaSoltaPesoM3 { get; set; }

        public int? TipoDocumentoId { get; set; }

        public int CriadoPor { get; set; }

        public override void Validar()
        {
            RuleFor(c => c.Descricao)
                .NotEmpty()
                .WithMessage("Informe a Descrição do Simulador");

            RuleFor(c => c.TipoDocumentoId)
                .NotNull()
                .WithMessage("Selecione o Tipo de Documento")
                .GreaterThan(0)
                .WithMessage("Documento Inválido");

            RuleFor(c => c.CifConteiner)
                .GreaterThan(0)
                .When(c => c.CifCargaSolta == 0)
                .WithMessage("Informe o CIF Contêiner");

            RuleFor(c => c.CifCargaSolta)
                .GreaterThan(0)
                .When(c => c.CifConteiner == 0)
                .WithMessage("Informe o CIF Carga Solta");

            RuleFor(c => c.Periodos)
                .GreaterThan(0)
                .WithMessage("Informe a quantidade de Períodos");

            RuleFor(c => c.Regime)
                .IsInEnum()
                .WithMessage("Informe o Regime");

            ValidationResult = Validate(this);
        }
    }
}
