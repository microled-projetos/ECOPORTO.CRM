using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Extensions;
using System;

namespace Ecoporto.CRM.Site.Models.Pdf
{
    public class Proposta
    {
        public DateTime Vigencia { get; set; }

        public string Referencia { get; set; }

        public string CondicoesIniciais { get; set; }

        public string Tabela { get; set; }

        public int? IdTabelaVinculada { get; set; }

        public string CondicoesGerais { get; set; }

        public string Identificacao { get; set; }

        public DateTime DataInicio { get; set; }

        public TipoServico TipoServico { get; set; }

        public string DescricaoTipoServico => TipoServico.ToName();

        public bool Validar()
        {
            return !string.IsNullOrEmpty(Tabela);
        }

        public override string ToString()
        {
            return $"{CondicoesIniciais} {Tabela} {CondicoesGerais}";
        }
    }
}