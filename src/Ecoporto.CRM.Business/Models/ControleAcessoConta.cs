using FluentValidation;

namespace Ecoporto.CRM.Business.Models
{
    public class ControleAcessoConta : Entidade<ControleAcessoConta>
    {
        public ControleAcessoConta()
        {

        }

        public ControleAcessoConta(int contaId, string descricao, string ipInicial, string ipFinal)
        {
            ContaId = contaId;
            Descricao = descricao;
            IPInicial = ipInicial;
            IPFinal = ipFinal;
        }

        public void Alterar(ControleAcessoConta controle)
        {
            ContaId = controle.ContaId;
            Descricao = controle.Descricao;
            IPInicial = controle.IPInicial;
            IPFinal = controle.IPFinal;
        }

        public int ContaId { get; set; }

        public string Descricao { get; set; }

        public string IPInicial { get; set; }

        public string IPFinal { get; set; }
        
        public override void Validar()
        {
            RuleFor(c => c.ContaId)
                .GreaterThan(0)
                .WithMessage("A Conta é obrigatória");

            RuleFor(c => c.IPInicial)
               .NotNull()
               .WithMessage("O IP inicial é obrigatório");

            RuleFor(c => c.IPFinal)
               .NotNull()
               .WithMessage("O IP final é obrigatório");

            ValidationResult = Validate(this);
        }
    }
}
