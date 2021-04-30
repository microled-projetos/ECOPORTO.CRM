using OfficeOpenXml.Style;
using System.Drawing;

namespace WsSimuladorCalculoTabelas.Models
{
    public class ExcelCelulaParametros
    {
        public ExcelCelulaParametros(string valor)
        {
            Valor = valor;
        }

        public ExcelCelulaParametros(string valor, double altura)
        {
            Valor = valor;
            Altura = altura;
        }

        public ExcelCelulaParametros(string valor, bool alinhaDireita, double altura)
        {
            Valor = valor;
            AlinhaDireita = alinhaDireita;
            Altura = altura;
        }

        public ExcelCelulaParametros(string valor, bool alinhaDireita, double altura, bool negrito, bool corDeFundo)
        {
            Valor = valor;
            AlinhaDireita = alinhaDireita;
            Altura = altura;
            Negrito = negrito;
            CorDeFundo = corDeFundo;
        }

        public ExcelCelulaParametros(string valor, bool alinhaDireita, double altura, bool corDeFundo)
        {
            Valor = valor;
            AlinhaDireita = alinhaDireita;
            Altura = altura;
            CorDeFundo = corDeFundo;
        }

        public ExcelCelulaParametros(string valor, bool negrito, bool centraliza)
        {
            Valor = valor;
            Negrito = negrito;
            Centraliza = centraliza;
        }

        public ExcelCelulaParametros(string valor, bool negrito, bool corDeFundo, bool centraliza)
        {
            Valor = valor;
            Negrito = negrito;
            CorDeFundo = corDeFundo;
            Centraliza = centraliza;
        }

        public ExcelCelulaParametros(string valor, bool negrito, bool centraliza, double altura)
        {
            Valor = valor;
            Negrito = negrito;
            Centraliza = centraliza;
            Altura = altura;
        }

        public ExcelCelulaParametros(string valor, bool corDeFundo, double altura, Color cor)
        {
            Valor = valor;
            CorDeFundo = corDeFundo;
            Altura = altura;
            Cor = cor;
        }

        public ExcelCelulaParametros(string valor, bool centraliza, bool corDeFundo,  Color cor)
        {
            Valor = valor;
            Centraliza = centraliza;
            CorDeFundo = corDeFundo;
            Cor = cor;
        }

        public ExcelCelulaParametros(string valor, bool centraliza, bool negrito, bool corDeFundo, Color cor)
        {
            Valor = valor;
            Centraliza = centraliza;
            Negrito = negrito;
            CorDeFundo = corDeFundo;
            Cor = cor;
        }

        public ExcelCelulaParametros(string valor, bool corDeFundo)
        {
            Valor = valor;
            CorDeFundo = corDeFundo;
        }

        //public int Linha { get; set; }

        public string Valor { get; set; }

        public bool Negrito { get; set; }

        public bool CorDeFundo { get; set; }

        public bool Centraliza { get; set; }

        public bool AlinhaDireita { get; set; }

        public double Altura { get; set; }

        public Color? Cor { get; set; }

        public ExcelBorderStyle TipoBorda { get; set; } = ExcelBorderStyle.Thin;
    }
}