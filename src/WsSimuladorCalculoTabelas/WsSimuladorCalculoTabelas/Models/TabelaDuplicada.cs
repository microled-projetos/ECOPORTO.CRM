namespace WsSimuladorCalculoTabelas.Models
{
    public class TabelaDuplicada
    {
        public int Total { get; set; }

        public int Min { get; set; }

        public int Max { get; set; }

        public int Servico { get; set; }

        public int MaxAuto { get; set; }

    }
        public class TabelaSemminimo
        {
           public int Autonum { get; set; }

           public int Servico { get; set; }

            public int N_Periodo { get; set; }

            public string Tipo_Carga { get; set; }

            public string Base_Calculo { get; set; }
            public string Variante_Local { get; set; }
            public int Lista { get; set; }
         
    }
     
}