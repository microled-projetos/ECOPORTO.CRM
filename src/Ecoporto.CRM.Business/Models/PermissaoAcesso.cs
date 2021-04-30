using FluentValidation;
using System.Collections.Generic;
using System.Linq;

namespace Ecoporto.CRM.Business.Models
{
    public class PermissaoAcesso : Entidade<PermissaoAcesso>
    {
        private readonly IList<PermissaoAcesso> _permissoesAcessos;

        public PermissaoAcesso()
        {
            _permissoesAcessos = new List<PermissaoAcesso>();
            Campos = new List<PermissaoAcessoMenuCampos>();
        }

        public int CargoId { get; set; }

        public int MenuId { get; set; }

        public bool Acessar { get; set; }

        public bool Cadastrar { get; set; }

        public bool Atualizar { get; set; }

        public bool Excluir { get; set; }

        public bool Logs { get; set; }

        public IEnumerable<PermissaoAcessoMenuCampos> Campos { get; set; } 

        public IReadOnlyCollection<PermissaoAcesso> PermissoesAcesso
          => _permissoesAcessos.ToList();

        public void IncluirPermissaoAcesso(PermissaoAcesso permissao)
        {
            if (permissao != null)
                _permissoesAcessos.Add(permissao);
        }

        public void RemoverPermissaoAcesso(PermissaoAcesso permissao)
        {
            if (permissao != null)
                _permissoesAcessos.Remove(permissao);
        }

        public override void Validar()
        {
            RuleFor(c => c.PermissoesAcesso.Where(x => x.CargoId > 0 && x.MenuId > 0).Count())
                .GreaterThan(0)
                .WithMessage("O Cargo é obrigatório");

            ValidationResult = Validate(this);
        }
    }
}
