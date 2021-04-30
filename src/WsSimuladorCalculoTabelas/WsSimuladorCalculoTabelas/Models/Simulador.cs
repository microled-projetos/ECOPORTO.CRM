using System.Text;

namespace WsSimuladorCalculoTabelas.Models
{
    public class Simulador
    {
        public int SimuladorId { get; set; }

        public string Descricao { get; set; }

        public int NumeroLotes { get; set; }

        public int TipoDocumento { get; set; }

        public int GrupoAtracacao { get; set; }

        public int LocalAtracacao { get; set; }

        public int Classe { get; set; }

        public decimal ValorCifConteiner { get; set; }

        public decimal ValorCifCargaSolta { get; set; }

        public decimal VolumeM3 { get; set; }

        public int Armador { get; set; }

        public int Periodos { get; set; }

        public string Margem { get; set; }

        public string Regime { get; set; }

        public string TipoCarga { get; set; }

        public string TipoCargaSQL { get; set; }

        public int ArmadorId { get; set; }

        public int LocalAtracacaoId { get; set; }

        public int Qtde20 { get; set; }

        public int Qtde40 { get; set; }

        public int CriadoPor { get; set; }

        public CargaConteiner CargaConteiner20 { get; set; }

        public CargaConteiner CargaConteiner40 { get; set; }

        public CargaSolta CargaSolta { get; set; }

        public void AdicionarCargaConteiner(CargaConteiner cargaConteiner20, CargaConteiner cargaConteiner40)
        {
            CargaConteiner20 = cargaConteiner20;
            CargaConteiner40 = cargaConteiner40;
        }

        public void AdicionarCargaSolta(CargaSolta cargaSolta)
        {
            CargaSolta = cargaSolta;
        }

        public string ObterFiltro()
        {
            var filtro = new StringBuilder();

            if (!string.IsNullOrEmpty(Regime))
            {
                filtro.Append($" Regime: {Regime} | ");
            }

            if (TipoDocumento > 0)
            {
                filtro.Append($" Documento: {TipoDocumento} | ");
            }

            if (Armador > 0)
            {
                filtro.Append($" Armador: {Armador} | ");
            }

            if (Regime == "FCL")
            {
                if (ValorCifConteiner > 0)
                {
                    filtro.Append($" CIF Contêiner: {string.Format("{0:C2}", ValorCifConteiner)} | ");

                    if (CargaConteiner20 != null)
                    {
                        filtro.Append($" Qtde CNTR '20: {CargaConteiner20.Quantidade} | ");
                        filtro.Append($" Peso Total CNTR '20: {CargaConteiner20.Peso} | ");
                    }

                    if (CargaConteiner40 != null)
                    {
                        filtro.Append($" Qtde CNTR '40: {CargaConteiner40.Quantidade} | ");
                        filtro.Append($" Peso Total CNTR '40: {CargaConteiner40.Peso} | ");
                    }
                }
            }
            else
            {
                if (ValorCifCargaSolta > 0)
                {
                    filtro.Append($" CIF Carga Solta: {string.Format("{0:C2}", ValorCifCargaSolta)} | ");

                    if (CargaSolta != null)
                    {
                        filtro.Append($" Qtde. C.S: {CargaSolta.Quantidade} | ");
                        filtro.Append($" Peso Total C.S: {CargaSolta.Peso} | ");
                        filtro.Append($" Volume Total C.S: {VolumeM3} | ");
                    }
                }
            }

            if (LocalAtracacao > 0)
            {
                filtro.Append($" Local Atracação: {LocalAtracacao} | ");
            }

            if (GrupoAtracacao > 0)
            {
                filtro.Append($" Grupo Atracação: {GrupoAtracacao} | ");
            }

            if (Periodos > 0)
            {
                filtro.Append($" Períodos: {Periodos} | ");
            }

            if (NumeroLotes > 0)
            {
                filtro.Append($" Lotes: {NumeroLotes} | ");
            }

            return filtro.ToString();
        }
    }
}