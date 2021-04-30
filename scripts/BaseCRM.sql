--------------------------------------------------------
--  Arquivo criado - Sexta-feira-Agosto-17-2018   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for View VW_CRM_CONTAS
--------------------------------------------------------

  CREATE OR REPLACE FORCE VIEW "CRM"."VW_CRM_CONTAS" ("ID", "DESCRICAO", "DOCUMENTO", "NOMEFANTASIA", "INSCRICAOESTADUAL", "TELEFONE", "VENDEDOR", "SITUACAOCADASTRAL", "SEGMENTO", "CLASSIFICACAOFISCAL", "LOGRADOURO", "BAIRRO", "ESTADO", "NUMERO", "COMPLEMENTO", "CEP", "CIDADE", "PAIS", "CRIADOPOR") AS 
  SELECT   A.Id,
            A.Descricao || ' (' || A.Documento || ')' AS Descricao,
            A.Documento,
            A.NomeFantasia,
            A.InscricaoEstadual,
            A.Telefone,
            C.Nome AS Vendedor,
            DECODE (A.SituacaoCadastral,
                    1,
                    'Ativo',
                    2,
                    'Baixado')
               AS SituacaoCadastral,
            DECODE (A.Segmento,
                    1,
                    'Importador',
                    2,
                    'Exportador',
                    3,
                    'Despachante',
                    4,
                    'Agente',
                    5,
                    'Freight Forwarder',
                    6,
                    'Coloader')
               AS Segmento,
            DECODE (A.ClassificacaoFiscal,
                    1,
                    'Pessoa Física',
                    2,
                    'Pessoa Jurídica',
                    3,
                    'Externo')
               AS ClassificacaoFiscal,
            A.Logradouro,
            A.Bairro,
            A.Estado,
            A.Numero,
            A.Complemento,
            A.CEP,
            C.Descricao AS Cidade,
            D.Descricao AS Pais,
            B.Login As CriadoPor
     FROM            CRM.TB_CRM_CONTAS A
                  LEFT JOIN
                     CRM.TB_CRM_USUARIOS B
                  ON A.CriadoPor = B.Id
                    LEFT JOIN
                     CRM.TB_CRM_USUARIOS C
                  ON A.VendedorId = C.Id
               LEFT JOIN
                  CRM.TB_CRM_CIDADES C
               ON A.CidadeId = C.Id
            LEFT JOIN
               CRM.TB_CRM_PAISES D
            ON A.PaisId = D.Id
;
--------------------------------------------------------
--  DDL for View VW_CRM_LAYOUTS
--------------------------------------------------------

  CREATE OR REPLACE FORCE VIEW "CRM"."VW_CRM_LAYOUTS" ("ID", "LINHA", "DESCRICAO", "MODELOID", "MODELO", "SERVICO", "BASECALCULO", "QTDEDIAS", "ADICIONALARMAZENAGEM", "ADICIONALGRC", "MINIMOGRC", "ADICIONALIMO", "VALORANVISA", "PERIODO", "MOEDA", "DESCRICAOVALOR", "VALOR", "VALOR20", "VALOR40", "VALORMINIMO", "VALORMINIMO20", "VALORMINIMO40", "TIPOCARGA", "MARGEM", "VALORMARGEMDIREITA", "VALORMARGEMESQUERDA", "VALORENTREMARGENS", "PESOMAXIMO", "ADICIONALPESO", "VALORMINIMOMARGEMDIREITA", "VALORMINIMOMARGEMESQUERDA", "VALORMINIMOENTREMARGENS", "LINHAREFERENCIA", "TIPOTRABALHO", "ORIGEM", "DESTINO", "CONDICOESGERAIS", "CONDICOESINICIAIS") AS 
  SELECT   A.Id,
            A.Linha,
            A.Descricao,
            A.ModeloId,
            B.Descricao AS Modelo,
            C.Descricao AS Servico,
            DECODE (A.BaseCalculo,
                    1,
                    'Unid.',
                    2,
                    'Ton.',
                    3,
                    'CIF',
                    4,
                    'CIFM',
                    5,
                    'CIF0',
                    6,
                    'BL',
                    7,
                    'VOLP')
               AS BaseCalculo,
            A.QtdeDias,
            A.AdicionalArmazenagem,
            A.AdicionalGRC,
            A.MinimoGRC,
            A.AdicionalIMO,
            A.ValorANVISA,
            A.Periodo,
            DECODE (A.Moeda,
                    1,
                    'R$',
                    2,
                    'US$')
               AS Moeda,
            A.DescricaoValor,
            A.Valor,
            A.Valor20,
            A.Valor40,
            A.ValorMinimo,
            A.ValorMinimo20,
            A.ValorMinimo40,
            DECODE (A.TipoCarga,
                    1,
                    'Contêiner',
                    2,
                    'Carga Solta',
                    3,
                    'Mudança de Regime')
               AS TipoCarga,
            DECODE (A.Margem,
                    1,
                    'Direita',
                    2,
                    'Esquerda',
                    3,
                    'Entre Margens')
               AS Margem,
            A.ValorMargemDireita,
            A.ValorMargemEsquerda,
            A.ValorEntreMargens,
            A.PesoMaximo,
            A.AdicionalPeso,
            A.ValorMinimoMargemDireita,
            A.ValorMinimoMargemEsquerda,
            A.ValorMinimoEntreMargens,
            A.LinhaReferencia,
            DECODE (A.TipoTrabalho,
                    1,
                    'Mecanizada',
                    2,
                    'Manual')
               AS TipoTrabalho,
            D.Fantasia AS Origem,
            E.Fantasia AS Destino,
            A.CondicoesGerais,
            A.CondicoesIniciais
     FROM               CRM.TB_CRM_LAYOUT A
                     INNER JOIN
                        CRM.TB_CRM_MODELO B
                     ON A.ModeloId = B.Id
                  LEFT JOIN
                     CRM.TB_CRM_SERVICOS C
                  ON A.ServicoId = C.Id
               LEFT JOIN
                  SGIPA.TB_CAD_PARCEIROS D
               ON A.Origem = D.AUTONUM
            LEFT JOIN
               SGIPA.TB_CAD_PARCEIROS E
            ON A.Destino = E.AUTONUM
;
--------------------------------------------------------
--  DDL for View VW_CRM_MODELOS
--------------------------------------------------------

  CREATE OR REPLACE FORCE VIEW "CRM"."VW_CRM_MODELOS" ("ID", "TIPOOPERACAO", "STATUS", "DESCRICAO", "FORMAPAGAMENTO", "DIASFREETIME", "QTDEDIAS", "VALIDADE", "TIPOVALIDADE") AS 
  SELECT   Id,
            DECODE (TIPOOPERACAO,
                    1,
                    'Recinto Alfandegado',
                    2,
                    'Operador',
                    3,
                    'Redex')
               AS TIPOOPERACAO,
            DECODE (STATUS,
                    0,
                    'Inativo',
                    1,
                    'Ativo')
               AS STATUS,
            DESCRICAO,
            DECODE (FORMAPAGAMENTO,
                    1,
                    'A Vista',
                    2,
                    'Faturado')
               AS FORMAPAGAMENTO,
            DIASFREETIME,
            QTDEDIAS,
            VALIDADE,
            TIPOVALIDADE
     FROM   CRM.TB_CRM_MODELO
;
--------------------------------------------------------
--  DDL for View VW_CRM_OPORTUNIDADES
--------------------------------------------------------

  CREATE OR REPLACE FORCE VIEW "CRM"."VW_CRM_OPORTUNIDADES" ("ID", "IDENTIFICACAO", "DESCRICAO", "STATUSOPORTUNIDADE", "SUCESSONEGOCIACAO", "ESTAGIONEGOCIACAO", "TIPODEPROPOSTA", "TIPOSERVICO", "DATAFECHAMENTO", "DATAINICIO", "DATATERMINO", "CRIADOPOR", "APROVADA", "PROBABILIDADE", "CLASSIFICACAOCLIENTE", "SEGMENTO", "MOTIVOPERDA", "TIPONEGOCIO", "TIPOOPERACAOOPORTUNIDADE", "OBSERVACAO", "FATURAMENTOMENSALLCL", "FATURAMENTOMENSALFCL", "VOLUMEMENSAL", "CIFMEDIO", "PREMIOPARCERIA", "DATACRIACAO", "ALTERADOPOR", "ULTIMAALTERACAO", "TIPOOPERACAO", "FORMAPAGAMENTO", "DIASFREETIME", "QTDEDIAS", "VALIDADE", "TIPOVALIDADE", "CONTA", "CONTADOCUMENTO", "CONTAENDERECO", "CONTACIDADE", "CONTAESTADO", "CONTACEP", "CONTATELEFONE", "CONTATO", "CONTATOEMAIL", "CONTATOTELEFONE", "CONTATODEPARTAMENTO", "CONTATOCARGO", "REVISAO", "MERCADORIA", "MODELO", "VENDEDOR", "IMPOSTO", "TABELAID") AS 
  SELECT   A.Id,
            A.IDENTIFICACAO,
            A.DESCRICAO,
            DECODE (A.STATUSOPORTUNIDADE,
                    1,
                    'Ativa',
                    2,
                    'Cancelada',
                    3,
                    'Revisada',
                    4,
                    'Recusada',
                    5,
                    'Vencida',
                    6,
                    'Enviada para Aprovação',
                    7,
                    'Rascunho')
               AS STATUSOPORTUNIDADE,
            DECODE (A.SUCESSONEGOCIACAO,
                    1,
                    'Prospect',
                    2,
                    'Em Negociação',
                    3,
                    'Aceito pelo Cliente',
                    4,
                    'Ganho',
                    5,
                    'Perdido')
               AS SUCESSONEGOCIACAO,
            DECODE (A.ESTAGIONEGOCIACAO,
                    1,
                    'Início do Processo',
                    2,
                    'Envio da Proposta',
                    3,
                    'Aceito pelo Cliente',
                    4,
                    'Ganho',
                    5,
                    'Perdido')
               AS ESTAGIONEGOCIACAO,
            DECODE (A.TIPODEPROPOSTA,
                    1,
                    'Geral',
                    2,
                    'Específica',
                    3,
                    'Reduzida')
               AS TIPODEPROPOSTA,
            DECODE (A.TIPOSERVICO,
                    1,
                    'FCL',
                    2,
                    'LCL',
                    3,
                    'FCL/LCL',
                    4,
                    'Break Bulk',
                    5,
                    'Transporte')
               AS TIPOSERVICO,
            TO_CHAR (A.DATAFECHAMENTO, 'DD/MM/YYYY') AS DATAFECHAMENTO,
            TO_CHAR (A.DATAINICIO, 'DD/MM/YYYY') AS DATAINICIO,
            TO_CHAR (A.DATATERMINO, 'DD/MM/YYYY') AS DATATERMINO,
            A.CRIADOPOR,
            A.APROVADA,
            A.PROBABILIDADE,
            DECODE (A.CLASSIFICACAOCLIENTE,
                    1,
                    'A',
                    2,
                    'B',
                    3,
                    'C')
               AS CLASSIFICACAOCLIENTE,
            DECODE (A.SEGMENTO,
                    1,
                    'Importador',
                    2,
                    'Exportador',
                    3,
                    'Despachante',
                    4,
                    'Agente',
                    5,
                    'Transportador',
                    6,
                    'Armador')
               AS SEGMENTO,
            DECODE (A.MOTIVOPERDA,
                    1,
                    'Atendimento',
                    2,
                    'Burocracia',
                    3,
                    'Estrutura Operacional',
                    4,
                    'Preço',
                    5,
                    'Outros')
               AS MOTIVOPERDA,
            DECODE (A.TIPONEGOCIO,
                    1,
                    'Novo',
                    2,
                    'Verticalização',
                    3,
                    'Revisão Ajuste')
               AS TIPONEGOCIO,
            DECODE (A.TIPOOPERACAOOPORTUNIDADE,
                    1,
                    'Regular',
                    2,
                    'Spot')
               AS TIPOOPERACAOOPORTUNIDADE,
            A.OBSERVACAO,
            A.FATURAMENTOMENSALLCL,
            A.FATURAMENTOMENSALFCL,
            A.VOLUMEMENSAL,
            A.CIFMEDIO,
            DECODE (A.PREMIOPARCERIA,
                    0,
                    'NÃO',
                    1,
                    'SIM')
               AS PREMIOPARCERIA,
            A.DATACRIACAO,
            A.ALTERADOPOR,
            A.ULTIMAALTERACAO,
            DECODE (A.TIPOOPERACAO,
                    1,
                    'RA',
                    2,
                    'OP',
                    3,
                    'RE')
               AS TIPOOPERACAO,
            DECODE (A.FORMAPAGAMENTO,
                    1,
                    'A Vista',
                    2,
                    'Faturada')
               AS FORMAPAGAMENTO,
            A.DIASFREETIME,
            A.QTDEDIAS,
            A.VALIDADE,
            DECODE (A.TIPOVALIDADE,
                    1,
                    'Dias',
                    2,
                    'Anos')
               TIPOVALIDADE,
            B.DESCRICAO AS CONTA,
            B.DOCUMENTO AS CONTADOCUMENTO,
            B.LOGRADOURO || ', ' || B.NUMERO AS CONTAENDERECO,
            I.DESCRICAO AS CONTACIDADE,
            J.DESCRICAO AS CONTAESTADO,
            B.CEP AS CONTACEP,
            B.TELEFONE AS CONTATELEFONE,
            C.NOME || ' ' || C.SOBRENOME AS CONTATO,
            C.EMAIL AS CONTATOEMAIL,
            C.TELEFONE AS CONTATOTELEFONE,
            C.DEPARTAMENTO AS CONTATODEPARTAMENTO,
            C.CARGO AS CONTATOCARGO,
            D.DESCRICAO AS REVISAO,
            E.DESCRICAO AS MERCADORIA,
            F.DESCRICAO AS MODELO,
            G.Nome AS VENDEDOR,
            H.DESCRICAO AS IMPOSTO,
            A.TABELAID
     FROM                              CRM.TB_CRM_OPORTUNIDADES A
                                    INNER JOIN
                                       CRM.TB_CRM_CONTAS B
                                    ON A.ContaId = B.Id
                                 LEFT JOIN
                                    CRM.TB_CRM_CONTATOS C
                                 ON A.ContatoId = C.Id
                              LEFT JOIN
                                 CRM.TB_CRM_OPORTUNIDADES D
                              ON A.RevisaoId = D.Id
                           LEFT JOIN
                              CRM.TB_CRM_MERCADORIAS E
                           ON A.MercadoriaId = E.Id
                        LEFT JOIN
                           CRM.TB_CRM_MODELO F
                        ON A.ModeloId = F.Id
                     LEFT JOIN
                        CRM.TB_CRM_USUARIOS G
                     ON A.VendedorId = G.Id
                  LEFT JOIN
                     CRM.TB_CRM_IMPOSTOS H
                  ON A.ImpostoId = H.Id
               LEFT JOIN
                  CRM.TB_CRM_CIDADES I
               ON B.CidadeId = I.Id
            LEFT JOIN
               CRM.TB_CRM_ESTADOS J
            ON I.Estado = J.Id
;
--------------------------------------------------------
--  DDL for View VW_CRM_PREMIOS_PARCERIA
--------------------------------------------------------

  CREATE OR REPLACE FORCE VIEW "CRM"."VW_CRM_PREMIOS_PARCERIA" ("ID", "OPORTUNIDADEID", "CONTATOID", "DESCRICAOCONTATO", "CONTATO", "FAVORECIDO1", "DESCRICAOFAVORECIDO1", "FAVORECIDO2", "DESCRICAOFAVORECIDO2", "FAVORECIDO3", "DESCRICAOFAVORECIDO3", "OPORTUNIDADEDESCRICAO", "STATUSPREMIOPARCERIA", "DESCRICAOSTATUSPREMIOPARCERIA", "INSTRUCAO", "DESCRICAOINSTRUCAO", "TIPOSERVICOPREMIOPARCERIA", "DESCRICAOSERVICOPREMIOPARCERIA", "OBSERVACOES", "URLPREMIO", "DATAURLPREMIO", "EMAILFAVORECIDO1", "EMAILFAVORECIDO2", "EMAILFAVORECIDO3", "CRIADOPOR", "TIPOOPERACAO", "DESCRICAOTIPOOPERACAO", "PREMIOREFERENCIAID", "ANEXOID", "ANEXO", "STATUSOPORTUNIDADE") AS 
  SELECT   A.Id,
            A.OportunidadeId,
            A.ContatoId,
            I.Nome AS DescricaoContato,
            C.Nome || ' ' || C.Sobrenome AS Contato,
            A.Favorecido1,
            F.Descricao AS DescricaoFavorecido1,
            A.Favorecido2,
            G.Descricao AS DescricaoFavorecido2,
            A.Favorecido3,
            H.Descricao AS DescricaoFavorecido3,
            B.Descricao AS OportunidadeDescricao,
            A.StatusPremioParceria,
            DECODE (A.StatusPremioParceria,
                    1,
                    'Em Andamento',
                    2,
                    'Em Aprovação',
                    3,
                    'Cadastrado',
                    4,
                    'Revisado',
                    5,
                    'Rejeitado',
                    6,
                    'Cancelado')
               AS DescricaoStatusPremioParceria,
            A.Instrucao,
            DECODE (A.Instrucao,
                    1,
                    'Geral',
                    2,
                    'Anterior',
                    3,
                    'Nova')
               AS DescricaoInstrucao,
            A.TipoServicoPremioParceria,
            DECODE (A.TipoServicoPremioParceria,
                    1,
                    'Importação',
                    2,
                    'Exportação',
                    3,
                    'LTL Exportação')
               AS DescricaoServicoPremioParceria,
            A.Observacoes,
            A.UrlPremio,
            A.DataUrlPremio,
            A.EmailFavorecido1,
            A.EmailFavorecido2,
            A.EmailFavorecido3,
            A.CriadoPor,
            B.TipoOperacao,
            DECODE (B.TipoOperacao,
                    1,
                    'RA',
                    2,
                    'OP',
                    3,
                    'RE')
               AS DescricaoTipoOperacao,
            D.Id AS PremioReferenciaId,
            A.AnexoId,
            E.Anexo,
            B.StatusOportunidade
     FROM                           CRM.TB_CRM_OPORTUNIDADE_PREMIOS A
                                 INNER JOIN
                                    CRM.TB_CRM_OPORTUNIDADES B
                                 ON A.OportunidadeId = B.Id
                              LEFT JOIN
                                 CRM.TB_CRM_CONTATOS C
                              ON A.ContatoId = C.Id
                           LEFT JOIN
                              CRM.TB_CRM_OPORTUNIDADE_PREMIOS D
                           ON A.PremioReferenciaId = D.Id
                        LEFT JOIN
                           CRM.TB_CRM_ANEXOS E
                        ON A.AnexoId = E.Id
                     LEFT JOIN
                        CRM.TB_CRM_CONTAS F
                     ON A.Favorecido1 = F.Id
                  LEFT JOIN
                     CRM.TB_CRM_CONTAS G
                  ON A.Favorecido2 = G.Id
               LEFT JOIN
                  CRM.TB_CRM_CONTAS H
               ON A.Favorecido3 = H.Id
            LEFT JOIN
               CRM.TB_CRM_CONTATOS I
            ON A.ContatoId = I.Id
;
--------------------------------------------------------
--  DDL for View VW_SOLICITACOES_COMERCIAIS
--------------------------------------------------------

  CREATE OR REPLACE FORCE VIEW "CRM"."VW_SOLICITACOES_COMERCIAIS" ("ID", "TIPOSOLICITACAO", "STATUSSOLICITACAO", "UNIDADESOLICITACAO", "AREAOCORRENCIASOLICITACAO", "TIPOOPERACAO", "OCORRENCIA", "MOTIVO", "CLIENTE", "CRIADOPOR") AS 
  SELECT 
    A.Id,
    A.TIPOSOLICITACAO,
    A.STATUSSOLICITACAO,
    A.UNIDADESOLICITACAO,
    A.AREAOCORRENCIASOLICITACAO, 
    A.TIPOOPERACAO,
    C.Descricao As Ocorrencia,
    D.Descricao As Motivo,
    CASE 
        WHEN A.TipoSolicitacao = 1 THEN 
        '' 
        WHEN A.TipoSolicitacao = 2 THEN
        '' 
        WHEN A.TipoSolicitacao = 3 THEN            
           (SELECT DISTINCT DECODE(Prorrogacao.NotaFiscalId, NULL, Conta.Descricao, Nota.NOMCLI) Cliente 
                FROM CRM.TB_CRM_SOLICITACAO_PRORROGACAO Prorrogacao LEFT JOIN CRM.TB_CRM_CONTAS Conta ON Prorrogacao.ContaId = Conta.Id LEFT JOIN 
                FATURA.FATURANOTA Nota ON Prorrogacao.NotaFiscalId = Nota.Id WHERE Prorrogacao.SolicitacaoId = A.Id)
        WHEN A.TipoSolicitacao = 4 THEN
            (SELECT DISTINCT DECODE(Restituicao.NotaFiscalId, NULL, Conta.Descricao, Nota.NOMCLI) Cliente 
                FROM CRM.TB_CRM_SOLICITACAO_RESTITUICAO Restituicao LEFT JOIN CRM.TB_CRM_CONTAS Conta ON Restituicao.FavorecidoId = Conta.Id LEFT JOIN 
                FATURA.FATURANOTA Nota ON Restituicao.NotaFiscalId = Nota.Id WHERE Restituicao.SolicitacaoId = A.Id)
    END Cliente, 
    B.Login As CriadoPor
FROM
    CRM.TB_CRM_SOLICITACOES A 
LEFT JOIN
    CRM.TB_CRM_USUARIOS B ON A.CriadoPor = B.Id
LEFT JOIN
    CRM.TB_CRM_SOLICITACAO_OCORRENCIAS C ON A.OcorrenciaId = C.Id
LEFT JOIN    
    CRM.TB_CRM_SOLICITACAO_MOTIVOS D ON A.MotivoId = D.Id
;
--------------------------------------------------------
--  DDL for Table TB_CRM_ADENDO_FORMA_PAGAMENTO
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_ADENDO_FORMA_PAGAMENTO" 
   (	"ID" NUMBER, 
	"ADENDOID" NUMBER, 
	"FORMAPAGAMENTO" NUMBER, 
	"ANEXOID" VARCHAR2(36 BYTE)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;

   COMMENT ON COLUMN "CRM"."TB_CRM_ADENDO_FORMA_PAGAMENTO"."ID" IS 'Chave primária';
   COMMENT ON COLUMN "CRM"."TB_CRM_ADENDO_FORMA_PAGAMENTO"."ADENDOID" IS 'Id do Adendo';
   COMMENT ON COLUMN "CRM"."TB_CRM_ADENDO_FORMA_PAGAMENTO"."FORMAPAGAMENTO" IS 'Forma Pagamento 1 -  A Vista / 2 - Faturado';
--------------------------------------------------------
--  DDL for Table TB_CRM_ADENDO_GRUPO_CNPJ
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_ADENDO_GRUPO_CNPJ" 
   (	"ID" NUMBER, 
	"ADENDOID" NUMBER, 
	"ACAO" NUMBER(1,0), 
	"GRUPOCNPJID" NUMBER, 
	"ANEXOID" VARCHAR2(36 BYTE)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;

   COMMENT ON COLUMN "CRM"."TB_CRM_ADENDO_GRUPO_CNPJ"."ID" IS 'Chave primária';
   COMMENT ON COLUMN "CRM"."TB_CRM_ADENDO_GRUPO_CNPJ"."ADENDOID" IS 'Id do Adendo';
   COMMENT ON COLUMN "CRM"."TB_CRM_ADENDO_GRUPO_CNPJ"."ACAO" IS 'Ação 1 - Inclusão / 2 - Exclusão';
--------------------------------------------------------
--  DDL for Table TB_CRM_ADENDO_SUB_CLIENTE
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_ADENDO_SUB_CLIENTE" 
   (	"ID" NUMBER, 
	"ADENDOID" NUMBER, 
	"SUBCLIENTEID" NUMBER, 
	"ACAO" NUMBER(1,0), 
	"SEGMENTO" NUMBER(1,0), 
	"ANEXOID" VARCHAR2(36 BYTE)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;

   COMMENT ON COLUMN "CRM"."TB_CRM_ADENDO_SUB_CLIENTE"."ID" IS 'Chave primária';
   COMMENT ON COLUMN "CRM"."TB_CRM_ADENDO_SUB_CLIENTE"."ADENDOID" IS 'Id do Adendo';
   COMMENT ON COLUMN "CRM"."TB_CRM_ADENDO_SUB_CLIENTE"."SUBCLIENTEID" IS 'Id do Sub Cliente';
   COMMENT ON COLUMN "CRM"."TB_CRM_ADENDO_SUB_CLIENTE"."ACAO" IS 'Ação 1 - Inclusão / 2 - Exclusão';
--------------------------------------------------------
--  DDL for Table TB_CRM_ADENDO_VENDEDOR
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_ADENDO_VENDEDOR" 
   (	"ID" NUMBER, 
	"ADENDOID" NUMBER, 
	"VENDEDORID" NUMBER
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;

   COMMENT ON COLUMN "CRM"."TB_CRM_ADENDO_VENDEDOR"."ID" IS 'Chave primária';
   COMMENT ON COLUMN "CRM"."TB_CRM_ADENDO_VENDEDOR"."ADENDOID" IS 'Id do Adendo';
   COMMENT ON COLUMN "CRM"."TB_CRM_ADENDO_VENDEDOR"."VENDEDORID" IS 'Id do Vendedor';
--------------------------------------------------------
--  DDL for Table TB_CRM_ANEXOS
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_ANEXOS" 
   (	"ID" NUMBER, 
	"IDPROCESSO" NUMBER, 
	"ANEXO" VARCHAR2(260 BYTE), 
	"DATACADASTRO" DATE, 
	"TIPOANEXO" NUMBER(1,0), 
	"VERSAO" NUMBER, 
	"CRIADOPOR" NUMBER, 
	"SALESID" VARCHAR2(30 BYTE), 
	"IDFILE" RAW(16), 
	"TIPODOCTO" NUMBER(2,0)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;

   COMMENT ON COLUMN "CRM"."TB_CRM_ANEXOS"."ID" IS 'Chave primária';
   COMMENT ON COLUMN "CRM"."TB_CRM_ANEXOS"."IDPROCESSO" IS 'Id da Oportunidade';
   COMMENT ON COLUMN "CRM"."TB_CRM_ANEXOS"."ANEXO" IS 'Caminho do anexo';
   COMMENT ON COLUMN "CRM"."TB_CRM_ANEXOS"."DATACADASTRO" IS 'Data de Cadastro';
   COMMENT ON COLUMN "CRM"."TB_CRM_ANEXOS"."TIPOANEXO" IS 'Tipo de Anexo - 1 - Ficha Faturamento / 2 - Cancelamento / 3 - Prêmio Parceria / 4 - Proposta / 5 - Outros';
   COMMENT ON COLUMN "CRM"."TB_CRM_ANEXOS"."VERSAO" IS 'Versão';
   COMMENT ON COLUMN "CRM"."TB_CRM_ANEXOS"."CRIADOPOR" IS 'Usuario de Criação';
   COMMENT ON COLUMN "CRM"."TB_CRM_ANEXOS"."TIPODOCTO" IS 'Tipo de Documento (1 - Oportunidade, 2 - Solicitação)';
--------------------------------------------------------
--  DDL for Table TB_CRM_AUDITORIA
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_AUDITORIA" 
   (	"ID" NUMBER, 
	"DATA" VARCHAR2(50 BYTE), 
	"NIVEL" VARCHAR2(50 BYTE), 
	"MENSAGEM" VARCHAR2(2000 BYTE), 
	"USUARIO" VARCHAR2(350 BYTE), 
	"MAQUINA" VARCHAR2(150 BYTE), 
	"ACTION" VARCHAR2(50 BYTE), 
	"CONTROLLER" VARCHAR2(50 BYTE), 
	"METODO" VARCHAR2(20 BYTE), 
	"ORIGEM" VARCHAR2(1000 BYTE), 
	"EXCEPTION" CLOB, 
	"STACKTRACE" VARCHAR2(2000 BYTE), 
	"TICKET" VARCHAR2(40 BYTE), 
	"OBJETO" VARCHAR2(4000 BYTE), 
	"CHAVE" NUMBER, 
	"IP" VARCHAR2(50 BYTE), 
	"REQUEST" VARCHAR2(2000 BYTE), 
	"ACAO" NUMBER(1,0)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" 
 LOB ("EXCEPTION") STORE AS BASICFILE (
  TABLESPACE "DADOS_TECONDI" ENABLE STORAGE IN ROW CHUNK 8192 RETENTION 
  NOCACHE LOGGING 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)) ;
--------------------------------------------------------
--  DDL for Table TB_CRM_BUSCA
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_BUSCA" 
   (	"ID" NUMBER, 
	"TERMO" VARCHAR2(250 BYTE), 
	"MENU" VARCHAR2(50 BYTE), 
	"CHAVE" NUMBER
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;

   COMMENT ON COLUMN "CRM"."TB_CRM_BUSCA"."ID" IS 'Chave primária';
   COMMENT ON COLUMN "CRM"."TB_CRM_BUSCA"."TERMO" IS 'Termo de busca';
   COMMENT ON COLUMN "CRM"."TB_CRM_BUSCA"."MENU" IS 'Nome do Menu';
   COMMENT ON COLUMN "CRM"."TB_CRM_BUSCA"."CHAVE" IS 'Chave primária do registro';
--------------------------------------------------------
--  DDL for Table TB_CRM_CIDADES
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_CIDADES" 
   (	"ID" NUMBER, 
	"DESCRICAO" VARCHAR2(150 BYTE), 
	"ESTADO" NUMBER
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;

   COMMENT ON COLUMN "CRM"."TB_CRM_CIDADES"."ID" IS 'Chave primária';
   COMMENT ON COLUMN "CRM"."TB_CRM_CIDADES"."DESCRICAO" IS 'Descrição da Cidade';
   COMMENT ON COLUMN "CRM"."TB_CRM_CIDADES"."ESTADO" IS 'Código do Estado ';
--------------------------------------------------------
--  DDL for Table TB_CRM_CONTAS
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_CONTAS" 
   (	"ID" NUMBER, 
	"DESCRICAO" VARCHAR2(255 BYTE), 
	"DOCUMENTO" VARCHAR2(18 BYTE), 
	"NOMEFANTASIA" VARCHAR2(150 BYTE), 
	"INSCRICAOESTADUAL" VARCHAR2(20 BYTE), 
	"TELEFONE" VARCHAR2(15 BYTE), 
	"VENDEDORID" NUMBER, 
	"SITUACAOCADASTRAL" NUMBER(1,0), 
	"SEGMENTO" NUMBER(1,0), 
	"CLASSIFICACAOFISCAL" NUMBER(1,0), 
	"LOGRADOURO" VARCHAR2(150 BYTE), 
	"BAIRRO" VARCHAR2(50 BYTE), 
	"ESTADO" NUMBER(2,0), 
	"NUMERO" NUMBER, 
	"COMPLEMENTO" VARCHAR2(150 BYTE), 
	"CEP" VARCHAR2(9 BYTE), 
	"CIDADEID" NUMBER, 
	"PAISID" NUMBER, 
	"CRIADOPOR" NUMBER, 
	"SALLESID" VARCHAR2(60 BYTE), 
	"DATACRIACAO" DATE DEFAULT SYSDATE
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;

   COMMENT ON COLUMN "CRM"."TB_CRM_CONTAS"."ID" IS 'Chave primária';
   COMMENT ON COLUMN "CRM"."TB_CRM_CONTAS"."DESCRICAO" IS 'Descrição da Conta';
   COMMENT ON COLUMN "CRM"."TB_CRM_CONTAS"."DOCUMENTO" IS 'CPF ou CNPJ';
   COMMENT ON COLUMN "CRM"."TB_CRM_CONTAS"."NOMEFANTASIA" IS 'Nome Fantasia';
   COMMENT ON COLUMN "CRM"."TB_CRM_CONTAS"."INSCRICAOESTADUAL" IS 'Inscrição Estadual';
   COMMENT ON COLUMN "CRM"."TB_CRM_CONTAS"."TELEFONE" IS 'Telefone';
   COMMENT ON COLUMN "CRM"."TB_CRM_CONTAS"."VENDEDORID" IS 'Id do Vendedor';
   COMMENT ON COLUMN "CRM"."TB_CRM_CONTAS"."SITUACAOCADASTRAL" IS 'Situação Cadastral 1 = Ativo / 2 = Baixado';
   COMMENT ON COLUMN "CRM"."TB_CRM_CONTAS"."SEGMENTO" IS 'Segmento 1 = Importador / 2 = Exportador / 3 = Despachante / 4 = Agente / 5 = Transportador / 6 = Armador';
   COMMENT ON COLUMN "CRM"."TB_CRM_CONTAS"."CLASSIFICACAOFISCAL" IS 'Classificação Fiscal 1 = Pesssoa Física / 2 = Pessoa Jurídica / 3 = Externo';
   COMMENT ON COLUMN "CRM"."TB_CRM_CONTAS"."LOGRADOURO" IS 'Endereço, Logradouro';
   COMMENT ON COLUMN "CRM"."TB_CRM_CONTAS"."BAIRRO" IS 'Bairro';
   COMMENT ON COLUMN "CRM"."TB_CRM_CONTAS"."ESTADO" IS 'Código do Estado (TB_CRM_ESTADOS)';
   COMMENT ON COLUMN "CRM"."TB_CRM_CONTAS"."NUMERO" IS 'Número';
   COMMENT ON COLUMN "CRM"."TB_CRM_CONTAS"."COMPLEMENTO" IS 'Complemento';
   COMMENT ON COLUMN "CRM"."TB_CRM_CONTAS"."CEP" IS 'CEP';
   COMMENT ON COLUMN "CRM"."TB_CRM_CONTAS"."CIDADEID" IS 'Código da Cidade (TB_CRM_CIDADES)';
   COMMENT ON COLUMN "CRM"."TB_CRM_CONTAS"."PAISID" IS 'Código do País (TB_CRM_PAISES)';
   COMMENT ON COLUMN "CRM"."TB_CRM_CONTAS"."CRIADOPOR" IS 'Usuário de Criação';
--------------------------------------------------------
--  DDL for Table TB_CRM_CONTATOS
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_CONTATOS" 
   (	"ID" NUMBER, 
	"NOME" VARCHAR2(150 BYTE), 
	"SOBRENOME" VARCHAR2(150 BYTE), 
	"TELEFONE" VARCHAR2(14 BYTE), 
	"CELULAR" VARCHAR2(15 BYTE), 
	"EMAIL" VARCHAR2(150 BYTE), 
	"CARGO" VARCHAR2(50 BYTE), 
	"DEPARTAMENTO" VARCHAR2(50 BYTE), 
	"DATANASCIMENTO" DATE, 
	"STATUS" NUMBER(1,0), 
	"CONTAID" NUMBER, 
	"SALESID" VARCHAR2(30 BYTE), 
	"DATACRIACAO" DATE DEFAULT SYSDATE
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;

   COMMENT ON COLUMN "CRM"."TB_CRM_CONTATOS"."ID" IS 'Chave primária';
   COMMENT ON COLUMN "CRM"."TB_CRM_CONTATOS"."NOME" IS 'Nome';
   COMMENT ON COLUMN "CRM"."TB_CRM_CONTATOS"."SOBRENOME" IS 'Sobrenome';
   COMMENT ON COLUMN "CRM"."TB_CRM_CONTATOS"."TELEFONE" IS 'Telefone';
   COMMENT ON COLUMN "CRM"."TB_CRM_CONTATOS"."CELULAR" IS 'Celular';
   COMMENT ON COLUMN "CRM"."TB_CRM_CONTATOS"."EMAIL" IS 'Email';
   COMMENT ON COLUMN "CRM"."TB_CRM_CONTATOS"."CARGO" IS 'Nome do Cargo';
   COMMENT ON COLUMN "CRM"."TB_CRM_CONTATOS"."DEPARTAMENTO" IS 'Departamento';
   COMMENT ON COLUMN "CRM"."TB_CRM_CONTATOS"."DATANASCIMENTO" IS 'Data de Nascimento';
   COMMENT ON COLUMN "CRM"."TB_CRM_CONTATOS"."STATUS" IS 'Status 0 - Inativo / 1 - Ativo';
   COMMENT ON COLUMN "CRM"."TB_CRM_CONTATOS"."CONTAID" IS 'Id da Conta';
--------------------------------------------------------
--  DDL for Table TB_CRM_EQUIPES_CONTA
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_EQUIPES_CONTA" 
   (	"ID" NUMBER(*,0), 
	"CONTAID" NUMBER, 
	"USUARIOID" NUMBER, 
	"ACESSOCONTA" NUMBER(1,0), 
	"ACESSOOPORTUNIDADE" NUMBER(1,0), 
	"DATACRIACAO" DATE DEFAULT SYSDATE, 
	"PAPELEQUIPE" NUMBER(1,0)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Table TB_CRM_EQUIPES_OPORTUNIDADE
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_EQUIPES_OPORTUNIDADE" 
   (	"ID" NUMBER(*,0), 
	"OPORTUNIDADEID" NUMBER, 
	"USUARIOID" NUMBER, 
	"ACESSOCONTA" NUMBER(1,0), 
	"ACESSOOPORTUNIDADE" NUMBER(1,0), 
	"DATACRIACAO" DATE DEFAULT SYSDATE, 
	"PAPELEQUIPE" NUMBER(1,0)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Table TB_CRM_EQUIPES_VENDEDOR
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_EQUIPES_VENDEDOR" 
   (	"ID" NUMBER(*,0), 
	"VENDEDORID" NUMBER, 
	"USUARIOID" NUMBER, 
	"DATACRIACAO" DATE DEFAULT SYSDATE, 
	"ACESSOCONTA" NUMBER(1,0), 
	"ACESSOOPORTUNIDADE" NUMBER(1,0), 
	"PAPELEQUIPE" NUMBER(1,0)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Table TB_CRM_ESTADOS
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_ESTADOS" 
   (	"ID" NUMBER, 
	"DESCRICAO" VARCHAR2(2 BYTE)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;

   COMMENT ON COLUMN "CRM"."TB_CRM_ESTADOS"."ID" IS 'Chave primária';
   COMMENT ON COLUMN "CRM"."TB_CRM_ESTADOS"."DESCRICAO" IS 'Descrição do estado';
--------------------------------------------------------
--  DDL for Table TB_CRM_IMPOSTOS
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_IMPOSTOS" 
   (	"ID" NUMBER, 
	"DESCRICAO" VARCHAR2(50 BYTE)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;

   COMMENT ON COLUMN "CRM"."TB_CRM_IMPOSTOS"."ID" IS 'Chave primária';
   COMMENT ON COLUMN "CRM"."TB_CRM_IMPOSTOS"."DESCRICAO" IS 'Descrição do Imposto';
--------------------------------------------------------
--  DDL for Table TB_CRM_LAYOUT
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_LAYOUT" 
   (	"ID" NUMBER, 
	"MODELOID" NUMBER(9,0), 
	"SERVICOID" NUMBER(9,0), 
	"LINHA" NUMBER(9,0), 
	"DESCRICAO" VARCHAR2(150 BYTE), 
	"TIPOREGISTRO" VARCHAR2(2 BYTE), 
	"BASECALCULO" VARCHAR2(10 BYTE), 
	"PERIODO" NUMBER(2,0), 
	"VALOR20" NUMBER(15,2), 
	"VALOR40" NUMBER(15,2), 
	"ADICIONALARMAZENAGEM" NUMBER(15,2), 
	"ADICIONALGRC" NUMBER(15,2), 
	"MINIMOGRC" NUMBER(15,2), 
	"VALORANVISA" NUMBER(15,2), 
	"MOEDA" NUMBER(2,0), 
	"VALORMINIMO20" NUMBER(15,2), 
	"VALORMINIMO40" NUMBER(15,2), 
	"VALORMARGEMDIREITA" NUMBER(15,2), 
	"VALORMARGEMESQUERDA" NUMBER(15,2), 
	"VALORENTREMARGENS" NUMBER(15,2), 
	"ADICIONALIMO" NUMBER(15,2), 
	"PESOMAXIMO" NUMBER(15,2), 
	"ADICIONALPESO" NUMBER(15,2), 
	"VALORMINIMOMARGEMDIREITA" NUMBER(15,2), 
	"VALORMINIMOMARGEMESQUERDA" NUMBER(15,2), 
	"VALORMINIMOENTREMARGENS" NUMBER(15,2), 
	"REEMBOLSO" NUMBER(1,0), 
	"VALORMINIMO" NUMBER(15,2), 
	"ORIGEM" NUMBER(9,0), 
	"DESTINO" NUMBER(9,0), 
	"VALOR" NUMBER(15,2), 
	"CONDICOESGERAIS" CLOB, 
	"TIPOTRABALHO" NUMBER(1,0), 
	"LINHAREFERENCIA" NUMBER, 
	"DESCRICAOVALOR" VARCHAR2(50 BYTE), 
	"TIPOCARGA" NUMBER(1,0), 
	"MARGEM" NUMBER(1,0), 
	"QTDEDIAS" NUMBER(2,0), 
	"CONDICOESINICIAIS" CLOB, 
	"CIFMINIMO" NUMBER, 
	"CIFMAXIMO" NUMBER, 
	"DESCRICAOPERIODO" VARCHAR2(150 BYTE), 
	"DESCRICAOCIF" VARCHAR2(150 BYTE)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" 
 LOB ("CONDICOESGERAIS") STORE AS BASICFILE (
  TABLESPACE "DADOS_TECONDI" ENABLE STORAGE IN ROW CHUNK 8192 RETENTION 
  NOCACHE LOGGING 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)) 
 LOB ("CONDICOESINICIAIS") STORE AS BASICFILE (
  TABLESPACE "DADOS_TECONDI" ENABLE STORAGE IN ROW CHUNK 8192 RETENTION 
  NOCACHE LOGGING 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)) ;

   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."ID" IS 'Chave primária';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."MODELOID" IS 'Id do Modelo';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."SERVICOID" IS 'Id do Serviço';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."LINHA" IS 'Número da Linha';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."DESCRICAO" IS 'Descrição da Linha';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."TIPOREGISTRO" IS 'Tipo do Registro Título = 1, SubTítulo = 2, SubTítulo Margem = 3, Armazenagem = 4, Armazenagem Mín. = 5, Arm. All In = 6, Serviço Margem = 7, Mín. margem = 8, Serv. Mec. Manual = 9, Mín. Mec Manual = 10, Serv. Liberação = 11, Serv. Hub Port = 12, Gerais = 13, Mín. geral = 14, Cond. Geral = 15';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."BASECALCULO" IS 'Base Cálculo Unidade = 1, Ton = 2, CIF = 3, BL = 4, VOLP = 5';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."PERIODO" IS 'Períodos';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."VALOR20" IS 'Valor 20';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."VALOR40" IS 'Valor 40';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."ADICIONALARMAZENAGEM" IS 'Adicional de Armazenagem';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."ADICIONALGRC" IS 'Adicional GRC';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."MINIMOGRC" IS 'Minimo GRC';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."VALORANVISA" IS 'Valor Anvisa';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."MOEDA" IS 'Moeda 1 - Real / 2 - Dollar';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."VALORMINIMO20" IS 'Valor minimo 20';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."VALORMINIMO40" IS 'Valor minimo 40';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."VALORMARGEMDIREITA" IS 'Valor Margem Direita';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."VALORMARGEMESQUERDA" IS 'Valor margem esquerda';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."VALORENTREMARGENS" IS 'Valor entre margens';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."ADICIONALIMO" IS 'Adicional IMO';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."PESOMAXIMO" IS 'Peso maximo';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."ADICIONALPESO" IS 'Adicional de peso';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."VALORMINIMOMARGEMDIREITA" IS 'Valor minimo margem direita';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."VALORMINIMOMARGEMESQUERDA" IS 'Valor minimo margem esquerda';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."VALORMINIMOENTREMARGENS" IS 'Valor minimo entre margens';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."REEMBOLSO" IS 'Reembolso';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."VALORMINIMO" IS 'Valor minimo';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."ORIGEM" IS 'Origem (AUTONUM.TB_CAD_PARCEIROS)  FLAG_EADI = 1';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."DESTINO" IS 'Destino (AUTONUM.TB_CAD_PARCEIROS) WHERE FLAG_EADI';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."VALOR" IS 'Valor';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."CONDICOESGERAIS" IS 'Condicoes Gerais';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."TIPOTRABALHO" IS 'Tipo Trabalho Mecanizada = 1, Manual = 2';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."LINHAREFERENCIA" IS 'Linha de referencia';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."DESCRICAOVALOR" IS 'Descricao do valor';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."TIPOCARGA" IS 'Tipo de Carga 1 = Contêiner, 2 = Carga';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."MARGEM" IS 'Margem';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."QTDEDIAS" IS 'Quantidade de dias';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT"."CONDICOESINICIAIS" IS 'Condições Iniciais';
--------------------------------------------------------
--  DDL for Table TB_CRM_LAYOUT_VL_CIF_BL
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_LAYOUT_VL_CIF_BL" 
   (	"ID" NUMBER, 
	"LAYOUTID" NUMBER(9,0), 
	"MINIMO" NUMBER(9,0), 
	"MAXIMO" NUMBER(9,0), 
	"MARGEM" NUMBER, 
	"VALOR20" NUMBER(15,2), 
	"VALOR40" NUMBER(15,2), 
	"DESCRICAO" VARCHAR2(50 BYTE), 
	"PERIODO" NUMBER, 
	"DESCRICAOPERIODO" VARCHAR2(250 BYTE), 
	"VALORMINIMO20" NUMBER, 
	"VALORMINIMO40" NUMBER, 
	"DESCRICAOVALORMINIMO" VARCHAR2(250 BYTE)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;

   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT_VL_CIF_BL"."ID" IS 'Chave primária';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT_VL_CIF_BL"."LAYOUTID" IS 'Id do Layout';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT_VL_CIF_BL"."MINIMO" IS 'Valor do minimo';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT_VL_CIF_BL"."MAXIMO" IS 'Valor do máximo';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT_VL_CIF_BL"."MARGEM" IS 'Margem 1 - Direita / 2 - Esquerda / 3 - Entre margens';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT_VL_CIF_BL"."VALOR20" IS 'Valor 20';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT_VL_CIF_BL"."VALOR40" IS 'Valor 40';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT_VL_CIF_BL"."DESCRICAO" IS 'Descrição';
--------------------------------------------------------
--  DDL for Table TB_CRM_LAYOUT_VL_MINIMO_BL
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_LAYOUT_VL_MINIMO_BL" 
   (	"ID" NUMBER, 
	"LAYOUTID" NUMBER(9,0), 
	"BLMINIMO" NUMBER(9,0), 
	"BLMAXIMO" NUMBER(9,0), 
	"VALORMINIMO" NUMBER(15,2)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;

   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT_VL_MINIMO_BL"."ID" IS 'Chave primária';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT_VL_MINIMO_BL"."LAYOUTID" IS 'Id do Layout';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT_VL_MINIMO_BL"."BLMINIMO" IS 'BL Mínimo';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT_VL_MINIMO_BL"."BLMAXIMO" IS 'BL Máximo';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT_VL_MINIMO_BL"."VALORMINIMO" IS 'Valor Mínimo';
--------------------------------------------------------
--  DDL for Table TB_CRM_LAYOUT_VL_PESO
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_LAYOUT_VL_PESO" 
   (	"ID" NUMBER, 
	"LAYOUTID" NUMBER(9,0), 
	"VALORINICIAL" NUMBER(15,2), 
	"VALORFINAL" NUMBER(15,2), 
	"PRECO" NUMBER(15,2)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;

   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT_VL_PESO"."ID" IS 'Chave primária';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT_VL_PESO"."LAYOUTID" IS 'Id do Layout';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT_VL_PESO"."VALORINICIAL" IS 'Valor Inicial';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT_VL_PESO"."VALORFINAL" IS 'Valor Final';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT_VL_PESO"."PRECO" IS 'Preço';
--------------------------------------------------------
--  DDL for Table TB_CRM_LAYOUT_VL_VOLUME
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_LAYOUT_VL_VOLUME" 
   (	"ID" NUMBER, 
	"LAYOUTID" NUMBER(9,0), 
	"VALORINICIAL" NUMBER(15,2), 
	"VALORFINAL" NUMBER(15,2), 
	"PRECO" NUMBER(15,2)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;

   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT_VL_VOLUME"."ID" IS 'Chave primária';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT_VL_VOLUME"."LAYOUTID" IS 'Id do Layout';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT_VL_VOLUME"."VALORINICIAL" IS 'Valor Inicial Volume';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT_VL_VOLUME"."VALORFINAL" IS 'Valor Final Volume';
   COMMENT ON COLUMN "CRM"."TB_CRM_LAYOUT_VL_VOLUME"."PRECO" IS 'Preço';
--------------------------------------------------------
--  DDL for Table TB_CRM_MENUS
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_MENUS" 
   (	"ID" NUMBER, 
	"DESCRICAO" VARCHAR2(150 BYTE), 
	"URL" VARCHAR2(256 BYTE), 
	"INATIVO" NUMBER(1,0), 
	"DINAMICO" NUMBER(1,0), 
	"DESCRICAOCOMPLETA" VARCHAR2(50 BYTE)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Table TB_CRM_MENUS_CAMPOS
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_MENUS_CAMPOS" 
   (	"ID" NUMBER, 
	"MENUID" NUMBER, 
	"OBJETOID" VARCHAR2(50 BYTE), 
	"OBJETODESCRICAO" VARCHAR2(50 BYTE), 
	"REQUERIDO" NUMBER(1,0)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Table TB_CRM_MERCADORIAS
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_MERCADORIAS" 
   (	"ID" NUMBER, 
	"DESCRICAO" VARCHAR2(150 BYTE), 
	"STATUS" NUMBER(1,0)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;

   COMMENT ON COLUMN "CRM"."TB_CRM_MERCADORIAS"."ID" IS 'Chave primária';
   COMMENT ON COLUMN "CRM"."TB_CRM_MERCADORIAS"."DESCRICAO" IS 'Descrição da Mercadoria';
   COMMENT ON COLUMN "CRM"."TB_CRM_MERCADORIAS"."STATUS" IS 'Status da Mercadoria 0 - Inativo / 1 - Ativo';
--------------------------------------------------------
--  DDL for Table TB_CRM_MODELO
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_MODELO" 
   (	"ID" NUMBER, 
	"TIPOOPERACAO" VARCHAR2(2 BYTE), 
	"STATUS" NUMBER(1,0), 
	"DESCRICAO" VARCHAR2(150 BYTE), 
	"FORMAPAGAMENTO" NUMBER, 
	"DIASFREETIME" NUMBER(2,0), 
	"QTDEDIAS" NUMBER(2,0), 
	"VALIDADE" NUMBER(2,0), 
	"TIPOVALIDADE" NUMBER(1,0), 
	"VENDEDORID" NUMBER, 
	"IMPOSTOID" NUMBER, 
	"DATACADASTRO" DATE, 
	"DATAINATIVIDADE" DATE, 
	"ACORDO" NUMBER(1,0) DEFAULT 0
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;

   COMMENT ON COLUMN "CRM"."TB_CRM_MODELO"."ID" IS 'Chave primária';
   COMMENT ON COLUMN "CRM"."TB_CRM_MODELO"."TIPOOPERACAO" IS 'Tipo de Operação 1 - RA / 2 - OP / 3 - RE';
   COMMENT ON COLUMN "CRM"."TB_CRM_MODELO"."STATUS" IS 'Status do Modelo 0 - Inativo / 1 - Ativo';
   COMMENT ON COLUMN "CRM"."TB_CRM_MODELO"."DESCRICAO" IS 'Descrição do Modelo';
   COMMENT ON COLUMN "CRM"."TB_CRM_MODELO"."FORMAPAGAMENTO" IS 'Forma Pagamento 1 - A Vista / 2 - Faturado';
   COMMENT ON COLUMN "CRM"."TB_CRM_MODELO"."DIASFREETIME" IS 'Dias Free Time';
   COMMENT ON COLUMN "CRM"."TB_CRM_MODELO"."QTDEDIAS" IS 'Quantidade de Dias';
   COMMENT ON COLUMN "CRM"."TB_CRM_MODELO"."VALIDADE" IS 'Validade';
   COMMENT ON COLUMN "CRM"."TB_CRM_MODELO"."TIPOVALIDADE" IS 'Tipo Validade 1 - Dias / 2 - Anos';
   COMMENT ON COLUMN "CRM"."TB_CRM_MODELO"."VENDEDORID" IS 'Id do Vendedor';
   COMMENT ON COLUMN "CRM"."TB_CRM_MODELO"."IMPOSTOID" IS 'Id do Imposto';
   COMMENT ON COLUMN "CRM"."TB_CRM_MODELO"."DATACADASTRO" IS 'Data de Cadastro';
   COMMENT ON COLUMN "CRM"."TB_CRM_MODELO"."DATAINATIVIDADE" IS 'Data de Inatividade';
--------------------------------------------------------
--  DDL for Table TB_CRM_OPORTUNIDADE_ADENDOS
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_OPORTUNIDADE_ADENDOS" 
   (	"ID" NUMBER, 
	"OPORTUNIDADEID" NUMBER, 
	"TIPOADENDO" NUMBER(1,0), 
	"STATUSADENDO" NUMBER(1,0), 
	"DATACADASTRO" DATE, 
	"CRIADOPOR" NUMBER, 
	"SALESID" VARCHAR2(30 BYTE)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;

   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_ADENDOS"."ID" IS 'Chave primária';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_ADENDOS"."OPORTUNIDADEID" IS 'Id da Oportunidade';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_ADENDOS"."TIPOADENDO" IS 'Tipo Adendo 1 - Alteração Vendedor / 2 - Forma Pagamento / 3 - Inclusão de Sub Cliente / 4 - Exclusão de Sub Cliente / 5 - Inclusão de grupo CNPJ / 6 - Exclusão de Grupo CNPJ';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_ADENDOS"."STATUSADENDO" IS '1 - Aberto / 2 - Enviado / 3 - Rejeitado / 4 - Aprovado';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_ADENDOS"."DATACADASTRO" IS 'Cada do Cadastro';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_ADENDOS"."CRIADOPOR" IS 'Usuário de Criação';
--------------------------------------------------------
--  DDL for Table TB_CRM_OPORTUNIDADE_CLIENTES
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_OPORTUNIDADE_CLIENTES" 
   (	"ID" NUMBER, 
	"CONTAID" NUMBER, 
	"SEGMENTO" NUMBER(1,0), 
	"OPORTUNIDADEID" NUMBER, 
	"CRIADOPOR" NUMBER, 
	"DATACRIACAO" DATE DEFAULT SYSDATE
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;

   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_CLIENTES"."ID" IS 'Chave primária';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_CLIENTES"."CONTAID" IS 'Id da Conta';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_CLIENTES"."SEGMENTO" IS 'Segmento 1 - Importador / 2 - Exportador / 3 - Despachante / 4 - Agente / 5 - Transportador / 6 - Armador';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_CLIENTES"."OPORTUNIDADEID" IS 'Id da Oportunidade';
--------------------------------------------------------
--  DDL for Table TB_CRM_OPORTUNIDADE_FICHA_FAT
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_OPORTUNIDADE_FICHA_FAT" 
   (	"ID" NUMBER, 
	"OPORTUNIDADEID" NUMBER, 
	"CONTAID" NUMBER, 
	"FATURADOCONTRAID" NUMBER, 
	"DIASSEMANA" VARCHAR2(50 BYTE), 
	"DIASFATURAMENTO" VARCHAR2(50 BYTE), 
	"DATACORTE" NUMBER, 
	"CONDICAOPAGAMENTOFATURAMENTOID" VARCHAR2(20 BYTE), 
	"EMAILFATURAMENTO" VARCHAR2(1000 BYTE), 
	"OBSERVACOESFATURAMENTO" VARCHAR2(4000 BYTE), 
	"STATUSFICHAFATURAMENTO" NUMBER(1,0), 
	"ANEXOFATURAMENTOID" NUMBER
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;

   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_FICHA_FAT"."ID" IS 'Chave primária';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_FICHA_FAT"."OPORTUNIDADEID" IS 'Id da Oportunidade';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_FICHA_FAT"."CONTAID" IS 'Id da Conta';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_FICHA_FAT"."FATURADOCONTRAID" IS 'Id da Conta (Faturado Contra)';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_FICHA_FAT"."DIASSEMANA" IS 'Dias da Semana (delimitado por virgula)';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_FICHA_FAT"."DIASFATURAMENTO" IS 'Dias para Faturamento (delimitado por virgula)';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_FICHA_FAT"."DATACORTE" IS 'Data Corte';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_FICHA_FAT"."CONDICAOPAGAMENTOFATURAMENTOID" IS 'Id da Condição de Pagamento (Banco Fatura)';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_FICHA_FAT"."EMAILFATURAMENTO" IS 'Emails para Faturamento';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_FICHA_FAT"."OBSERVACOESFATURAMENTO" IS 'Observações de Faturamento';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_FICHA_FAT"."STATUSFICHAFATURAMENTO" IS 'Status Ficha Faturamento 1 - Em andamento / 2 - Em aprovação / 3 - Aprovado / 4 -Rejeitado / 5 - Cancelado';
--------------------------------------------------------
--  DDL for Table TB_CRM_OPORTUNIDADE_GRUPO_CNPJ
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_OPORTUNIDADE_GRUPO_CNPJ" 
   (	"ID" NUMBER, 
	"CONTAID" NUMBER, 
	"OPORTUNIDADEID" NUMBER, 
	"CRIADOPOR" NUMBER, 
	"DATACRIACAO" DATE DEFAULT SYSDATE
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;

   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_GRUPO_CNPJ"."ID" IS 'Chave primária';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_GRUPO_CNPJ"."CONTAID" IS 'Id da Conta';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_GRUPO_CNPJ"."OPORTUNIDADEID" IS 'Id da Oportunidade';
--------------------------------------------------------
--  DDL for Table TB_CRM_OPORTUNIDADE_LAYOUT
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_OPORTUNIDADE_LAYOUT" 
   (	"ID" NUMBER, 
	"OPORTUNIDADEID" NUMBER(9,0), 
	"MODELOID" NUMBER(9,0), 
	"SERVICOID" NUMBER(9,0), 
	"LINHA" NUMBER(9,0), 
	"DESCRICAO" VARCHAR2(150 BYTE), 
	"TIPOREGISTRO" VARCHAR2(2 BYTE), 
	"BASECALCULO" VARCHAR2(10 BYTE), 
	"PERIODO" NUMBER(2,0), 
	"VALOR20" NUMBER(15,2), 
	"VALOR40" NUMBER(15,2), 
	"ADICIONALARMAZENAGEM" NUMBER(15,2), 
	"ADICIONALGRC" NUMBER(15,2), 
	"MINIMOGRC" NUMBER(15,2), 
	"VALORANVISA" NUMBER(15,2), 
	"MOEDA" NUMBER(2,0), 
	"VALORMINIMO20" NUMBER(15,2), 
	"VALORMINIMO40" NUMBER(15,2), 
	"VALORMARGEMDIREITA" NUMBER(15,2), 
	"VALORMARGEMESQUERDA" NUMBER(15,2), 
	"VALORENTREMARGENS" NUMBER(15,2), 
	"ADICIONALIMO" NUMBER(15,2), 
	"PESOMAXIMO" NUMBER(15,2), 
	"ADICIONALPESO" NUMBER(15,2), 
	"VALORMINIMOMARGEMDIREITA" NUMBER(15,2), 
	"VALORMINIMOMARGEMESQUERDA" NUMBER(15,2), 
	"VALORMINIMOENTREMARGENS" NUMBER(15,2), 
	"REEMBOLSO" NUMBER(1,0), 
	"VALORMINIMO" NUMBER(15,2), 
	"ORIGEM" NUMBER(9,0), 
	"DESTINO" NUMBER(9,0), 
	"VALOR" NUMBER(15,2), 
	"CONDICOESGERAIS" CLOB, 
	"TIPOTRABALHO" NUMBER(1,0), 
	"LINHAREFERENCIA" NUMBER, 
	"DESCRICAOVALOR" VARCHAR2(50 BYTE), 
	"TIPOCARGA" NUMBER(1,0), 
	"MARGEM" NUMBER(1,0), 
	"QTDEDIAS" NUMBER(2,0), 
	"CONDICOESINICIAIS" CLOB, 
	"DESCRICAOPERIODO" VARCHAR2(150 BYTE), 
	"DESCRICAOCIF" VARCHAR2(150 BYTE), 
	"CIFMINIMO" NUMBER, 
	"CIFMAXIMO" NUMBER
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" 
 LOB ("CONDICOESGERAIS") STORE AS BASICFILE (
  TABLESPACE "DADOS_TECONDI" ENABLE STORAGE IN ROW CHUNK 8192 RETENTION 
  NOCACHE LOGGING 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)) 
 LOB ("CONDICOESINICIAIS") STORE AS BASICFILE (
  TABLESPACE "DADOS_TECONDI" ENABLE STORAGE IN ROW CHUNK 8192 RETENTION 
  NOCACHE LOGGING 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)) ;
--------------------------------------------------------
--  DDL for Table TB_CRM_OPORTUNIDADE_PREMIOS
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_OPORTUNIDADE_PREMIOS" 
   (	"ID" NUMBER, 
	"OPORTUNIDADEID" NUMBER, 
	"CRIADOPOR" NUMBER, 
	"STATUSPREMIOPARCERIA" NUMBER(1,0), 
	"FAVORECIDO1" NUMBER, 
	"FAVORECIDO2" NUMBER, 
	"FAVORECIDO3" NUMBER, 
	"INSTRUCAO" NUMBER(1,0), 
	"CONTATOID" NUMBER, 
	"PREMIOREFERENCIAID" NUMBER, 
	"TIPOSERVICOPREMIOPARCERIA" NUMBER(1,0), 
	"OBSERVACOES" VARCHAR2(1000 BYTE), 
	"URLPREMIO" VARCHAR2(250 BYTE), 
	"DATAURLPREMIO" DATE, 
	"EMAILFAVORECIDO1" VARCHAR2(250 BYTE), 
	"EMAILFAVORECIDO2" VARCHAR2(250 BYTE), 
	"EMAILFAVORECIDO3" VARCHAR2(250 BYTE), 
	"DATACADASTRO" DATE, 
	"SALLESID" VARCHAR2(25 BYTE), 
	"PREMIOREVISAOID" NUMBER, 
	"CANCELADO" NUMBER(1,0), 
	"ANEXOID" VARCHAR2(36 BYTE)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;

   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_PREMIOS"."ID" IS 'Chave primária';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_PREMIOS"."OPORTUNIDADEID" IS 'Id (autonum) da Oportunidade';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_PREMIOS"."CRIADOPOR" IS 'Usuário de criação';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_PREMIOS"."STATUSPREMIOPARCERIA" IS '1 - Em Andamento / 2 - Em Aprovação / 3 - Cadastrado / 4 - Revisado / 5 - Rejeitado';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_PREMIOS"."FAVORECIDO1" IS 'Nome do Favorecido 1';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_PREMIOS"."FAVORECIDO2" IS 'Nome do Favorecido 2';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_PREMIOS"."FAVORECIDO3" IS 'Nome do Favorecido 3';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_PREMIOS"."INSTRUCAO" IS '1 - Geral / 2 - Anterior / 3 - Nova';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_PREMIOS"."CONTATOID" IS 'Id do Contato';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_PREMIOS"."PREMIOREFERENCIAID" IS 'Id do Prêmio Parceria';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_PREMIOS"."TIPOSERVICOPREMIOPARCERIA" IS '1 - Importação / 2 - Exportação / 3 - LTL Exportação';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_PREMIOS"."OBSERVACOES" IS 'Observações Gerais';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_PREMIOS"."URLPREMIO" IS 'Url do prêmio (sistema de comissões)';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_PREMIOS"."DATAURLPREMIO" IS 'Data Url prêmio  (sistema de comissões)';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_PREMIOS"."EMAILFAVORECIDO1" IS 'Email do Favorecido 1  (sistema de comissões)';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_PREMIOS"."EMAILFAVORECIDO2" IS 'Email do Favorecido 2  (sistema de comissões)';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_PREMIOS"."EMAILFAVORECIDO3" IS 'Email do Favorecido 3  (sistema de comissões)';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADE_PREMIOS"."DATACADASTRO" IS 'Data do cadastro';
--------------------------------------------------------
--  DDL for Table TB_CRM_OPORTUNIDADES
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_OPORTUNIDADES" 
   (	"ID" NUMBER, 
	"CONTAID" NUMBER, 
	"APROVADA" NUMBER(1,0), 
	"DESCRICAO" VARCHAR2(150 BYTE), 
	"DATAFECHAMENTO" DATE, 
	"DATAINICIO" DATE, 
	"DATATERMINO" DATE, 
	"TABELAID" NUMBER, 
	"CONTATOID" NUMBER, 
	"PROBABILIDADE" NUMBER, 
	"SUCESSONEGOCIACAO" NUMBER(1,0), 
	"CLASSIFICACAOCLIENTE" NUMBER(1,0), 
	"SEGMENTO" NUMBER(1,0), 
	"ESTAGIONEGOCIACAO" NUMBER(1,0), 
	"STATUSOPORTUNIDADE" NUMBER(1,0), 
	"MOTIVOPERDA" NUMBER(1,0), 
	"TIPODEPROPOSTA" NUMBER(1,0), 
	"TIPOSERVICO" NUMBER(1,0), 
	"TIPONEGOCIO" NUMBER(1,0), 
	"TIPOOPERACAOOPORTUNIDADE" NUMBER(1,0), 
	"REVISAOID" NUMBER, 
	"MERCADORIAID" NUMBER, 
	"OBSERVACAO" VARCHAR2(1000 BYTE), 
	"FATURAMENTOMENSALLCL" NUMBER, 
	"FATURAMENTOMENSALFCL" NUMBER, 
	"VOLUMEMENSAL" NUMBER, 
	"CIFMEDIO" NUMBER, 
	"PREMIOPARCERIA" NUMBER(1,0), 
	"DATACRIACAO" DATE, 
	"ALTERADOPOR" NUMBER, 
	"ULTIMAALTERACAO" DATE, 
	"TIPOOPERACAO" NUMBER(1,0), 
	"MODELOID" NUMBER, 
	"FORMAPAGAMENTO" NUMBER(1,0), 
	"QTDEDIAS" NUMBER(3,0), 
	"VALIDADE" NUMBER(3,0), 
	"TIPOVALIDADE" NUMBER(1,0), 
	"DIASFREETIME" NUMBER(2,0), 
	"VENDEDORID" NUMBER, 
	"IMPOSTOID" NUMBER, 
	"CRIADOPOR" NUMBER, 
	"SALLESID" VARCHAR2(18 BYTE), 
	"CANCELADO" NUMBER(1,0), 
	"IDENTIFICACAO" NUMBER(8,0)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;

   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."ID" IS 'Chave primaria';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."CONTAID" IS 'Id da Conta';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."APROVADA" IS 'Flag aprovada';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."DESCRICAO" IS 'Descrição da Oportunidade';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."DATAFECHAMENTO" IS 'Data de Fechamento';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."DATAINICIO" IS 'Data de Inicio';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."DATATERMINO" IS 'Data de Termino';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."TABELAID" IS 'Id da tabela';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."CONTATOID" IS 'Id do Contato';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."PROBABILIDADE" IS 'Percentual de probabilidade';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."SUCESSONEGOCIACAO" IS 'Sucesso da Negociação 1 - Prospect / 2 - Em Negociação / 3 - Aceito pelo cliente / 4 - Ganho / 5 - Perdido';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."CLASSIFICACAOCLIENTE" IS 'Classificação do Cliente 1 - A / 2 - B / 3 - C';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."SEGMENTO" IS 'Segmento 1 - Importador / 2 - Exportador / 3 - Despachante / 4 - Agente / 5 - Transportador / 6 - Armador';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."ESTAGIONEGOCIACAO" IS 'Estagio Negociação 1 - Inicio do Processo / 2 - Envio da Proposta / 3 - Aceito pelo Cliente / 4 - Ganho / 5 - Perdido';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."STATUSOPORTUNIDADE" IS 'Status da Oportunidade 1 - Ativa / 2 - Cancelada / 3 - Revisada / 4 - Recusada / 5 - Vencida / 6 - Enviado para Aprovação / 7 - Rascunho';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."MOTIVOPERDA" IS 'Motivo Perda 1 - Atendimento / 2 - Burocracia / 3 - Estrutura Operacional / 4 - Preço / 5 - Outros';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."TIPODEPROPOSTA" IS 'Tipo de Proposta 1 - Geral / 2 - Específica / 3 - Reduzida';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."TIPOSERVICO" IS 'Tipo de Serviço 1 - FCL / 2 - LCL / 3 - FCL-LCL / 4 - Break Bulk / 5 - Transporte';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."TIPONEGOCIO" IS 'Tipo de Negócio 1 - Novo / 2 - Verticalização / 3 - Revisão Ajuste';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."TIPOOPERACAOOPORTUNIDADE" IS 'Tipo de Operação da Oportunidade 1 - Regular / 2 - Spot';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."REVISAOID" IS 'Id da Oportunidade (revisão)';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."MERCADORIAID" IS 'Id da Mercadoria';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."OBSERVACAO" IS 'Observações';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."FATURAMENTOMENSALLCL" IS 'Valor de Faturamento mensal LCL';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."FATURAMENTOMENSALFCL" IS 'Valor de Faturamento mensal FCL';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."VOLUMEMENSAL" IS 'Volume mensal';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."CIFMEDIO" IS 'Valor do CIF médio';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."PREMIOPARCERIA" IS 'Premio parceria 0 - Não / 1 -  Sim';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."DATACRIACAO" IS 'Data de Criação da  Oportunidade';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."ALTERADOPOR" IS 'Usuário da última alteração';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."ULTIMAALTERACAO" IS 'Data da última alteração';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."TIPOOPERACAO" IS 'Tipo de Operação 1 - RA / 2 - OP / 3 - RE';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."MODELOID" IS 'Id do Modelo';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."FORMAPAGAMENTO" IS 'Forma de Pagamento 1 - A Vista / 2 - Faturada';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."QTDEDIAS" IS 'Quantidade de dias da proposta';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."VALIDADE" IS 'Validade da Proposta';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."TIPOVALIDADE" IS 'Tipo validade da proposta 1 - Dias / 2 - Anos';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."DIASFREETIME" IS 'Dias Free Time da proposta';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."VENDEDORID" IS 'Id do vendedor da propsota';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."IMPOSTOID" IS 'Id do imposto da proposta';
   COMMENT ON COLUMN "CRM"."TB_CRM_OPORTUNIDADES"."CRIADOPOR" IS 'Usuário de criação da oportunidade';
--------------------------------------------------------
--  DDL for Table TB_CRM_PAISES
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_PAISES" 
   (	"ID" NUMBER, 
	"DESCRICAO" VARCHAR2(150 BYTE), 
	"SIGLA" VARCHAR2(3 BYTE)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;

   COMMENT ON COLUMN "CRM"."TB_CRM_PAISES"."ID" IS 'Chave primária';
   COMMENT ON COLUMN "CRM"."TB_CRM_PAISES"."DESCRICAO" IS 'Descrição do País';
   COMMENT ON COLUMN "CRM"."TB_CRM_PAISES"."SIGLA" IS 'Sigla do País';
--------------------------------------------------------
--  DDL for Table TB_CRM_PARAMETROS
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_PARAMETROS" 
   (	"MORA" NUMBER(8,3), 
	"MULTA" NUMBER(8,3)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Table TB_CRM_PREMIOS_MODALIDADES
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_PREMIOS_MODALIDADES" 
   (	"ID" NUMBER, 
	"OPORTUNIDADEID" NUMBER, 
	"OPORTUNIDADEPREMIOID" NUMBER, 
	"MODALIDADE" NUMBER(1,0)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;

   COMMENT ON COLUMN "CRM"."TB_CRM_PREMIOS_MODALIDADES"."ID" IS 'Chave primária';
   COMMENT ON COLUMN "CRM"."TB_CRM_PREMIOS_MODALIDADES"."OPORTUNIDADEID" IS 'Id da Oportunidade';
   COMMENT ON COLUMN "CRM"."TB_CRM_PREMIOS_MODALIDADES"."OPORTUNIDADEPREMIOID" IS 'Id do Prêmio Parceria';
   COMMENT ON COLUMN "CRM"."TB_CRM_PREMIOS_MODALIDADES"."MODALIDADE" IS 'Tipo de Modalidade 1 - GR Paga / 2 - BL Com Entrada / 3 - BL Pago / 4 - Contêiner com Entrada / 5 - Contêiner Pago / 6 - Valor Fixo / 7 - Pacote / 8 - Contêiner';
--------------------------------------------------------
--  DDL for Table TB_CRM_SERVICO_IPA
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_SERVICO_IPA" 
   (	"ID" NUMBER, 
	"SERVICOID" NUMBER, 
	"SERVICOFATURAMENTOID" NUMBER
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;

   COMMENT ON COLUMN "CRM"."TB_CRM_SERVICO_IPA"."ID" IS 'Chave primária';
   COMMENT ON COLUMN "CRM"."TB_CRM_SERVICO_IPA"."SERVICOID" IS 'Id do Serviço CRM';
   COMMENT ON COLUMN "CRM"."TB_CRM_SERVICO_IPA"."SERVICOFATURAMENTOID" IS 'Id Serviços Faturados (IPA)';
--------------------------------------------------------
--  DDL for Table TB_CRM_SERVICOS
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_SERVICOS" 
   (	"ID" NUMBER, 
	"DESCRICAO" VARCHAR2(150 BYTE), 
	"STATUS" NUMBER(1,0), 
	"RECINTOALFANDEGADO" NUMBER(1,0), 
	"OPERADOR" NUMBER(1,0), 
	"REDEX" NUMBER(1,0)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;

   COMMENT ON COLUMN "CRM"."TB_CRM_SERVICOS"."ID" IS 'Chave primária';
   COMMENT ON COLUMN "CRM"."TB_CRM_SERVICOS"."DESCRICAO" IS 'Descrição do Serviço';
   COMMENT ON COLUMN "CRM"."TB_CRM_SERVICOS"."STATUS" IS 'Status 0 - Inativo / 1 - Ativo';
   COMMENT ON COLUMN "CRM"."TB_CRM_SERVICOS"."RECINTOALFANDEGADO" IS 'Flag Recinto Alfandegado';
   COMMENT ON COLUMN "CRM"."TB_CRM_SERVICOS"."OPERADOR" IS 'Flag Operador';
   COMMENT ON COLUMN "CRM"."TB_CRM_SERVICOS"."REDEX" IS 'Flag Redex';
--------------------------------------------------------
--  DDL for Table TB_CRM_SOL_DESC_SERV_IMPOSTO
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_SOL_DESC_SERV_IMPOSTO" 
   (	"AUTONUM" NUMBER(8,0), 
	"AUTONUM_SERVICO_FATURADO" NUMBER(8,0), 
	"AUTONUM_IMPOSTO" NUMBER(3,0), 
	"VALOR_IMPOSTO" NUMBER(15,2), 
	"SOLICITACAOID" NUMBER(10,0)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Table TB_CRM_SOLICITACAO_CANCEL_NF
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_SOLICITACAO_CANCEL_NF" 
   (	"ID" NUMBER(*,0), 
	"SOLICITACAOID" NUMBER, 
	"TIPOPESQUISA" NUMBER(1,0), 
	"TIPOPESQUISANUMERO" VARCHAR2(30 BYTE), 
	"LOTE" NUMBER(8,0), 
	"NOTAFISCALID" NUMBER, 
	"VALORNF" NUMBER, 
	"FORMAPAGAMENTO" NUMBER(1,0), 
	"DATAEMISSAO" DATE, 
	"DESCONTO" NUMBER, 
	"VALORNOVANF" NUMBER, 
	"DATAPRORROGACAO" DATE, 
	"NFE" VARCHAR2(20 BYTE), 
	"CONTAID" NUMBER, 
	"CRIADOPOR" NUMBER, 
	"DATACADASTRO" DATE DEFAULT SYSDATE
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Table TB_CRM_SOLICITACAO_DESCONTO
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_SOLICITACAO_DESCONTO" 
   (	"ID" NUMBER(*,0), 
	"SOLICITACAOID" NUMBER, 
	"MINUTAGRID" NUMBER, 
	"VALORGR" NUMBER, 
	"CLIENTEID" NUMBER, 
	"INDICADORID" NUMBER, 
	"PROPOSTA" VARCHAR2(50 BYTE), 
	"VENCIMENTOGR" DATE, 
	"FREETIMEGR" DATE, 
	"PERIODO" NUMBER, 
	"FORMAPAGAMENTO" NUMBER(1,0), 
	"TIPODESCONTO" NUMBER(1,0), 
	"DESCONTO" NUMBER, 
	"DESCONTONOSERVICO" NUMBER, 
	"DESCONTOFINAL" NUMBER, 
	"VENCIMENTO" DATE, 
	"FREETIME" DATE, 
	"PORSERVICO" NUMBER(1,0), 
	"MINUTA" NUMBER, 
	"LOTE" NUMBER, 
	"DESCONTOCOMIMPOSTO" NUMBER, 
	"SERVICOFATURADOID" NUMBER, 
	"TIPOPESQUISA" NUMBER(1,0), 
	"CRIADOPOR" NUMBER, 
	"DATACADASTRO" DATE DEFAULT SYSDATE
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Table TB_CRM_SOLICITACAO_MOTIVOS
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_SOLICITACAO_MOTIVOS" 
   (	"ID" NUMBER, 
	"DESCRICAO" VARCHAR2(250 BYTE), 
	"PRORROGACAOBOLETO" NUMBER(1,0), 
	"CANCELAMENTONF" NUMBER(1,0), 
	"DESCONTO" NUMBER(1,0), 
	"RESTITUICAO" NUMBER(1,0), 
	"STATUS" NUMBER(1,0)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Table TB_CRM_SOLICITACAO_OCORRENCIAS
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_SOLICITACAO_OCORRENCIAS" 
   (	"ID" NUMBER, 
	"DESCRICAO" VARCHAR2(250 BYTE), 
	"CANCELAMENTONF" NUMBER(1,0), 
	"DESCONTO" NUMBER(1,0), 
	"RESTITUICAO" NUMBER(1,0), 
	"STATUS" NUMBER(1,0), 
	"PRORROGACAOBOLETO" NUMBER(1,0)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Table TB_CRM_SOLICITACAO_PRORROGACAO
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_SOLICITACAO_PRORROGACAO" 
   (	"ID" NUMBER(*,0), 
	"SOLICITACAOID" NUMBER, 
	"NOTAFISCALID" NUMBER, 
	"VALORNF" NUMBER, 
	"VENCIMENTOORIGINAL" DATE, 
	"DATAPRORROGACAO" DATE, 
	"NUMEROPRORROGACAO" NUMBER, 
	"ISENTARJUROS" NUMBER(1,0), 
	"VALORJUROS" NUMBER, 
	"VALORTOTALCOMJUROS" NUMBER, 
	"OBSERVACOES" VARCHAR2(4000 BYTE), 
	"CRIADOPOR" NUMBER, 
	"DATACRIACAO" DATE DEFAULT SYSDATE, 
	"CONTAID" NUMBER, 
	"NFE" VARCHAR2(20 BYTE), 
	"DATACADASTRO" DATE DEFAULT SYSDATE
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Table TB_CRM_SOLICITACAO_RESTITUICAO
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_SOLICITACAO_RESTITUICAO" 
   (	"ID" NUMBER(*,0), 
	"SOLICITACAOID" NUMBER, 
	"TIPOPESQUISA" NUMBER(1,0), 
	"TIPOPESQUISANUMERO" NUMBER, 
	"NOTAFISCALID" NUMBER, 
	"VALORNF" NUMBER, 
	"RPS" NUMBER, 
	"LOTE" NUMBER, 
	"DOCUMENTO" VARCHAR2(50 BYTE), 
	"FAVORECIDOID" NUMBER, 
	"BANCOID" NUMBER, 
	"AGENCIA" VARCHAR2(20 BYTE), 
	"FORNECEDORSAP" VARCHAR2(50 BYTE), 
	"DATAVENCIMENTO" DATE, 
	"CONTACORRENTE" VARCHAR2(20 BYTE), 
	"OBSERVACOES" VARCHAR2(4000 BYTE), 
	"NFE" VARCHAR2(20 BYTE), 
	"DATACADASTRO" DATE DEFAULT SYSDATE, 
	"CRIADOPOR" NUMBER
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Table TB_CRM_SOLICITACOES
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_SOLICITACOES" 
   (	"ID" NUMBER(*,0), 
	"TIPOSOLICITACAO" NUMBER(1,0), 
	"STATUSSOLICITACAO" NUMBER(1,0), 
	"UNIDADESOLICITACAO" NUMBER(1,0), 
	"AREAOCORRENCIASOLICITACAO" NUMBER(1,0), 
	"OCORRENCIAID" NUMBER, 
	"MOTIVOID" NUMBER, 
	"JUSTIFICATIVA" VARCHAR2(4000 BYTE), 
	"CRIADOPOR" NUMBER, 
	"DATACRIACAO" DATE DEFAULT SYSDATE, 
	"TIPOOPERACAO" NUMBER(1,0), 
	"VALORDEVIDO" NUMBER, 
	"VALORCOBRADO" NUMBER, 
	"VALORCREDITO" NUMBER
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Table TB_CRM_SUB_MENUS
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_SUB_MENUS" 
   (	"ID" NUMBER, 
	"MENUID" NUMBER, 
	"DESCRICAO" VARCHAR2(50 BYTE), 
	"URL" VARCHAR2(250 BYTE)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Table TB_CRM_USUARIOS
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_USUARIOS" 
   (	"ID" NUMBER, 
	"LOGIN" VARCHAR2(50 BYTE), 
	"NOME" VARCHAR2(200 BYTE), 
	"EMAIL" VARCHAR2(150 BYTE), 
	"ADMINISTRADOR" NUMBER(1,0), 
	"ATIVO" NUMBER(1,0), 
	"CARGOID" NUMBER, 
	"SALESID" VARCHAR2(30 BYTE), 
	"LOGINWORKFLOW" VARCHAR2(50 BYTE)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;

   COMMENT ON COLUMN "CRM"."TB_CRM_USUARIOS"."ID" IS 'Chave primária';
   COMMENT ON COLUMN "CRM"."TB_CRM_USUARIOS"."LOGIN" IS 'Login (AD)';
   COMMENT ON COLUMN "CRM"."TB_CRM_USUARIOS"."NOME" IS 'Nome do Usuário';
   COMMENT ON COLUMN "CRM"."TB_CRM_USUARIOS"."EMAIL" IS 'Email do Usuário';
   COMMENT ON COLUMN "CRM"."TB_CRM_USUARIOS"."ADMINISTRADOR" IS 'Flag Administrador';
   COMMENT ON COLUMN "CRM"."TB_CRM_USUARIOS"."ATIVO" IS 'Flag Ativo';
   COMMENT ON COLUMN "CRM"."TB_CRM_USUARIOS"."CARGOID" IS 'Id do Cargo';
--------------------------------------------------------
--  DDL for Table TB_CRM_USUARIOS_CARGOS
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_USUARIOS_CARGOS" 
   (	"ID" NUMBER, 
	"DESCRICAO" VARCHAR2(150 BYTE), 
	"VENDEDOR" NUMBER(1,0)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;

   COMMENT ON COLUMN "CRM"."TB_CRM_USUARIOS_CARGOS"."ID" IS 'Chave primária';
   COMMENT ON COLUMN "CRM"."TB_CRM_USUARIOS_CARGOS"."DESCRICAO" IS 'Descrição dos Cargos';
--------------------------------------------------------
--  DDL for Table TB_CRM_USUARIOS_PERMISSAO_TELA
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_USUARIOS_PERMISSAO_TELA" 
   (	"ID" NUMBER, 
	"PERMISSAOID" NUMBER, 
	"CAMPOID" NUMBER, 
	"SOMENTELEITURA" NUMBER(1,0)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Table TB_CRM_USUARIOS_PERMISSOES
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_USUARIOS_PERMISSOES" 
   (	"ID" NUMBER, 
	"MENUID" NUMBER, 
	"CARGOID" NUMBER, 
	"ACESSAR" NUMBER(1,0), 
	"CADASTRAR" NUMBER(1,0), 
	"ATUALIZAR" NUMBER(1,0), 
	"EXCLUIR" NUMBER(1,0), 
	"LOGS" NUMBER(1,0)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Table TB_CRM_WORKFLOW
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_CRM_WORKFLOW" 
   (	"ID" NUMBER, 
	"PROCESSOID" NUMBER, 
	"PROTOCOLO" NUMBER, 
	"MENSAGEM" VARCHAR2(1000 BYTE), 
	"DATACADASTRO" DATE, 
	"PROCESSO" NUMBER(1,0), 
	"CRIADOPOR" NUMBER
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;

   COMMENT ON COLUMN "CRM"."TB_CRM_WORKFLOW"."ID" IS 'Chave primaria';
   COMMENT ON COLUMN "CRM"."TB_CRM_WORKFLOW"."PROCESSOID" IS 'Id da Oportunidade';
   COMMENT ON COLUMN "CRM"."TB_CRM_WORKFLOW"."PROTOCOLO" IS 'Número do protocolo retornado pela API';
   COMMENT ON COLUMN "CRM"."TB_CRM_WORKFLOW"."MENSAGEM" IS 'Mensagem retornada pela API';
   COMMENT ON COLUMN "CRM"."TB_CRM_WORKFLOW"."DATACADASTRO" IS 'Data do Cadastro';
   COMMENT ON COLUMN "CRM"."TB_CRM_WORKFLOW"."PROCESSO" IS 'Tipo do Processo';
   COMMENT ON COLUMN "CRM"."TB_CRM_WORKFLOW"."CRIADOPOR" IS 'Usuário de Criação';
--------------------------------------------------------
--  DDL for Table TB_LISTA_CFG_VALORMINIMO
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_LISTA_CFG_VALORMINIMO" 
   (	"AUTONUM" NUMBER(8,0), 
	"AUTONUMSV" NUMBER(8,0), 
	"NBLS" NUMBER(*,0) DEFAULT 0, 
	"VALORMINIMO" NUMBER(15,2) DEFAULT 0, 
	"TIPO" VARCHAR2(6 BYTE), 
	"PERCMULTA" NUMBER(15,2)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Table TB_LISTA_FAIXA_PESO
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_LISTA_FAIXA_PESO" 
   (	"AUTONUM" NUMBER(8,0), 
	"AUTONUMSV" NUMBER(8,0), 
	"PESOINICIAL" NUMBER(15,2) DEFAULT 0, 
	"PESOFINAL" NUMBER(15,2) DEFAULT 0, 
	"PRECO" NUMBER(15,2) DEFAULT 0, 
	"TIPO" VARCHAR2(1 BYTE)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Table TB_LISTA_FAIXA_VOLUME
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_LISTA_FAIXA_VOLUME" 
   (	"AUTONUM" NUMBER(8,0), 
	"AUTONUMSV" NUMBER(8,0), 
	"VOLUMEINICIAL" NUMBER(15,2) DEFAULT 0, 
	"VOLUMEFINAL" NUMBER(15,2) DEFAULT 0, 
	"PRECO" NUMBER(5,2) DEFAULT 0, 
	"TIPO" VARCHAR2(1 BYTE)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Table TB_LISTA_PRECO_SERVICOS_FIXOS
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_LISTA_PRECO_SERVICOS_FIXOS" 
   (	"AUTONUM" NUMBER(8,0), 
	"LISTA" NUMBER(8,0) DEFAULT 0, 
	"SERVICO" NUMBER(5,0), 
	"TIPO_CARGA" VARCHAR2(6 BYTE), 
	"BASE_CALCULO" VARCHAR2(5 BYTE), 
	"VARIANTE_LOCAL" VARCHAR2(4 BYTE), 
	"PRECO_UNITARIO" NUMBER(13,5) DEFAULT 0, 
	"MOEDA" NUMBER(5,0), 
	"PRECO_MINIMO" NUMBER(13,5) DEFAULT 0, 
	"VALOR_ACRESCIMO" NUMBER(5,2) DEFAULT 0, 
	"LOCAL_ATRACACAO" VARCHAR2(3 BYTE), 
	"GRUPO_ATRACACAO" NUMBER(2,0) DEFAULT 0, 
	"VALOR_ACRESC_PESO" NUMBER(5,2) DEFAULT 0, 
	"PESO_LIMITE" NUMBER(9,0) DEFAULT 0, 
	"TIPO_OPER" VARCHAR2(20 BYTE), 
	"PORTO_HUBPORT" NUMBER(5,0), 
	"TIPO_DOC" NUMBER(5,0) DEFAULT (0), 
	"BASE_EXCESSO" VARCHAR2(20 BYTE), 
	"VALOR_EXCESSO" NUMBER(12,2) DEFAULT (0), 
	"PRECO_MAXIMO" NUMBER(13,5) DEFAULT 0, 
	"VALOR_ANVISA" NUMBER(5,2) DEFAULT 0, 
	"FLAG_COBRAR_NVOCC" NUMBER(1,0), 
	"FORMA_PAGAMENTO_NVOCC" NUMBER(2,0)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Table TB_LISTA_P_S_FAIXASCIF_FIX
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_LISTA_P_S_FAIXASCIF_FIX" 
   (	"AUTONUM" NUMBER(8,0), 
	"AUTONUMSV" NUMBER(8,0), 
	"VALORINICIAL" NUMBER(15,2) DEFAULT 0, 
	"VALORFINAL" NUMBER(15,2) DEFAULT 0, 
	"PERCENTUAL" NUMBER(12,5) DEFAULT 0, 
	"MINIMO" NUMBER(15,2) DEFAULT 0, 
	"TIPO" VARCHAR2(1 BYTE) DEFAULT 'C'
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Table TB_LISTA_P_S_FAIXASCIF_PER
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_LISTA_P_S_FAIXASCIF_PER" 
   (	"AUTONUM" NUMBER(8,0), 
	"AUTONUMSV" NUMBER(8,0), 
	"VALORINICIAL" NUMBER(15,2) DEFAULT 0, 
	"VALORFINAL" NUMBER(15,2) DEFAULT 0, 
	"PERCENTUAL" NUMBER(12,5) DEFAULT 0, 
	"MINIMO" NUMBER(15,2) DEFAULT 0, 
	"TIPO" VARCHAR2(1 BYTE) DEFAULT 'C'
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Table TB_LISTA_P_S_PERIODO
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_LISTA_P_S_PERIODO" 
   (	"AUTONUM" NUMBER(8,0), 
	"LISTA" NUMBER(8,0) DEFAULT 0, 
	"SERVICO" NUMBER(5,0), 
	"N_PERIODO" NUMBER(2,0), 
	"QTDE_DIAS" NUMBER(3,0), 
	"TIPO_CARGA" VARCHAR2(6 BYTE), 
	"BASE_CALCULO" VARCHAR2(5 BYTE), 
	"VARIANTE_LOCAL" VARCHAR2(4 BYTE), 
	"PRECO_UNITARIO" NUMBER(13,5), 
	"MOEDA" NUMBER(5,0), 
	"PRECO_MINIMO" NUMBER(8,2), 
	"VALOR_ACRESCIMO" NUMBER(5,2), 
	"LOCAL_ATRACACAO" VARCHAR2(3 BYTE), 
	"FLAG_PRORATA" NUMBER(1,0) DEFAULT 0, 
	"GRUPO_ATRACACAO" NUMBER(2,0) DEFAULT 0, 
	"VALOR_ACRESC_PESO" NUMBER(5,2) DEFAULT 0, 
	"PESO_LIMITE" NUMBER(9,0) DEFAULT 0, 
	"TIPO_DOC" NUMBER(5,0) DEFAULT (0), 
	"BASE_EXCESSO" VARCHAR2(20 BYTE), 
	"VALOR_EXCESSO" NUMBER(12,2) DEFAULT (0), 
	"PRECO_MAXIMO" NUMBER(13,5) DEFAULT 0, 
	"VALOR_ANVISA" NUMBER(5,2) DEFAULT 0, 
	"FLAG_COBRAR_NVOCC" NUMBER(1,0), 
	"FORMA_PAGAMENTO_NVOCC" NUMBER(2,0)
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Table TB_LISTAS_PRECOS
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_LISTAS_PRECOS" 
   (	"AUTONUM" NUMBER(8,0), 
	"IMPORTADOR" NUMBER(8,0), 
	"DESPACHANTE" NUMBER(8,0), 
	"CAPTADOR" NUMBER(8,0), 
	"COLOADER" NUMBER(8,0) DEFAULT 0, 
	"COCOLOADER" NUMBER(10,0) DEFAULT 0, 
	"COCOLOADER2" NUMBER(10,0) DEFAULT 0, 
	"DESCR" VARCHAR2(50 BYTE), 
	"DIAS_APOS_GR" NUMBER(3,0), 
	"FORMA_PAGAMENTO" NUMBER(2,0) DEFAULT 1, 
	"PROPOSTA" VARCHAR2(15 BYTE), 
	"MINIMO_PARTELOTE" NUMBER(1,0) DEFAULT 0, 
	"FLAG_HUBPORT" NUMBER(1,0) DEFAULT 0
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Table TB_LP_IMPOSTOS
--------------------------------------------------------

  CREATE TABLE "CRM"."TB_LP_IMPOSTOS" 
   (	"ID_TABELA" NUMBER(6,0), 
	"ID_IMPOSTO" NUMBER(3,0), 
	"PADRAO" NUMBER(1,0) DEFAULT 0
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 0 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_ADENDO_FORMA_PAGAMENTO
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_ADENDO_FORMA_PAGAMENTO"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 1 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_ADENDO_GRUPO_CNPJ
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_ADENDO_GRUPO_CNPJ"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 4347 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_ADENDO_SUB_CLIENTE
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_ADENDO_SUB_CLIENTE"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 141029 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_ADENDO_VENDEDOR
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_ADENDO_VENDEDOR"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 274 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_ANEXOS
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_ANEXOS"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 9422510 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_AUDITORIA
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_AUDITORIA"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 142 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_BUSCA
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_BUSCA"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 432170 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_CONTAS
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_CONTAS"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 155477 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_CONTATOS
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_CONTATOS"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 31428 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_EQUIPES_CONTA
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_EQUIPES_CONTA"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 279829 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_EQUIPES_OPORTUNIDADE
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_EQUIPES_OPORTUNIDADE"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 186568 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_EQUIPES_VENDEDOR
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_EQUIPES_VENDEDOR"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 155 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_LAYOUT
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_LAYOUT"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 2587 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_LAYOUT_VL_CIF_BL
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_LAYOUT_VL_CIF_BL"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 15 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_LAYOUT_VL_MINIMO_BL
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_LAYOUT_VL_MINIMO_BL"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 17 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_LAYOUT_VL_PESO
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_LAYOUT_VL_PESO"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 24 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_LAYOUT_VL_VOLUME
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_LAYOUT_VL_VOLUME"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 5 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_LOGS
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_LOGS"  MINVALUE 1 MAXVALUE 999999999999999999999999999 INCREMENT BY 1 START WITH 1 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_MENUS_CAMPOS
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_MENUS_CAMPOS"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 114 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_MERCADORIAS
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_MERCADORIAS"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 4 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_MODELOS
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_MODELOS"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 20 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_OPORTUNIDADE_ADENDOS
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_OPORTUNIDADE_ADENDOS"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 14214 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_OPORTUNIDADE_ANEXOS
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_OPORTUNIDADE_ANEXOS"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 65 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_OPORTUNIDADE_CLIENTES
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_OPORTUNIDADE_CLIENTES"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 434754 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_OPORTUNIDADE_FICHA_FAT
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_OPORTUNIDADE_FICHA_FAT"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 82 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_OPORTUNIDADE_GRUPOCNPJ
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_OPORTUNIDADE_GRUPOCNPJ"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 25320 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_OPORTUNIDADE_IDENT
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_OPORTUNIDADE_IDENT"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 22034 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_OPORTUNIDADE_LAYOUT
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_OPORTUNIDADE_LAYOUT"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 6091 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_OPORTUNIDADE_PREMIOS
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_OPORTUNIDADE_PREMIOS"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 78480 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_OPORTUNIDADE_PROPOSTA
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_OPORTUNIDADE_PROPOSTA"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 1 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_OPORTUNIDADES
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_OPORTUNIDADES"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 133865 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_PREMIOS_MODALIDADES
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_PREMIOS_MODALIDADES"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 115136 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_SERVICO_IPA
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_SERVICO_IPA"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 146 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_SERVICOS
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_SERVICOS"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 73 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_SERVICOS_TIPOS_OPER
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_SERVICOS_TIPOS_OPER"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 4 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_SOL_DESC_SERV_IMPOSTO
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_SOL_DESC_SERV_IMPOSTO"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 9946 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_SOLICITACAO_DESCONTO
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_SOLICITACAO_DESCONTO"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 231 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_SOLICITACAO_MOTIVOS
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_SOLICITACAO_MOTIVOS"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 25 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_SOLICITACAO_OCORRENCIA
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_SOLICITACAO_OCORRENCIA"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 8 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_SOLICITACAO_PRORROG
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_SOLICITACAO_PRORROG"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 103 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_SOLICITACOES
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_SOLICITACOES"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 219 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_SOLICITACOES_DADOS_FIN
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_SOLICITACOES_DADOS_FIN"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 108 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_SOLICIT_RESTITUICAO
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_SOLICIT_RESTITUICAO"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 84 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_USUARIOS
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_USUARIOS"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 184 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_USUARIOS_CARGOS
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_USUARIOS_CARGOS"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 13 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_USUARIOS_PERMISSOES
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_USUARIOS_PERMISSOES"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 2665 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_USUARIOS_PERM_TELA
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_USUARIOS_PERM_TELA"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 43962 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Sequence SEQ_CRM_WORKFLOW
--------------------------------------------------------

   CREATE SEQUENCE  "CRM"."SEQ_CRM_WORKFLOW"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 254 NOCACHE  NOORDER  NOCYCLE ;
--------------------------------------------------------
--  DDL for Index IDX_LISTA_P_S_FAIXACIF_FIX
--------------------------------------------------------

  CREATE INDEX "CRM"."IDX_LISTA_P_S_FAIXACIF_FIX" ON "CRM"."TB_LISTA_P_S_FAIXASCIF_FIX" ("AUTONUMSV", "VALORINICIAL", "VALORFINAL") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Index IDX_CRM_MODELO
--------------------------------------------------------

  CREATE INDEX "CRM"."IDX_CRM_MODELO" ON "CRM"."TB_CRM_MODELO" ("DESCRICAO") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Index ID_LISTA_FAIXA_PESO
--------------------------------------------------------

  CREATE INDEX "CRM"."ID_LISTA_FAIXA_PESO" ON "CRM"."TB_LISTA_FAIXA_PESO" ("AUTONUMSV") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Index IDX_CRM_OPORTUNIDADES
--------------------------------------------------------

  CREATE INDEX "CRM"."IDX_CRM_OPORTUNIDADES" ON "CRM"."TB_CRM_OPORTUNIDADES" ("DESCRICAO") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Index IDX_CRM_SERVICOS
--------------------------------------------------------

  CREATE INDEX "CRM"."IDX_CRM_SERVICOS" ON "CRM"."TB_CRM_SERVICOS" ("DESCRICAO") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Index PK_LISTA_P_S_PERIODO
--------------------------------------------------------

  CREATE UNIQUE INDEX "CRM"."PK_LISTA_P_S_PERIODO" ON "CRM"."TB_LISTA_P_S_PERIODO" ("AUTONUM") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Index ID_FAIXA_VOLUME_TIPO
--------------------------------------------------------

  CREATE INDEX "CRM"."ID_FAIXA_VOLUME_TIPO" ON "CRM"."TB_LISTA_FAIXA_VOLUME" ("TIPO") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Index IDX_LISTA_P_S_FAIXACIF_PER
--------------------------------------------------------

  CREATE INDEX "CRM"."IDX_LISTA_P_S_FAIXACIF_PER" ON "CRM"."TB_LISTA_P_S_FAIXASCIF_PER" ("AUTONUMSV", "VALORINICIAL", "VALORFINAL") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Index PK_TB_LISTA_FAIXA_VOLUME
--------------------------------------------------------

  CREATE UNIQUE INDEX "CRM"."PK_TB_LISTA_FAIXA_VOLUME" ON "CRM"."TB_LISTA_FAIXA_VOLUME" ("AUTONUM") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Index PK_TB_LISTA_P_S_FAIXASCIF
--------------------------------------------------------

  CREATE UNIQUE INDEX "CRM"."PK_TB_LISTA_P_S_FAIXASCIF" ON "CRM"."TB_LISTA_P_S_FAIXASCIF_PER" ("AUTONUM") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Index PK_LISTA_PRECO_FIXO
--------------------------------------------------------

  CREATE UNIQUE INDEX "CRM"."PK_LISTA_PRECO_FIXO" ON "CRM"."TB_LISTA_PRECO_SERVICOS_FIXOS" ("AUTONUM") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Index ID_LISTA_FAIXA_VOLUME
--------------------------------------------------------

  CREATE INDEX "CRM"."ID_LISTA_FAIXA_VOLUME" ON "CRM"."TB_LISTA_FAIXA_VOLUME" ("AUTONUMSV") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Index IDX_CRM_CIDADES
--------------------------------------------------------

  CREATE INDEX "CRM"."IDX_CRM_CIDADES" ON "CRM"."TB_CRM_CIDADES" ("DESCRICAO") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Index IDX_LISTA_CFG_VALORMINIMO
--------------------------------------------------------

  CREATE INDEX "CRM"."IDX_LISTA_CFG_VALORMINIMO" ON "CRM"."TB_LISTA_CFG_VALORMINIMO" ("AUTONUMSV") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Index PK_TB_LISTA_FAIXA_PESO
--------------------------------------------------------

  CREATE UNIQUE INDEX "CRM"."PK_TB_LISTA_FAIXA_PESO" ON "CRM"."TB_LISTA_FAIXA_PESO" ("AUTONUM") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Index IND_PRECO_FIXO_LISTA
--------------------------------------------------------

  CREATE INDEX "CRM"."IND_PRECO_FIXO_LISTA" ON "CRM"."TB_LISTA_PRECO_SERVICOS_FIXOS" ("LISTA", "SERVICO") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Index IDX_CRM_BUSCA
--------------------------------------------------------

  CREATE INDEX "CRM"."IDX_CRM_BUSCA" ON "CRM"."TB_CRM_BUSCA" ("TERMO") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Index IDX_CRM_MERCADORIAS
--------------------------------------------------------

  CREATE INDEX "CRM"."IDX_CRM_MERCADORIAS" ON "CRM"."TB_CRM_MERCADORIAS" ("DESCRICAO") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Index IDX_CRM_CONTATOS
--------------------------------------------------------

  CREATE INDEX "CRM"."IDX_CRM_CONTATOS" ON "CRM"."TB_CRM_CONTATOS" ("NOME") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Index ID_FAIXA_PESO_TIPO
--------------------------------------------------------

  CREATE INDEX "CRM"."ID_FAIXA_PESO_TIPO" ON "CRM"."TB_LISTA_FAIXA_PESO" ("TIPO") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Index IDX_CRM_CONTAS
--------------------------------------------------------

  CREATE INDEX "CRM"."IDX_CRM_CONTAS" ON "CRM"."TB_CRM_CONTAS" ("DESCRICAO") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Index PK_LISTA_CFG_VALORMINIMO
--------------------------------------------------------

  CREATE UNIQUE INDEX "CRM"."PK_LISTA_CFG_VALORMINIMO" ON "CRM"."TB_LISTA_CFG_VALORMINIMO" ("AUTONUM") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Index IDX_LISTA_P_S_PERIODO
--------------------------------------------------------

  CREATE INDEX "CRM"."IDX_LISTA_P_S_PERIODO" ON "CRM"."TB_LISTA_P_S_PERIODO" ("LISTA", "SERVICO") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Index PK_TB_LP_IMPOSTOS
--------------------------------------------------------

  CREATE UNIQUE INDEX "CRM"."PK_TB_LP_IMPOSTOS" ON "CRM"."TB_LP_IMPOSTOS" ("ID_TABELA", "ID_IMPOSTO") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Index PK_LISTA_P_S_FAIXASCIF_FIX
--------------------------------------------------------

  CREATE UNIQUE INDEX "CRM"."PK_LISTA_P_S_FAIXASCIF_FIX" ON "CRM"."TB_LISTA_P_S_FAIXASCIF_FIX" ("AUTONUM") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI" ;
--------------------------------------------------------
--  DDL for Trigger TGR_CRM_CONTAS_DELETE
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "CRM"."TGR_CRM_CONTAS_DELETE" 
AFTER DELETE
   ON CRM.TB_CRM_CONTAS FOR EACH ROW
BEGIN
   DELETE FROM CRM.TB_CRM_BUSCA WHERE CHAVE = :old.ID AND Menu = 'Contas';
END;
/
ALTER TRIGGER "CRM"."TGR_CRM_CONTAS_DELETE" ENABLE;
--------------------------------------------------------
--  DDL for Trigger TGR_CRM_CONTAS_INSERT
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "CRM"."TGR_CRM_CONTAS_INSERT" 
AFTER INSERT
   ON CRM.TB_CRM_CONTAS FOR EACH ROW
BEGIN
   INSERT INTO CRM.TB_CRM_BUSCA (ID, TERMO, MENU, CHAVE) VALUES (CRM.SEQ_CRM_BUSCA.NEXTVAL, :new.DESCRICAO || ' (' || :new.DOCUMENTO || ')' || ' ' || REPLACE(REPLACE(REPLACE(:new.DOCUMENTO, '.', ''), '/', ''), '-',''), 'Contas', :new.ID);
END;
/
ALTER TRIGGER "CRM"."TGR_CRM_CONTAS_INSERT" ENABLE;
--------------------------------------------------------
--  DDL for Trigger TGR_CRM_CONTAS_UPDATE
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "CRM"."TGR_CRM_CONTAS_UPDATE" 
AFTER UPDATE
   ON CRM.TB_CRM_CONTAS FOR EACH ROW
BEGIN
   UPDATE CRM.TB_CRM_BUSCA SET TERMO = :new.DESCRICAO || ' (' || :new.DOCUMENTO || ')' || ' ' || REPLACE(REPLACE(REPLACE(:new.DOCUMENTO, '.', ''), '/', ''), '-','') WHERE CHAVE = :old.ID AND Menu = 'Contas';
END;
/
ALTER TRIGGER "CRM"."TGR_CRM_CONTAS_UPDATE" ENABLE;
--------------------------------------------------------
--  DDL for Trigger TGR_CRM_CONTATOS_DELETE
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "CRM"."TGR_CRM_CONTATOS_DELETE" 
AFTER DELETE
   ON CRM.TB_CRM_CONTATOS FOR EACH ROW
BEGIN
   DELETE FROM CRM.TB_CRM_BUSCA WHERE CHAVE = :old.ID  AND Menu = 'Contatos';
END;
/
ALTER TRIGGER "CRM"."TGR_CRM_CONTATOS_DELETE" ENABLE;
--------------------------------------------------------
--  DDL for Trigger TGR_CRM_CONTATOS_INSERT
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "CRM"."TGR_CRM_CONTATOS_INSERT" 
AFTER INSERT
   ON CRM.TB_CRM_CONTATOS FOR EACH ROW
BEGIN
   INSERT INTO CRM.TB_CRM_BUSCA (ID, TERMO, MENU, CHAVE) VALUES (CRM.SEQ_CRM_BUSCA.NEXTVAL, :new.NOME, 'Contatos', :new.ID);
END;
/
ALTER TRIGGER "CRM"."TGR_CRM_CONTATOS_INSERT" ENABLE;
--------------------------------------------------------
--  DDL for Trigger TGR_CRM_CONTATOS_UPDATE
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "CRM"."TGR_CRM_CONTATOS_UPDATE" 
AFTER UPDATE
   ON CRM.TB_CRM_CONTATOS FOR EACH ROW
BEGIN
   UPDATE CRM.TB_CRM_BUSCA SET TERMO = :new.NOME WHERE CHAVE = :old.ID AND Menu = 'Contatos';
END;
/
ALTER TRIGGER "CRM"."TGR_CRM_CONTATOS_UPDATE" ENABLE;
--------------------------------------------------------
--  DDL for Trigger TGR_CRM_MERCADORIAS_DELETE
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "CRM"."TGR_CRM_MERCADORIAS_DELETE" 
AFTER DELETE
   ON CRM.TB_CRM_MERCADORIAS FOR EACH ROW
BEGIN
     DELETE FROM CRM.TB_CRM_BUSCA WHERE CHAVE = :old.ID  AND Menu = 'Mercadorias';
END;
/
ALTER TRIGGER "CRM"."TGR_CRM_MERCADORIAS_DELETE" ENABLE;
--------------------------------------------------------
--  DDL for Trigger TGR_CRM_MERCADORIAS_INSERT
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "CRM"."TGR_CRM_MERCADORIAS_INSERT" 
AFTER INSERT
   ON CRM.TB_CRM_MERCADORIAS FOR EACH ROW
BEGIN
   INSERT INTO CRM.TB_CRM_BUSCA (ID, TERMO, MENU, CHAVE) VALUES (CRM.SEQ_CRM_BUSCA.NEXTVAL, :new.DESCRICAO, 'Mercadorias', :new.ID);
END;
/
ALTER TRIGGER "CRM"."TGR_CRM_MERCADORIAS_INSERT" ENABLE;
--------------------------------------------------------
--  DDL for Trigger TGR_CRM_MERCADORIAS_UPDATE
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "CRM"."TGR_CRM_MERCADORIAS_UPDATE" 
AFTER UPDATE
   ON CRM.TB_CRM_MERCADORIAS FOR EACH ROW
BEGIN
     UPDATE CRM.TB_CRM_BUSCA SET TERMO = :new.DESCRICAO WHERE CHAVE = :old.ID AND Menu = 'Mercadorias';
END;
/
ALTER TRIGGER "CRM"."TGR_CRM_MERCADORIAS_UPDATE" ENABLE;
--------------------------------------------------------
--  DDL for Trigger TGR_CRM_MODELOS_DELETE
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "CRM"."TGR_CRM_MODELOS_DELETE" 
AFTER DELETE
   ON CRM.TB_CRM_MODELO FOR EACH ROW
BEGIN
     DELETE FROM CRM.TB_CRM_BUSCA WHERE CHAVE = :old.ID  AND Menu = 'Modelos';
END;
/
ALTER TRIGGER "CRM"."TGR_CRM_MODELOS_DELETE" ENABLE;
--------------------------------------------------------
--  DDL for Trigger TGR_CRM_MODELOS_INSERT
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "CRM"."TGR_CRM_MODELOS_INSERT" 
AFTER INSERT
   ON CRM.TB_CRM_MODELO FOR EACH ROW
BEGIN
   INSERT INTO CRM.TB_CRM_BUSCA (ID, TERMO, MENU, CHAVE) VALUES (CRM.SEQ_CRM_BUSCA.NEXTVAL, :new.DESCRICAO, 'Modelos', :new.ID);
END;
/
ALTER TRIGGER "CRM"."TGR_CRM_MODELOS_INSERT" ENABLE;
--------------------------------------------------------
--  DDL for Trigger TGR_CRM_MODELOS_UPDATE
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "CRM"."TGR_CRM_MODELOS_UPDATE" 
AFTER UPDATE
   ON CRM.TB_CRM_MODELO FOR EACH ROW
BEGIN
     UPDATE CRM.TB_CRM_BUSCA SET TERMO = :new.DESCRICAO WHERE CHAVE = :old.ID AND Menu = 'Modelos';
END;
/
ALTER TRIGGER "CRM"."TGR_CRM_MODELOS_UPDATE" ENABLE;
--------------------------------------------------------
--  DDL for Trigger TGR_CRM_OPORTUNIDADES_DELETE
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "CRM"."TGR_CRM_OPORTUNIDADES_DELETE" 
AFTER DELETE
   ON CRM.TB_CRM_OPORTUNIDADES FOR EACH ROW
BEGIN
     DELETE FROM CRM.TB_CRM_BUSCA WHERE CHAVE = :old.ID  AND Menu = 'Oportunidades';
END;
/
ALTER TRIGGER "CRM"."TGR_CRM_OPORTUNIDADES_DELETE" ENABLE;
--------------------------------------------------------
--  DDL for Trigger TGR_CRM_OPORTUNIDADES_INSERT
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "CRM"."TGR_CRM_OPORTUNIDADES_INSERT" 
AFTER INSERT
   ON CRM.TB_CRM_OPORTUNIDADES FOR EACH ROW
BEGIN
   INSERT INTO CRM.TB_CRM_BUSCA (ID, TERMO, MENU, CHAVE) VALUES (CRM.SEQ_CRM_BUSCA.NEXTVAL, :new.IDENTIFICACAO || ' ' || :new.DESCRICAO, 'Oportunidades', :new.ID);
END;
/
ALTER TRIGGER "CRM"."TGR_CRM_OPORTUNIDADES_INSERT" ENABLE;
--------------------------------------------------------
--  DDL for Trigger TGR_CRM_OPORTUNIDADES_UPDATE
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "CRM"."TGR_CRM_OPORTUNIDADES_UPDATE" 
AFTER UPDATE
   ON CRM.TB_CRM_OPORTUNIDADES FOR EACH ROW
BEGIN
     UPDATE CRM.TB_CRM_BUSCA SET TERMO = :new.DESCRICAO WHERE CHAVE = :old.ID AND Menu = 'Oportunidades';
END;
/
ALTER TRIGGER "CRM"."TGR_CRM_OPORTUNIDADES_UPDATE" ENABLE;
--------------------------------------------------------
--  DDL for Trigger TGR_CRM_SERVICOS_DELETE
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "CRM"."TGR_CRM_SERVICOS_DELETE" 
AFTER DELETE
   ON CRM.TB_CRM_SERVICOS FOR EACH ROW
BEGIN
     DELETE FROM CRM.TB_CRM_BUSCA WHERE CHAVE = :old.ID  AND Menu = 'Serviços';
END;
/
ALTER TRIGGER "CRM"."TGR_CRM_SERVICOS_DELETE" ENABLE;
--------------------------------------------------------
--  DDL for Trigger TGR_CRM_SERVICOS_INSERT
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "CRM"."TGR_CRM_SERVICOS_INSERT" 
AFTER INSERT
   ON CRM.TB_CRM_SERVICOS FOR EACH ROW
BEGIN
   INSERT INTO CRM.TB_CRM_BUSCA (ID, TERMO, MENU, CHAVE) VALUES (CRM.SEQ_CRM_BUSCA.NEXTVAL, :new.DESCRICAO, 'Serviços', :new.ID);
END;
/
ALTER TRIGGER "CRM"."TGR_CRM_SERVICOS_INSERT" ENABLE;
--------------------------------------------------------
--  DDL for Trigger TGR_CRM_SERVICOS_UPDATE
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "CRM"."TGR_CRM_SERVICOS_UPDATE" 
AFTER UPDATE
   ON CRM.TB_CRM_SERVICOS FOR EACH ROW
BEGIN
     UPDATE CRM.TB_CRM_BUSCA SET TERMO = :new.DESCRICAO WHERE CHAVE = :old.ID AND Menu = 'Serviços';
END;
/
ALTER TRIGGER "CRM"."TGR_CRM_SERVICOS_UPDATE" ENABLE;
--------------------------------------------------------
--  DDL for Function SAFE_TO_NUMBER
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "CRM"."SAFE_TO_NUMBER" (p varchar2) return number is
    v number;
  begin
    
    v := to_number(replace(LTRIM(rtrim(p)), '.', ',' ));
    return v;
  exception when others then return 0;
end; 

/
--------------------------------------------------------
--  Constraints for Table TB_CRM_OPORTUNIDADES
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_OPORTUNIDADES" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
  ALTER TABLE "CRM"."TB_CRM_OPORTUNIDADES" MODIFY ("CONTAID" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_OPORTUNIDADES" MODIFY ("ID" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table TB_CRM_USUARIOS_CARGOS
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_USUARIOS_CARGOS" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
  ALTER TABLE "CRM"."TB_CRM_USUARIOS_CARGOS" MODIFY ("ID" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table TB_CRM_IMPOSTOS
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_IMPOSTOS" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
--------------------------------------------------------
--  Constraints for Table TB_CRM_MODELO
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_MODELO" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
--------------------------------------------------------
--  Constraints for Table TB_LISTA_FAIXA_PESO
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_LISTA_FAIXA_PESO" MODIFY ("PRECO" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_LISTA_FAIXA_PESO" ADD CONSTRAINT "PK_TB_LISTA_FAIXA_PESO" PRIMARY KEY ("AUTONUM")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
--------------------------------------------------------
--  Constraints for Table TB_CRM_LAYOUT_VL_PESO
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_LAYOUT_VL_PESO" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
--------------------------------------------------------
--  Constraints for Table TB_CRM_ESTADOS
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_ESTADOS" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
--------------------------------------------------------
--  Constraints for Table TB_CRM_ANEXOS
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_ANEXOS" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
  ALTER TABLE "CRM"."TB_CRM_ANEXOS" MODIFY ("TIPOANEXO" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_ANEXOS" MODIFY ("DATACADASTRO" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_ANEXOS" MODIFY ("ANEXO" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_ANEXOS" MODIFY ("IDPROCESSO" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_ANEXOS" MODIFY ("ID" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table TB_CRM_USUARIOS_PERMISSOES
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_USUARIOS_PERMISSOES" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
  ALTER TABLE "CRM"."TB_CRM_USUARIOS_PERMISSOES" MODIFY ("CARGOID" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_USUARIOS_PERMISSOES" MODIFY ("MENUID" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_USUARIOS_PERMISSOES" MODIFY ("ID" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table TB_CRM_SERVICOS
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_SERVICOS" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
--------------------------------------------------------
--  Constraints for Table TB_CRM_OPORTUNIDADE_GRUPO_CNPJ
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_OPORTUNIDADE_GRUPO_CNPJ" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
  ALTER TABLE "CRM"."TB_CRM_OPORTUNIDADE_GRUPO_CNPJ" MODIFY ("CONTAID" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_OPORTUNIDADE_GRUPO_CNPJ" MODIFY ("ID" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table TB_LISTA_CFG_VALORMINIMO
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_LISTA_CFG_VALORMINIMO" ADD CONSTRAINT "PK_LISTA_CFG_VALORMINIMO" PRIMARY KEY ("AUTONUM")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
--------------------------------------------------------
--  Constraints for Table TB_CRM_OPORTUNIDADE_PREMIOS
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_OPORTUNIDADE_PREMIOS" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
  ALTER TABLE "CRM"."TB_CRM_OPORTUNIDADE_PREMIOS" MODIFY ("CRIADOPOR" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_OPORTUNIDADE_PREMIOS" MODIFY ("OPORTUNIDADEID" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_OPORTUNIDADE_PREMIOS" MODIFY ("ID" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table TB_CRM_CONTATOS
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_CONTATOS" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
--------------------------------------------------------
--  Constraints for Table TB_CRM_ADENDO_GRUPO_CNPJ
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_ADENDO_GRUPO_CNPJ" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
  ALTER TABLE "CRM"."TB_CRM_ADENDO_GRUPO_CNPJ" MODIFY ("ADENDOID" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_ADENDO_GRUPO_CNPJ" MODIFY ("ID" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table TB_CRM_ADENDO_SUB_CLIENTE
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_ADENDO_SUB_CLIENTE" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
  ALTER TABLE "CRM"."TB_CRM_ADENDO_SUB_CLIENTE" MODIFY ("SUBCLIENTEID" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_ADENDO_SUB_CLIENTE" MODIFY ("ADENDOID" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_ADENDO_SUB_CLIENTE" MODIFY ("ID" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table TB_CRM_ADENDO_FORMA_PAGAMENTO
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_ADENDO_FORMA_PAGAMENTO" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
  ALTER TABLE "CRM"."TB_CRM_ADENDO_FORMA_PAGAMENTO" MODIFY ("FORMAPAGAMENTO" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_ADENDO_FORMA_PAGAMENTO" MODIFY ("ADENDOID" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_ADENDO_FORMA_PAGAMENTO" MODIFY ("ID" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table TB_CRM_SOLICITACAO_CANCEL_NF
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_SOLICITACAO_CANCEL_NF" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
  ALTER TABLE "CRM"."TB_CRM_SOLICITACAO_CANCEL_NF" MODIFY ("ID" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table TB_CRM_SOLICITACOES
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_SOLICITACOES" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
  ALTER TABLE "CRM"."TB_CRM_SOLICITACOES" MODIFY ("ID" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table TB_CRM_SOLICITACAO_PRORROGACAO
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_SOLICITACAO_PRORROGACAO" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
  ALTER TABLE "CRM"."TB_CRM_SOLICITACAO_PRORROGACAO" MODIFY ("SOLICITACAOID" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_SOLICITACAO_PRORROGACAO" MODIFY ("ID" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table TB_CRM_LAYOUT_VL_CIF_BL
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_LAYOUT_VL_CIF_BL" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
--------------------------------------------------------
--  Constraints for Table TB_LISTA_P_S_FAIXASCIF_PER
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_LISTA_P_S_FAIXASCIF_PER" MODIFY ("PERCENTUAL" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_LISTA_P_S_FAIXASCIF_PER" ADD CONSTRAINT "PK_TB_LISTA_P_S_FAIXASCIF" PRIMARY KEY ("AUTONUM")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
--------------------------------------------------------
--  Constraints for Table TB_CRM_SERVICO_IPA
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_SERVICO_IPA" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
--------------------------------------------------------
--  Constraints for Table TB_CRM_OPORTUNIDADE_FICHA_FAT
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_OPORTUNIDADE_FICHA_FAT" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
  ALTER TABLE "CRM"."TB_CRM_OPORTUNIDADE_FICHA_FAT" MODIFY ("CONTAID" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_OPORTUNIDADE_FICHA_FAT" MODIFY ("OPORTUNIDADEID" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_OPORTUNIDADE_FICHA_FAT" MODIFY ("ID" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table TB_CRM_AUDITORIA
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_AUDITORIA" MODIFY ("ID" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table TB_CRM_BUSCA
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_BUSCA" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
  ALTER TABLE "CRM"."TB_CRM_BUSCA" MODIFY ("CHAVE" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_BUSCA" MODIFY ("MENU" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_BUSCA" MODIFY ("ID" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table TB_CRM_EQUIPES_VENDEDOR
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_EQUIPES_VENDEDOR" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
  ALTER TABLE "CRM"."TB_CRM_EQUIPES_VENDEDOR" MODIFY ("USUARIOID" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_EQUIPES_VENDEDOR" MODIFY ("VENDEDORID" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_EQUIPES_VENDEDOR" MODIFY ("ID" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table TB_CRM_MENUS_CAMPOS
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_MENUS_CAMPOS" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
  ALTER TABLE "CRM"."TB_CRM_MENUS_CAMPOS" MODIFY ("OBJETOID" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_MENUS_CAMPOS" MODIFY ("MENUID" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_MENUS_CAMPOS" MODIFY ("ID" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table TB_CRM_EQUIPES_CONTA
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_EQUIPES_CONTA" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
  ALTER TABLE "CRM"."TB_CRM_EQUIPES_CONTA" MODIFY ("USUARIOID" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_EQUIPES_CONTA" MODIFY ("CONTAID" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_EQUIPES_CONTA" MODIFY ("ID" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table TB_CRM_SOLICITACAO_DESCONTO
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_SOLICITACAO_DESCONTO" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
  ALTER TABLE "CRM"."TB_CRM_SOLICITACAO_DESCONTO" MODIFY ("SOLICITACAOID" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_SOLICITACAO_DESCONTO" MODIFY ("ID" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table TB_CRM_SOLICITACAO_RESTITUICAO
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_SOLICITACAO_RESTITUICAO" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
  ALTER TABLE "CRM"."TB_CRM_SOLICITACAO_RESTITUICAO" MODIFY ("SOLICITACAOID" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_SOLICITACAO_RESTITUICAO" MODIFY ("ID" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table TB_LISTA_PRECO_SERVICOS_FIXOS
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_LISTA_PRECO_SERVICOS_FIXOS" MODIFY ("PESO_LIMITE" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_LISTA_PRECO_SERVICOS_FIXOS" MODIFY ("VALOR_ACRESC_PESO" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_LISTA_PRECO_SERVICOS_FIXOS" MODIFY ("VALOR_ACRESCIMO" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_LISTA_PRECO_SERVICOS_FIXOS" MODIFY ("PRECO_MINIMO" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_LISTA_PRECO_SERVICOS_FIXOS" MODIFY ("PRECO_UNITARIO" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_LISTA_PRECO_SERVICOS_FIXOS" ADD CONSTRAINT "PK_LISTA_PRECO_FIXO" PRIMARY KEY ("AUTONUM")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
--------------------------------------------------------
--  Constraints for Table TB_CRM_WORKFLOW
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_WORKFLOW" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
  ALTER TABLE "CRM"."TB_CRM_WORKFLOW" MODIFY ("DATACADASTRO" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_WORKFLOW" MODIFY ("PROCESSOID" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_WORKFLOW" MODIFY ("ID" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table TB_LISTA_P_S_PERIODO
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_LISTA_P_S_PERIODO" ADD CONSTRAINT "PK_LISTA_P_S_PERIODO" PRIMARY KEY ("AUTONUM")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
--------------------------------------------------------
--  Constraints for Table TB_CRM_OPORTUNIDADE_CLIENTES
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_OPORTUNIDADE_CLIENTES" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
  ALTER TABLE "CRM"."TB_CRM_OPORTUNIDADE_CLIENTES" MODIFY ("SEGMENTO" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_OPORTUNIDADE_CLIENTES" MODIFY ("CONTAID" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_OPORTUNIDADE_CLIENTES" MODIFY ("ID" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table TB_CRM_LAYOUT_VL_VOLUME
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_LAYOUT_VL_VOLUME" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
--------------------------------------------------------
--  Constraints for Table TB_LISTA_P_S_FAIXASCIF_FIX
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_LISTA_P_S_FAIXASCIF_FIX" MODIFY ("PERCENTUAL" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_LISTA_P_S_FAIXASCIF_FIX" ADD CONSTRAINT "PK_LISTA_P_S_FAIXASCIF_FIX" PRIMARY KEY ("AUTONUM")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
--------------------------------------------------------
--  Constraints for Table TB_CRM_SUB_MENUS
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_SUB_MENUS" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
  ALTER TABLE "CRM"."TB_CRM_SUB_MENUS" MODIFY ("MENUID" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_SUB_MENUS" MODIFY ("ID" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table TB_CRM_CIDADES
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_CIDADES" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
--------------------------------------------------------
--  Constraints for Table TB_CRM_EQUIPES_OPORTUNIDADE
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_EQUIPES_OPORTUNIDADE" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
  ALTER TABLE "CRM"."TB_CRM_EQUIPES_OPORTUNIDADE" MODIFY ("USUARIOID" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_EQUIPES_OPORTUNIDADE" MODIFY ("OPORTUNIDADEID" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_EQUIPES_OPORTUNIDADE" MODIFY ("ID" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table TB_CRM_LAYOUT
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_LAYOUT" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
--------------------------------------------------------
--  Constraints for Table TB_LP_IMPOSTOS
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_LP_IMPOSTOS" ADD CONSTRAINT "PK_TB_LP_IMPOSTOS" PRIMARY KEY ("ID_TABELA", "ID_IMPOSTO")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
--------------------------------------------------------
--  Constraints for Table TB_CRM_USUARIOS
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_USUARIOS" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
  ALTER TABLE "CRM"."TB_CRM_USUARIOS" MODIFY ("CARGOID" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_USUARIOS" MODIFY ("ATIVO" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_USUARIOS" MODIFY ("ADMINISTRADOR" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_USUARIOS" MODIFY ("NOME" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_USUARIOS" MODIFY ("LOGIN" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_USUARIOS" MODIFY ("ID" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table TB_CRM_USUARIOS_PERMISSAO_TELA
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_USUARIOS_PERMISSAO_TELA" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
  ALTER TABLE "CRM"."TB_CRM_USUARIOS_PERMISSAO_TELA" MODIFY ("CAMPOID" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_USUARIOS_PERMISSAO_TELA" MODIFY ("PERMISSAOID" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_USUARIOS_PERMISSAO_TELA" MODIFY ("ID" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table TB_CRM_MENUS
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_MENUS" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
  ALTER TABLE "CRM"."TB_CRM_MENUS" MODIFY ("ID" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table TB_CRM_LAYOUT_VL_MINIMO_BL
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_LAYOUT_VL_MINIMO_BL" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
--------------------------------------------------------
--  Constraints for Table TB_CRM_ADENDO_VENDEDOR
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_ADENDO_VENDEDOR" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
  ALTER TABLE "CRM"."TB_CRM_ADENDO_VENDEDOR" MODIFY ("VENDEDORID" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_ADENDO_VENDEDOR" MODIFY ("ADENDOID" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_ADENDO_VENDEDOR" MODIFY ("ID" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table TB_LISTA_FAIXA_VOLUME
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_LISTA_FAIXA_VOLUME" MODIFY ("PRECO" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_LISTA_FAIXA_VOLUME" ADD CONSTRAINT "PK_TB_LISTA_FAIXA_VOLUME" PRIMARY KEY ("AUTONUM")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
--------------------------------------------------------
--  Constraints for Table TB_CRM_SOLICITACAO_MOTIVOS
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_SOLICITACAO_MOTIVOS" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
  ALTER TABLE "CRM"."TB_CRM_SOLICITACAO_MOTIVOS" MODIFY ("ID" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table TB_CRM_MERCADORIAS
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_MERCADORIAS" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
  ALTER TABLE "CRM"."TB_CRM_MERCADORIAS" MODIFY ("ID" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table TB_CRM_OPORTUNIDADE_ADENDOS
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_OPORTUNIDADE_ADENDOS" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
  ALTER TABLE "CRM"."TB_CRM_OPORTUNIDADE_ADENDOS" MODIFY ("DATACADASTRO" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_OPORTUNIDADE_ADENDOS" MODIFY ("STATUSADENDO" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_OPORTUNIDADE_ADENDOS" MODIFY ("TIPOADENDO" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_OPORTUNIDADE_ADENDOS" MODIFY ("OPORTUNIDADEID" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_OPORTUNIDADE_ADENDOS" MODIFY ("ID" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table TB_CRM_PAISES
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_PAISES" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
--------------------------------------------------------
--  Constraints for Table TB_CRM_SOLICITACAO_OCORRENCIAS
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_SOLICITACAO_OCORRENCIAS" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
  ALTER TABLE "CRM"."TB_CRM_SOLICITACAO_OCORRENCIAS" MODIFY ("ID" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table TB_CRM_PREMIOS_MODALIDADES
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_PREMIOS_MODALIDADES" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
  ALTER TABLE "CRM"."TB_CRM_PREMIOS_MODALIDADES" MODIFY ("OPORTUNIDADEPREMIOID" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_PREMIOS_MODALIDADES" MODIFY ("OPORTUNIDADEID" NOT NULL ENABLE);
  ALTER TABLE "CRM"."TB_CRM_PREMIOS_MODALIDADES" MODIFY ("ID" NOT NULL ENABLE);
--------------------------------------------------------
--  Constraints for Table TB_CRM_CONTAS
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_CONTAS" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
--------------------------------------------------------
--  Constraints for Table TB_CRM_OPORTUNIDADE_LAYOUT
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_OPORTUNIDADE_LAYOUT" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "DADOS_TECONDI"  ENABLE;
--------------------------------------------------------
--  Ref Constraints for Table TB_CRM_USUARIOS_PERMISSAO_TELA
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_USUARIOS_PERMISSAO_TELA" ADD CONSTRAINT "FK_CAMPOS" FOREIGN KEY ("CAMPOID")
	  REFERENCES "CRM"."TB_CRM_MENUS_CAMPOS" ("ID") ENABLE;
--------------------------------------------------------
--  Ref Constraints for Table TB_CRM_USUARIOS_PERMISSOES
--------------------------------------------------------

  ALTER TABLE "CRM"."TB_CRM_USUARIOS_PERMISSOES" ADD CONSTRAINT "FK_CARGO" FOREIGN KEY ("CARGOID")
	  REFERENCES "CRM"."TB_CRM_USUARIOS_CARGOS" ("ID") ENABLE;
  ALTER TABLE "CRM"."TB_CRM_USUARIOS_PERMISSOES" ADD CONSTRAINT "FK_MENU_MENU" FOREIGN KEY ("MENUID")
	  REFERENCES "CRM"."TB_CRM_MENUS" ("ID") ENABLE;
