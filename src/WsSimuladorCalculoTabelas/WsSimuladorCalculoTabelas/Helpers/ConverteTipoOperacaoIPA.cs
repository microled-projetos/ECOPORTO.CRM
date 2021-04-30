namespace WsSimuladorCalculoTabelas.Helpers
{
    public static class ConverteTipoOperacaoIPA
    {
        public static string TipoOperacaoIPA(int tipoOperacaoCRM)
        {
            if (tipoOperacaoCRM == 1)
                return "MECANIZADA";

            if (tipoOperacaoCRM == 2)
                return "MANUAL";

            return string.Empty;
        }
    }
}