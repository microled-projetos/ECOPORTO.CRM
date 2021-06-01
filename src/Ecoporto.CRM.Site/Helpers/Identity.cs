using Ecoporto.CRM.Site.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Security.Claims;
using System.Web;
using System.Web.Security;

namespace Ecoporto.CRM.Site.Helpers
{
    public class Identity
    {
        public static void Autenticar()
        {
            var authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie != null)
            {
                var ticket = FormsAuthentication.Decrypt(authCookie.Value);

                FormsIdentity formsIdentity = new FormsIdentity(ticket);

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(formsIdentity);

                var usuario = JsonConvert.DeserializeObject<UsuarioAutenticado>(ticket.UserData);

                if (usuario != null)
                {
                    claimsIdentity.AddClaims(ObterClaims(usuario));

                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    HttpContext.Current.User = claimsPrincipal;
                }
            }
        }

        public static List<Claim> ObterClaims(UsuarioAutenticado usuario)
        {
            MemoryCache cache = MemoryCache.Default;

            var claims = new List<Claim>();

            var permissoesCache = cache[$"U-{HttpContext.Current.User.Identity.Name}.Permissoes"] as string;

            if (permissoesCache != null)
            {
                var permissoes = permissoesCache.Split(',');

                claims = new List<Claim>
                {
                    new Claim("Id", usuario.Id.ToString()),
                    new Claim("Nome", usuario.Nome),
                    new Claim("Email", usuario.Email),
                    new Claim("Login", usuario.Login)
                };

                if (permissoes != null)
                {
                    foreach (var perfil in permissoes)
                        claims.Add(new Claim(ClaimTypes.Role, perfil));
                }
            }
            else
            {
                FormsAuthentication.SignOut();

                HttpContext.Current.Response.Redirect("/Home", true);
            }

            return claims;
        }
    }
}