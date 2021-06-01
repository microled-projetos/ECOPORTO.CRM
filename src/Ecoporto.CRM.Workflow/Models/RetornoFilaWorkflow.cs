using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Ecoporto.CRM.Workflow.Models
{
    [DataContract]
    public class RetornoFilaWorkflow
    {
        [DataMember(Name = "$id")]
        public string id { get; set; }
        public bool sucesso { get; set; }
        public object erroCodigo { get; set; }
        public object mensagem { get; set; }
        public object protocolo { get; set; }
        public int totalRows { get; set; }
        public List<Workflows> list { get; set; }
    }

    [DataContract]
    public class Workflows
    {
        public int id { get; set; }
        public int id_WorkFlow { get; set; }
        public string autonum_Sistema_Processo_Requisitante { get; set; }
        public int id_Status { get; set; }
        public string status { get; set; }
        public int id_Sistema { get; set; }
        public string sistema_Nome { get; set; }
        public int id_Processo { get; set; }
        public string processo_Nome { get; set; }
        public int? ultimo_Aprovador_Usuario_Id { get; set; }
        public string ultimo_Aprovador_Usuario_Login { get; set; }
        public string ultimo_Aprovador_Usuario_Nome { get; set; }
        public string ultimo_Aprovador_Usuario_Email { get; set; }
        public object aprovador_Final_Usuario_Id { get; set; }
        public object aprovador_Final_Usuario_Login { get; set; }
        public object aprovador_Final_Usuario_Nome { get; set; }
        public object aprovador_Final_Usuario_Email { get; set; }
        public DateTime? data_Aprovacao_Final { get; set; }
        public string comentario_Rejeicao_Final { get; set; }
        public int id_Empresa { get; set; }
        public string empresa_Nome { get; set; }
        public DateTime data_Cadastro { get; set; }
        public object data_Cancelamento { get; set; }
    }   
}
