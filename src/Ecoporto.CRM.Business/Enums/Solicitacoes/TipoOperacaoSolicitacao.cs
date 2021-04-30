using System;
using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum _TipoOperacaoSolicitacao
    {
        [Display(Name = "Recinto Alfandegado")]
        RA = 1,
        [Display(Name = "Operador")]
        OP,
        [Display(Name = "Redex")]
        RE,
        [Display(Name = "Transporte")]
        TR,
        [Display(Name = "Pátio Regulador")]
        PATIO_REGULADOR,
        [Display(Name = "Depot TP")]
        DEPOT_TP,
        [Display(Name = "Depot Armador")]
        PATIO_ARMADOR
    }

    //public static class TipoOperacaoSolicitacaoExtension
    //{
    //    public static string DescricaoResumida(this TipoOperacaoSolicitacao tipoOperacaoSolicitacao)
    //    {
    //        if (Enum.IsDefined(typeof(TipoOperacaoSolicitacao), tipoOperacaoSolicitacao))
    //        {
    //            switch (tipoOperacaoSolicitacao)
    //            {
    //                case TipoOperacaoSolicitacao.RA:
    //                    return "RA";
    //                case TipoOperacaoSolicitacao.OP:
    //                    return "OP";
    //                case 3:
    //                    return "RE";
    //                case 4:
    //                    return "TR";
    //                case TipoOperacaoSolicitacao.PATIO_REGULADOR:
    //                    return "PR";
    //                case 6:
    //                    return "DTP";
    //                case TipoOperacaoSolicitacao.PATIO_ARMADOR:
    //                    return "PA";
    //            }                
    //        }

    //        return string.Empty;
    //    }
    //}
}
