using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Utils;
using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Models
{
    public class OportunidadePremioParceria : Entidade<OportunidadePremioParceria>
    {
        public OportunidadePremioParceria()
        {

        }

        public OportunidadePremioParceria(
            int oportunidadeId,
            StatusPremioParceria statusPremioParceria,
            int favorecido1,
            int favorecido2,
            int favorecido3,
            Instrucao instrucao,
            int contatoId,
            int premioReferencia,
            TipoServicoPremioParceria tipoServicoPremioParceria,
            string observacoes,
            int anexoId,
            int premioRevisaoId,
            string urlPremio,
            DateTime? dataUrlPremio,
            string emailFavorecido1,
            string emailFavorecido2,
            string emailFavorecido3,
            int criadoPor)
        {
            OportunidadeId = oportunidadeId;
            StatusPremioParceria = statusPremioParceria;
            Favorecido1 = favorecido1;
            Favorecido2 = favorecido2;
            Favorecido3 = favorecido3;
            Instrucao = instrucao;
            ContatoId = contatoId;
            PremioReferenciaId = PremioReferenciaId;
            TipoServicoPremioParceria = tipoServicoPremioParceria;
            Observacoes = observacoes;
            AnexoId = anexoId;
            PremioRevisaoId = premioRevisaoId;
            UrlPremio = urlPremio;
            DataUrlPremio = dataUrlPremio;
            EmailFavorecido1 = emailFavorecido1;
            EmailFavorecido2 = emailFavorecido2;
            EmailFavorecido3 = emailFavorecido3;
            CriadoPor = criadoPor;
        }

        public int OportunidadeId { get; set; }

        public StatusPremioParceria StatusPremioParceria { get; set; }

        public int Favorecido1 { get; set; }

        public int Favorecido2 { get; set; }

        public int Favorecido3 { get; set; }

        public Instrucao Instrucao { get; set; }

        public int ContatoId { get; set; }

        public int PremioReferenciaId { get; set; }

        public TipoServicoPremioParceria TipoServicoPremioParceria { get; set; }

        public string Observacoes { get; set; }

        public int AnexoId { get; set; }

        public int PremioRevisaoId { get; set; }

        public string UrlPremio { get; set; }

        public DateTime? DataUrlPremio { get; set; }

        public string EmailFavorecido1 { get; set; }

        public string EmailFavorecido2 { get; set; }

        public string EmailFavorecido3 { get; set; }

        public int CriadoPor { get; set; }

        public bool Cancelado { get; set; }

        public List<Contato> Contatos { get; set; }

        public List<Contato> Premios { get; set; }

        public List<ModalidadesComissionamento> Modalidades { get; set; }
            = new List<ModalidadesComissionamento>();

        public void AdicionarModalidade(ModalidadesComissionamento modalidade)
            => Modalidades.Add(modalidade);

        public void RemoverModalidades() => Modalidades.Clear();

        public void Alterar(OportunidadePremioParceria premioParceria)
        {
            StatusPremioParceria = premioParceria.StatusPremioParceria;
            Favorecido1 = premioParceria.Favorecido1;
            Favorecido2 = premioParceria.Favorecido2;
            Favorecido3 = premioParceria.Favorecido3;
            Instrucao = premioParceria.Instrucao;
            ContatoId = premioParceria.ContatoId;
            PremioReferenciaId = premioParceria.PremioReferenciaId;
            TipoServicoPremioParceria = premioParceria.TipoServicoPremioParceria;
            Observacoes = premioParceria.Observacoes;
            AnexoId = premioParceria.AnexoId;
            UrlPremio = premioParceria.UrlPremio;
            DataUrlPremio = premioParceria.DataUrlPremio;
            EmailFavorecido1 = premioParceria.EmailFavorecido1;
            EmailFavorecido2 = premioParceria.EmailFavorecido2;
            EmailFavorecido3 = premioParceria.EmailFavorecido3;
        }

        public override void Validar()
        {
            RuleFor(c => c.StatusPremioParceria)
                .IsInEnum()
                .WithMessage("Selecione o Status do Prêmio Parceria");

            RuleFor(c => c.Instrucao)
                .IsInEnum()
                .WithMessage("Selecione a Instrução do Prêmio Parceria");

            RuleFor(c => c.ContatoId)
                .GreaterThan(0)
                .WithMessage("Selecione o Contato do Prêmio Parceria");

            ValidationResult = Validate(this);

            if (this.Favorecido2 > 0 && this.Favorecido1 == 0)            
                ValidationResult.Errors.Add(new ValidationFailure(nameof(this.Favorecido2), $"Não é permitido cadatsrar o Favorecido 2 sem o Favorecido 1"));

            if (this.Favorecido3 > 0 && (this.Favorecido1 == 0 || this.Favorecido2 == 0))
                ValidationResult.Errors.Add(new ValidationFailure(nameof(this.Favorecido2), $"Não é permitido cadatsrar o Favorecido 3 sem o Favorecido 1 e 2"));            
        }
    }
}
