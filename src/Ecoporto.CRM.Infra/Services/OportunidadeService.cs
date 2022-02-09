using Dapper;
using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Interfaces.Servicos;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Linq;

namespace Ecoporto.CRM.Infra.Services
{
    public sealed class OportunidadeService : IOportunidadeService
    {
        public void ImportarLayoutNaOportunidade(int id, int modeloId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Open();

                using (var transaction = con.BeginTransaction())
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "OportunidadeId", value: id, direction: ParameterDirection.Input);
                    parametros.Add(name: "ModeloId", value: modeloId, direction: ParameterDirection.Input);

                    con.Execute("DELETE FROM TB_CRM_OPORTUNIDADE_L_CIF_BL WHERE OportunidadeLayoutId = :OportunidadeId", parametros, transaction);
                    con.Execute("DELETE FROM TB_CRM_OPORTUNIDADE_L_MIN_BL WHERE OportunidadeLayoutId = :OportunidadeId", parametros, transaction);
                    con.Execute("DELETE FROM TB_CRM_OPORTUNIDADE_VL_PESO WHERE OportunidadeLayoutId = :OportunidadeId", parametros, transaction);
                    con.Execute("DELETE FROM TB_CRM_OPORTUNIDADE_VL_VOLUME WHERE OportunidadeLayoutId = :OportunidadeId", parametros, transaction);
                    con.Execute("DELETE FROM CRM.TB_CRM_OPORTUNIDADE_LAYOUT WHERE OportunidadeId = :OportunidadeId", parametros, transaction);

                    con.Query(@"
                        INSERT INTO CRM.TB_CRM_OPORTUNIDADE_LAYOUT (
                            Id,
                            OportunidadeId,
                            ModeloId,
                            ServicoId,
                            Linha,
                            Ocultar,
                            Descricao,
                            TipoRegistro,
                            BaseCalculo,
                            QtdeDias,
                            Periodo,
                            Valor20,
                            Valor40,
                            AdicionalArmazenagem,
                            AdicionalGRC,
                            MinimoGRC,
                            ValorANVISA,
                            Moeda,
                            ValorMinimo20,
                            ValorMinimo40,
                            ValorMargemDireita,
                            ValorMargemEsquerda,
                            ValorEntreMargens,
                            AdicionalIMO,
                            PesoMaximo,
                            AdicionalPeso,
                            ValorMinimoMargemDireita,
                            ValorMinimoMargemEsquerda,
                            ValorMinimoEntreMargens,
                            Reembolso,
                            ValorMinimo,
                            Origem,
                            Destino,
                            Valor,
                            ValorCif,
                            CondicoesGerais,
                            CondicoesIniciais,
                            TipoTrabalho,
                            LinhaReferencia,
                            DescricaoValor,
                            TipoCarga,
                            TipoDocumentoId, 
                            BaseExcesso, 
                            ValorExcesso, 
                            PesoLimite, 
                            GrupoAtracacaoId, 
                            FormaPagamentoNVOCC, 
                            ProRata,
                            LimiteBls,
                            AnvisaGRC,
                            AdicionalIMOGRC,
                            Margem,
                            Exercito
                        )
                        SELECT
                            CRM.SEQ_CRM_OPORTUNIDADE_LAYOUT.NEXTVAL,
                            :OportunidadeId,
                            ModeloId,
                            ServicoId,
                            Linha,
                            Ocultar,
                            Descricao,
                            TipoRegistro,
                            BaseCalculo,
                            QtdeDias,
                            Periodo,
                            Valor20,
                            Valor40,
                            AdicionalArmazenagem,
                            AdicionalGRC,
                            MinimoGRC,
                            ValorANVISA,
                            Moeda,
                            ValorMinimo20,
                            ValorMinimo40,
                            ValorMargemDireita,
                            ValorMargemEsquerda,
                            ValorEntreMargens,
                            AdicionalIMO,
                            PesoMaximo,
                            AdicionalPeso,
                            ValorMinimoMargemDireita,
                            ValorMinimoMargemEsquerda,
                            ValorMinimoEntreMargens,
                            Reembolso,
                            ValorMinimo,
                            Origem,
                            Destino,
                            Valor,
                            ValorCif,
                            CondicoesGerais,
                            CondicoesIniciais,
                            TipoTrabalho,
                            LinhaReferencia,
                            DescricaoValor,
                            TipoCarga,
                            TipoDocumentoId, 
                            BaseExcesso, 
                            ValorExcesso, 
                            PesoLimite, 
                            GrupoAtracacaoId, 
                            FormaPagamentoNVOCC, 
                            ProRata,
                            LimiteBls,
                            AnvisaGRC,
                            AdicionalIMOGRC,
                            Margem,
                            Exercito
                        FROM 
                            CRM.TB_CRM_LAYOUT
                        WHERE 
                            ModeloId = :ModeloId", parametros, transaction);

                    con.Query(@"
                        INSERT INTO
                            CRM.TB_CRM_OPORTUNIDADE_L_CIF_BL
                                (
                                    Id,
                                    OportunidadeLayoutId,
                                    Minimo,
                                    Maximo,
                                    Margem,
                                    Valor20,
                                    Valor40,
                                    Descricao,
                                    Periodo,
                                    DescricaoPeriodo,
                                    ValorMinimo20,
                                    ValorMinimo40,
                                    DescricaoValorMinimo
                                ) 
                            SELECT
                                CRM.SEQ_CRM_LAYOUT_VL_CIF_BL.NEXTVAL,
                                :OportunidadeId,
                                A.Minimo,
                                A.Maximo,
                                A.Margem,
                                A.Valor20,
                                A.Valor40,
                                A.Descricao,
                                A.Periodo,
                                A.DescricaoPeriodo,
                                A.ValorMinimo20,
                                A.ValorMinimo40,
                                A.DescricaoValorMinimo
                            FROM
                                CRM.TB_CRM_LAYOUT_VL_CIF_BL A
                            INNER JOIN
                                CRM.TB_CRM_LAYOUT B ON A.LayoutId = B.Id
                            WHERE
                                B.ModeloId = :ModeloId", parametros, transaction);

                    con.Query(@"
                        INSERT INTO
                            CRM.TB_CRM_OPORTUNIDADE_L_MIN_BL
                                (
                                    Id,
                                    OportunidadeLayoutId,
                                    BLMinimo,
                                    BLMaximo,
                                    ValorMinimo        
                                )
                            SELECT
                                CRM.SEQ_CRM_LAYOUT_VL_MINIMO_BL.NEXTVAL,
                                :OportunidadeId,
                                A.BLMinimo,
                                A.BLMaximo,
                                A.ValorMinimo
                            FROM
                                CRM.TB_CRM_LAYOUT_VL_MINIMO_BL A
                            INNER JOIN
                                CRM.TB_CRM_LAYOUT B ON A.LayoutId = B.Id
                            WHERE
                                B.ModeloId = :ModeloId", parametros, transaction);

                    con.Query(@"
                       INSERT INTO
                            CRM.TB_CRM_OPORTUNIDADE_VL_PESO
                                (
                                    Id,
                                    OportunidadeLayoutId,
                                    ValorInicial,
                                    ValorFinal,
                                    Preco
                                )
                            SELECT
                                CRM.SEQ_CRM_LAYOUT_VL_PESO.NEXTVAL,
                                :OportunidadeId,
                                A.ValorInicial,
                                A.ValorFinal,
                                A.Preco
                            FROM
                                CRM.TB_CRM_LAYOUT_VL_PESO A
                            INNER JOIN
                                CRM.TB_CRM_LAYOUT B ON A.LayoutId = B.Id
                            WHERE
                                B.ModeloId = :ModeloId", parametros, transaction);

                    con.Query(@"
                       INSERT INTO
                            CRM.TB_CRM_OPORTUNIDADE_VL_VOLUME
                                (
                                    Id,
                                    OportunidadeLayoutId,
                                    ValorInicial,
                                    ValorFinal,
                                    Preco
                                )
                            SELECT
                                CRM.SEQ_CRM_LAYOUT_VL_VOLUME.NEXTVAL,
                                :OportunidadeId,
                                A.ValorInicial,
                                A.ValorFinal,
                                A.Preco
                            FROM
                                CRM.TB_CRM_LAYOUT_VL_VOLUME A
                            INNER JOIN
                                CRM.TB_CRM_LAYOUT B ON A.LayoutId = B.Id
                            WHERE
                                B.ModeloId = :ModeloId", parametros, transaction);

                    transaction.Commit();
                }
            }
        }

        public int ClonarOportunidade(ClonarOportunidadeDTO clone, int contaId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Open();

                using (var transaction = con.BeginTransaction())
                {
                    var parametros = new DynamicParameters();

                    var sequencia = con.Query<int>(@"SELECT CRM.SEQ_CRM_OPORTUNIDADES.NEXTVAL FROM DUAL").Single();

                    parametros.Add(name: "Sequencia", value: sequencia, direction: ParameterDirection.Input);
                    parametros.Add(name: "CriadoPor", value: clone.CriadoPor, direction: ParameterDirection.Input);
                    parametros.Add(name: "Descricao", value: clone.Descricao, direction: ParameterDirection.Input);
                    parametros.Add(name: "OportunidadeOriginal", value: clone.OportunidadeOriginal, direction: ParameterDirection.Input);
                    parametros.Add(name: "ContaId", value: (contaId > 0 ? contaId : 0), direction: ParameterDirection.Input);

                    var oportunidadeOriginalBusca = con.Query<Oportunidade>("SELECT Descricao, Identificacao, TabelaId FROM TB_CRM_OPORTUNIDADES WHERE Id = :OportunidadeOriginal", parametros).FirstOrDefault();

                    if (oportunidadeOriginalBusca == null)
                        throw new Exception("Oportunidade não encontrada ou já excluída");

                    if (oportunidadeOriginalBusca != null)
                    {
                        if (oportunidadeOriginalBusca.TabelaId > 0)                        
                            parametros.Add(name: "OrigemClone", value: $"Clone Oportunidade {oportunidadeOriginalBusca.Identificacao} Tabela ID {oportunidadeOriginalBusca.TabelaId}", direction: ParameterDirection.Input);
                        else                        
                            parametros.Add(name: "OrigemClone", value: $"Clone Oportunidade {oportunidadeOriginalBusca.Identificacao}", direction: ParameterDirection.Input);
                    }
                    
                    con.Execute(@"
                        INSERT INTO
                            CRM.TB_CRM_OPORTUNIDADES
                                (
                                    Id,
                                    Identificacao, 
                                    ContaId, 
                                    Aprovada, 
                                    Descricao,  
                                    DataFechamento, 
                                    ContatoId, 
                                    DataInicio, 
                                    DataTermino, 
                                    ClassificacaoCliente, 
                                    Probabilidade, 
                                    Segmento, 
                                    RevisaoId, 
                                    TipoOperacaoOportunidade, 
                                    TipoNegocio, 
                                    TipoDeProposta, 
                                    TipoServico, 
                                    MotivoPerda, 
                                    StatusOportunidade, 
                                    PremioParceria, 
                                    FaturamentoMensalLCL, 
                                    FaturamentoMensalFCL, 
                                    VolumeMensal, 
                                    CIFMedio, 
                                    MercadoriaId, 
                                    Observacao,
                                    ModeloId, 
                                    TipoOperacao, 
                                    FormaPagamento, 
                                    DiasFreeTime, 
                                    VendedorId, 
                                    QtdeDias, 
                                    Validade, 
                                    TipoValidade, 
                                    ImpostoId,
                                    CriadoPor,
                                    DataCriacao,
                                    Acordo,
                                    HubPort, 
                                    CobrancaEspecial, 
                                    DesovaParcial, 
                                    FatorCP, 
                                    PosicIsento,
                                    EmpresaId,
                                    OrigemClone
                                ) 
                                    SELECT
                                        :Sequencia,
                                        CRM.SEQ_CRM_OPORTUNIDADE_IDENT.NEXTVAL, 
                                        :ContaId, 
                                        0, 
                                        :Descricao, 
                                        NULL, 
                                        0, 
                                        NULL, 
                                        NULL, 
                                        ClassificacaoCliente, 
                                        Probabilidade, 
                                        Segmento, 
                                        RevisaoId, 
                                        TipoOperacaoOportunidade, 
                                        TipoNegocio, 
                                        TipoDeProposta, 
                                        TipoServico, 
                                        MotivoPerda, 
                                        0, 
                                        PremioParceria, 
                                        FaturamentoMensalLCL, 
                                        FaturamentoMensalFCL, 
                                        VolumeMensal, 
                                        CIFMedio, 
                                        MercadoriaId, 
                                        Observacao,
                                        ModeloId, 
                                        TipoOperacao, 
                                        FormaPagamento, 
                                        DiasFreeTime, 
                                        VendedorId, 
                                        QtdeDias, 
                                        Validade, 
                                        TipoValidade, 
                                        ImpostoId,
                                        :CriadoPor,
                                        SYSDATE,
                                        Acordo,
                                        HubPort, 
                                        CobrancaEspecial, 
                                        DesovaParcial, 
                                        FatorCP, 
                                        PosicIsento,
                                        EmpresaId,
                                        :OrigemClone
                                    FROM
                                        CRM.TB_CRM_OPORTUNIDADES
                                    WHERE
                                        Id = :OportunidadeOriginal", parametros);

                    if (clone.SubClientesSelecionados.Length > 0)
                    {
                        parametros = new DynamicParameters();

                        parametros.Add(name: "OportunidadeId", value: sequencia, direction: ParameterDirection.Input);
                        parametros.Add(name: "OportunidadeOriginal", value: clone.OportunidadeOriginal, direction: ParameterDirection.Input);
                        parametros.Add(name: "CriadoPor", value: clone.CriadoPor, direction: ParameterDirection.Input);
                        parametros.Add(name: "SubClientesSelecionados", value: clone.SubClientesSelecionados, direction: ParameterDirection.Input);

                        con.Execute(@"
                            INSERT INTO 
                                CRM.TB_CRM_OPORTUNIDADE_CLIENTES 
                                    (
                                        Id, 
                                        Segmento,
                                        ContaId, 
                                        OportunidadeId,
                                        CriadoPor
                                    ) 
                                    SELECT
                                        CRM.SEQ_CRM_OPORTUNIDADE_CLIENTES.NEXTVAL,
                                        Segmento,
                                        ContaId,
                                        :OportunidadeId,
                                        :CriadoPor
                                    FROM
                                        CRM.TB_CRM_OPORTUNIDADE_CLIENTES 
                                    WHERE
                                        OportunidadeId = :OportunidadeOriginal
                                    AND
                                        Id IN :SubClientesSelecionados", parametros);
                    }

                    if (clone.ClientesGrupoCnpjSelecionados.Length > 0)
                    {
                        parametros = new DynamicParameters();

                        parametros.Add(name: "OportunidadeId", value: sequencia, direction: ParameterDirection.Input);
                        parametros.Add(name: "OportunidadeOriginal", value: clone.OportunidadeOriginal, direction: ParameterDirection.Input);
                        parametros.Add(name: "CriadoPor", value: clone.CriadoPor, direction: ParameterDirection.Input);
                        parametros.Add(name: "GrupoCnpjSelecionados", value: clone.ClientesGrupoCnpjSelecionados, direction: ParameterDirection.Input);

                        con.Execute(@"
                            INSERT INTO 
                                CRM.TB_CRM_OPORTUNIDADE_GRUPO_CNPJ 
                                    (
                                        Id, 
                                        ContaId, 
                                        OportunidadeId,
                                        CriadoPor
                                    ) 
                                    SELECT
                                        CRM.SEQ_CRM_OPORTUNIDADE_GRUPOCNPJ.NEXTVAL,
                                        ContaId,
                                        :OportunidadeId,
                                        :CriadoPor
                                    FROM
                                        CRM.TB_CRM_OPORTUNIDADE_GRUPO_CNPJ 
                                    WHERE
                                        OportunidadeId = :OportunidadeOriginal
                                    AND
                                        Id IN :GrupoCnpjSelecionados", parametros);
                    }

                    parametros = new DynamicParameters();
                    parametros.Add(name: "OportunidadeId", value: sequencia, direction: ParameterDirection.Input);
                    parametros.Add(name: "OportunidadeOriginal", value: clone.OportunidadeOriginal, direction: ParameterDirection.Input);

                    con.Execute(@"
                        INSERT INTO TB_CRM_OPORTUNIDADE_LAYOUT (
                            Id,
                            OportunidadeId,
                            ModeloId,
                            ServicoId,
                            Linha,
                            Descricao,
                            TipoRegistro,
                            BaseCalculo,
                            QtdeDias,
                            Periodo,
                            Valor20,
                            Valor40,
                            AdicionalArmazenagem,
                            AdicionalGRC,
                            MinimoGRC,
                            ValorANVISA,
                            Moeda,
                            ValorMinimo20,
                            ValorMinimo40,
                            ValorMargemDireita,
                            ValorMargemEsquerda,
                            ValorEntreMargens,
                            AdicionalIMO,
                            PesoMaximo,
                            AdicionalPeso,
                            ValorMinimoMargemDireita,
                            ValorMinimoMargemEsquerda,
                            ValorMinimoEntreMargens,
                            Reembolso,
                            ValorMinimo,
                            Origem,
                            Destino,
                            Valor,
                            ValorCif,
                            CondicoesGerais,
                            CondicoesIniciais,
                            TipoTrabalho,
                            LinhaReferencia,
                            DescricaoValor,
                            TipoCarga,
                            TipoDocumentoId, 
                            BaseExcesso, 
                            ValorExcesso, 
                            PesoLimite, 
                            GrupoAtracacaoId, 
                            FormaPagamentoNVOCC, 
                            ProRata,
                            LimiteBls,
                            AnvisaGRC,
                            AdicionalIMOGRC,
                            Ocultar,
                            Margem,
                            Exercito
                        )
                        SELECT
                            CRM.SEQ_CRM_OPORTUNIDADE_LAYOUT.NEXTVAL,
                            :OportunidadeId,
                            ModeloId,
                            ServicoId,
                            Linha,
                            Descricao,
                            TipoRegistro,
                            BaseCalculo,
                            QtdeDias,
                            Periodo,
                            Valor20,
                            Valor40,
                            AdicionalArmazenagem,
                            AdicionalGRC,
                            MinimoGRC,
                            ValorANVISA,
                            Moeda,
                            ValorMinimo20,
                            ValorMinimo40,
                            ValorMargemDireita,
                            ValorMargemEsquerda,
                            ValorEntreMargens,
                            AdicionalIMO,
                            PesoMaximo,
                            AdicionalPeso,
                            ValorMinimoMargemDireita,
                            ValorMinimoMargemEsquerda,
                            ValorMinimoEntreMargens,
                            Reembolso,
                            ValorMinimo,
                            Origem,
                            Destino,
                            Valor,
                            ValorCif,
                            CondicoesGerais,
                            CondicoesIniciais,
                            TipoTrabalho,
                            LinhaReferencia,
                            DescricaoValor,
                            TipoCarga,
                            TipoDocumentoId, 
                            BaseExcesso, 
                            ValorExcesso, 
                            PesoLimite, 
                            GrupoAtracacaoId, 
                            FormaPagamentoNVOCC, 
                            ProRata,
                            LimiteBls,
                            AnvisaGRC,
                            AdicionalIMOGRC,
                            Ocultar,
                            Margem,
                            Exercito
                        FROM 
                            CRM.TB_CRM_OPORTUNIDADE_LAYOUT
                        WHERE 
                            OportunidadeId = :OportunidadeOriginal", parametros);

                    con.Execute(@"
                             INSERT INTO
                                CRM.TB_CRM_OPORTUNIDADES_IMPOSTOS
                                    (
                                        ID,
                                        ModeloId,
                                        OportunidadeId,
                                        ServicoId,
                                        PIS,
                                        ISS,
                                        COFINS,
                                        VALORPIS,
                                        VALORISS,
                                        VALORCOFINS,
                                        TIPO
                                    )
                                SELECT
                                    CRM.SEQ_CRM_OPORTUNIDADES_IMPOSTOS.NEXTVAL,
                                    ModeloId,
                                    :OportunidadeId,
                                    ServicoId,
                                    PIS,
                                    ISS,
                                    COFINS,
                                    VALORPIS,
                                    VALORISS,
                                    VALORCOFINS,
                                    TIPO
                                FROM
                                   CRM.TB_CRM_OPORTUNIDADES_IMPOSTOS
                                WHERE
                                    OportunidadeId = :OportunidadeOriginal", parametros, transaction);

                    if (clone.FormaPagamentoOportunidade == FormaPagamento.FATURADO)
                    {
                        parametros.Add(name: "StatusFichaFaturamento", value: StatusFichaFaturamento.EM_ANDAMENTO, direction: ParameterDirection.Input);

                        con.Execute(@"
                            INSERT INTO 
                                TB_CRM_OPORTUNIDADE_FICHA_FAT
                                    (
                                        ID,
                                        OPORTUNIDADEID,
                                        CONTAID,
                                        FATURADOCONTRAID,
                                        DIASSEMANA,
                                        DIASFATURAMENTO,
                                        DATACORTE,
                                        CONDICAOPAGAMENTOFATURAMENTOID,
                                        EMAILFATURAMENTO,
                                        OBSERVACOESFATURAMENTO,
                                        STATUSFICHAFATURAMENTO,
                                        CONDICAOPGTODIA,
                                        CONDICAOPGTODIASEMANA,
                                        ENTREGAMANUAL,
                                        CORREIOSSEDEX,
                                        DIAUTIL,
                                        ULTIMODIADOMES,
                                        ENTREGAELETRONICA,
                                        FONTEPAGADORAID
                                    ) SELECT
                                        SEQ_CRM_OPORTUNIDADE_FICHA_FAT.NEXTVAL,
                                        :OportunidadeId,
                                        CONTAID,
                                        FATURADOCONTRAID,
                                        DIASSEMANA,
                                        DIASFATURAMENTO,
                                        DATACORTE,
                                        CONDICAOPAGAMENTOFATURAMENTOID,
                                        EMAILFATURAMENTO,
                                        OBSERVACOESFATURAMENTO,
                                        :StatusFichaFaturamento,
                                        CONDICAOPGTODIA,
                                        CONDICAOPGTODIASEMANA,
                                        ENTREGAMANUAL,
                                        CORREIOSSEDEX,
                                        DIAUTIL,
                                        ULTIMODIADOMES,
                                        ENTREGAELETRONICA,
                                        FONTEPAGADORAID
                                    FROM 
                                        TB_CRM_OPORTUNIDADE_FICHA_FAT
                                    WHERE
                                        OportunidadeId = :OportunidadeOriginal
                                    AND
                                        (StatusFichaFaturamento <> 4 AND StatusFichaFaturamento <> 6)", parametros, transaction);
                    }                    

                    transaction.Commit();

                    return sequencia;
                }
            }
        }

        public void ClonarProposta(int oportunidadeOrigemId, int oportunidadeDestinoId, FormaPagamento formaPagamento)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                con.Open();

                using (var transaction = con.BeginTransaction())
                {
                    var parametros = new DynamicParameters();

                    parametros.Add(name: "OportunidadeOrigemId", value: oportunidadeOrigemId, direction: ParameterDirection.Input);
                    parametros.Add(name: "OportunidadeDestinoId", value: oportunidadeDestinoId, direction: ParameterDirection.Input);                    

                    con.Execute(@"DELETE FROM TB_CRM_OPORTUNIDADE_LAYOUT WHERE OportunidadeId = :OportunidadeDestinoId", parametros, transaction);

                    var oportunidadeOriginalBusca = con.Query<Oportunidade>("SELECT Descricao, Identificacao, TabelaId FROM TB_CRM_OPORTUNIDADES WHERE Id = :OportunidadeOrigemId", parametros).FirstOrDefault();

                    if (oportunidadeOriginalBusca == null)
                        throw new Exception("Oportunidade não encontrada ou já excluída");

                    if (oportunidadeOriginalBusca != null)
                    {
                        if (oportunidadeOriginalBusca.TabelaId > 0)
                            parametros.Add(name: "OrigemClone", value: $"Clone proposta {oportunidadeOriginalBusca.Identificacao} Tabela ID {oportunidadeOriginalBusca.TabelaId}", direction: ParameterDirection.Input);
                        else
                            parametros.Add(name: "OrigemClone", value: $"Clone proposta {oportunidadeOriginalBusca.Identificacao}", direction: ParameterDirection.Input);
                    }

                    con.Execute(@"UPDATE TB_CRM_OPORTUNIDADES SET OrigemClone = :OrigemClone WHERE Id = :OportunidadeDestinoId", parametros, transaction);

                    con.Execute(@"UPDATE TB_CRM_OPORTUNIDADES SET ImpostoId = (SELECT ImpostoId FROM TB_CRM_OPORTUNIDADES WHERE Id = :OportunidadeOrigemId) WHERE Id = :OportunidadeDestinoId", parametros, transaction);

                    con.Execute(@"
                        INSERT INTO 
                            CRM.TB_CRM_OPORTUNIDADE_LAYOUT
                                (
                                    ID,
                                    OPORTUNIDADEID,
                                    MODELOID,
                                    SERVICOID,
                                    LINHA,
                                    DESCRICAO,
                                    TIPOREGISTRO,
                                    BASECALCULO,
                                    PERIODO,
                                    VALOR20,
                                    VALOR40,
                                    ADICIONALARMAZENAGEM,
                                    ADICIONALGRC,
                                    MINIMOGRC,
                                    VALORANVISA,
                                    MOEDA,
                                    VALORMINIMO20,
                                    VALORMINIMO40,
                                    VALORMARGEMDIREITA,
                                    VALORMARGEMESQUERDA,
                                    VALORENTREMARGENS,
                                    ADICIONALIMO,
                                    PESOMAXIMO,
                                    ADICIONALPESO,
                                    VALORMINIMOMARGEMDIREITA,
                                    VALORMINIMOMARGEMESQUERDA,
                                    VALORMINIMOENTREMARGENS,
                                    REEMBOLSO,
                                    VALORMINIMO,
                                    ORIGEM,
                                    DESTINO,
                                    VALOR,
                                    CONDICOESINICIAIS,
                                    CONDICOESGERAIS,
                                    TIPOTRABALHO,
                                    LINHAREFERENCIA,
                                    DESCRICAOVALOR,
                                    TIPOCARGA,
                                    MARGEM,
                                    QTDEDIAS,
                                    DESCRICAOPERIODO,
                                    DESCRICAOCIF,
                                    CIFMINIMO,
                                    CIFMAXIMO,
                                    TIPODOCUMENTOID, 
                                    BASEEXCESSO, 
                                    VALOREXCESSO, 
                                    PESOLIMITE, 
                                    GRUPOATRACACAOID, 
                                    FORMAPAGAMENTONVOCC, 
                                    PRORATA,
                                    LIMITEBLS,
                                    ANVISAGRC,
                                    ADICIONALIMOGRC,
                                    OCULTAR,
                                    VALORCIF,
                                    EXERCITO
                                )
                        SELECT        
                            CRM.SEQ_CRM_OPORTUNIDADE_LAYOUT.NEXTVAL,
                            :OportunidadeDestinoId,
                            MODELOID,
                            SERVICOID,
                            LINHA,
                            DESCRICAO,
                            TIPOREGISTRO,
                            BASECALCULO,
                            PERIODO,
                            VALOR20,
                            VALOR40,
                            ADICIONALARMAZENAGEM,
                            ADICIONALGRC,
                            MINIMOGRC,
                            VALORANVISA,
                            MOEDA,
                            VALORMINIMO20,
                            VALORMINIMO40,
                            VALORMARGEMDIREITA,
                            VALORMARGEMESQUERDA,
                            VALORENTREMARGENS,
                            ADICIONALIMO,
                            PESOMAXIMO,
                            ADICIONALPESO,
                            VALORMINIMOMARGEMDIREITA,
                            VALORMINIMOMARGEMESQUERDA,
                            VALORMINIMOENTREMARGENS,
                            REEMBOLSO,
                            VALORMINIMO,
                            ORIGEM,
                            DESTINO,
                            VALOR,
                            CONDICOESINICIAIS,
                            CONDICOESGERAIS,
                            TIPOTRABALHO,
                            LINHAREFERENCIA,
                            DESCRICAOVALOR,
                            TIPOCARGA,
                            MARGEM,
                            QTDEDIAS,
                            DESCRICAOPERIODO,
                            DESCRICAOCIF,
                            CIFMINIMO,
                            CIFMAXIMO,
                            TIPODOCUMENTOID, 
                            BASEEXCESSO, 
                            VALOREXCESSO, 
                            PESOLIMITE, 
                            GRUPOATRACACAOID, 
                            FORMAPAGAMENTONVOCC, 
                            PRORATA,
                            LIMITEBLS,
                            ANVISAGRC,
                            ADICIONALIMOGRC,
                            OCULTAR,
                            VALORCIF,
                            EXERCITO
                        FROM
                            TB_CRM_OPORTUNIDADE_LAYOUT
                        WHERE
                            OPORTUNIDADEID = :OportunidadeOrigemId", parametros, transaction);

                    con.Execute(@"DELETE FROM CRM.TB_CRM_OPORTUNIDADES_IMPOSTOS WHERE OportunidadeId = :OportunidadeDestinoId", parametros, transaction);

                    con.Execute(@"
                             INSERT INTO
                                CRM.TB_CRM_OPORTUNIDADES_IMPOSTOS
                                    (
                                        ID,
                                        ModeloId,
                                        OportunidadeId,
                                        ServicoId,
                                        PIS,
                                        ISS,
                                        COFINS,
                                        VALORPIS,
                                        VALORISS,
                                        VALORCOFINS,
                                        TIPO
                                    )
                                SELECT
                                    CRM.SEQ_CRM_OPORTUNIDADES_IMPOSTOS.NEXTVAL,
                                    ModeloId,
                                    :OportunidadeDestinoId,
                                    ServicoId,
                                    PIS,
                                    ISS,
                                    COFINS,
                                    VALORPIS,
                                    VALORISS,
                                    VALORCOFINS,
                                    TIPO
                                FROM
                                   CRM.TB_CRM_OPORTUNIDADES_IMPOSTOS
                                WHERE
                                    OportunidadeId = :OportunidadeOrigemId", parametros, transaction);

                    if (formaPagamento == FormaPagamento.FATURADO)
                    {
                        parametros.Add(name: "StatusFichaFaturamento", value: StatusFichaFaturamento.EM_ANDAMENTO, direction: ParameterDirection.Input);

                        con.Execute(@"
                            INSERT INTO 
                                TB_CRM_OPORTUNIDADE_FICHA_FAT
                                    (
                                        ID,
                                        OPORTUNIDADEID,
                                        CONTAID,
                                        FATURADOCONTRAID,
                                        DIASSEMANA,
                                        DIASFATURAMENTO,
                                        DATACORTE,
                                        CONDICAOPAGAMENTOFATURAMENTOID,
                                        EMAILFATURAMENTO,
                                        OBSERVACOESFATURAMENTO,
                                        STATUSFICHAFATURAMENTO,
                                        CONDICAOPGTODIA,
                                        CONDICAOPGTODIASEMANA,
                                        ENTREGAMANUAL,
                                        CORREIOSSEDEX,
                                        DIAUTIL,
                                        ULTIMODIADOMES,
                                        ENTREGAELETRONICA,
                                        FONTEPAGADORAID
                                    ) SELECT
                                        SEQ_CRM_OPORTUNIDADE_FICHA_FAT.NEXTVAL,
                                        :OportunidadeDestinoId,
                                        CONTAID,
                                        FATURADOCONTRAID,
                                        DIASSEMANA,
                                        DIASFATURAMENTO,
                                        DATACORTE,
                                        CONDICAOPAGAMENTOFATURAMENTOID,
                                        EMAILFATURAMENTO,
                                        OBSERVACOESFATURAMENTO,
                                        :StatusFichaFaturamento,
                                        CONDICAOPGTODIA,
                                        CONDICAOPGTODIASEMANA,
                                        ENTREGAMANUAL,
                                        CORREIOSSEDEX,
                                        DIAUTIL,
                                        ULTIMODIADOMES,
                                        ENTREGAELETRONICA,
                                        FONTEPAGADORAID
                                    FROM 
                                        TB_CRM_OPORTUNIDADE_FICHA_FAT
                                    WHERE
                                        OportunidadeId = :OportunidadeOrigemId
                                    AND
                                        (StatusFichaFaturamento <> 4 AND StatusFichaFaturamento <> 6)", parametros, transaction);
                    }                    

                    transaction.Commit();
                }
            }
        }
    }
}
