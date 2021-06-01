namespace Ecoporto.CRM.Workflow.Models
{
    public class CadastroRecall
    {
        public CadastroRecall(
            int id_Workflow, 
            string usuario_Cancelamento_Login, 
            string usuario_Cancelamento_Nome, 
            string usuario_Cancelamento_Email, 
            string motivo_Cancelamento)
        {
            Id_Workflow = id_Workflow;
            Usuario_Cancelamento_Login = usuario_Cancelamento_Login;
            Usuario_Cancelamento_Nome = usuario_Cancelamento_Nome;
            Usuario_Cancelamento_Email = usuario_Cancelamento_Email;
            Motivo_Cancelamento = motivo_Cancelamento;
        }

        public int Id_Workflow { get; set; }
        
        public string Usuario_Cancelamento_Login { get; set; }

        public string Usuario_Cancelamento_Nome { get; set; }

        public string Usuario_Cancelamento_Email { get; set; }

        public string Motivo_Cancelamento { get; set; }
    }
}
