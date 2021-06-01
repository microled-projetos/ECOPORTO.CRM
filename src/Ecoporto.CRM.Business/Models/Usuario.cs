using Ecoporto.CRM.Business.Helpers;
using FluentValidation;

namespace Ecoporto.CRM.Business.Models
{
    public class Usuario : Entidade<Usuario>
    {
        public Usuario()
        {

        }
       
        public Usuario(
            string login,
            string loginExterno,
            string senha,
            string loginWorkflow,
            string nome,
            string email,
            string cpf,
            int cargoId,
            bool administrador,
            bool externo,
            bool remoto,
            bool ativo,
            bool validarIP)
        {
            Login = login;
            LoginExterno = loginExterno;
            Senha = senha;
            LoginWorkflow = loginWorkflow;
            Nome = nome;
            Email = email;
            CPF = cpf;
            CargoId = cargoId;
            Administrador = administrador;
            Externo = externo;
            Remoto = remoto;
            Ativo = ativo;
            ValidarIP = validarIP;            
        }

        public string Login { get; set; }

        public string LoginExterno { get; set; }

        public string Senha { get; set; }

        public string LoginWorkflow { get; set; }

        public string Nome { get; set; }

        public string Email { get; set; }

        public string CPF { get; set; }

        public int CargoId { get; set; }

        public bool Administrador { get; set; }

        public bool Externo { get; set; }

        public bool Remoto { get; set; }

        public bool Ativo { get; set; }

        public int Dominio { get; set; }

        public bool Vendedor { get; set; }

        public bool ValidarIP { get; set; }

        public bool Autenticar(string login, string senha)
        {
            return ((Login == login || LoginExterno == login) && Senha == Criptografia.Encriptar(senha));
        }

        public void AlterarSenha(string senha)
        {
            Senha = Criptografia.Encriptar(senha);
        }

        public override void Validar()
        {
            RuleFor(c => c.Login)
                .NotEmpty()
                .When(c => c.Externo == false)
                .WithMessage("O Login do usuário é obrigatório");

            RuleFor(c => c.LoginExterno)
                .NotEmpty()
                .When(c => c.Externo)
                .WithMessage("O Login Externo do usuário é obrigatório");

            RuleFor(c => c.LoginExterno)
                .Matches(@"^[a-zA-Z0-9]*$")
                .When(c => c.Externo)
                .WithMessage("O Login do usuário não pode conter espaços ou caracteres especiais");

            RuleFor(c => c.Nome)
                .NotEmpty()
                .WithMessage("O Nome do usuário é obrigatório");

            RuleFor(c => c.CargoId)
                .GreaterThan(0)
                .WithMessage("O Cargo do usuário é obrigatório");
          
            RuleFor(c => c.CPF)
                .NotEmpty()
                .When(c => c.Externo)
                .WithMessage("Informe o CPF do usuário");

            RuleFor(c => c.Senha)
                .NotEmpty()
                .When(c => c.Externo)
                .WithMessage("A senha é obrigatória apenas para Usuários Externos");

            RuleFor(c => c.Remoto)
                .Must(ValidarAcessoRemoto)
                .WithMessage("Opção Remoto permitida apenas para usuários internos");

            ValidationResult = Validate(this);
        }

        private bool ValidarAcessoRemoto(bool remoto)
        {
            if (remoto)
            {
                if (!string.IsNullOrEmpty(LoginExterno))
                    return false;
            }

            return true;
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(Login) ? LoginExterno : Login; 
        }
    }
}
