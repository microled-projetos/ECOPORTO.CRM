namespace Ecoporto.CRM.IntegraChronosAPI.Security
{
    public class Usuario : System.Web.Services.Protocols.SoapHeader
    {
        public string Login { get; set; }

        public string Senha { get; set; }
    }
}