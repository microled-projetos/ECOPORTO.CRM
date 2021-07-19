namespace WsSimuladorCalculoTabelas.Helpers
{
    public static class ConverteMargemIPA
    {
        public static string MargemIPA(int margemCRM)
        {
            string margemIPA = "SVAR";

            if (margemCRM == 1)
            {
                margemIPA = "MDIR";
            }

            if (margemCRM == 2)
            {
                margemIPA = "MESQ";
            }

            if (margemCRM == 3)
            {
                margemIPA = "ENTR";
            }

            return margemIPA;
        }
    }
}