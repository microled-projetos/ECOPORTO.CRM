namespace WsSimuladorCalculoTabelas.Helpers
{
    public static class ConverteMoedaIPA
    {
        public static int MoedaIPA(int moedaCRM)
        {
            if (moedaCRM == 0)
                return 21;

            return moedaCRM == 1 ? 21 : 22;
        }
    }
}