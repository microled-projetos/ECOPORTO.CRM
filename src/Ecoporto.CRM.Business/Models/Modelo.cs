using Ecoporto.CRM.Business.Enums;
using FluentValidation;
using System;

namespace Ecoporto.CRM.Business.Models
{
    public class Modelo : Entidade<Modelo>
    {
        public Modelo()
        {

        }

        public Modelo(
            TipoOperacao tipoOperacao,
            string descricao,
            Status status,
            FormaPagamento? formaPagamento,
            int diasFreeTime,
            int qtdeDias,
            int validade,
            TipoValidade tipoValidade,
            TipoServico tipoServico,
            int? vendedorId,
            int impostoId,
            bool acordo,
            bool escalonado,
            bool parametroLote,
            bool parametroBL,
            bool parametroConteiner,
            bool parametroIdTabela,
            bool hubPort, 
            bool cobrancaEspecial, 
            bool integraChronos,
            bool simular,
            decimal desovaParcial, 
            decimal fatorCP, 
            int posicIsento)
        {
            TipoOperacao = tipoOperacao;
            Descricao = descricao;
            Status = status;
            FormaPagamento = formaPagamento;
            DiasFreeTime = diasFreeTime;
            QtdeDias = qtdeDias;
            Validade = validade;
            TipoValidade = tipoValidade;
            VendedorId = vendedorId;
            ImpostoId = impostoId;
            Acordo = acordo;
            Escalonado = escalonado;
            ParametroLote = parametroLote;
            ParametroBL = parametroBL;
            ParametroConteiner = parametroConteiner;
            ParametroIdTabela = parametroIdTabela;
            HubPort = hubPort;
            CobrancaEspecial = cobrancaEspecial;
            DesovaParcial = desovaParcial;
            IntegraChronos = integraChronos;
            Simular = simular;
            FatorCP = fatorCP;
            PosicIsento = posicIsento;
            TipoServico = tipoServico;

            DataCadastro = DateTime.Now;
        }

        public TipoOperacao TipoOperacao { get; set; }

        public string Descricao { get; set; }

        public Status Status { get; set; }

        public FormaPagamento? FormaPagamento { get; set; }

        public int DiasFreeTime { get; set; }

        public int QtdeDias { get; set; }

        public int Validade { get; set; }

        public TipoValidade TipoValidade { get; set; }
        public TipoServico TipoServico { get; set; }        

        public Vendedor Vendedor { get; set; }

        public int? VendedorId { get; set; }

        public Imposto Imposto { get; set; }

        public int ImpostoId { get; set; }

        public DateTime DataCadastro { get; set; }

        public DateTime? DataInatividade { get; set; }

        public bool Acordo { get; set; }

        public bool Escalonado { get; set; }

        public bool ParametroBL { get; set; }

        public bool ParametroLote { get; set; }

        public bool ParametroConteiner { get; set; }

        public bool ParametroIdTabela { get; set; }

        public bool HubPort { get; set; }

        public bool CobrancaEspecial { get; set; }

        public decimal DesovaParcial { get; set; }

        public decimal FatorCP { get; set; }

        public int PosicIsento { get; set; }

        public bool IntegraChronos { get; set; }

        public bool Simular { get; set; }

        public void InativarCadastro()
        {
            DataInatividade = DateTime.Now;
        }

        public void Alterar(Modelo modelo)
        {
            this.TipoOperacao = modelo.TipoOperacao;
            this.Descricao = modelo.Descricao;
            this.Status = modelo.Status;
            this.FormaPagamento = modelo.FormaPagamento;
            this.DiasFreeTime = modelo.DiasFreeTime;
            this.QtdeDias = modelo.QtdeDias;
            this.Validade = modelo.Validade;
            this.TipoValidade = modelo.TipoValidade;
            this.VendedorId = modelo.VendedorId;
            this.ImpostoId = modelo.ImpostoId;
            this.Acordo = modelo.Acordo;
            this.Escalonado = modelo.Escalonado;
            this.ParametroLote = modelo.ParametroLote;
            this.ParametroBL = modelo.ParametroBL;
            this.ParametroConteiner = modelo.ParametroConteiner;
            this.ParametroIdTabela = modelo.ParametroIdTabela;
            this.CobrancaEspecial = modelo.CobrancaEspecial;
            this.HubPort = modelo.HubPort;
            this.DesovaParcial = modelo.DesovaParcial;
            this.FatorCP = modelo.FatorCP;
            this.PosicIsento = modelo.PosicIsento;
            this.TipoServico = modelo.TipoServico;
            this.IntegraChronos = modelo.IntegraChronos;
            this.Simular = modelo.Simular;
        }

        public override void Validar()
        {
            RuleFor(c => c.Descricao)
               .NotNull()
               .WithMessage("A descrição do Modelo é obrigatória")
               .MinimumLength(3).WithMessage("Tamanho mínimo de 3 caracteres")
               .MaximumLength(150).WithMessage("Tamanho máximo de 150 caracteres");

            RuleFor(c => c.TipoOperacao)
               .IsInEnum()
               .WithMessage("Selecione o tipo de Operação");

            RuleFor(c => c.Status)
               .IsInEnum()
               .WithMessage("Selecione o Status");            

            RuleFor(c => c.TipoValidade)
               .IsInEnum()
               .WithMessage("Informe o Tipo de Validade");

            RuleFor(c => c.ImpostoId)
               .GreaterThan(0)
               .WithMessage("Informe o Imposto");

            ValidationResult = Validate(this);
        }
    }
}