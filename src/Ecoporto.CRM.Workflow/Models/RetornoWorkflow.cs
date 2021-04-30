namespace Ecoporto.CRM.Workflow.Models
{
    public class RetornoWorkflow
    {
        public bool sucesso { get; set; }
        public object erroCodigo { get; set; }
        public string mensagem { get; set; }
        public string protocolo { get; set; }
        public int totalRows { get; set; }
        public object[] list { get; set; }
    }
}
