using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecoporto.CRM.Site.Models
{
    public class UsuarioViewModel
    {
        public UsuarioViewModel()
        {
            Usuarios = new List<Usuario>();
            Dominios = new List<Dominio>();
            Cargos = new List<Cargo>();
            Vinculos = new List<EquipeVendedorUsuarioDTO>();
            Contas = new List<UsuarioContaDTO>();
        }

        public int Id { get; set; }

        public string Login { get; set; }

        [Display(Name = "Nome Completo")]
        public string Nome { get; set; }

        public string Email { get; set; }

        public string CPF { get; set; }

        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [Display(Name = "Ativo?")]
        public bool Ativo { get; set; }

        [Display(Name = "Administrador?")]
        public bool Administrador { get; set; }

        [Display(Name = "Externo?")]
        public bool Externo { get; set; }

        [Display(Name = "Remoto?")]
        public bool Remoto { get; set; }

        public bool Lembrar { get; set; }

        [Display(Name = "Validar IP")]
        public bool ValidarIP { get; set; }

        [Display(Name = "Cargo")]
        public int CargoId { get; set; }

        [Display(Name = "Login Externo")]
        public string LoginExterno { get; set; }

        public string ReturnUrl { get; set; }

        [Display(Name = "Login Workflow")]
        public string LoginWorkflow { get; set; }

        [Display(Name = "Selecione a Conta")]
        public int ContaId { get; set; }

        public IEnumerable<Usuario> Usuarios { get; set; }

        public IEnumerable<Dominio> Dominios { get; set; }

        public IEnumerable<Cargo> Cargos { get; set; }

        public IEnumerable<EquipeVendedorUsuarioDTO> Vinculos { get; set; }

        public IEnumerable<UsuarioContaDTO> Contas { get; set; }
    }
}