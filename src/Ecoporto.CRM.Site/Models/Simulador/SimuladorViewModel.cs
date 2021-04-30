using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class SimuladorViewModel
    {
        public SimuladorViewModel()
        {
            TiposDocumentos = new List<Documento>();
            Armadores = new List<Parceiro>();
            LocaisAtracacao = new List<Armazem>();
            GruposAtracacao = new List<Terminal>();
            Simuladores = new List<SimuladorDTO>();
            CargasConteiner = new List<SimuladorCargaConteiner>();
            CargasSolta = new List<SimuladorCargaSolta>();
            Tabelas = new List<SimuladorTabelasDTO>();
            Margens = new List<string>();
        }

        public int Id { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        public Regime Regime { get; set; }

        [Display(Name = "Nº Lotes")]
        public int NumeroLotes { get; set; }

        [Display(Name = "Armador")]
        public int? ArmadorId { get; set; }

        public string ArmadorDescricao { get; set; }

        public string ArmadorDocumento { get; set; }

        [Display(Name = "CIF Contêiner")]
        public decimal CifConteiner { get; set; }

        [Display(Name = "CIF Carga Solta")]
        public decimal CifCargaSolta { get; set; }

        public string Margem { get; set; }

        [Display(Name = "Local Atracação")]
        public int? LocalAtracacaoId { get; set; }

        [Display(Name = "Grupo Atracação")]
        public int? GrupoAtracacaoId { get; set; }

        [Display(Name = "Volume M3")]
        public decimal VolumeM3 { get; set; }

        public int Periodos { get; set; }

        [Display(Name = "Tamanho")]
        public int ConteinerTamanho { get; set; }

        [Display(Name = "Peso Total")]
        public decimal ConteinerPesoTotal { get; set; }

        [Display(Name = "Quantidade")]
        public int ConteinerQuantidade { get; set; }

        [Display(Name = "Quantidade")]
        public int CargaSoltaQuantidade { get; set; }

        [Display(Name = "Peso/M3")]
        public decimal CargaSoltaPesoM3 { get; set; }

        [Display(Name = "Tipo Documento")]
        public int? TipoDocumentoId { get; set; }

        public int SimuladorSelecionado { get; set; }

        public List<Documento> TiposDocumentos { get; set; }

        public List<Parceiro> Armadores { get; set; }

        public List<Armazem> LocaisAtracacao { get; set; }

        public List<Terminal> GruposAtracacao { get; set; }

        public List<SimuladorDTO> Simuladores { get; set; }

        public List<SimuladorCargaConteiner> CargasConteiner { get; set; }

        public List<SimuladorCargaSolta> CargasSolta { get; set; }

        public List<SimuladorTabelasDTO> Tabelas { get; set; }

        public List<string> Margens { get; set; }
    }
}