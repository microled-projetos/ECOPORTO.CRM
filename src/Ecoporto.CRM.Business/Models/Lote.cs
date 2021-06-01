using System;

namespace Ecoporto.CRM.Business.Models
{
    public class Lote : Entidade<Lote>
    {
        public int NumeroLote { get; set; }

        public int ImportadorId { get; set; }

        public string Numero { get; set; }

        public string ImportadorDescricao { get; set; }

        public int DespachanteId { get; set; }

        public string DespachanteDescricao { get; set; }

        public int TipoDocumento { get; set; }

        public bool Ativo { get; set; }

        public override void Validar()
        {
            throw new NotImplementedException();
        }
    }
}
