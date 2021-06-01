using Ecoporto.CRM.Workflow.Enums;

namespace Ecoporto.CRM.Workflow.Models
{
    public class CadastroWorkflow
    {
        public CadastroWorkflow(
            Processo id_Processo,
            int id_empresa,
            int autonum_Sistema_Processo_Requisitante,
            string usuario_Requisitante_Login, 
            string usuario_Requisitante_Nome, 
            string usuario_Requisitante_Email,            
            string objCampos)
        {
            Id_Processo = id_Processo;
            Id_Empresa = id_empresa;
            Autonum_Sistema_Processo_Requisitante = autonum_Sistema_Processo_Requisitante;
            Usuario_Requisitante_Login = usuario_Requisitante_Login;
            Usuario_Requisitante_Nome = usuario_Requisitante_Nome;
            Usuario_Requisitante_Email = usuario_Requisitante_Email;
            ObjCampos = objCampos;
        }

        public Processo Id_Processo { get; set; }

        public int Id_Empresa { get; set; }

        public int Autonum_Sistema_Processo_Requisitante { get; set; }

        public string Usuario_Requisitante_Login { get; set; }

        public string Usuario_Requisitante_Nome { get; set; }

        public string Usuario_Requisitante_Email { get; set; }

        public string ObjCampos { get; set; }
    }
}
