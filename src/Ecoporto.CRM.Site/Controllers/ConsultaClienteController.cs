using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Site.Extensions;
using Ecoporto.CRM.Site.Filtros;
using Ecoporto.CRM.Site.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;

namespace Ecoporto.CRM.Site.Controllers
{
    [Authorize]
    public class ConsultaClienteController : Controller
    {
        private readonly IContaRepositorio _contaRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;

        public ConsultaClienteController(
            IContaRepositorio contaRepositorio,
            IUsuarioRepositorio usuarioRepositorio)
        {
            _contaRepositorio = contaRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [UsuarioExternoFilter]
        public PartialViewResult ConsultarContasPorDescricao(string descricao)
        {
            IEnumerable<Conta> resultado = new List<Conta>();

            if (!string.IsNullOrWhiteSpace(descricao))
            {
                resultado = _contaRepositorio
                    .ObterContasPorDescricao(descricao.Trim(), (int?)ViewBag.UsuarioExternoId);
            }

            return PartialView("_PesquisarContasConsulta", resultado);
        }

        [HttpGet]
        public PartialViewResult ConsultarImportadoresPorDescricao(string descricao)
        {
            IEnumerable<Conta> resultado = new List<Conta>();

            if (!string.IsNullOrWhiteSpace(descricao))
            {
                resultado = _contaRepositorio
                    .ObterContasPorDescricao(descricao.Trim(), null);
            }

            return PartialView("_PesquisarImportadoresConsulta", resultado);
        }       

        [HttpPost]
        [UsuarioExternoFilter]
        public ActionResult ConsultaClientesArquivos(ConsultaClienteViewModel viewModel)
        {
            using (var ws = new WsConsultaClientes.Service1())
            {
                var retorno = ws.Importacao_Interna_Cliente_V2(viewModel.SolicitanteCNPJ, viewModel.ImportadorCNPJ, viewModel.Acao.ToBoolean(), User.ObterId().ToString(), string.Empty);

                var objRetorno = new ConsultaClienteRetorno();

                if (retorno != null)
                {
                    objRetorno.Sucesso = true;
                    objRetorno.Mensagem = retorno.Resposta;

                    objRetorno.Mensagem = objRetorno.Mensagem.Replace(" / / ", "<br /><br />");
                    objRetorno.Mensagem = objRetorno.Mensagem.Replace(" / ", "<br />");

                    if (retorno.Arquivos != null)
                    {
                        foreach (WsConsultaClientes.Arquivo arquivo in retorno.Arquivos)
                        {
                            var base64 = Convert.ToBase64String(arquivo.dataArray);

                            objRetorno.Arquivos.Add(new ConsultaClienteRetornoArquivo(arquivo.Nome, arquivo.Extensao.ToLower(), base64));
                        }
                    }                   

                    return Json(objRetorno);
                }
            }

            return null;
        }
    }
}