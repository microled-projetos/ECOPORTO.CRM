using Dapper;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Ecoporto.CRM.Infra.Repositorios
{
    public class NotaFiscalRepositorio : INotaFiscalRepositorio
    {             
        public IEnumerable<NotaFiscal> ObterNotasFiscais(int nfe)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "NFE", value: nfe, direction: ParameterDirection.Input);

                return con.Query<NotaFiscal>(@"SELECT Id, NFE, Valor, NOMCLI As Cliente, DT_VENCIMENTO As DataVencimento, DT_EMISSAO As DataEmissao FROM FATURA.FATURANOTA WHERE NFE = :NFE AND StatusNFE = 3", parametros);
            }
        }

        public NotaFiscal ObterDetalhesNotaFiscal(int nfeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: nfeId, direction: ParameterDirection.Input);

                return con.Query<NotaFiscal>(@"
                    SELECT 
                        DISTINCT
                            A.Id, 
                            A.NFE, 
                            A.Valor, 
                            A.GR,
                            A.CGCCPF As DocumentoCliente,
                            A.NOMCLI As Cliente, 
                            A.DT_VENCIMENTO As DataVencimento,
                            A.DOCUMENTO_ORIGEM As Documento,
                            B.RPSNUM As RPS,                                                
                            DECODE(D.FORMA_PAGAMENTO, 2, 1, 3, 2) As FormaPagamento, 
                            A.DT_EMISSAO As DataEmissao,
                            D.BL As Lote,
                            A.CIDCOB As Cidade
                        FROM 
                            FATURA.FATURANOTA A
                        LEFT JOIN
                            FATURA.RPSFAT B ON B.FATSEQ = A.ID
                        LEFT JOIN 
                            FATURA.FAT_GR C ON C.FATID = A.ID 
                        LEFT JOIN 
                            SGIPA.TB_GR_BL D ON C.SEQ_GR = D.SEQ_GR  
                        WHERE 
                            Id = :Id", parametros).FirstOrDefault();
            }
        }

        public NotaFiscal ObterDetalhesNotaFiscalRedex(int nfeId)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Id", value: nfeId, direction: ParameterDirection.Input);

                return con.Query<NotaFiscal>(@"
                    SELECT 
                        A.Id, 
                        A.NFE, 
                        A.Valor, 
                        A.GR,
                        A.CGCCPF As DocumentoCliente,
                        E.Reference As Reserva,
                        A.NOMCLI As Cliente, 
                        A.DT_VENCIMENTO As DataVencimento,
                        A.DOCUMENTO_ORIGEM As Documento,
                        B.RPSNUM As RPS,                                                
                        DECODE(D.FORMA_PAGAMENTO, 2, 1, 3, 2) As FormaPagamento, 
                        A.DT_EMISSAO As DataEmissao
                    FROM 
                        FATURA.FATURANOTA A
                    LEFT JOIN
                        FATURA.RPSFAT B ON B.FATSEQ = A.ID
                    LEFT JOIN 
                        FATURA.FAT_GR_RED C ON C.FATID = A.ID 
                    LEFT JOIN 
                        REDEX.TB_GR_BOOKING D ON D.SEQ_GR = C.SEQ_GR 
                    LEFT JOIN 
                        REDEX.TB_BOOKING E ON D.Booking = E.AUTONUM_BOO 
                    WHERE 
                        Id = :Id", parametros).FirstOrDefault();
            }
        }
    }
}
