using Ecoporto.CRM.Business.Enums;
using FluentValidation;

namespace Ecoporto.CRM.Business.Models
{
    public class EquipeOportunidade : Entidade<EquipeOportunidade>
    {
        public EquipeOportunidade()
        {

        }

        public EquipeOportunidade(int oportunidadeId, int usuarioId, int acessoConta, int acessoOportunidade, PapelEquipe papel)
        {
            OportunidadeId = oportunidadeId;
            UsuarioId = usuarioId;
            AcessoConta = acessoConta;
            AcessoOportunidade = acessoOportunidade;
            PapelEquipe = papel;
        }

        public int OportunidadeId { get; set; }

        public int UsuarioId { get; set; }

        public string UsuarioDescricao { get; set; }

        public int AcessoConta { get; set; }

        public int AcessoOportunidade { get; set; }

        public PapelEquipe PapelEquipe { get; set; }

        public override void Validar()
        {
            RuleFor(c => c.OportunidadeId)
                .GreaterThan(0)
                .WithMessage("Oportunidade não informada");

            RuleFor(c => c.UsuarioId)
                .GreaterThan(0)
                .WithMessage("Usuário não informado");

            ValidationResult = Validate(this);
        }
    }
}
