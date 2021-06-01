using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Utils;
using FluentValidation;
using System;

namespace Ecoporto.CRM.Business.Models
{
    public class Conta : Entidade<Conta>
    {
        public Conta()
        {

        }

        public Conta(
            int criadoPor, 
            string descricao,
            string documento, 
            string nomeFantasia, 
            string inscricaoEstadual, 
            string telefone, 
            int vendedorId,  
            SituacaoCadastral situacaoCadastral, 
            Segmento segmento, 
            ClassificacaoFiscal classificacaoFiscal, 
            string logradouro, 
            string bairro, 
            int numero, 
            string complemento, 
            string cep,
            Estado estado, 
            int? cidadeId, 
            int? paisId,
            bool blacklist)
        {
            CriadoPor = criadoPor;
            Descricao = descricao;
            Documento = documento;
            NomeFantasia = nomeFantasia;
            InscricaoEstadual = inscricaoEstadual;
            Telefone = telefone;
            VendedorId = vendedorId;
            SituacaoCadastral = situacaoCadastral;
            Segmento = segmento;
            ClassificacaoFiscal = classificacaoFiscal;
            Logradouro = logradouro;
            Bairro = bairro;
            Numero = numero;
            Complemento = complemento;
            CEP = cep;
            Estado = estado;
            CidadeId = cidadeId;
            PaisId = paisId;
            Blacklist = blacklist;
        }

        public int CriadoPor { get; set; }

        public string Descricao { get; set; }

        public string Documento { get; set; }

        public string NomeFantasia { get; set; }

        public string InscricaoEstadual { get; set; }

        public string Telefone { get; set; }

        public int VendedorId { get; set; }

        public string Vendedor { get; set; }

        public SituacaoCadastral SituacaoCadastral { get; set; }

        public Segmento Segmento { get; set; }

        public ClassificacaoFiscal ClassificacaoFiscal { get; set; }

        public string Logradouro { get; set; }

        public string Bairro { get; set; }

        public int Numero { get; set; }

        public string Complemento { get; set; }

        public string CEP { get; set; }

        public Estado Estado { get; set; }

        public Cidade Cidade { get; set; }

        public int? CidadeId { get; set; }

        public int? PaisId { get; set; }

        public Pais Pais { get; set; }

        public DateTime DataCriacao { get; set; }

        public bool Blacklist { get; set; }

        public void Alterar(Conta conta)
        {
            CriadoPor = conta.CriadoPor;
            Descricao = conta.Descricao;
            Documento = conta.Documento;
            NomeFantasia = conta.NomeFantasia;
            InscricaoEstadual = conta.InscricaoEstadual;
            Telefone = conta.Telefone;
            VendedorId = conta.VendedorId;
            SituacaoCadastral = conta.SituacaoCadastral;
            Segmento = conta.Segmento;
            ClassificacaoFiscal = conta.ClassificacaoFiscal;
            Logradouro = conta.Logradouro;
            Bairro = conta.Bairro;
            Numero = conta.Numero;
            Complemento = conta.Complemento;
            CEP = conta.CEP;
            Estado = conta.Estado;
            CidadeId = conta.CidadeId;
            PaisId = conta.PaisId;
            Blacklist = conta.Blacklist;
        }

        public override void Validar()
        {
            RuleFor(c => c.Descricao)
                .NotEmpty()
                .WithMessage("O Nome da Conta é obrigatório")
                .MinimumLength(3).WithMessage("Tamanho mínimo de 3 caracteres")
                .MaximumLength(255).WithMessage("Tamanho máximo de 255 caracteres");

            RuleFor(c => c.Segmento)
                .IsInEnum()
                .WithMessage("Informe o Segmento");

            RuleFor(c => c.ClassificacaoFiscal)
                .IsInEnum()
                .WithMessage("O Classificação Fiscal é obrigatória");

            RuleFor(c => c.Documento)
                .Must(Validacoes.CPFValido)
                .When(c => c.ClassificacaoFiscal == ClassificacaoFiscal.PF)
                .WithMessage("Documento CPF inválido");

            RuleFor(c => c.Documento)
                .Must(Validacoes.CNPJValido)
                .When(c => c.ClassificacaoFiscal == ClassificacaoFiscal.PJ)
                .WithMessage("Documento CNPJ inválido");

            RuleFor(c => c.Documento)
                .Empty()
                .When(c => c.ClassificacaoFiscal == ClassificacaoFiscal.EXTERNO)
                .WithMessage("Classificação Fiscal EXTERNO não deverá possuir número de documento");

            RuleFor(c => c.Descricao)
                .NotEmpty()
                .WithMessage("O Nome Fantasia é obrigatório");

            RuleFor(c => c.VendedorId)
               .GreaterThan(0)
               .WithMessage("Informe o Vendedor");

            ValidationResult = Validate(this);
        }
    }  
}
