using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class OportunidadesAdendosViewModel
    {
        public OportunidadesAdendosViewModel()
        {
            Adendos = new List<OportunidadeAdendosDTO>();
            ClientesGrupoCNPJ = new List<ClientePropostaDTO>();
            SubClientes = new List<ClientePropostaDTO>();
            AdendoContas = new List<Conta>();
            AdendosClientesGrupoInclusao = new int[0];
            AdendosClientesGrupoExclusao = new int[0];
            AdendosSubClientesInclusao = new string[0];
            AdendosSubClientesExclusao = new string[0];
        }

        public int Id { get; set; }

        public int AdendoOportunidadeId { get; set; }

        [Display(Name = "Tipo de Adendo")]
        public TipoAdendo TipoAdendo { get; set; }

        [Display(Name = "Status")]
        public StatusAdendo StatusAdendo { get; set; }
        
        public List<OportunidadeAdendosDTO> Adendos { get; set; }

        [Display(Name = "Vendedor")]
        public int AdendoVendedorId { get; set; }

        [Display(Name = "Forma Pagamento")]
        public FormaPagamento AdendoFormaPagamento { get; set; }

        [Display(Name = "Anexo")]
        public string AnexoFormaPagamento { get; set; }

        [Display(Name = "Segmento")]
        public SegmentoSubCliente AdendoSegmento { get; set; }

        [Display(Name = "Sub Cliente")]
        public int AdendoSubClienteId { get; set; }

        [Display(Name = "Cliente Grupo CNPJ")]
        public int AdendoClienteGrupoCNPJId { get; set; }

        [Display(Name = "Anexo")]
        public string AnexoExclusaoGrupoCNPJ { get; set; }

        [Display(Name = "Anexo")]
        public string AnexoExclusaoSubClientes { get; set; }

        public string DescricaoTipoAdendo
           => TipoAdendo.ToName();

        public string DescricaoStatusAdendo
            => StatusAdendo.ToName();

        public List<Vendedor> Vendedores { get; set; }

        public List<ClientePropostaDTO> ClientesGrupoCNPJ { get; set; }

        public List<ClientePropostaDTO> SubClientes { get; set; }

        public List<Conta> AdendoContas { get; set; }

        public int[] AdendosClientesGrupoInclusao { get; set; }

        public int[] AdendosClientesGrupoExclusao { get; set; }

        public string[] AdendosSubClientesInclusao { get; set; }

        public string[] AdendosSubClientesExclusao { get; set; }        
    }
}