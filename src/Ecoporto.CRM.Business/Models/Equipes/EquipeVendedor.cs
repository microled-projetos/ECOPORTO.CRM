using Ecoporto.CRM.Business.Enums;
using FluentValidation;

namespace Ecoporto.CRM.Business.Models
{
    public class EquipeVendedor : Entidade<EquipeVendedor>
    {
        public EquipeVendedor()
        {

        }

        public EquipeVendedor(int vendedorId, int usuarioId, int acessoConta, int acessoOportunidade, PapelEquipe papel)
        {
            VendedorId = vendedorId;
            UsuarioId = usuarioId;
            AcessoConta = acessoConta;
            AcessoOportunidade = acessoOportunidade;
            PapelEquipe = papel;
        }

        public int VendedorId { get; set; }

        public int UsuarioId { get; set; }

        public string UsuarioDescricao { get; set; }

        public int AcessoConta { get; set; }

        public int AcessoOportunidade { get; set; }

        public PapelEquipe PapelEquipe { get; set; }

        public override void Validar()
        {
            RuleFor(c => c.VendedorId)
                .GreaterThan(0)
                .WithMessage("Vendedor não informado");

            RuleFor(c => c.UsuarioId)
                .GreaterThan(0)
                .WithMessage("Usuário não informado");

            ValidationResult = Validate(this);
        }
    }
}
