using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class ModeloSimuladorViewModel
    {
        public ModeloSimuladorViewModel()
        {
            ServicosIPA = new List<ServicoFaturamento>();
            ServicoIPASelecionados = new int[0];
        }

        public int Id { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Display(Name = "Observações")]
        public string Observacoes { get; set; }

        [Display(Name = "Tipo Serviço")]
        public Regime Regime { get; set; }

        public List<ModeloSimulador> Modelos { get; set; }

        public List<ServicoFaturamento> ServicosIPA { get; set; }

        public int[] ServicoIPASelecionados { get; set; }

        public int[] ServicosVinculados { get; set; }
    }
}