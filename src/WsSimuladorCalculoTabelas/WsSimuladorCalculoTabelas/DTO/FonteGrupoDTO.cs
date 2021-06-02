namespace WsSimuladorCalculoTabelas.DTO
{
    public class FonteGrupoDTO
    {
        public int AUTONUM { get; set; }

        public int TabelaId { get; set; }

        public int AUTONUM_CLIENTE_NOTA { get; set; }

        public int AUTONUM_CLIENTE_ENVIO_NOTA { get; set; }

        public int AUTONUM_CLIENTE_PAGAMENTO { get; set; }

        public int AUTONUM_GRUPO_LISTA { get; set; }

        public string CORTE { get; set; }

        public string DIA { get; set; }

        public string AUTONUM_FORMA_PAGAMENTO { get; set; }

        public string EMAIL { get; set; }

        public int FLAG_ENTREGA_ELETRONICA { get; set; }

        public int FLAG_ENTREGA_MANUAL { get; set; }

        public int FLAG_ENVIO_CORREIO_COMUM { get; set; }

        public int FLAG_ENVIO_CORREIO_SEDEX { get; set; }

        public int FLAG_ULTIMO_DIA_DO_MES_CORTE { get; set; }

        public int FLAG_ULTIMO_DIA_DO_MES_VCTO { get; set; }

        public int FLAG_VENCIMENTO_DIA_UTIL { get; set; }
    }
}