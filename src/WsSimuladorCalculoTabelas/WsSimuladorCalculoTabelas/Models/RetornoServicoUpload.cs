using System.Collections.Generic;
using System.Linq;

namespace WsSimuladorCalculoTabelas.Models
{
    public class RetornoServicoUpload
    {
        public bool success { get; set; }
        public object errorCode { get; set; }
        public string message { get; set; }
        public string protocol { get; set; }
        public List<DadosRetornoServicoUpload> list { get; set; }

        public DadosRetornoServicoUpload Arquivo => success ? list.FirstOrDefault() : null;
    }

    public class DadosRetornoServicoUpload
    {
        public string id { get; set; }
        public string path { get; set; }
        public string name { get; set; }
        public string extension { get; set; }
        public string dataArray { get; set; }
        public bool isError { get; set; }
        public string message { get; set; }
    }
}