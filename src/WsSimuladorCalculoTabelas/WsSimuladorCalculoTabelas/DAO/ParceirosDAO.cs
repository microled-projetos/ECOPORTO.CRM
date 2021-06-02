using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using WsSimuladorCalculoTabelas.Configuracao;
using WsSimuladorCalculoTabelas.Models;

namespace WsSimuladorCalculoTabelas.DAO
{
    public class ParceirosDAO
    {
        public Parceiro ObterRazaoSocial(bool dadosDoCliente, int classe, int tabelaId)
        {
            string filtro = string.Empty;
            string SQL = string.Empty;

            if (Configuracoes.BancoEmUso() == "ORACLE")
            {
                if (dadosDoCliente)
                    filtro = " A.FANTASIA || ' (' || A.CGC || ')' || ' ' || DECODE(FORMA_PAGAMENTO,2,'A VISTA','FATURADO') AS Descricao ";
                else
                    filtro = " A.FANTASIA AS CLIENTE ";

                switch (classe)
                {
                    case 0:
                    case 1:
                        SQL = $"SELECT {filtro} FROM SGIPA.TB_CAD_PARCEIROS A INNER JOIN SGIPA.TB_LISTAS_PRECOS B ON A.AUTONUM = B.IMPORTADOR WHERE B.AUTONUM = :TabelaId";
                        break;
                    case 2:
                        SQL = $"SELECT {filtro} FROM SGIPA.TB_CAD_PARCEIROS A INNER JOIN SGIPA.TB_LISTAS_PRECOS B ON A.AUTONUM = B.DESPACHANTE WHERE B.AUTONUM = :TabelaId";
                        break;
                    case 3:
                        SQL = $"SELECT {filtro} FROM SGIPA.TB_CAD_PARCEIROS A INNER JOIN SGIPA.TB_LISTAS_PRECOS B ON A.AUTONUM = B.CAPTADOR WHERE B.AUTONUM = :TabelaId";
                        break;
                    case 4:
                        SQL = $"SELECT {filtro} FROM SGIPA.TB_CAD_PARCEIROS A INNER JOIN SGIPA.TB_LISTAS_PRECOS B ON A.AUTONUM = B.COLOADER WHERE B.AUTONUM = :TabelaId";
                        break;
                }

                using (OracleConnection con = new OracleConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                    return con.Query<Parceiro>(SQL, parametros).FirstOrDefault();
                }
            }
            else
            {                
                if (dadosDoCliente)
                    filtro = " A.FANTASIA + ' (' + A.CGC + ')' + ' ' + CASE WHEN FORMA_PAGAMENTO = 2 THEN 'A VISTA' ELSE 'FATURADO' END AS Descricao ";
                else
                    filtro = " A.FANTASIA AS CLIENTE ";

                switch (classe)
                {
                    case 0:
                    case 1:
                        SQL = $"SELECT {filtro} FROM SGIPA..TB_CAD_PARCEIROS A INNER JOIN SGIPA..TB_LISTAS_PRECOS B ON A.AUTONUM = B.IMPORTADOR WHERE B.AUTONUM = @TabelaId";
                        break;
                    case 2:
                        SQL = $"SELECT {filtro} FROM SGIPA..TB_CAD_PARCEIROS A INNER JOIN SGIPA..TB_LISTAS_PRECOS B ON A.AUTONUM = B.DESPACHANTE WHERE B.AUTONUM = @TabelaId";
                        break;
                    case 3:
                        SQL = $"SELECT {filtro} FROM SGIPA..TB_CAD_PARCEIROS A INNER JOIN SGIPA..TB_LISTAS_PRECOS B ON A.AUTONUM = B.CAPTADOR WHERE B.AUTONUM = @TabelaId";
                        break;
                    case 4:
                        SQL = $"SELECT {filtro} FROM SGIPA..TB_CAD_PARCEIROS A INNER JOIN SGIPA..TB_LISTAS_PRECOS B ON A.AUTONUM = B.COLOADER WHERE B.AUTONUM = @TabelaId";
                        break;
                }

                using (SqlConnection con = new SqlConnection(Configuracoes.StringConexao()))
                {
                    var parametros = new DynamicParameters();
                    parametros.Add(name: "TabelaId", value: tabelaId, direction: ParameterDirection.Input);

                    return con.Query<Parceiro>(SQL, parametros).FirstOrDefault();
                }
            }                
        }
    }
}