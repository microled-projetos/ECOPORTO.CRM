namespace WsSimuladorCalculoTabelas.Helpers
{
    public static class ConverteFormaPagamentoIPA
    {
        public static int FormaPagamentoIPA(int formaPagamento)
        {
            return formaPagamento == 1 ? 2 : 3;
        }
    }
}