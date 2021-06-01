using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface IControleAcessoRepositorio
    {
        IEnumerable<PermissaoAcessoMenu> ObterMenus();
        void AplicarPermissoes(int cargoId, IReadOnlyCollection<PermissaoAcesso> permissoes);
        IEnumerable<PermissaoAcessoMenu> ObterPermissoes(int cargoId);
        IEnumerable<Menu> ObterMenusDinamicos();
        bool ExistePermissaoNoCargo(int cargoId);
    }
}
