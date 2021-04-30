using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Extensions;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Site.Filtros;
using Ecoporto.CRM.Site.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Ecoporto.CRM.Site.Controllers
{
    [Authorize]
    public class BuscaController : Controller
    {
        private readonly IContaRepositorio _contaRepositorio;
        private readonly IContatoRepositorio _contatoRepositorio;
        private readonly IMercadoriaRepositorio _mercadoriaRepositorio;
        private readonly IModeloRepositorio _modeloRepositorio;
        private readonly IOportunidadeRepositorio _oportunidadeRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IServicoRepositorio _servicoRepositorio;

        public BuscaController(
            IContaRepositorio contaRepositorio,
            IContatoRepositorio contatoRepositorio,
            IMercadoriaRepositorio mercadoriaRepositorio,
            IModeloRepositorio modeloRepositorio,
            IOportunidadeRepositorio oportunidadeRepositorio,
            IUsuarioRepositorio usuarioRepositorio,
            IServicoRepositorio servicoRepositorio)
        {
            _contaRepositorio = contaRepositorio;
            _contatoRepositorio = contatoRepositorio;
            _mercadoriaRepositorio = mercadoriaRepositorio;
            _modeloRepositorio = modeloRepositorio;
            _oportunidadeRepositorio = oportunidadeRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
            _servicoRepositorio = servicoRepositorio;
        }

        [HttpGet]
        [UsuarioExternoFilter]
        public ActionResult Index(string termo, string chave, string menu)
        {
            if (!string.IsNullOrWhiteSpace(termo))
            {
                var contas = _contaRepositorio.ObterContasPorDescricao(termo, (int?)ViewBag.UsuarioExternoId);
                var contatos = _contatoRepositorio.ObterContatosPorDescricao(termo, (int?)ViewBag.UsuarioExternoId);
                var mercadorias = _mercadoriaRepositorio.ObterMercadoriaPorDescricao(termo);
                var modelos = _modeloRepositorio.ObterModelosPorDescricao(termo);
                var oportunidades = _oportunidadeRepositorio.ObterOportunidadesPorDescricao(termo, (int?)ViewBag.UsuarioExternoId);
                var subClientes = _oportunidadeRepositorio.ObterSubClientesPorDescricao(termo, (int?)ViewBag.UsuarioExternoId);
                var clientesGrupo = _oportunidadeRepositorio.ObterClientesGrupoCNPJPorDescricao(termo, (int?)ViewBag.UsuarioExternoId);
                var adendosSubClientes = _oportunidadeRepositorio.ObterAdendosPorSubClientes(termo, (int?)ViewBag.UsuarioExternoId);
                var servicos = _servicoRepositorio.ObterServicosPorDescricao(termo);

                return View(new BuscaViewModel
                {
                    Termo = termo,
                    Contas = contas.ToList(),
                    Contatos = contatos.ToList(),
                    Mercadorias = mercadorias.ToList(),
                    Modelos = modelos.ToList(),
                    Oportunidades = oportunidades.ToList(),
                    SubClientes = subClientes.ToList(),
                    ClientesGrupoCNPJ = clientesGrupo.ToList(),
                    Servicos = servicos.ToList(),
                    AdendosSubClientes = adendosSubClientes.ToList()
                });
            }

            if (!string.IsNullOrWhiteSpace(chave))
            {
                var id = chave.ToInt();

                Conta conta = new Conta();
                Contato contato = new Contato();
                Mercadoria mercadoria = new Mercadoria();
                Modelo modelo = new Modelo();
                Servico servico = new Servico();
                Oportunidade oportunidade = new Oportunidade();
                List<OportunidadeDTO> oportunidades = new List<OportunidadeDTO>();
                List<ClientePropostaDTO> subClientes = new List<ClientePropostaDTO>();
                List<ClientePropostaDTO> clientesGrupo = new List<ClientePropostaDTO>();
                List<OportunidadeAdendoClientesDTO> adendosSubClientes = new List<OportunidadeAdendoClientesDTO>();

                var viewModel = new BuscaViewModel();

                switch (menu)
                {
                    case "Contas":

                        conta = _contaRepositorio.ObterContaPorId(id);

                        if (ViewBag.UsuarioExternoId != null)
                        {
                            if (!_usuarioRepositorio.ExisteVinculoConta(conta.Id, ViewBag.UsuarioExternoId))                            
                                break;                            
                        }                       

                        if (conta != null)
                        {
                            viewModel.Contas.Add(conta);

                            subClientes = _oportunidadeRepositorio.ObterSubClientesPorConta(conta.Id).ToList();

                            if (subClientes != null)
                                viewModel.SubClientes.AddRange(subClientes);

                            clientesGrupo = _oportunidadeRepositorio.ObterClientesGrupoCNPJPorConta(conta.Id).ToList();

                            if (clientesGrupo != null)
                                viewModel.ClientesGrupoCNPJ.AddRange(clientesGrupo);

                            oportunidades = _oportunidadeRepositorio.ObterOportunidadesPorConta(conta.Id).ToList();

                            if (oportunidades != null)
                                viewModel.Oportunidades.AddRange(oportunidades);
                        }

                        break;
                    case "Contatos":
                        contato = _contatoRepositorio.ObterContatoPorId(id);

                        if (contato != null)
                            viewModel.Contatos.Add(contato);

                        break;
                    case "Mercadorias":
                        mercadoria = _mercadoriaRepositorio.ObterMercadoriaPorId(id);

                        if (mercadoria != null)
                            viewModel.Mercadorias.Add(mercadoria);

                        break;
                    case "Modelos":
                        modelo = _modeloRepositorio.ObterModeloPorId(id);

                        if (modelo != null)
                            viewModel.Modelos.Add(modelo);
                        break;
                    case "Oportunidades":

                        oportunidade = _oportunidadeRepositorio.ObterOportunidadePorId(id);

                        if (ViewBag.UsuarioExternoId != null)
                        {
                            if (!_usuarioRepositorio.ExisteVinculoConta(oportunidade.ContaId, ViewBag.UsuarioExternoId))
                                break;
                        }

                        if (oportunidade != null)
                            viewModel.Oportunidades.Add(new OportunidadeDTO
                            {
                                Id = oportunidade.Id,
                                Identificacao = oportunidade.Identificacao,
                                Descricao = oportunidade.Descricao,
                                ContaDescricao = oportunidade.Conta.Descricao,
                                SucessoNegociacao = oportunidade.SucessoNegociacao,
                                StatusOportunidade = oportunidade.StatusOportunidade
                            });
                        break;
                    case "Serviços":
                        servico = _servicoRepositorio.ObterServicoPorId(id);

                        if (servico != null)
                            viewModel.Servicos.Add(servico);
                        break;
                    default:
                        break;
                }

                return View(viewModel);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}