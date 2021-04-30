using Ecoporto.CRM.Business.Enums;
using FluentValidation;

namespace Ecoporto.CRM.Business.Models
{
    public class EquipeConta : Entidade<EquipeConta>
    {
        public EquipeConta()
        {

        }

        public EquipeConta(int contaId, int usuarioId, int acessoConta, int acessoOportunidade, PapelEquipe papel)
        {
            ContaId = contaId;
            UsuarioId = usuarioId;
            AcessoConta = acessoConta;
            AcessoOportunidade = acessoOportunidade;
            PapelEquipe = papel;
        }

        public int ContaId { get; set; }

        public int UsuarioId { get; set; }

        public string UsuarioDescricao { get; set; }

        public int AcessoConta { get; set; }

        public int AcessoOportunidade { get; set; }

        public PapelEquipe PapelEquipe { get; set; }

        public override void Validar()
        {
            RuleFor(c => c.ContaId)
                .GreaterThan(0)
                .WithMessage("Conta não informada");

            RuleFor(c => c.UsuarioId)
                .GreaterThan(0)
                .WithMessage("Usuário não informado");

            ValidationResult = Validate(this);
        }
    }
}
