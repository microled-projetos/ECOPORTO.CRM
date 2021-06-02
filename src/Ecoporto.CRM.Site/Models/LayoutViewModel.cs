using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class LayoutViewModel
    {
        public LayoutViewModel()
        {
            FaixasCIFViewModel = new FaixasCIFViewModel();
            FaixasBLViewModel = new FaixasBLViewModel();
            FaixasPesoViewModel = new FaixasPesoViewModel();
            FaixasVolumeViewModel = new FaixasVolumeViewModel();

            FaixasBL = new List<FaixaBL>();
            FaixasCIF = new List<FaixaCIF>();
            FaixasPeso = new List<FaixaPeso>();
            FaixasVolume = new List<FaixaVolume>();

            var moedaDefault = Moeda.REAL;

            MoedaArmazenagem = moedaDefault;
            MoedaArmazenagemAllIn = moedaDefault;
            MoedaHubPort = moedaDefault;
            MoedaServGerais = moedaDefault;
            MoedaServLib = moedaDefault;
            MoedaServMargem = moedaDefault;
            MoedaServMecMan = moedaDefault;
        }

        public int Id { get; set; }

        public int? OportunidadeId { get; set; }

        [Display(Name = "Modelo")]
        public int ModeloId { get; set; }

        [Display(Name = "Serviço")]
        public int ServicoId { get; set; }

        [Display(Name = "Linha")]
        public int Linha { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Display(Name = "Tipo Registro")]
        public TipoRegistro TipoRegistro { get; set; }

        [Display(Name = "Tipo Oper.")]
        public TipoTrabalho TipoTrabalho { get; set; }

        [Display(Name = "Margem")]
        public Margem MargemArmAllIn { get; set; }

        [Display(Name = "Margem")]
        public Margem MargemArmazenagem { get; set; }

        [Display(Name = "Margem")]
        public Margem MargemArmazenagemMinimo { get; set; }

        [Display(Name = "Margem")]
        public Margem MargemArmazenagemCIF { get; set; }

        [Display(Name = "Margem")]
        public Margem MargemServLib { get; set; }

        [Display(Name = "ANVISA")]
        public decimal ValorANVISA { get; set; }

        [Display(Name = "ANVISA")]
        public decimal ValorANVISACIF { get; set; }

        [Display(Name = "% Arm.")]
        public decimal AdicionalArmazenagem { get; set; }
        [Display(Name = "% Arm.")]
        public decimal AdicionalArmazenagemCIF { get; set; }

        [Display(Name = "% GRC")]
        public decimal AdicionalGRC { get; set; }
        [Display(Name = "% GRC")]
        public decimal AdicionalGRCCIF { get; set; }

        [Display(Name = "Min. GRC")]
        public decimal MinimoGRC { get; set; }
        [Display(Name = "Min. GRC")]
        public decimal MinimoGRCCIF { get; set; }

        [Display(Name = "Condições Gerais")]
        public string CondicoesGerais { get; set; }

        [Display(Name = "Condições Iniciais")]
        public string CondicoesIniciais { get; set; }

        public int Origem { get; set; }

        public int Destino { get; set; }

        public int Reembolso { get; set; }

        public bool LayoutProposta { get; set; }

        #region Descricao Valor

        [Display(Name = "Descrição Valor")]
        public string DescricaoValorArmazenagem { get; set; }

        [Display(Name = "Descrição Valor")]
        public string DescricaoValorArmazenagemCIF { get; set; }

        [Display(Name = "Descrição Valor")]
        public string DescricaoValorArmazenagemMin { get; set; }

        [Display(Name = "Descrição Valor")]
        public string DescricaoValorArmazenagemMinCIF { get; set; }

        [Display(Name = "Descrição Valor")]
        public string DescricaoValorArmazenagemAllIn { get; set; }

        [Display(Name = "Descrição Valor")]
        public string DescricaoValorMinGerais { get; set; }

        [Display(Name = "Descrição Valor")]
        public string DescricaoValorMinimoMecManual { get; set; }

        [Display(Name = "Descrição Valor")]
        public string DescricaoValorMinimoMargem { get; set; }

        [Display(Name = "Descrição Valor")]
        public string DescricaoValorHubPort { get; set; }

        [Display(Name = "Descrição Valor")]
        public string DescricaoValorServLib { get; set; }

        [Display(Name = "Descrição Valor")]
        public string DescricaoValorServMecManual { get; set; }

        [Display(Name = "Descrição Valor")]
        public string DescricaoValorServMargem { get; set; }

        [Display(Name = "Descrição Valor")]
        public string DescricaoValorServicosGerais { get; set; }

        [Display(Name = "IMO")]
        public decimal AdicionalIMOServicosGerais { get; set; }

        [Display(Name = "Exército")]
        public decimal ExercitoServicosGerais { get; set; }

        [Display(Name = "Descrição Valor")]
        public string DescricaoValorPeriodoPadrao { get; set; }

        #endregion

        #region Base Calculo

        [Display(Name = "B. Cálc.")]
        public BaseCalculo BaseCalculoArmazenagem { get; set; }
        [Display(Name = "B. Cálc.")]
        public BaseCalculo BaseCalculoArmazenagemCIF { get; set; }

        [Display(Name = "B. Cálc.")]
        public BaseCalculo BaseCalculoArmazenagemAllIn { get; set; }

        [Display(Name = "B. Cálc.")]
        public BaseCalculo BaseCalculoServMargem { get; set; }

        [Display(Name = "B. Cálc.")]
        public BaseCalculo BaseCalculoServMecManual { get; set; }

        [Display(Name = "B. Cálc.")]
        public BaseCalculo BaseCalculoServLib { get; set; }

        [Display(Name = "B. Cálc.")]
        public BaseCalculo BaseCalculoHubPort { get; set; }

        [Display(Name = "B. Cálc.")]
        public BaseCalculo BaseCalculoServGerais { get; set; }

        [Display(Name = "B. Cálc.")]
        public BaseCalculo BaseCalculoPeriodoPadrao { get; set; }

        #endregion

        #region Moeda

        [Display(Name = "Moeda")]
        public Moeda MoedaArmazenagem { get; set; }

        [Display(Name = "Moeda")]
        public Moeda MoedaArmazenagemCIF { get; set; }

        [Display(Name = "Moeda")]
        public Moeda MoedaArmazenagemAllIn { get; set; }

        [Display(Name = "Moeda")]
        public Moeda MoedaHubPort { get; set; }

        [Display(Name = "Moeda")]
        public Moeda MoedaServLib { get; set; }

        [Display(Name = "Moeda")]
        public Moeda MoedaServMecMan { get; set; }

        [Display(Name = "Moeda")]
        public Moeda MoedaServMargem { get; set; }

        [Display(Name = "Moeda")]
        public Moeda MoedaServGerais { get; set; }

        #endregion

        #region Tipo Carga

        [Display(Name = "Tipo Carga")]
        public TipoCarga TipoCargaArmazenagem { get; set; }
        [Display(Name = "Tipo Carga")]
        public TipoCarga TipoCargaArmazenagemCIF { get; set; }

        [Display(Name = "Tipo Carga")]
        public TipoCarga TipoCargaArmazenagemMinimo { get; set; }

        [Display(Name = "Tipo Carga")]
        public TipoCarga TipoCargaArmazenagemMinimoCIF { get; set; }

        [Display(Name = "Tipo Carga")]
        public TipoCarga TipoCargaArmazenagemAllIn { get; set; }

        [Display(Name = "Tipo Carga")]
        public TipoCarga TipoCargaServLib { get; set; }

        [Display(Name = "Tipo Carga")]
        public TipoCarga TipoCargaServGerais { get; set; }

        [Display(Name = "Tipo Carga")]
        public TipoCarga TipoCargaMinimoGerais { get; set; }

        [Display(Name = "Tipo Carga")]
        public TipoCarga TipoCargaServMargem { get; set; }

        [Display(Name = "Tipo Carga")]
        public TipoCarga TipoCargaPeriodoPadrao { get; set; }

        [Display(Name = "Tipo Carga")]
        public TipoCarga TipoCargaServMecManual { get; set; }

        #endregion        

        #region Periodo

        [Display(Name = "Período")]
        public int PeriodoArmazenagem { get; set; }
        [Display(Name = "Período")]
        public int PeriodoArmazenagemCIF { get; set; }

        [Display(Name = "Período")]
        public int PeriodoPadrao { get; set; }

        [Display(Name = "Período")]
        public int PeriodoArmazenagemAllIn { get; set; }

        [Display(Name = "Descrição Período")]
        public string DescricaoPeriodoAllIn { get; set; }
        

        #endregion

        #region Qtde Dias

        [Display(Name = "Qtd. Dias")]
        public int QtdeDiasArmazenagem { get; set; }
        [Display(Name = "Qtd. Dias")]
        public int QtdeDiasArmazenagemCIF { get; set; }

        [Display(Name = "Qtd. Dias")]
        public int QtdeDiasPeriodoPadrao { get; set; }

        #endregion

        #region Valor 20

        [Display(Name = "Valor '20")]
        public decimal Valor20Armazenagem { get; set; }
        [Display(Name = "Valor '20")]
        public decimal Valor20ArmazenagemCIF { get; set; }

        [Display(Name = "Valor '20")]
        public decimal Valor20ArmazenagemAllIn { get; set; }

        [Display(Name = "Valor '20")]
        public decimal Valor20ServMecManual { get; set; }

        [Display(Name = "Valor '20")]
        public decimal Valor20ServLib { get; set; }

        [Display(Name = "Valor '20")]
        public decimal Valor20ServGerais { get; set; }

        [Display(Name = "Valor '20")]
        public decimal Valor20PeriodoPadrao { get; set; }

        #endregion

        #region Valor 40

        [Display(Name = "Valor '40")]
        public decimal Valor40Armazenagem { get; set; }
        [Display(Name = "Valor '40")]
        public decimal Valor40ArmazenagemCIF { get; set; }

        [Display(Name = "Valor '40")]
        public decimal Valor40ArmazenagemAllIn { get; set; }

        [Display(Name = "Valor '40")]
        public decimal Valor40ServMecManual { get; set; }

        [Display(Name = "Valor '40")]
        public decimal Valor40ServLib { get; set; }

        [Display(Name = "Valor '40")]
        public decimal Valor40ServGerais { get; set; }

        [Display(Name = "Valor '40")]
        public decimal Valor40PeriodoPadrao { get; set; }

        #endregion

        #region Valor

        [Display(Name = "Valor")]
        public decimal ValorArmazenagem { get; set; }
        [Display(Name = "Valor")]
        public decimal ValorArmazenagemCIF { get; set; }

        [Display(Name = "Valor")]
        public decimal ValorServLib { get; set; }

        [Display(Name = "Valor")]
        public decimal ValorHubPort { get; set; }

        [Display(Name = "Valor")]
        public decimal ValorServGerais { get; set; }

        [Display(Name = "Valor")]
        public decimal ValorPeriodoPadrao { get; set; }

        [Display(Name = "Valor")]
        public decimal ValorServMecManual { get; set; }

        #endregion

        #region TipoDocumento

        [Display(Name = "Tipo Doc.")]
        public int TipoDocumentoArmazenagem { get; set; }

        [Display(Name = "Tipo Doc.")]
        public int TipoDocumentoArmazenagemCIF { get; set; }

        [Display(Name = "Tipo Doc.")]
        public int TipoDocumentoAllIn { get; set; }

        [Display(Name = "Tipo Doc.")]
        public int TipoDocumentoServMargem { get; set; }

        [Display(Name = "Tipo Doc.")]
        public int TipoDocumentoServLib { get; set; }

        [Display(Name = "Tipo Doc.")]
        public int TipoDocumentoGerais { get; set; }

        [Display(Name = "Tipo Doc.")]
        public IEnumerable<Documento> TiposDocumentos { get; set; }

        #endregion

        #region BaseExcesso

        [Display(Name = "B.Excesso")]
        public BaseCalculoExcesso BaseExcessoArmazenagem { get; set; }

        [Display(Name = "B.Excesso")]
        public BaseCalculoExcesso BaseExcessoArmazenagemCIF { get; set; }

        [Display(Name = "B.Excesso")]
        public BaseCalculoExcesso BaseExcessoAllIn { get; set; }

        [Display(Name = "B.Excesso")]
        public BaseCalculoExcesso BaseExcessoServMargem { get; set; }

        [Display(Name = "B.Excesso")]
        public BaseCalculoExcesso BaseExcessoGerais { get; set; }

        #endregion

        #region ValorExcesso

        [Display(Name = "Valor Excesso")]
        public decimal ValorExcessoArmazenagem { get; set; }

        [Display(Name = "Valor Excesso")]
        public decimal ValorExcessoArmazenagemCIF { get; set; }

        [Display(Name = "Valor Excesso")]
        public decimal ValorExcessoAllIn { get; set; }

        [Display(Name = "Valor Excesso")]
        public decimal ValorExcessoServMargem { get; set; }

        [Display(Name = "Valor Excesso")]
        public decimal ValorExcessoGerais { get; set; }

        #endregion

        #region AcrescimoPeso

        [Display(Name = "Acrés. Peso")]
        public decimal AcrescimoPesoArmazenagem { get; set; }

        [Display(Name = "Acrés. Peso")]
        public decimal AcrescimoPesoArmazenagemCIF { get; set; }

        [Display(Name = "Acrés. Peso")]
        public decimal AcrescimoPesoAllIn { get; set; }

        [Display(Name = "Acrés. Peso")]
        public decimal AcrescimoPesoServMargem { get; set; }

        #endregion

        #region PesoLimite

        [Display(Name = "Peso Limite")]
        public decimal PesoLimiteArmazenagem { get; set; }

        [Display(Name = "Peso Limite")]
        public decimal PesoLimiteArmazenagemCIF { get; set; }

        [Display(Name = "Peso Limite")]
        public decimal PesoLimiteAllIn { get; set; }

        [Display(Name = "Peso Limite")]
        public decimal PesoLimiteServMargem { get; set; }

        #endregion

        #region ProRata

        [Display(Name = "Pro Rata")]
        public bool ProRataArmazenagem { get; set; }

        [Display(Name = "Pro Rata")]
        public bool ProRataArmazenagemCIF { get; set; }

        [Display(Name = "Pro Rata")]
        public bool ProRataAllIn { get; set; }

        [Display(Name = "Pro Rata")]
        public bool ProRataServMargem { get; set; }

        #endregion

        [Display(Name = "CIF")]
        public decimal CIFArmazenagemCIF { get; set; }        

        [Display(Name = "CIF")]
        public decimal CIFArmazenagemMinimoCIF { get; set; }

        [Display(Name = "CIF Mínimo")]
        public decimal CIFMinimoAllIn { get; set; }

        [Display(Name = "CIF Máximo")]
        public decimal CIFMaximoAllIn { get; set; }

        [Display(Name = "Descrição CIF")]
        public string DescricaoCifAllIn { get; set; }

        #region Adicional IMO

        [Display(Name = "% IMO")]
        public decimal AdicionalIMOArmazenagem { get; set; }

        [Display(Name = "Exército")]
        public decimal ExercitoArmazenagem { get; set; }

        [Display(Name = "% IMO")]
        public decimal AdicionalIMOArmazenagemCIF { get; set; }

        [Display(Name = "Exército")]
        public decimal ExercitoArmazenagemCIF { get; set; }

        [Display(Name = "% IMO")]
        public decimal AdicionalIMOServMecManual { get; set; }

        [Display(Name = "Exército")]
        public decimal ExercitoServMecManual { get; set; }

        [Display(Name = "% IMO")]
        public decimal AdicionalIMOServMargem { get; set; }

        [Display(Name = "Exército")]
        public decimal ExercitoServMargem { get; set; }

        [Display(Name = "% IMO")]
        public decimal AdicionalIMOServLib { get; set; }

        [Display(Name = "Exército")]
        public decimal ExercitoServLib { get; set; }
        #endregion

        #region Valor Minimo

        [Display(Name = "Valor Min. CS")]
        public decimal ValorMinimoArmazenagemMin { get; set; }

        [Display(Name = "Valor Min. CS")]
        public decimal ValorMinimoArmazenagemMinCIF { get; set; }

        [Display(Name = "Valor Min.")]
        public decimal ValorMinimoGeral { get; set; }

        [Display(Name = "Valor Min.")]
        public decimal ValorMinimoAllIn { get; set; }

        [Display(Name = "Valor Min.")]
        public decimal ValorMinimoServLib { get; set; }

        #endregion

        #region Valor Minimo 20

        [Display(Name = "Min. '20")]
        public decimal ValorMinimo20ArmazenagemMin { get; set; }

        [Display(Name = "Min. '20")]
        public decimal ValorMinimo20ArmazenagemMinCIF { get; set; }

        [Display(Name = "Min. '20")]
        public decimal ValorMinimo20Geral { get; set; }

        [Display(Name = "Min. '20")]
        public decimal ValorMinimo20MecManual { get; set; }

        #endregion

        #region Valor Minimo 40

        [Display(Name = "Min. '40")]
        public decimal ValorMinimo40ArmazenagemMin { get; set; }

        [Display(Name = "Min. '40")]
        public decimal ValorMinimo40ArmazenagemMinCIF { get; set; }

        [Display(Name = "Min. '40")]
        public decimal ValorMinimo40Geral { get; set; }

        [Display(Name = "Min. '40")]
        public decimal ValorMinimo40MecManual { get; set; }

        #endregion

        #region Peso Maximo

        [Display(Name = "Peso Máx.")]
        public decimal PesoMaximoServicoParaMargem { get; set; }

        [Display(Name = "Peso Máx.")]
        public decimal PesoMaximoServicoMecanicaManual { get; set; }

        #endregion    

        #region Adicional de Peso

        [Display(Name = "% Peso")]
        public decimal AdicionalPesoServicoParaMargem { get; set; }

        [Display(Name = "% Peso")]
        public decimal AdicionalPesoServicoMecanicaManual { get; set; }

        #endregion

        #region Margem

        [Display(Name = "Direita")]
        public decimal ValorMargemDireitaServMargem { get; set; }

        [Display(Name = "Esquerda")]
        public decimal ValorMargemEsquerdaServMargem { get; set; }

        [Display(Name = "E. Margens")]
        public decimal ValorEntreMargensServMargem { get; set; }

        [Display(Name = "Direita")]
        public decimal ValorMinimoMargemDireita { get; set; }

        [Display(Name = "Esquerda")]
        public decimal ValorMinimoMargemEsquerda { get; set; }

        [Display(Name = "E. Margens")]
        public decimal ValorMinimoEntreMargens { get; set; }

        #endregion

        #region Linha Referencia

        [Display(Name = "Linha Ref.")]
        public int LinhaReferenciaArmazenagemMin { get; set; }

        [Display(Name = "Linha Ref.")]
        public int LinhaReferenciaArmazenagemMinCIF { get; set; }

        [Display(Name = "Linha Ref.")]
        public int LinhaReferenciaMinimoParaMargem { get; set; }

        [Display(Name = "Linha Ref.")]
        public int LinhaReferenciaMinimoMecanicaManual { get; set; }

        [Display(Name = "Linha Ref.")]
        public int LinhaReferenciaMinimoGeral { get; set; }

        #endregion

        [Display(Name = "Ocultar")]
        public bool Ocultar { get; set; }

        [Display(Name = "Grupo Atracação")]
        public int GrupoAtracacaoServLiv { get; set; }

        [Display(Name = "Grupo Atracação")]
        public int GrupoAtracacaoArmazenagem { get; set; }

        [Display(Name = "Limite Bl's")]
        public int LimiteBls { get; set; }

        [Display(Name = "Limite Bl's")]
        public int LimiteBlsCIF { get; set; }

        [Display(Name = "% ANVISA GRC")]
        public decimal AnvisaGRC { get; set; }

        [Display(Name = "% ANVISA GRC")]
        public decimal AnvisaGRCCIF { get; set; }

        [Display(Name = "% IMO GRC")]
        public decimal AdicionalIMOGRC { get; set; }
        [Display(Name = "% IMO GRC")]
        public decimal AdicionalIMOGRCCIF { get; set; }

        [Display(Name = "Forma Pgto NVOCC")]
        public FormaPagamento FormaPagamentoNVOCCGerais { get; set; }

        [Display(Name = "Forma Pgto NVOCC")]
        public FormaPagamento FormaPagamentoNVOCCHubPort { get; set; }

        public IEnumerable<FaixaBL> FaixasBL { get; set; }

        public IEnumerable<FaixaCIF> FaixasCIF { get; set; }

        public IEnumerable<FaixaPeso> FaixasPeso { get; set; }

        public IEnumerable<FaixaVolume> FaixasVolume { get; set; }

        public IEnumerable<Servico> Servicos { get; set; }

        public IEnumerable<Modelo> Modelos { get; set; }

        public IEnumerable<ClienteHubPort> ClientesHubPort { get; set; }

        public IEnumerable<Terminal> GruposAtracacao { get; set; }

        public FaixasCIFViewModel FaixasCIFViewModel { get; set; }

        public FaixasBLViewModel FaixasBLViewModel { get; set; }

        public FaixasPesoViewModel FaixasPesoViewModel { get; set; }

        public FaixasVolumeViewModel FaixasVolumeViewModel { get; set; }
    }
}