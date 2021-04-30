using Dapper;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Linq;

namespace Ecoporto.CRM.Infra.Repositorios
{
    public class BookingRepositorio : IBookingRepositorio
    {       
        public Booking ObterBookingPorReserva(string reserva)
        {
            using (OracleConnection con = new OracleConnection(Config.StringConexao()))
            {
                var parametros = new DynamicParameters();
                parametros.Add(name: "Reference", value: reserva, direction: ParameterDirection.Input);

                return con.Query<Booking>(@"
                    SELECT 
                        A.AUTONUM_BOO As Id, 
                        A.Reference, 
                        A.AUTONUM_PARCEIRO As ExportadorId,
                        C.CGC As ExportadorCnpj
                   FROM 
                        REDEX.TB_BOOKING A
                    INNER JOIN
                        REDEX.TB_GR_BOOKING B ON B.Booking = A.AUTONUM_BOO
                    LEFT JOIN
	                    REDEX.TB_CAD_PARCEIROS C ON B.CLIENTE_FATURA = C.AUTONUM
                   WHERE 
                        A.Reference = :Reference", parametros).FirstOrDefault();
            }
        }
    }
}
