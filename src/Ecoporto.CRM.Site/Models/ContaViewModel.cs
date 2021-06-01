using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class ContaViewModel
    {
        public ContaViewModel()
        {
            Oportunidades = new List<OportunidadeDTO>();
            OportunidadesInativas = new List<OportunidadeDTO>();
            VinculoIPS = new List<ControleAcessoConta>();
        }

        public int Id { get; set; }

        [Display(Name = "Nome da Conta")]
        public string Descricao { get; set; }

        [Display(Name = "CNPJ/CPF")]
        public string Documento { get; set; }

        [Display(Name = "Nome Fantasia")]
        public string NomeFantasia { get; set; }

        [Display(Name = "Inscrição Estadual")]
        public string InscricaoEstadual { get; set; }

        [Display(Name = "Telefone")]
        public string Telefone { get; set; }

        [Display(Name = "Criado Por")]
        public int CriadoPor { get; set; }

        [Display(Name = "Vendedor")]
        public int VendedorId { get; set; }

        [Display(Name = "Situação Cadastral")]
        public SituacaoCadastral SituacaoCadastral { get; set; }

        [Display(Name = "Segmento")]
        public Segmento Segmento { get; set; }

        [Display(Name = "Classificação Fiscal")]
        public ClassificacaoFiscal ClassificacaoFiscal { get; set; }

        [Display(Name = "Logradouro")]
        public string Logradouro { get; set; }

        [Display(Name = "Bairro")]
        public string Bairro { get; set; }

        [Display(Name = "Número")]
        public int Numero { get; set; }

        [Display(Name = "Complemento")]
        public string Complemento { get; set; }

        [Display(Name = "CEP")]
        public string CEP { get; set; }

        [Display(Name = "UF")]
        public Estado Estado { get; set; }

        [Display(Name = "Cidade")]
        public int? CidadeId { get; set; }

        [Display(Name = "País")]
        public int? PaisId { get; set; }

        public bool Blacklist { get; set; }

        public bool SomenteLeitura { get; set; }

        public bool PermiteEditar { get; set; }

        public bool PermiteExcluir { get; set; }

        public string DescricaoSituacaoCadastral
            => SituacaoCadastral.ToName();

        public string DescricaoSegmento
            => Segmento.ToName();

        public string DescricaoClassificacaoFiscal
            => ClassificacaoFiscal.ToName();

        public List<Vendedor> Vendedores { get; set; }

        public List<Cidade> Cidades { get; set; }

        public List<Pais> Paises { get; set; }

        public List<Contato> Contatos { get; set; }

        public List<OportunidadeDTO> Oportunidades { get; set; }

        public List<OportunidadeDTO> OportunidadesInativas { get; set; }

        public List<ControleAcessoConta> VinculoIPS { get; set; }
    }
}