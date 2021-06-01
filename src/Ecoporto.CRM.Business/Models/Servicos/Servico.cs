using Ecoporto.CRM.Business.Enums;
using System.Collections.Generic;
using FluentValidation;

namespace Ecoporto.CRM.Business.Models
{
    public class Servico : Entidade<Servico>
    {
        public Servico()
        {            
        }

        public Servico(
            string descricao, 
            Status status, 
            bool recintoAlfandegado, 
            bool operador, 
            bool redex)
        {
            Descricao = descricao;
            Status = status;
            RecintoAlfandegado = recintoAlfandegado;
            Operador = operador;
            Redex = redex;            
        }

        public string Descricao { get; set; }

        public Status Status { get; set; }
        
        public bool RecintoAlfandegado { get; set; }

        public bool Operador { get; set; }

        public bool Redex { get; set; }

        public List<ServicoFaturamento> ServicosVinculados { get; set; } 
            = new List<ServicoFaturamento>();

        public void Alterar(Servico servico)
        {
            this.Descricao = servico.Descricao;
            this.Status = servico.Status;
            this.RecintoAlfandegado = servico.RecintoAlfandegado;
            this.Operador = servico.Operador;
            this.Redex = servico.Redex;
        }
        
        public void AdicionarServicoVinculado(ServicoFaturamento servicosFaturamento)
            => ServicosVinculados.Add(servicosFaturamento);
        
        public override void Validar()
        {
            RuleFor(c => c.Descricao)
               .NotNull()
               .WithMessage("A descrição do serviço é obrigatória")
               .MinimumLength(3).WithMessage("Tamanho mínimo de 3 caracteres")
               .MaximumLength(150).WithMessage("Tamanho máximo de 150 caracteres");

            RuleFor(c => c.ServicosVinculados.Count)
               .GreaterThan(0)
               .When(c => c.RecintoAlfandegado)
               .WithMessage("É necessário vincular um ou mais serviços");

            ValidationResult = Validate(this);
        }
    }
}