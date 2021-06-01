using System;
using System.Collections.Generic;

namespace Ecoporto.CRM.Workflow.Models
{
    public class Aprovacao
    {
        public string id { get; set; }
        public int id_Aprovacao { get; set; }
        public int id_Status { get; set; }
        public string status { get; set; }
        public int usuario_Aprovador_Id { get; set; }
        public string usuario_Aprovador_Login { get; set; }
        public string usuario_Aprovador_Nome { get; set; }
        public string usuario_Aprovador_Email { get; set; }
        public bool flag_Foi_Aprovacao_Final_Da_Etapa { get; set; }
        public object comentario { get; set; }
        public object data_Aprovacao { get; set; }
        public DateTime data_Cadastro { get; set; }
    }

    public class Etapa
    {
        public string id { get; set; }
        public int id_Etapa { get; set; }
        public string etapa_Nome { get; set; }
        public int id_Status { get; set; }
        public string status { get; set; }
        public bool flag_Aprovacao_Por_Usuario_Unico { get; set; }
        public object usuario_Aprovador_Id { get; set; }
        public object usuario_Aprovador_Login { get; set; }
        public object usuario_Aprovador_Nome { get; set; }
        public object usuario_Aprovador_Email { get; set; }
        public bool flag_Aprovacao_Por_Grupo { get; set; }
        public int grupo_Aprovador_Id { get; set; }
        public string grupo_Aprovador_Nome { get; set; }
        public object ultimo_Aprovador_Usuario_Id { get; set; }
        public object ultimo_Aprovador_Usuario_Login { get; set; }
        public object ultimo_Aprovador_Usuario_Nome { get; set; }
        public object ultimo_Aprovador_Usuario_Email { get; set; }
        public object aprovador_Final_Usuario_Id { get; set; }
        public object aprovador_Final_Usuario_Login { get; set; }
        public object aprovador_Final_Usuario_Nome { get; set; }
        public object aprovador_Final_Usuario_Email { get; set; }
        public object data_Aprovacao_Final { get; set; }
        public DateTime data_Cadastro { get; set; }
        public List<Aprovacao> aprovacoes { get; set; }
    }

    public class WorkFlow
    {
        public string id { get; set; }
        public int Processo_Id { get; set; }
        public int id_WorkFlow { get; set; }
        public int id_Status { get; set; }
        public string status { get; set; }
        public int id_EtapaAtual { get; set; }
        public int id_Sistema { get; set; }
        public string sistema_Nome { get; set; }
        public int id_Processo { get; set; }
        public string processo_Nome { get; set; }
        public string usuario_Cad_Nome { get; set; }
        public string usuario_Cad_Login { get; set; }
        public string usuario_Cad_Email { get; set; }
        public object ultimo_Aprovador_Usuario_Id { get; set; }
        public object ultimo_Aprovador_Usuario_Login { get; set; }
        public object ultimo_Aprovador_Usuario_Nome { get; set; }
        public object ultimo_Aprovador_Usuario_Email { get; set; }
        public object aprovador_Final_Usuario_Id { get; set; }
        public object aprovador_Final_Usuario_Login { get; set; }
        public object aprovador_Final_Usuario_Nome { get; set; }
        public object aprovador_Final_Usuario_Email { get; set; }
        public object data_Aprovacao_Final { get; set; }
        public object comentario_Rejeicao_Final { get; set; }
        public int id_Empresa { get; set; }
        public string empresa_Nome { get; set; }
        public DateTime data_Cadastro { get; set; }
        public DateTime data_Cancelamento { get; set; }
        public string motivo_Cancelamento { get; set; }
        public string usuario_Canc_Nome { get; set; }
        public string usuario_Canc_Login { get; set; }
        public string usuario_Canc_Email { get; set; }
        public List<Etapa> etapas { get; set; }
    }

    public class ListaWorkflow
    {
        public string id { get; set; }
        //public WorkFlow workFlow { get; set; }
        public List<WorkFlow> workFlows { get; set; }
    }

    public class RetornoHistoricoWorkflow
    {
        public string id { get; set; }
        public bool sucesso { get; set; }
        public object erroCodigo { get; set; }
        public object mensagem { get; set; }
        public object protocolo { get; set; }
        public int totalRows { get; set; }
        public List<ListaWorkflow> list { get; set; } = new List<ListaWorkflow>();
    }
}
