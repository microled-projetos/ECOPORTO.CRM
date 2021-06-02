using WsSimuladorCalculoTabelas.Extensions;

namespace WsSimuladorCalculoTabelas.Helpers
{
    public static class ConverteDiaSemanaIPA
    {
        public static int? DiaSemana(string diaSemanaCRM)
        {
            var diaSemana = diaSemanaCRM.ToInt();

            if (diaSemana >= 1 && diaSemana <= 7)
                return diaSemana - 1;

            return null;
        }
    }
}