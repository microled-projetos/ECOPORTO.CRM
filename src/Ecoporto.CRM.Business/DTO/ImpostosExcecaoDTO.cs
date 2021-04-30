using Ecoporto.CRM.Business.Enums;

namespace Ecoporto.CRM.Business.DTO
{
    public class ImpostosExcecaoDTO
    {
        public int Id { get; set; }

        public string Servico { get; set; }

        public string Excecao { get; set; }

        public int ServicoId { get; set; }

        public int ModeloId { get; set; }

        public int OportunidadeId { get; set; }

        public TiposExcecoesImpostos Tipo { get; set; }

        public int[] ServicosSelecionados { get; set; }

        public bool Selecionado { get; set; }

        public bool ISS { get; set; }

        public bool PIS { get; set; }

        public bool COFINS { get; set; }

        public decimal ValorISS { get; set; }

        public decimal ValorPIS { get; set; }

        public decimal ValorCOFINS { get; set; }
    }
}
