using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Helpers;
using Ecoporto.CRM.Infra.Configuracao;
using Newtonsoft.Json;
using System;
using System.Security.Claims;
using System.Security.Principal;

namespace Ecoporto.CRM.Site.Extensions
{
    public static class UserExtensions
    {
        public static int ObterId(this IPrincipal user)
        {
            var claim = ((ClaimsIdentity)user.Identity).FindFirst("Id");

            return claim == null
                ? 0
                : claim.Value.ToInt();
        }

        public static string ObterNome(this IPrincipal user)
        {
            var claim = ((ClaimsIdentity)user.Identity).FindFirst("Nome");

            return claim?.Value;
        }

        public static string ObterEmail(this IPrincipal user)
        {
            var claim = ((ClaimsIdentity)user.Identity).FindFirst("Email");

            return claim?.Value;
        }

        public static string ObterLogin(this IPrincipal user)
        {
            var claim = ((ClaimsIdentity)user.Identity).FindFirst("Login");

            return claim?.Value;
        }      
    }
}