using Ecoporto.CRM.Business.Enums;
using FluentValidation;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Models
{
    public class OportunidadeAdendoGrupoCNPJ : Entidade<OportunidadeAdendoGrupoCNPJ>
    {
        public OportunidadeAdendoGrupoCNPJ()
        {
            Clientes = new List<int>();
        }

        public int AdendoId { get; set; }

        public int AnexoId { get; set; }

        public int ClienteId { get; set; }

        public AdendoAcao Acao { get; set; }

        public List<int> Clientes { get; set; }

        public void AdicionarClientesGrupoCNPJ(List<int> clientes)
        {
            if (clientes != null)
                clientes.ForEach(cliente => Clientes.Add(cliente));
        }

        public override void Validar()
        {
            RuleFor(c => c.AdendoId)
               .GreaterThan(0)
               .WithMessage("Adendo não informado");

            RuleFor(c => c.Clientes.Count)
               .GreaterThan(0)
               .WithMessage("Nenhum Cliente Grupo CNPJ selecionado");

            RuleFor(c => c.Acao)
               .IsInEnum()
               .WithMessage("Nenhuma Ação informada: Inclusão / Exclusão");
            
            ValidationResult = Validate(this);
        }
    }
}
