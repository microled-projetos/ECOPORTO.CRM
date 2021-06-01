using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class ServicoViewModel
    {
        public ServicoViewModel()
        {
            ServicosFaturamento = new List<ServicoFaturamento>();
            ServicosVinculados = new List<ServicoFaturamento>();
            ServicosSelecionados = new int[0];
        }

        public int? Id { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Display(Name = "Status")]
        public Status Status { get; set; }

        [Display(Name = "Recinto Alfandegado")]
        public bool RecintoAlfandegado { get; set; }

        [Display(Name = "Operador")]
        public bool Operador { get; set; }

        [Display(Name = "Redex")]
        public bool Redex { get; set; }

        [Display(Name = "Serviços IPA")]
        public List<ServicoFaturamento> ServicosFaturamento { get; set; }

        [Display(Name = "Serviços Vinculados")]
        public List<ServicoFaturamento> ServicosVinculados { get; set; }

        public int[] ServicosSelecionados { get; set; }

        public string DescricaoStatus => Status.ToName();
    }
}