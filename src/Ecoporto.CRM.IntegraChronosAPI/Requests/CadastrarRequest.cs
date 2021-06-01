namespace Ecoporto.CRM.IntegraChronosAPI.Requests
{
    public class CadastrarRequest
    {
        public int Id_Processo { get; set; }

        public int Tipo_Processo { get; set; }

        public int Id_Workflow { get; set; }

        public int Id_Etapa { get; set; }

        public int Acao { get; set; }
    }
}