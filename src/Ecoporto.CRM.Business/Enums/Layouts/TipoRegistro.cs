using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Business.Enums
{
    public enum TipoRegistro
    {
        [Display(Name = "Condições Iniciais")]
        CONDICAO_INICIAL = 1,
        [Display(Name = "Título Master")]
        TITULO_MASTER,
        [Display(Name = "Título")]
        TITULO,
        [Display(Name = "Sub Título")]
        SUB_TITULO,
        [Display(Name = "Sub Título Margem")]        
        SUB_TITULO_MARGEM,
        [Display(Name = "Sub Título All In")]
        SUB_TITULO_ALL_IN,
        [Display(Name = "Armazenagem")]        
        ARMAZENAGEM,
        [Display(Name = "Armazenagem Mínimo")]
        ARMAZENAGEM_MINIMO,        
        [Display(Name = "Armazenagem All In")]
        ARMAZENAMEM_ALL_IN, 
        [Display(Name = "Serviço para Margem")]
        SERVIÇO_PARA_MARGEM, 
        [Display(Name = "Mínimo para Margem")]
        MINIMO_PARA_MARGEM, 
        [Display(Name = "Serviço Mecânica Manual")]
        SERVICO_MECANICA_MANUAL, 
        [Display(Name = "Mínimo Mecânica Manual")]
        MINIMO_MECANICA_MANUAL, 
        [Display(Name = "Serviço Liberação")]
        SERVICO_LIBERACAO, 
        [Display(Name = "Serviço Hub Port")]
        SERVICO_HUBPORT, 
        [Display(Name = "Gerais")]
        GERAIS, 
        [Display(Name = "Mínimo Geral")]
        MINIMO_GERAL,
        [Display(Name = "Condição Geral")]
        CONDICAO_GERAL,
        [Display(Name = "Período Padrão")]
        PERIODO_PADRAO,
        [Display(Name = "Sub Título Margem (D/E)")]
        SUB_TITULO_MARGEM_D_E,
        [Display(Name = "Armazenagem CIF")]
        ARMAZENAGEM_CIF,
        [Display(Name = "Armazenagem Mínimo CIF")]
        ARMAZENAGEM_MINIMO_CIF
    }
}