using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Business.ValueObjects;

namespace Ecoporto.CRM.Business.Factory
{
    public abstract class LayoutFactory
    {
        public static LayoutCondicoesIniciais NovoLayoutCondicoesIniciais(
           int modeloId,
           int linha,
           string descricao,
           string condicoesIniciais,
           bool ocultar)
        {
            var cabecalho = new Cabecalho(modeloId, linha, descricao, TipoRegistro.CONDICAO_INICIAL, ocultar);

            return new LayoutCondicoesIniciais(
                cabecalho,
                condicoesIniciais);
        }

        public static LayoutCondicoesGerais NovoLayoutCondicoesGerais(
            int modeloId,
            int linha,
            string descricao,
            string condicoesGerais,
            bool ocultar)
        {
            var cabecalho = new Cabecalho(modeloId, linha, descricao, TipoRegistro.CONDICAO_GERAL, ocultar);

            return new LayoutCondicoesGerais(
                cabecalho,
                condicoesGerais);
        }

        public static LayoutMinimoGeral NovoLayoutMinimoGeral(
            int modeloId,
            int linha,
            string descricao,
            decimal valor,
            decimal valor20Geral,
            decimal valor40Geral,
            TipoCarga tipoCarga,
            BaseCalculo baseCalculo,
            int linhaReferencia,
            string descricaoValor,
            bool ocultar)
        {
            var cabecalho = new Cabecalho(modeloId, linha, descricao, TipoRegistro.MINIMO_GERAL, ocultar);

            var valorCarga = new ValorCargaMinimo(
                valor,
                valor20Geral,
                valor40Geral,
                tipoCarga,
                baseCalculo);

            var layoutMinimoGeral = new LayoutMinimoGeral(
                cabecalho,
                baseCalculo,
                valorCarga,
                linhaReferencia,
                descricaoValor);

            return layoutMinimoGeral;
        }

        public static LayoutServicosGerais NovoLayoutServicosGerais(
            int modeloId,
            int linha,
            string descricao,
            int servicoId,
            decimal valor,
            decimal valor20,
            decimal valor40,
            decimal adicionalIMO,
            decimal exercito,
            TipoCarga tipoCarga,
            BaseCalculo baseCalculo,
            Moeda moeda,
            string descricaoValor,
            int tipoDocumentoId,
            BaseCalculoExcesso baseExcesso,
            decimal valorExcesso,
            FormaPagamento formaPagamentoNVOCC,
            bool ocultar)
        {
            var cabecalho = new Cabecalho(modeloId, linha, descricao, TipoRegistro.GERAIS, ocultar);

            var valorCarga = new ValorCarga(
                valor,
                valor20,
                valor40,
                tipoCarga);

            var layoutServicosGerais = new LayoutServicosGerais(
                cabecalho,
                servicoId,
                baseCalculo,
                valorCarga,
                moeda,
                descricaoValor,
                adicionalIMO,
                exercito,
                tipoDocumentoId,
                baseExcesso,
                valorExcesso,
                formaPagamentoNVOCC);

            return layoutServicosGerais;
        }

        public static LayoutServicoHubPort NovoLayoutServicoHubPort(
            int modeloId,
            int linha,
            string descricao,
            int servicoId,
            BaseCalculo baseCalculo,
            decimal valor,
            int origem,
            int destino,
            Moeda moeda,
            FormaPagamento formaPagamentoNVOCC,
            string descricaoValor,
            bool ocultar)
        {
            var cabecalho = new Cabecalho(modeloId, linha, descricao, TipoRegistro.SERVICO_HUBPORT, ocultar);

            var layoutServicoHubPort = new LayoutServicoHubPort(
                cabecalho,
                servicoId,
                baseCalculo,
                valor,
                origem,
                destino,
                moeda,
                formaPagamentoNVOCC,
                descricaoValor);

            return layoutServicoHubPort;
        }

        public static LayoutServicoLiberacao NovoLayoutServicoLiberacao(
            int modeloId,
            int linha,
            string descricao,
            decimal valor,
            decimal valor20,
            decimal valor40,
            TipoCarga tipoCarga,
            int servicoId,
            BaseCalculo baseCalculo,
            Margem margem,
            int reembolso,
            Moeda moeda,
            string descricaoValor,
            int tipoDocumentoId,
            int grupoAtracacaoId,
            decimal adicionalIMO,
            decimal exercito,
            bool ocultar)
        {
            var cabecalho = new Cabecalho(modeloId, linha, descricao, TipoRegistro.SERVICO_LIBERACAO, ocultar);

            var valorCarga = new ValorCarga(
               valor,
               valor20,
               valor40,
               tipoCarga);

            var layoutServicoLiberacao = new LayoutServicoLiberacao(
                cabecalho,
                servicoId,
                baseCalculo,
                margem,
                tipoCarga,
                valorCarga,
                reembolso,
                moeda,
                descricaoValor,
                tipoDocumentoId,
                grupoAtracacaoId,
                adicionalIMO,
                exercito);

            return layoutServicoLiberacao;
        }

        public static LayoutMinimoMecanicaManual NovoLayoutMinimoMecanicaManual(
            int modeloId,
            int linha,
            string descricao,
            decimal valor20,
            decimal valor40,
            int linhaReferencia,
            string descricaoValor,
            bool ocultar)
        {
            var cabecalho = new Cabecalho(modeloId, linha, descricao, TipoRegistro.MINIMO_MECANICA_MANUAL, ocultar);

            var valorCarga = new ValorCargaMinimo(
                valor20,
                valor40);

            var layoutMinimoMecanicaManual = new LayoutMinimoMecanicaManual(
                cabecalho,
                valorCarga,
                linhaReferencia,
                descricaoValor);

            return layoutMinimoMecanicaManual;
        }

        public static LayoutServicoMecanicaManual NovoLayoutServicoMecanicaManual(
            int modeloId,
            int linha,
            string descricao,
            decimal valor,
            decimal valor20,
            decimal valor40,
            int servicoId,
            BaseCalculo baseCalculo,
            TipoCarga tipoCarga,
            decimal adicionalIMO,
            decimal exercito,
            Moeda moeda,
            decimal pesoMaximo,
            decimal adicionalPeso,
            TipoTrabalho tipoTrabalho,
            string descricaoValor,
            bool ocultar)
        {
            var cabecalho = new Cabecalho(modeloId, linha, descricao, TipoRegistro.SERVICO_MECANICA_MANUAL, ocultar);

            var valorCarga = new ValorCarga(
                valor,
                valor20,
                valor40, 
                tipoCarga);

            var layoutServicoMecanicaManual = new LayoutServicoMecanicaManual(
                cabecalho,
                servicoId,
                baseCalculo,
                valorCarga,
                adicionalIMO,
                exercito,
                moeda,
                pesoMaximo,
                adicionalPeso,
                tipoTrabalho,
                descricaoValor);

            return layoutServicoMecanicaManual;
        }

        public static LayoutMinimoParaMargem NovoLayoutMinimoParaMargem(
            int modeloId,
            int linha,
            string descricao,
            int servicoId,
            decimal valorMargemDireita,
            decimal valorMargemEsquerda,
            decimal valorEntreMargens,
            int linhaReferencia,
            string descricaoValor,
            bool ocultar)
        {
            var cabecalho = new Cabecalho(modeloId, linha, descricao, TipoRegistro.MINIMO_PARA_MARGEM, ocultar);

            return new LayoutMinimoParaMargem(
               cabecalho,
               servicoId,
               valorMargemDireita,
               valorMargemEsquerda,
               valorEntreMargens,
               linhaReferencia,
               descricaoValor);
        }

        public static LayoutServicoParaMargem NovoLayoutServicoParaMargem(
            int modeloId,
            int linha,
            string descricao,
            int servicoId,
            BaseCalculo baseCalculo,
            TipoCarga tipoCarga,
            decimal valorMargemDireita,
            decimal valorMargemEsquerda,
            decimal valorEntreMargens,
            decimal adicionalIMO,
            decimal exercito,
            Moeda moeda,
            decimal pesoMaximo,
            decimal adicionalPeso,
            string descricaoValor,
            int tipoDocumentoId,
            BaseCalculoExcesso baseExcesso,
            decimal valorExcesso,
            decimal pesoLimite,
            bool proRata,
            bool ocultar)
        {
            var cabecalho = new Cabecalho(modeloId, linha, descricao, TipoRegistro.SERVIÇO_PARA_MARGEM, ocultar);

            return new LayoutServicoParaMargem(
                cabecalho,
                servicoId,
                baseCalculo,
                tipoCarga,
                valorMargemDireita,
                valorMargemEsquerda,
                valorEntreMargens,
                adicionalIMO,
                exercito,
                moeda,
                pesoMaximo,
                adicionalPeso,
                descricaoValor,
                tipoDocumentoId,
                baseExcesso,
                valorExcesso,
                pesoLimite,
                proRata);
        }

        public static LayoutArmazenagemAllIn NovoLayoutArmazenagemAllIn(
            int modeloId,
            int linha,
            string descricao,
            decimal valorMinimo,
            decimal valor20,
            decimal valor40,
            decimal cifMinimo,
            decimal cifMaximo,
            string descricaoCif,
            int servicoId,
            BaseCalculo baseCalculo,
            int periodo,
            string descricaoPeriodo,
            Margem margem,
            Moeda moeda,
            string descricaoValor,
            int tipoDocumentoId,
            BaseCalculoExcesso baseExcesso,
            decimal valorExcesso,
            decimal adicionalPeso,
            decimal pesoLimite,
            bool proRata,
            bool ocultar)
        {
            var cabecalho = new Cabecalho(modeloId, linha, descricao, TipoRegistro.ARMAZENAMEM_ALL_IN, ocultar);

            var layoutArmazenagemAllIn = new LayoutArmazenagemAllIn(
                cabecalho,
                servicoId,
                baseCalculo,
                periodo,
                descricaoPeriodo,
                cifMinimo,
                cifMaximo,
                descricaoCif,
                margem,
                valor20,
                valor40,
                valorMinimo,
                moeda,
                descricaoValor,
                tipoDocumentoId,
                baseExcesso,
                valorExcesso,
                adicionalPeso,
                pesoLimite,
                proRata);

            return layoutArmazenagemAllIn;
        }

        public static LayoutArmazenagemMinimo NovoLayoutArmazenagemMinimo(
            int modeloId,
            int linha,
            string descricao,
            decimal valor,
            decimal valor20,
            decimal valor40,
            TipoCarga tipoCarga,
            BaseCalculo baseCalculo,
            Margem margem,
            int servicoId,
            int linhaReferencia,
            string descricaoValor,
            int limiteBls,
            bool ocultar)
        {
            var cabecalho = new Cabecalho(modeloId, linha, descricao, TipoRegistro.ARMAZENAGEM_MINIMO, ocultar);

            var valorCarga = new ValorCargaMinimo(
                valor,
                valor20,
                valor40,
                tipoCarga,
                baseCalculo);

            var layoutArmazenagemMinimo = new LayoutArmazenagemMinimo(
                cabecalho,
                servicoId,
                baseCalculo,
                valorCarga,
                margem,
                linhaReferencia,
                limiteBls,
                descricaoValor);

            return layoutArmazenagemMinimo;
        }

        public static LayoutArmazenagemMinimoCIF NovoLayoutArmazenagemMinimoCIF(
           int modeloId,
           int linha,
           string descricao,
           decimal valorCIF,
           decimal valor,
           decimal valor20,
           decimal valor40,
           TipoCarga tipoCarga,
           int servicoId,
           int linhaReferencia,
           string descricaoValor,
           int limiteBls,
           bool ocultar)
        {
            var cabecalho = new Cabecalho(modeloId, linha, descricao, TipoRegistro.ARMAZENAGEM_MINIMO_CIF, ocultar);

            var valorCarga = new ValorCargaMinimo(
                valor,
                valor20,
                valor40,
                tipoCarga,0);

            var layoutArmazenagemMinimoCIF = new LayoutArmazenagemMinimoCIF(
                cabecalho,
                servicoId,
                valorCarga,
                valorCIF,
                linhaReferencia,
                limiteBls,
                descricaoValor);

            return layoutArmazenagemMinimoCIF;
        }

        public static LayoutArmazenagem NovoLayoutArmazenagem(
            int modeloId,
            int linha,
            string descricao,
            decimal valor,
            decimal valor20,
            decimal valor40,
            TipoCarga tipoCarga,
            int servicoId,
            BaseCalculo baseCalculo,
            int qtdeDias,
            decimal adicionalArmazenagem,
            decimal adicionalGRC,
            decimal minimoGRC,
            decimal adicionalIMO,
            decimal exercito,
            decimal adicionalIMOGRC,
            decimal valorANVISA,
            decimal anvisaGRC,
            int periodo,
            Moeda moeda,
            string descricaoValor,
            int tipoDocumentoId,
            BaseCalculoExcesso baseExcesso,
            Margem margem,
            decimal valorExcesso,
            decimal adicionalPeso,
            decimal pesoLimite,
            int grupoAtracacaoId,
            bool proRata,
            bool ocultar)
        {
            var cabecalho = new Cabecalho(modeloId, linha, descricao, TipoRegistro.ARMAZENAGEM, ocultar);

            var valorArmazenagem = new ValorCarga(
                valor,
                valor20,
                valor40,
                tipoCarga);

            var layoutArmazenagem = new LayoutArmazenagem(
                cabecalho,
                servicoId,
                baseCalculo,
                qtdeDias,
                valorArmazenagem,
                adicionalArmazenagem,
                adicionalGRC,
                minimoGRC,
                adicionalIMO,
                exercito,
                adicionalIMOGRC,
                valorANVISA,
                anvisaGRC,
                periodo,
                moeda,
                descricaoValor, 
                tipoDocumentoId, 
                baseExcesso,
                margem,
                valorExcesso, 
                adicionalPeso, 
                pesoLimite,
                grupoAtracacaoId,
                proRata);

            return layoutArmazenagem;
        }

        public static LayoutArmazenagemCIF NovoLayoutArmazenagemCIF(
            int modeloId,
            int linha,
            string descricao,
            decimal valorCIF,
            decimal valor,
            decimal valor20,
            decimal valor40,
            TipoCarga tipoCarga,
            int servicoId,
            BaseCalculo baseCalculo,
            int qtdeDias,
            decimal adicionalArmazenagem,
            decimal adicionalGRC,
            decimal minimoGRC,
            decimal adicionalIMO,
            decimal exercito,
            decimal adicionalIMOGRC,
            decimal valorANVISA,
            decimal anvisaGRC,
            int periodo,
            Moeda moeda,
            string descricaoValor,
            int tipoDocumentoId,
            BaseCalculoExcesso baseExcesso,
            Margem margem,
            decimal valorExcesso,
            decimal adicionalPeso,
            decimal pesoLimite,
            bool proRata,
            bool ocultar)
        {
            var cabecalho = new Cabecalho(modeloId, linha, descricao, TipoRegistro.ARMAZENAGEM_CIF, ocultar);

            var valorArmazenagem = new ValorCarga(
                valor,
                valor20,
                valor40,
                tipoCarga);

            var layoutArmazenagemCIF = new LayoutArmazenagemCIF(
                cabecalho,
                servicoId,
                baseCalculo,
                valorCIF,
                valorArmazenagem,
                qtdeDias,
                adicionalArmazenagem,
                adicionalGRC,
                minimoGRC,
                adicionalIMO,
                exercito,
                adicionalIMOGRC,
                valorANVISA,
                anvisaGRC,
                periodo,
                moeda,
                descricaoValor,
                tipoDocumentoId,
                baseExcesso,
                margem,
                valorExcesso,
                adicionalPeso,
                pesoLimite,
                proRata);

            return layoutArmazenagemCIF;
        }

        public static LayoutPeriodoPadrao NovoLayoutPeriodoPadrao(
           int modeloId,
           int linha,
           string descricao,
           decimal valor,
           decimal valor20,
           decimal valor40,
           TipoCarga tipoCarga,
           int servicoId,
           BaseCalculo baseCalculo,
           int qtdeDias,   
           int periodo,
           string descricaoValor,
           bool ocultar)
        {
            var cabecalho = new Cabecalho(modeloId, linha, descricao, TipoRegistro.PERIODO_PADRAO, ocultar);

            var valorPeriodoPadrao = new ValorCarga(
                valor,
                valor20,
                valor40,
                tipoCarga);

            var layoutPeriodoPadrao = new LayoutPeriodoPadrao(cabecalho, servicoId, baseCalculo, qtdeDias, valorPeriodoPadrao, periodo, descricaoValor);

            return layoutPeriodoPadrao;
        }
    }
}
