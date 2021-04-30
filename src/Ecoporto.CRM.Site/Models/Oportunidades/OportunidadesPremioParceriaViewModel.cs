using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Workflow.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class OportunidadesPremioParceriaViewModel
    {
        public OportunidadesPremioParceriaViewModel()
        {
            Contatos = new List<Contato>();
            Premios = new List<PremioParceriaDTO>();
            Contas = new List<Conta>();
            ModalidadesSelecionadas = new ModalidadesComissionamento[0];
        }

        public int PremioParceriaId { get; set; }

        public int PremioParceriaOportunidadeId { get; set; }

        public int PremioParceriaContaId { get; set; }

        [Display(Name = "Contato")]
        public int PremioParceriaContatoId { get; set; }

        [Display(Name = "Status")]
        public StatusPremioParceria StatusPremioParceria { get; set; }

        [Display(Name = "Favorecido 1")]
        public int Favorecido1 { get; set; }

        [Display(Name = "Favorecido 2")]
        public int Favorecido2 { get; set; }

        [Display(Name = "Favorecido 3")]
        public int Favorecido3 { get; set; }

        [Display(Name = "Instrução")]
        public Instrucao Instrucao { get; set; }
      
        [Display(Name = "Prêmio Ref.")]
        public int PremioReferenciaId { get; set; }

        [Display(Name = "Tipo Serviço")]
        public TipoServicoPremioParceria TipoServicoPremioParceria { get; set; }

        [Display(Name = "Observações")]
        public string Observacoes { get; set; }

        public int AnexoPremioId { get; set; }

        public int PremioRevisaoId { get; set; }

        [Display(Name = "Anexo Prêmio")]
        public string AnexoPremio { get; set; }

        [Display(Name = "URL Prêmio")]
        public string UrlPremio { get; set; }

        [Display(Name = "Data URL Prêmio")]
        public DateTime? DataUrlPremio { get; set; }

        [Display(Name = "Email Favorecido 1")]
        public string EmailFavorecido1 { get; set; }

        [Display(Name = "Email Favorecido 2")]
        public string EmailFavorecido2 { get; set; }

        [Display(Name = "Email Favorecido 3")]
        public string EmailFavorecido3 { get; set; }

        public List<Contato> Contatos { get; set; }

        public List<Conta> Contas { get; set; }

        public List<PremioParceriaDTO> Premios { get; set; }

        [Display(Name = "Modalidades")]
        public IEnumerable<ModalidadesComissionamento> Modalidades { get; set; }

        [Display(Name = "Modalidades Selecionadas")]
        public ModalidadesComissionamento[] ModalidadesSelecionadas { get; set; }
    }
}