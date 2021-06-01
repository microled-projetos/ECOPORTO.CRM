using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Site.Models
{
    public class BuscaViewModel
    {
        public BuscaViewModel()
        {
            Contas = new List<Conta>();
            Contatos = new List<Contato>();
            Mercadorias = new List<Mercadoria>();
            Modelos = new List<Modelo>();
            Oportunidades = new List<OportunidadeDTO>();
            Servicos = new List<Servico>();
            SubClientes = new List<ClientePropostaDTO>();
            ClientesGrupoCNPJ = new List<ClientePropostaDTO>();
            AdendosSubClientes = new List<OportunidadeAdendoClientesDTO>();
        }

        public string Termo { get; set; }

        public List<Conta> Contas { get; set; }

        public List<Contato> Contatos { get; set; }

        public List<Mercadoria> Mercadorias { get; set; }

        public List<Modelo> Modelos { get; set; }

        public List<OportunidadeDTO> Oportunidades { get; set; }

        public List<ClientePropostaDTO> SubClientes { get; set; }

        public List<ClientePropostaDTO> ClientesGrupoCNPJ { get; set; }

        public List<Servico> Servicos { get; set; }

        public List<OportunidadeAdendoClientesDTO> AdendosSubClientes { get; set; }

        public int TotalContas => Contas.Count;

        public int TotalContatos => Contatos.Count;

        public int TotalMercadorias => Mercadorias.Count;

        public int TotalModelos => Modelos.Count;

        public int TotalOportunidades => Oportunidades.Count;

        public int TotalSubClientes => SubClientes.Count;

        public int TotalClientesGrupoCNPJ => ClientesGrupoCNPJ.Count;

        public int TotalAdendosSubClientes => AdendosSubClientes.Count;

        public int TotalServicos => Servicos.Count;
    }
}