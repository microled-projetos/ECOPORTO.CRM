using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Models.SPC;
using RestSharp;
using RestSharp.Deserializers;
using System;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Ecoporto.CRM.Infra.Services
{
    public class _SPCService
    {
        private readonly string _url;
        private readonly string _senha;

        public _SPCService(string url, string usuario, string senha)
        {
            _url = url;
            _senha = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{usuario}:{senha}"));
        }

        public ResultadoSPC Consultar(ClassificacaoFiscal classificacaoFiscal, string documento)
        {
            ResultadoSPC retorno = null;



            //var client = new RestClient(_url);
            //client.AddHandler("application/xml", new DotNetXmlDeserializer());

            //var request = new RestRequest(Method.POST);
            //request.AddHeader("content-type", "text/xml");
            //request.AddHeader("authorization", $"Basic {_senha}");

            //var xmlTipoConsulta = classificacaoFiscal == ClassificacaoFiscal.PF ? "F" : "J";

            //StringBuilder xml = new StringBuilder();

            //xml.Clear();
            //xml.Append("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:web=\"http://webservice.consulta.spcjava.spcbrasil.org/\">");
            //xml.Append("	<soapenv:Header/>");
            //xml.Append("		<soapenv:Body>");
            //xml.Append("			<web:filtro>");
            //xml.Append("				<codigo-produto>198</codigo-produto>");
            //xml.Append($"				<tipo-consumidor>{xmlTipoConsulta}</tipo-consumidor>");
            //xml.Append($"				<documento-consumidor>{documento}</documento-consumidor>");
            //xml.Append("			</web:filtro>");
            //xml.Append("		</soapenv:Body>");
            //xml.Append("</soapenv:Envelope>");

            //request.AddParameter("text/xml", xml.ToString(), ParameterType.RequestBody);
           
            //var response = client.Execute<ResultadoSPC>(request);

            //if (response.Data != null)
            //{
            //    retorno = response.Data;
            //}

            //retorno.XML = response.Content;

            //using (TextReader reader = new StringReader(retorno.XML))
            //{
            //    var xdoc = XDocument.Load(reader);
            //    XNamespace soap = "http://schemas.xmlsoap.org/soap/envelope/";
            //    XNamespace m = "http://webservice.consulta.spcjava.spcbrasil.org/";
            //    var responseXml = xdoc.Element(soap + "Envelope").Element(soap + "Body")
            //                          .Element(m + "resultado");

            //    var serializer = new XmlSerializer(typeof(ResultadoSPC));
            //    var responseObj =
            //          (ResultadoSPC)serializer.Deserialize(responseXml.CreateReader());

            //    //XmlSerializer serializer = new XmlSerializer(typeof(ResultadoSPC));
            //    //var x = (ResultadoSPC)serializer.Deserialize(reader);
            //    var ss = 1;
            //}

            return retorno;
        }
    }
}
