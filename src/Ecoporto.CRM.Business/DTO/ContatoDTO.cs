using Ecoporto.CRM.Business.Enums;
using System;

namespace Ecoporto.CRM.Business.DTO
{
    public class ContatoDTO
    {
        public virtual int Id { get; set; }

        public virtual string Nome { get; set; }

        public virtual string Sobrenome { get; set; }

        public virtual string Telefone { get; set; }

        public virtual string Celular { get; set; }

        public virtual string Email { get; set; }

        public virtual string Cargo { get; set; }

        public virtual string Departamento { get; set; }

        public virtual DateTime? DataNascimento { get; set; }

        public virtual Status Status { get; set; }

        public virtual int ContaId { get; set; }

        public virtual string ContaDescricao { get; set; }

        public virtual string ContaDocumento { get; set; }
    }
}
