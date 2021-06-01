using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Models;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Ecoporto.CRM.Site.Helpers
{
    public class PropostaWordHelper
    {
        private readonly Oportunidade _oportunidade;
        
        public PropostaWordHelper(Oportunidade oportunidade)
        {
            _oportunidade = oportunidade;
        }

        public void GerarPropostaWord()
        {            
            if (_oportunidade.OportunidadeProposta == null)
                throw new Exception("Proposta da Oportunidade não encontrada");

            var url = HttpContext.Current.Request.Url.OriginalString;

            url = url.Replace("GerarProposta", $"PropostaOnline/{_oportunidade.Id}");

            HttpWebRequest wreq = (HttpWebRequest)HttpWebRequest.Create(url);

            var httpCookie = HttpContext.Current.Request.Cookies[".ASPXAUTH_CRM_ECOPORTO"];

            wreq.CookieContainer = new CookieContainer();
            wreq.CookieContainer.Add(new Cookie(httpCookie.Name, httpCookie.Value, httpCookie.Path, HttpContext.Current.Request.Url.Host));

            HttpWebResponse wres = (HttpWebResponse)wreq.GetResponse();

            var nomeArquivo = $"{_oportunidade.Descricao}-{_oportunidade.Identificacao}";

            using (Stream s = wres.GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(s))
                {
                    var conteudo = sr.ReadToEnd();

                    conteudo = $"{Regex.Replace(conteudo, @"\t|\n|\r", "")}";

                    var strBody = new StringBuilder();

                    strBody.Append(@"
                        <html xmlns:v='urn:schemas-microsoft-com:vml'
                              xmlns:o='urn:schemas-microsoft-com:office:office'
                              xmlns:w='urn:schemas-microsoft-com:office:word'
                              xmlns:m='http://schemas.microsoft.com/office/2004/12/omml'
                              xmlns='http://www.w3.org/TR/REC-html40'>
                        <head>
                            <meta http-equiv=Content-Type content='text/html; charset=utf-8'>
                            <title></title>
                            <style>
                                v\:* {
                                    behavior: url(#default#VML);
                                }

                                o\:* {
                                    behavior: url(#default#VML);
                                }

                                w\:* {
                                    behavior: url(#default#VML);
                                }

                                .shape {
                                    behavior: url(#default#VML);
                                }
                                body.Section1
                                {
                                font-size: 9pt;
                                font-family:Verdana
                                }
                                @font-face
                                 {font-family:Verdana;}
                            </style>
                            <style>
                                @page {
                                    size:595.45pt 841.7pt;
                                    margin:0.5in 0.5in 0.5in 0.5in;
                                    mso-header-margin:.5in;
                                    mso-footer-margin:.5in;
                                    mso-paper-source:0;	
                                    font-size:9pt;
                                }

                                @page Section1 {
                                    mso-header-margin: .5in;
                                    mso-footer-margin: .5in;
                                    mso-header: h1;
                                    mso-footer: f1;
                                    font-size:9pt;
                                }

                                div.Section1 {
                                    page: Section1;
                                }

                                table#hrdftrtbl {
                                    margin: 0in 0in 0in 900in;
                                    width: 1px;
                                    height: 1px;
                                    overflow: hidden;
                                }

                                p.MsoFooter, li.MsoFooter, div.MsoFooter {
                                    margin: 0in;
                                    margin-bottom: .0001pt;
                                    mso-pagination: widow-orphan;
                                    tab-stops: center 3.0in right 6.0in;
                                    font-size: 12.0pt;
                                }

                                p.MsoHeader {
                                    margin-top: 150px;
                                }
                            </style>
                            <xml>
                                <w:WordDocument>
                                    <w:View>Print</w:View>
                                    <w:Zoom>100</w:Zoom>
                                    <w:DoNotOptimizeForBrowser />
                                </w:WordDocument>
                            </xml>
                        </head>

                        <body>
                            <div class='Section1'>");

                    strBody.Append(conteudo);

                    strBody.Append(@"
                                <table id='hrdftrtbl' border='0' cellspacing='0' cellpadding='0'>
                                    <tr>
                                        <td>
                                            <div style='mso-element:header' id='h1'>
                                                <!-- HEADER-tags -->
                                                <p class='MsoHeader'>                                                    
                                                    <table width='100%' border='0' style='margin-top:50pt;'>");
                    strBody.Append($@"<tr>
                                        <td width='200px' style='font-family:verdana;font-size:9pt;'><strong>ECO {_oportunidade.Identificacao}</strong>&nbsp; {_oportunidade.TipoServico.ToName()}&nbsp; {_oportunidade.Referencia}</td>                                        
                                     </tr>");

                    strBody.Append(@"</table>
                                                </p>
                                                <!-- end HEADER-tags -->
                                            </div>
                                        </td>
                                        <td>
                                            <div style='mso-element:footer' id='f1'>
                                                <span style='position:relative;z-index:-1'>
                                                    <!-- FOOTER-tags -->

                                                    <span style='mso-no-proof:yes'>
                                                        <!--[if gte vml 1]><v:shapetype
                                                         id='_x0000_t75' coordsize='21600,21600' o:spt='75' o:preferrelative='t'
                                                         path='m@4@5l@4@11@9@11@9@5xe' filled='f' stroked='f'>
                                                         <v:formulas>
                                                          <v:f eqn='if lineDrawn pixelLineWidth 0'/>
                                                          <v:f eqn='sum @0 1 0'/>
                                                          <v:f eqn='sum 0 0 @1'/>
                                                          <v:f eqn='prod @2 1 2'/>
                                                          <v:f eqn='prod @3 21600 pixelWidth'/>
                                                          <v:f eqn='prod @3 21600 pixelHeight'/>
                                                          <v:f eqn='sum @0 0 1'/>
                                                          <v:f eqn='prod @6 1 2'/>
                                                          <v:f eqn='prod @7 21600 pixelWidth'/>
                                                          <v:f eqn='sum @8 21600 0'/>
                                                          <v:f eqn='prod @7 21600 pixelHeight'/>
                                                          <v:f eqn='sum @10 21600 0'/>
                                                         </v:formulas>
                                                         <v:path o:extrusionok='f' gradientshapeok='t' o:connecttype='rect'/>
                                                         <o:lock v:ext='edit' aspectratio='t'/>
                                                        </v:shapetype><v:shape id='Picture_x0020_1' o:spid='_x0000_s3073' type='#_x0000_t75'
                                                         alt='VHB' style='position:absolute;
                                                         margin-left:-40pt;margin-right:0pt;margin-top:-780pt;width:788px;height:118px;
                                                         z-index:-1;
                                                         visibility:visible;mso-wrap-style:square;mso-wrap-distance-left:9pt;
                                                         mso-wrap-distance-top:0;mso-wrap-distance-right:9pt;
                                                         mso-wrap-distance-bottom:0;mso-position-horizontal:absolute;
                                                         mso-position-horizontal-relative:text;mso-position-vertical:absolute;
                                                         mso-position-vertical-relative:text'>
                                                         <v:imagedata src='http://op.ecoportosantos.com.br/crm/content/img/header-pdf.png'/>
                                                        </v:shape><![endif]-->
                                                    </span>
                                                    <p class='MsoFooter'>
                                                        <span style='mso-tab-count:2'></span>
                                                        <span style='mso-field-code: PAGE '>
                                                            <span style='mso-no-proof:yes'></span> de <span style='mso-field-code: NUMPAGES '></span>
                                                        </span>
                                                    </p>
                                            </div>

                                            <div style='mso-element:header' id='fh1'>
                                                <p class=MsoHeader><span lang=EN-US style='mso-ansi-language:EN-US'>&nbsp;<o:p></o:p></span></p>
                                            </div>
                                            <div style='mso-element:footer' id='ff1'>
                                                <p class=MsoFooter><span lang=EN-US style='mso-ansi-language:EN-US'>&nbsp;<o:p></o:p></span></p>
                                            </div>

                                        </td>
                                    </tr>
                                </table>
                            </div>

                        </body>
                        </html>");

                    object arquivoDoc = HttpContext.Current.Server.MapPath($"~/App_Data/{nomeArquivo.CorrigirNomeArquivo()}.doc");

                    File.WriteAllText(arquivoDoc.ToString(), strBody.ToString());
                }
            }
        }
    }
}