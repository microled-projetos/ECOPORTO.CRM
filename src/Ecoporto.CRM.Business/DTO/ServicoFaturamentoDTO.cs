namespace Ecoporto.CRM.Business.DTO
{
    public class ServicoFaturamentoDTO
    {
        public int Id { get; set; }

        public int Servico { get; set; }

        public string Descricao { get; set; }

        public string ClienteDescricao { get; set; }

        public decimal Valor { get; set; }
    }
}
