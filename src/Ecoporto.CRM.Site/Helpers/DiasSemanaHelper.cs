using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ecoporto.CRM.Site.Helpers
{
    public static class DiasSemanaHelper
    {
        public static string DiaSemanaPorExtenso(int dia)
        {
            string diaSemana = string.Empty;

            switch (dia)
            {
                case 1:
                    diaSemana = "Domingo";
                    break;
                case 2:
                    diaSemana = "Segunda-Feira";
                    break;
                case 3:
                    diaSemana = "Terça-Feira";
                    break;
                case 4:
                    diaSemana = "Quarta-Feira";
                    break;
                case 5:
                    diaSemana = "Quinta-Feira";
                    break;
                case 6:
                    diaSemana = "Sexta-Feira";
                    break;
                case 7:
                    diaSemana = "Sábado";
                    break;
            }

            return diaSemana;
        }
    }
}