using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Runtime.Caching;

namespace Ecoporto.CRM.Infra.LDAP
{
    public class ActiveDirectoryService
    {
        private readonly string _servidorLDAP;
        private readonly string _usuario;
        private readonly string _senha;

        public ActiveDirectoryService(string servidorLDAP, string usuario, string senha)
        {
            _servidorLDAP = servidorLDAP;
            _usuario = usuario;
            _senha = senha;
        }

        public DirectorySearcher DirectorySearcher()
        {
            using (var root = new DirectoryEntry(_servidorLDAP, _usuario, _senha))
            {
                return new DirectorySearcher(root, "(&(objectClass=user)(objectCategory=person))");
            }
        }

        public IEnumerable<Dominio> ObterDominios()
        {
            MemoryCache cache = MemoryCache.Default;

            var dominios = cache["ActiveDirectoryService.ObterDominios"] as List<Dominio>;

            if (dominios == null)
            {
                var dominiosAD = new List<Dominio>();

                using (var forest = Forest.GetCurrentForest())
                {
                    foreach (Domain dominio in forest.Domains)
                    {
                        dominiosAD.Add(new Dominio(dominio.Name));
                    }
                }

                cache["ActiveDirectoryService.ObterDominios"] = dominiosAD;
            }

            return dominios;
        }

        public IEnumerable<Usuario> ObterUsuarios()
        {
            //MemoryCache cache = MemoryCache.Default;

            //var usuarios = cache["ActiveDirectoryService.ObterUsuarios"] as List<Usuario>;

            //if (usuarios == null)
            //{
                var usuarios = new List<Usuario>();

                foreach (SearchResult usuario in DirectorySearcher().FindAll())
                {
                    var usuarioAD = new Usuario
                    {
                        Login = usuario.Properties["sAMAccountName"][0].ToString(),
                        Nome = usuario.Properties["cn"][0].ToString(),
                        Email = string.Empty
                    };

                    if (usuario.Properties.Contains("mail"))
                    {
                        usuarioAD.Email = usuario.Properties["mail"][0].ToString();
                    }

                    usuarios.Add(usuarioAD);
                }

                //cache["ActiveDirectoryService.ObterUsuarios"] = usuarios;
            //}

            return usuarios;
        }
    }
}
