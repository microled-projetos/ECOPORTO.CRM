using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Helpers;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Sharepoint.Models;
using Ecoporto.CRM.Site.Extensions;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Ecoporto.CRM.Site.Helpers
{
    public class PropostaPdfHelper
    {
        private readonly IAnexoRepositorio _anexoRepositorio;

        private readonly Oportunidade _oportunidade;        
        private readonly bool _anexar;
        private readonly bool _externo;
        private readonly string _url;

        public PropostaPdfHelper(IAnexoRepositorio anexoRepositorio, Oportunidade oportunidade, bool anexar = false, string url = "", bool externo = false)
        {
            _anexoRepositorio = anexoRepositorio;

            _oportunidade = oportunidade;
            _anexar = anexar;
            _externo = externo;
            _url = url;
        }

        public void GerarPropostaPdf()
        {            
            if (_oportunidade.OportunidadeProposta == null)
                throw new Exception("Proposta da Oportunidade não encontrada");     

            HttpWebRequest wreq = (HttpWebRequest)HttpWebRequest.Create(_url);

            if (_externo == false)
            {
                var httpCookie = HttpContext.Current.Request.Cookies[".ASPXAUTH_CRM_ECOPORTO"];

                wreq.CookieContainer = new CookieContainer();
                wreq.CookieContainer.Add(new Cookie(httpCookie.Name, httpCookie.Value, httpCookie.Path, HttpContext.Current.Request.Url.Host));
            }

            HttpWebResponse wres = (HttpWebResponse)wreq.GetResponse();            

            var nomeArquivo = $"{_oportunidade.Descricao}-{_oportunidade.Identificacao}";

            using (Stream s = wres.GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(s))
                {
                    var conteudo = sr.ReadToEnd();

                    conteudo = $"{Regex.Replace(conteudo, @"\t|\n|\r", "")}";
                                  
                    object arquivoPdf = HttpContext.Current.Server.MapPath($"~/App_Data/{nomeArquivo.CorrigirNomeArquivo()}.pdf");
                 
                    conteudo = Regex.Replace(conteudo, "page-break-before: always;", "");

                    MemoryStream PDFData = new MemoryStream();
                    Document doc = new Document(PageSize.A4);

                    doc.SetMargins(36, 36, 110, 36);

                    PdfWriter writer = PdfWriter.GetInstance(doc, PDFData);
                    writer.PageEvent = new PdfHelpers($"ECO {_oportunidade.Identificacao} {_oportunidade.TipoServico.ToName()} {_oportunidade.Referencia}");
                    doc.Open();

                    PdfPTable tab = new PdfPTable(2);

                    tab.DefaultCell.Border = Rectangle.NO_BORDER;
                    tab.TotalWidth = 800;

                    var css = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Content/css/pdf.css"));

                    using (var cssMemoryStream = new MemoryStream(Encoding.UTF8.GetBytes(css)))
                    {
                        using (var htmlMemoryStream = new MemoryStream(Encoding.UTF8.GetBytes(conteudo)))
                        {
                            XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, htmlMemoryStream, cssMemoryStream);
                        }
                    }

                    doc.Add(tab);
                    doc.Close();

                    var bytes = PDFData.ToArray();
                    if (_anexar)
                        IncluirAnexo(_oportunidade.Id, arquivoPdf.ToString(), TipoAnexo.PROPOSTA, bytes, _oportunidade.CriadoPor);

                    File.WriteAllBytes(arquivoPdf.ToString(), bytes);
                }
            }
        }

        private int IncluirAnexo(int oportunidadeId, string nomeArquivo, TipoAnexo tipoAnexo, byte[] anexo, int? usuarioId = 0)
        {
            if (anexo != null && anexo.Length > 0)
            {
                var token = Sharepoint.Services.Autenticador.Autenticar();

                if (token == null)
                    throw new HttpException(404, "Não foi possível se autenticar no serviço de Anexos");

                byte[] byteArray = anexo;

                var dados = new DadosArquivoUpload
                {
                    Name = Path.GetFileName(nomeArquivo),
                    Extension = Path.GetExtension(nomeArquivo),
                    System = 3,
                    DataArray = Convert.ToBase64String(byteArray)
                };

                var retornoUpload = new Sharepoint.Services.AnexosService(token)
                    .EnviarArquivo(dados);

                 if (!retornoUpload.success)
                    throw new HttpException(500, "Retorno API anexos: " + retornoUpload.message);

                _anexoRepositorio.ExcluirAnexosOportunidadePorTipo(oportunidadeId, TipoAnexo.PROPOSTA);

                var anexoInclusaoId = _anexoRepositorio.IncluirAnexo(
                    new Anexo
                    {
                        IdProcesso = oportunidadeId,
                        Arquivo = Path.GetFileName(nomeArquivo),
                        CriadoPor = usuarioId == null ? HttpContext.Current.User.ObterId() : usuarioId.Value,
                        TipoAnexo = tipoAnexo,
                        TipoDoc = 1,
                        IdArquivo = Converters.GuidToRaw(retornoUpload.Arquivo.id)
                    });

                return anexoInclusaoId;
            }

            return 0;
        }
    }
}