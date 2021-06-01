using Ecoporto.CRM.Business.Enums;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Models
{
    public class SimuladorPropostaParametros
    {
        public SimuladorPropostaParametros(
            int oportunidadeId,
            int modeloSimuladorId,
            string regime, 
            string margem, 
            int grupoAtracacaoId, 
            decimal volumeM3, 
            decimal peso,
            int periodos, 
            int? tipoDocumentoId, 
            int qtde20,
            int qtde40,
            string observacoes,
            decimal valorCif,
            int criadoPor)
        {
            OportunidadeId = oportunidadeId;
            ModeloSimuladorId = modeloSimuladorId;
            Regime = regime;
            Margem = margem;
            GrupoAtracacaoId = grupoAtracacaoId;
            VolumeM3 = volumeM3;
            Peso = peso;
            Periodos = periodos;
            TipoDocumentoId = tipoDocumentoId;
            Qtde20 = qtde20;
            Qtde40 = qtde40;
            Observacoes = observacoes;
            ValorCif = valorCif;
            CriadoPor = criadoPor;
        }

        public int Id { get; set; }

        public int OportunidadeId { get; set; }

        public int ModeloSimuladorId { get; set; }

        public string Regime { get; set; }

        public int NumeroLotes { get; set; }

        public string Margem { get; set; }

        public List<string> Margens { get; set; }
        
        public int GrupoAtracacaoId { get; set; }

        public List<Terminal> GruposAtracacao { get; set; }

        public decimal VolumeM3 { get; set; }

        public decimal Peso { get; set; }

        public decimal ValorCif { get; set; }

        public int Periodos { get; set; }

        public int? TipoDocumentoId { get; set; }

        public List<Documento> TiposDocumentos { get; set; }

        public int Qtde20 { get; set; }

        public int Qtde40 { get; set; }

        public string Observacoes { get; set; }

        public int CriadoPor { get; set; }
    }
}
