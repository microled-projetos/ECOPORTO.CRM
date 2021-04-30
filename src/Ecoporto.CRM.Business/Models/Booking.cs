namespace Ecoporto.CRM.Business.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public string Reference { get; set; }

        public int ClienteFatura { get; set; }

        public int ExportadorId { get; set; }

        public string ExportadorCnpj { get; set; }
    }
}
