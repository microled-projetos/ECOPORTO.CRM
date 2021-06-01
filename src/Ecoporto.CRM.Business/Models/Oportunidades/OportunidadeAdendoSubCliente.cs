using Ecoporto.CRM.Business.Enums;
using FluentValidation;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Models
{
    public class OportunidadeAdendoSubCliente : Entidade<OportunidadeAdendoSubCliente>
    {
        public OportunidadeAdendoSubCliente()
        {
            Clientes = new List<string>();
        }

        public int AdendoId { get; set; }

        public AdendoAcao Acao { get; set; }

        public int AnexoId { get; set; }

        public int ClienteId { get; set; }

        public int Segmento { get; set; }

        public List<string> Clientes { get; set; }

        public void AdicionarSubClientes(List<string> clientes)
        {
            if (clientes != null)
                clientes.ForEach(cliente =>
                {
                    if (!Clientes.Contains(cliente))
                        Clientes.Add(cliente);
                });
        }

        public override void Validar()
        {
            RuleFor(c => c.AdendoId)
               .GreaterThan(0)
               .WithMessage("Adendo não informado");

            RuleFor(c => c.Clientes.Count)
             .GreaterThan(0)
             .WithMessage("Nenhum Sub Cliente selecionado");

            RuleFor(c => c.Acao)
               .IsInEnum()
               .WithMessage("Nenhuma Ação informada: Inclusão / Exclusão");

            ValidationResult = Validate(this);
        }
    }
}
