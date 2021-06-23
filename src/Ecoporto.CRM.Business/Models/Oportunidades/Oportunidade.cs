using Ecoporto.CRM.Business.Enums;
using FluentValidation;
using System;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Models
{
    public class Oportunidade : Entidade<Oportunidade>
    {
        public Oportunidade()
        {

        }

        public Oportunidade(
            string identificacao,
            int contaId,
            int empresaId,
            bool aprovada,
            bool consultaTabela,
            string descricao,
            DateTime? dataFechamento,
            int? tabelaId,
            int contatoId,
            decimal probabilidade,
            SucessoNegociacao sucessoNegociacao,
            ClassificacaoCliente classificacaoCliente,
            Segmento segmento,
            EstagioNegociacao estagioNegociacao,
            StatusOportunidade statusOportunidade,
            MotivoPerda motivoPerda,
            TipoDeProposta tipoDeProposta,
            TipoServico tipoServico,
            TipoNegocio tipoNegocio,
            TipoOperacaoOportunidade tipoOperacaoOportunidade,
            int? revisaoId,
            int mercadoriaId,
            string observacao,
            decimal faturamentoMensalLCL,
            decimal faturamentoMensalFCL,
            decimal volumeMensal,
            decimal cIFMedio,
            Boleano premioParceria,
            int criadoPor)
        {
            Identificacao = identificacao;
            ContaId = contaId;
            EmpresaId = empresaId;
            Aprovada = aprovada;
            ConsultaTabela = consultaTabela;
            Descricao = descricao;
            DataFechamento = dataFechamento;
            TabelaId = tabelaId;
            ContatoId = contatoId;
            Probabilidade = probabilidade;
            SucessoNegociacao = sucessoNegociacao;
            ClassificacaoCliente = classificacaoCliente;
            Segmento = segmento;
            EstagioNegociacao = estagioNegociacao;
            StatusOportunidade = statusOportunidade;
            MotivoPerda = motivoPerda;
            TipoDeProposta = tipoDeProposta;
            TipoServico = tipoServico;
            TipoNegocio = tipoNegocio;
            TipoOperacaoOportunidade = tipoOperacaoOportunidade;
            RevisaoId = revisaoId;
            MercadoriaId = mercadoriaId;
            Observacao = observacao;
            FaturamentoMensalLCL = faturamentoMensalLCL;
            FaturamentoMensalFCL = faturamentoMensalFCL;
            VolumeMensal = volumeMensal;
            CIFMedio = cIFMedio;
            PremioParceria = premioParceria;
            CriadoPor = criadoPor;
        }

        public string Identificacao { get; set; }

        public int ContaId { get; set; }

        public int EmpresaId { get; set; }

        public bool Aprovada { get; set; }

        public bool ConsultaTabela { get; set; }

        public string Descricao { get; set; }

        public DateTime? DataFechamento { get; set; }

        public int? TabelaId { get; set; }

        public int? TabelaRevisadaId { get; set; }

        public int ContatoId { get; set; }

        public decimal Probabilidade { get; set; }

        public SucessoNegociacao SucessoNegociacao { get; set; }

        public ClassificacaoCliente ClassificacaoCliente { get; set; }

        public Segmento Segmento { get; set; }

        public EstagioNegociacao EstagioNegociacao { get; set; }

        public StatusOportunidade StatusOportunidade { get; set; }

        public MotivoPerda MotivoPerda { get; set; }

        public TipoServico TipoServico { get; set; }

        public TipoDeProposta TipoDeProposta { get; set; }

        public TipoNegocio TipoNegocio { get; set; }

        public TipoOperacaoOportunidade TipoOperacaoOportunidade { get; set; }

        public string Referencia { get; set; }

        public int? RevisaoId { get; set; }

        public int MercadoriaId { get; set; }

        public string Observacao { get; set; }

        public decimal FaturamentoMensalLCL { get; set; }

        public decimal FaturamentoMensalFCL { get; set; }

        public decimal VolumeMensal { get; set; }

        public decimal CIFMedio { get; set; }

        public Boleano PremioParceria { get; set; }

        public int CriadoPor { get; set; }

        public DateTime DataCriacao { get; set; }

        public string AlteradoPor { get; set; }

        public bool Cancelado { get; set; }

        public string SallesId { get; set; }

        public string OrigemClone { get; set; }

        public string OrigemPropostaClone { get; set; }

        public int VendedorId { get; set; }

        public bool PermiteAlterarDataCancelamento { get; set; }

        public Conta Conta { get; set; }

        public DateTime UltimaAlteracao { get; set; }

        public DateTime? DataCancelamento { get; set; }

        public OportunidadeProposta OportunidadeProposta { get; set; }

        public List<Oportunidade> Revisoes { get; set; } = new List<Oportunidade>();

        public List<Mercadoria> Mercadorias { get; set; } = new List<Mercadoria>();

        public List<Contato> Contatos { get; set; } = new List<Contato>();

        public List<Conta> Contas { get; set; } = new List<Conta>();

        public List<Anexo> Anexos { get; set; } = new List<Anexo>();

        public List<OportunidadeFichaFaturamento> FichasFaturamento { get; set; } = new List<OportunidadeFichaFaturamento>();

        public void AlterarInformacoesIniciais(Oportunidade oportunidade)
        {
            ContaId = oportunidade.ContaId;
            EmpresaId = oportunidade.EmpresaId;
            Aprovada = oportunidade.Aprovada;
            ConsultaTabela = oportunidade.ConsultaTabela;
            Descricao = oportunidade.Descricao;
            DataFechamento = oportunidade.DataFechamento;
            TabelaId = oportunidade.TabelaId;
            ContatoId = oportunidade.ContatoId;
            Probabilidade = oportunidade.Probabilidade;
            SucessoNegociacao = oportunidade.SucessoNegociacao;
            ClassificacaoCliente = oportunidade.ClassificacaoCliente;
            Cancelado = oportunidade.Cancelado;
            DataCancelamento = oportunidade.DataCancelamento;
            Segmento = oportunidade.Segmento;
            EstagioNegociacao = oportunidade.EstagioNegociacao;
            StatusOportunidade = oportunidade.StatusOportunidade;
            MotivoPerda = oportunidade.MotivoPerda;
            TipoDeProposta = oportunidade.TipoDeProposta;
            TipoServico = oportunidade.TipoServico;
            TipoNegocio = oportunidade.TipoNegocio;
            TipoOperacaoOportunidade = oportunidade.TipoOperacaoOportunidade;
            RevisaoId = oportunidade.RevisaoId;
            MercadoriaId = oportunidade.MercadoriaId;
            Observacao = oportunidade.Observacao;
            FaturamentoMensalLCL = oportunidade.FaturamentoMensalLCL;
            FaturamentoMensalFCL = oportunidade.FaturamentoMensalFCL;
            VolumeMensal = oportunidade.VolumeMensal;
            CIFMedio = oportunidade.CIFMedio;
            PremioParceria = oportunidade.PremioParceria;
            Cancelado = oportunidade.Cancelado;
            DataCancelamento = oportunidade.DataCancelamento;

            AlteradoPor = oportunidade.AlteradoPor;
            UltimaAlteracao = DateTime.Now;
        }

        public override void Validar()
        {
            RuleFor(c => c.Descricao)
                .NotNull()
                .WithMessage("A Descrição/Nome da Oportunidade é obrigatória")
                .MinimumLength(3).WithMessage("Tamanho mínimo de 3 caracteres")
                .MaximumLength(150).WithMessage("Tamanho máximo de 150 caracteres");

            RuleFor(c => c.EmpresaId)
                .GreaterThan(0)
                .WithMessage("Selecione a Empresa");

            RuleFor(c => c.ContaId)
                .GreaterThan(0)
                .WithMessage("A Conta é obrigatória");

            RuleFor(c => c.DataFechamento)
                .GreaterThan(c => c.DataCriacao)
                .When(c => c.Id != 0 && c.DataFechamento != default(DateTime))
                .WithMessage("A Data de Fechamento deve ser superior a data do cadastro");

            RuleFor(c => c.SucessoNegociacao)
                .IsInEnum()
                .WithMessage("O Sucesso Negociação é obrigatório");

            RuleFor(c => c.Segmento)
               .IsInEnum()
               .WithMessage("O Segmento da Oportunidade é obrigatório");

            RuleFor(c => c.TipoDeProposta)
                .IsInEnum()
                .WithMessage("O Tipo de Proposta é obrigatório");

            RuleFor(c => c.TipoServico)
                .IsInEnum()
                .WithMessage("O Tipo de Serviço é obrigatório");

            RuleFor(c => c.TipoNegocio)
                .IsInEnum()
                .WithMessage("O Tipo de Negócio é obrigatório");

            RuleFor(c => c.TipoOperacaoOportunidade)
                .IsInEnum()
                .WithMessage("O Tipo da Operação é obrigatório");

            RuleFor(c => c.VolumeMensal)
                .GreaterThan(0)
                .When(c => c.TipoOperacaoOportunidade != TipoOperacaoOportunidade.SPOT)
                .WithMessage("O Volume Mensal é obrigatório");

            RuleFor(c => c.FaturamentoMensalFCL)
               .GreaterThan(0)
               .When(c => (c.TipoServico == TipoServico.FCL || c.TipoServico == TipoServico.FCL_LCL) && c.TipoServico != TipoServico.BREAK_BULK)
               .WithMessage("O Faturamento Mensal FCL é obrigatório");

            RuleFor(c => c.FaturamentoMensalLCL)
                .GreaterThan(0)
                .When(c => (c.TipoServico == TipoServico.LCL || c.TipoServico == TipoServico.FCL_LCL) && c.TipoServico != TipoServico.BREAK_BULK)
                .WithMessage("O Faturamento Mensal LCL é obrigatório");

            RuleFor(c => c.CIFMedio)
                .GreaterThan(0)
                .WithMessage("O CIF Médio é obrigatório");

         

                ValidationResult = Validate(this);
        }
    }
}
