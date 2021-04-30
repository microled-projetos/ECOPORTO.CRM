using System;
using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum TipoOperacao
    {
        [Display(Name = "Recinto Alfandegado")]
        RA = 1,
        [Display(Name = "Operador")]
        OP,
        [Display(Name = "Redex")]
        RE
    }

    public static class TipoOperacaoExtension
    {
        public static string DescricaoResumida(this TipoOperacao tipoOperacao)
        {
            if (Enum.IsDefined(typeof(TipoOperacao), tipoOperacao))
            {
                switch (tipoOperacao)
                {
                    case TipoOperacao.RA:
                        return "RA";
                    case TipoOperacao.OP:
                        return "OP";
                    case TipoOperacao.RE:
                        return "RE";
                }                
            }

            return string.Empty;
        }
    }
}
