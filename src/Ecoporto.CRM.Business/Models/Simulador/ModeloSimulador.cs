using System.Collections.Generic;
using Ecoporto.CRM.Business.Enums;
using FluentValidation;

namespace Ecoporto.CRM.Business.Models
{
    public class ModeloSimulador : Entidade<ModeloSimulador>
    {
        public ModeloSimulador()
        {

        }

        public ModeloSimulador(string descricao, string observacoes, Regime regime, int[] servicoIPA)
        {
            Descricao = descricao;
            Observacoes = observacoes;
            Regime = regime;
            ServicoIPA = servicoIPA;
        }

        public int ModeloId { get; set; }

        public string Descricao { get; set; }

        public string Observacoes { get; set; }

        public Regime Regime { get; set; }

        public int[] ServicoIPA { get; set; }

        public List<int> ServicoVinculados { get; set; } = new List<int>();

        public override void Validar()
        {           
            RuleFor(c => c.Descricao)
               .NotEmpty()
               .WithMessage("Informe a Descrição do Modelo");
      
            ValidationResult = Validate(this);
        }
    }
}
