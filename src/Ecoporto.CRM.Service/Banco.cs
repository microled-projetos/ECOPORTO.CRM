using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Ecoporto.CRM.Service
{
    public class Banco
    {
        private static readonly string _stringConexao;

        static Banco()
        {
            _stringConexao = ConfigurationManager.ConnectionStrings["StringConexaoOracle"].ConnectionString;
        }

        public static bool Execute(string SQL, OracleParameter[] parametros)
        {
            object retorno = null;

            using (OracleConnection con = new OracleConnection(_stringConexao))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = SQL;
                    cmd.Connection = con;

                    cmd.Parameters.AddRange(parametros);

                    try
                    {
                        con.Open();
                        retorno = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    catch
                    {

                        throw;
                    }


                    return Convert.ToInt32(retorno) > 0;
                }
            }
        }

        public static object ExecuteScalar(string SQL, List<OracleParameter> parametros)
        {
            object retorno = null;

            using (OracleConnection con = new OracleConnection(_stringConexao))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = SQL;
                    cmd.Connection = con;

                    foreach (var parametro in parametros)
                        cmd.Parameters.Add(parametro);

                    con.Open();
                    retorno = cmd.ExecuteScalar();
                    con.Close();

                    return retorno;
                }
            }
        }

        public static OracleDataReader ExecuteReader(string SQL, OracleConnection con, List<OracleParameter> parametros)
        {
            using (OracleCommand cmd = new OracleCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = SQL;
                cmd.Connection = con;

                foreach (var parametro in parametros)
                    cmd.Parameters.Add(parametro);

                con.Open();
                var retorno = cmd.ExecuteReader();

                return retorno;
            }
        }
    }
}
