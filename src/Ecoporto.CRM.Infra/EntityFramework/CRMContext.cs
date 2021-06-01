using Ecoporto.CRM.Business.Models;
using System.Data.Entity;

namespace Ecoporto.CRM.Infra.EntityFramework
{
    public class CRMContext : DbContext
    {
        public CRMContext() : base("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=10.1.51.30)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=patiodev)));User Id=CRM;Password=Ec0CRM#0503@;")
        {
        }

        public DbSet<Conta> Contas { get; set; }
    }
}
