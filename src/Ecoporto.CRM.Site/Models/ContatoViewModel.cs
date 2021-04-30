using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class ContatoViewModel
    {
        public ContatoViewModel()
        {
            Contas = new List<Conta>();
        }

        public int Id { get; set; }

        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [Display(Name = "Sobrenome")]
        public string Sobrenome { get; set; }

        [Display(Name = "Telefone")]
        public string Telefone { get; set; }

        [Display(Name = "Celular")]
        public string Celular { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Cargo")]
        public string Cargo { get; set; }

        [Display(Name = "Departamento")]
        public string Departamento { get; set; }

        [Display(Name = "Data de Nascimento")]
        public DateTime? DataNascimento { get; set; }

        [Display(Name = "Status")]
        public Status Status { get; set; }

        [Display(Name = "Conta")]
        public int ContaId { get; set; }

        [Display(Name = "Conta")]
        public Conta Conta { get; set; }

        public bool AbaContatos { get; set; }

        public List<Conta> Contas { get; set; }

        public bool TelaContas { get; set; }

        public string DescricaoStatus
            => Status.ToName();        
    }
}