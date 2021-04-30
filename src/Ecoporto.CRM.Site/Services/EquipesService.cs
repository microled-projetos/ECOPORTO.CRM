using Ecoporto.CRM.Business.Interfaces.Repositorios;
using System;
using System.Web;

namespace Ecoporto.CRM.Site.Services
{
    public interface IEquipesService
    {
        bool ValidarEquipeOportunidade(int oportunidadeId);
    }

    public class EquipesService : IEquipesService
    {
        private readonly IOportunidadeRepositorio _oportunidadeRepositorio;
        private readonly IEquipeContaRepositorio _equipeContaRepositorio;

        public EquipesService(IOportunidadeRepositorio oportunidadeRepositorio, IEquipeContaRepositorio equipeContaRepositorio)
        {
            _oportunidadeRepositorio = oportunidadeRepositorio;
            _equipeContaRepositorio = equipeContaRepositorio;
        }

        public bool ValidarEquipeOportunidade(int oportunidadeId)
        {
            if (HttpContext.Current.User.IsInRole("Administrador"))
                return true;

            if (HttpContext.Current.User.IsInRole("UsuarioExterno"))
                return true;

            var oportunidadeBusca = _oportunidadeRepositorio.ObterOportunidadePorId(oportunidadeId);

            if (oportunidadeBusca == null)
                throw new Exception("Oportunidade não encontrada ou já excluída");

            var login = HttpContext.Current.User.Identity.Name;

            var permissoesPorVendedor = _equipeContaRepositorio
                .ObterPermissoesOportunidadePorVendedor(oportunidadeBusca.OportunidadeProposta.VendedorId, login);

            // Usuário esta vinculado a equipe do vendedor da oportunidade?

            if (permissoesPorVendedor != null)
            {
                if (permissoesPorVendedor.AcessoOportunidade == 1)
                {
                    return true;
                }
            }
            else
            {
                // Usuário esta vinculado a equipe da conta relacionada à oportunidade?

                var permissoesPorConta = _equipeContaRepositorio
                    .ObterPermissoesContaPorConta(oportunidadeBusca.ContaId, login);

                if (permissoesPorConta != null)
                {
                    if (permissoesPorConta.AcessoOportunidade == 1)
                    {
                        return true;
                    }
                }
                else
                {
                    // Usuário esta vinculado a equipe da oportunidade?

                    var permissoesPorOportunidade = _equipeContaRepositorio.ObterPermissoesPorOportunidade(oportunidadeBusca.Id, login);

                    if (permissoesPorOportunidade != null)
                    {
                        if (permissoesPorOportunidade.AcessoOportunidade == 1)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}