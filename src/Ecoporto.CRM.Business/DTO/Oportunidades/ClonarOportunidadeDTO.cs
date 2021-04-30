using Ecoporto.CRM.Business.Enums;

namespace Ecoporto.CRM.Business.DTO
{
    public class ClonarOportunidadeDTO
    {       
        public ClonarOportunidadeDTO(
            string descricao, 
            int oportunidadeOriginal, 
            int criadoPor, 
            FormaPagamento formaPagamentoOportunidade,
            int[] subClientesSelecionados,
            int[] clientesGrupoCnpjSelecionados)
        {
            Descricao = descricao;
            OportunidadeOriginal = oportunidadeOriginal;
            CriadoPor = criadoPor;
            FormaPagamentoOportunidade = formaPagamentoOportunidade;
            SubClientesSelecionados = subClientesSelecionados;
            ClientesGrupoCnpjSelecionados = clientesGrupoCnpjSelecionados;
        }

        public string Descricao { get; set; }

        public int OportunidadeOriginal { get; set; }

        public int CriadoPor { get; set; }

        public FormaPagamento FormaPagamentoOportunidade { get; set; }

        public int[] SubClientesSelecionados { get; set; }

        public int[] ClientesGrupoCnpjSelecionados { get; set; }
    }
}
