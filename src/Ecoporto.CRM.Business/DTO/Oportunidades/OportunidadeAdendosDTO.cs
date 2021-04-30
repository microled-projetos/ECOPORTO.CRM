using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Extensions;
using System;

namespace Ecoporto.CRM.Business.DTO
{
    public class OportunidadeAdendosDTO
    {
        public int Id { get; set; }

        public int OportunidadeId { get; set; }

        public TipoAdendo TipoAdendo { get; set; }

        public StatusAdendo StatusAdendo { get; set; }

        public DateTime DataCadastro { get; set; }

        public string CriadoPor { get; set; }

        public string IdFile { get; set; }

        public string DescricaoTipoAdendo
            => TipoAdendo.ToName();

        public string DescricaoStatusAdendo
            => StatusAdendo.ToName();
    }
}