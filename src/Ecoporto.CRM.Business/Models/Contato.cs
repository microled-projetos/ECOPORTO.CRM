using Ecoporto.CRM.Business.Enums;
using FluentValidation;
using System;

namespace Ecoporto.CRM.Business.Models
{
    public class Contato : Entidade<Contato>
    {
        public Contato()
        {

        }

        public Contato(
            string nome,
            string sobrenome,
            string telefone,
            string celular,
            string email,
            string cargo,
            string departamento,
            DateTime? dataNascimento,
            Status status,
            int contaId)
        {
            Nome = nome;
            Sobrenome = sobrenome;
            Telefone = telefone;
            Celular = celular;
            Email = email;
            Cargo = cargo;
            Departamento = departamento;
            DataNascimento = dataNascimento;
            Status = status;
            ContaId = contaId;
        }

        public string Nome { get; set; }

        public string Sobrenome { get; set; }

        public string Telefone { get; set; }

        public string Celular { get; set; }

        public string Email { get; set; }

        public string Cargo { get; set; }

        public string Departamento { get; set; }

        public DateTime? DataNascimento { get; set; }

        public Status Status { get; set; }

        public int ContaId { get; set; }

        public Conta Conta { get; set; }

        public string NomeCompleto => $"{Nome} {Sobrenome}";

        public void Alterar(Contato contato)
        {
            Nome = contato.Nome;
            Sobrenome = contato.Sobrenome;
            Telefone = contato.Telefone;
            Celular = contato.Celular;
            Email = contato.Email;
            Cargo = contato.Cargo;
            Departamento = contato.Departamento;
            DataNascimento = contato.DataNascimento;
            Status = contato.Status;
            ContaId = contato.ContaId;
        }

        public override void Validar()
        {
            RuleFor(c => c.ContaId)
               .GreaterThan(0)
               .WithMessage("O Contato deve estar vinculado a uma Conta");

            RuleFor(c => c.Nome)
               .NotNull()
               .WithMessage("O Nome do contato é obrigatório")
               .MinimumLength(3).WithMessage("Tamanho mínimo de 3 caracteres")
               .MaximumLength(150).WithMessage("Tamanho máximo de 150 caracteres");

            RuleFor(c => c.Sobrenome)
               .NotNull()
               .WithMessage("O Sobrenome do contato é obrigatório")
               .MinimumLength(3).WithMessage("Tamanho mínimo de 3 caracteres")
               .MaximumLength(150).WithMessage("Tamanho máximo de 150 caracteres");

            RuleFor(c => c.Email)
               .NotNull()
               .WithMessage("O Email do contato é obrigatório")
               .MinimumLength(3).WithMessage("Tamanho mínimo de 3 caracteres")
               .MaximumLength(150).WithMessage("Tamanho máximo de 150 caracteres");

            ValidationResult = Validate(this);
        }

        public override string ToString()
        {
            return this.Nome + " " + this.Sobrenome;
        }
    }
}
