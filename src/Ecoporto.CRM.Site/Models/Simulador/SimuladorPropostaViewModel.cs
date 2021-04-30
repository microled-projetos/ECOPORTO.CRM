using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class SimuladorPropostaViewModel
    {
        public int SimuladorPropostaId { get; set; }        

        public int SimuladorPropostaOportunidadeId { get; set; }

        [Display(Name = "Modelo Simulador")]
        public int SimuladorPropostaModeloId { get; set; }

        [Display(Name = "Períodos")]
        public int SimuladorPropostaPeriodos { get; set; }

        //[Display(Name = "Nº Lotes")]
        //public int SimuladorPropostaNumeroLotes { get; set; }       

        [Display(Name = "Volume M3")]
        public decimal SimuladorPropostaVolumeM3 { get; set; }

        [Display(Name = "Tonelada")]
        public decimal SimuladorPropostaPeso { get; set; }

        [Display(Name = "Tipo Documento")]
        public int? SimuladorPropostaTipoDocumentoId { get; set; }        

        [Display(Name = "Grupo Atracação")]
        public int SimuladorPropostaGrupoAtracacaoId { get; set; }        

        [Display(Name = "Margem")]
        public string SimuladorPropostaMargem { get; set; }

        [Display(Name = "Qtde 20'")]
        public int SimuladorPropostaQtde20 { get; set; }

        [Display(Name = "Qtde '40")]
        public int SimuladorPropostaQtde40 { get; set; }

        [Display(Name = "CIF")]
        public decimal SimuladorPropostaCif { get; set; }

        [Display(Name = "Observações")]
        public string Observacoes { get; set; }

        public List<Documento> SimuladorPropostaTiposDocumentos { get; set; } = new List<Documento>();

        public List<Terminal> SimuladorPropostaGruposAtracacao { get; set; } = new List<Terminal>();

        public List<VinculoModeloSimuladoDTO> SimuladorPropostaModelos { get; set; } = new List<VinculoModeloSimuladoDTO>();

        public List<string> SimuladorPropostaMargens { get; set; } = new List<string>();

        public List<OportunidadeParametrosSimuladorDTO> SimuladoresCadastrados { get; set; } = new List<OportunidadeParametrosSimuladorDTO>();
    }
}
