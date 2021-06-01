using Ecoporto.CRM.Business.Enums;
using FluentValidation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ecoporto.CRM.Business.Models
{
    public class SolicitacaoComercial : Entidade<SolicitacaoComercial>
    {
        public SolicitacaoComercial()
        {

        }

        public SolicitacaoComercial(
            TipoSolicitacao tipoSolicitacao,
            int unidadeSolicitacao,
            AreaOcorrenciaSolicitacao areaOcorrenciaSolicitacao,
            int? tipoOperacao,
            int? ocorrenciaId,
            int? motivoId,
            string justificativa,
            int criadoPor,
            bool usuarioExterno)
        {
            TipoSolicitacao = tipoSolicitacao;
            UnidadeSolicitacao = unidadeSolicitacao;
            AreaOcorrenciaSolicitacao = areaOcorrenciaSolicitacao;
            TipoOperacao = tipoOperacao;
            OcorrenciaId = ocorrenciaId;
            MotivoId = motivoId;
            Justificativa = justificativa;
            CriadoPor = criadoPor;
            UsuarioExterno = usuarioExterno;

            StatusSolicitacao = StatusSolicitacao.NOVO;
        }

        public TipoSolicitacao TipoSolicitacao { get; set; }

        public StatusSolicitacao StatusSolicitacao { get; set; }

        public int UnidadeSolicitacao { get; set; }

        public AreaOcorrenciaSolicitacao AreaOcorrenciaSolicitacao { get; set; }

        public int? TipoOperacao { get; set; }

        public int? OcorrenciaId { get; set; }

        public int? MotivoId { get; set; }

        public string Justificativa { get; set; }

        public decimal ValorDevido { get; set; }

        public decimal ValorCobrado { get; set; }

        public decimal ValorCredito { get; set; }

        public bool HabilitaValorDevido { get; set; }

        public int CriadoPor { get; set; }

        public DateTime DataCriacao { get; set; }

        public bool UsuarioExterno { get; set; }

        public List<SolicitacaoComercialMotivo> Motivos { get; set; } = new List<SolicitacaoComercialMotivo>();

        public List<SolicitacaoComercialOcorrencia> Ocorrencias { get; set; } = new List<SolicitacaoComercialOcorrencia>();

        public void AdicionarMotivos(List<SolicitacaoComercialMotivo> motivos)
        {
            if (motivos != null)
                Motivos.AddRange(motivos);
        }

        public void AdicionarOcorrencias(List<SolicitacaoComercialOcorrencia> ocorrencias)
        {
            if (ocorrencias != null)
                Ocorrencias.AddRange(ocorrencias);
        }

        public void Alterar(SolicitacaoComercial solicitacao)
        {
            UnidadeSolicitacao = solicitacao.UnidadeSolicitacao;
            AreaOcorrenciaSolicitacao = solicitacao.AreaOcorrenciaSolicitacao;
            TipoOperacao = solicitacao.TipoOperacao;
            OcorrenciaId = solicitacao.OcorrenciaId;
            MotivoId = solicitacao.MotivoId;
            Justificativa = solicitacao.Justificativa;
        }

        public void AlterarResumo(SolicitacaoComercial solicitacao)
        {
            ValorDevido = solicitacao.ValorDevido;
            ValorCobrado = solicitacao.ValorCobrado;
            ValorCredito = solicitacao.ValorCredito;
            HabilitaValorDevido = solicitacao.HabilitaValorDevido;
        }

        public override void Validar()
        {
            RuleFor(c => c.TipoSolicitacao)
                .IsInEnum()
                .When(c => c.Id == 0)
                .WithMessage("Selecione o Tipo de Solicitação");

            
            RuleFor(c => c.UnidadeSolicitacao)
                .GreaterThan(0)
                .WithMessage("Selecione a Unidade da Solicitação");

            RuleFor(c => c.Justificativa)
                .MaximumLength(1000)
                .WithMessage("A justificativa deve ter no máximo 1000 caracteres");

            RuleFor(c => c.MotivoId)
                .GreaterThan(0)
                .When(c => c.UsuarioExterno == false && c.Motivos.Any())
                .WithMessage("Selecione um Motivo");

            RuleFor(c => c.OcorrenciaId)
                .GreaterThan(0)
                .When(c => c.UsuarioExterno == false && c.Ocorrencias.Any())
                .WithMessage("Selecione uma Ocorrência");

            RuleFor(c => c.AreaOcorrenciaSolicitacao)
                .IsInEnum()
                .When(c => c.UsuarioExterno == false && c.TipoSolicitacao != TipoSolicitacao.PRORROGACAO_BOLETO && c.TipoSolicitacao != TipoSolicitacao.RESTITUICAO)
                .WithMessage("Selecione uma Ocorrência");

            ValidationResult = Validate(this);
        }
    }
}