using  Ecoporto.CRM.Business.Enums;
using System;
using System.Linq;
using System.Web.Services;
using WsSimuladorCalculoTabelas.DAO;
using WsSimuladorCalculoTabelas.Enums;
using WsSimuladorCalculoTabelas.Extensions;
using WsSimuladorCalculoTabelas.Helpers;
using WsSimuladorCalculoTabelas.Models;
using WsSimuladorCalculoTabelas.Responses;
using WsSimuladorCalculoTabelas.Services.IPA;

namespace WsSimuladorCalculoTabelas
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]

    public class IntegraChronos : WebService
    {
        private readonly TabelasDAO _tabelasDAO;
        private readonly OportunidadeDAO _oportunidadeDAO;
        private readonly PagamentoPixDAO _pagamentoPixDAO;

        public IntegraChronos()
        {
            _tabelasDAO = new TabelasDAO(false);
            _oportunidadeDAO = new OportunidadeDAO();
            _pagamentoPixDAO = new PagamentoPixDAO();
        }

        [WebMethod(Description = "Exportar Serviços e criar a Tabela de Cobranças a partir de uma Oportunidade")]
        public Response ExportarTabelas(int oportunidadeId, int usuarioId)
        {
            try
            {
                var oportunidade = _oportunidadeDAO.ObterOportunidadePorId(oportunidadeId);

                if (oportunidade == null)
                {
                    return new Response
                    {
                        Sucesso = false,
                        Mensagem = $"Oportunidade {oportunidadeId} não encontrada"
                    };
                }

                if (oportunidade.TipoOperacao != TipoOperacao.RA)
                {
                    return new Response
                    {
                        Sucesso = false,
                        Mensagem = $"Disponível apenas para Tipo de Operação {oportunidade.TipoOperacao.ToName()}"
                    };
                }

                var tabelaCobrancaService = new TabelaCobrancaService(oportunidadeId)
                    .ExportarTabela(usuarioId);

                if (oportunidade.FormaPagamentoId == FormaPagamento.FATURADO)
                {
                    var fichasFaturamento = _oportunidadeDAO.ObterFichasFaturamento(oportunidade.Id);

                    foreach (var ficha in fichasFaturamento)
                    {
                        IntregrarFichasChronos(oportunidade.Id, ficha.Id);
                    }
                }

                return tabelaCobrancaService;
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Sucesso = false,
                    Mensagem = $"Falha ao simular a Oportunidade. {ex.Message}"
                };
            }
        }

        [WebMethod(Description = "Exportar Serviços e criar a Tabela de Cobranças a partir de uma Oportunidade")]
        public Response CancelarTabela(int oportunidadeId, int usuarioId)
        {
            try
            {
                var oportunidade = _oportunidadeDAO.ObterOportunidadePorId(oportunidadeId);

                if (oportunidade == null)
                {
                    return new Response
                    {
                        Sucesso = false,
                        Mensagem = $"Oportunidade {oportunidadeId} não encontrada"
                    };
                }

                if (oportunidade.TipoOperacao != TipoOperacao.RA)
                {
                    return new Response
                    {
                        Sucesso = false,
                        Mensagem = $"Disponível apenas para Tipo de Operação {oportunidade.TipoOperacao.ToName()}"
                    };
                }

                var tabelaCobrancaService = new TabelaCobrancaService(oportunidadeId)
                    .CancelarTabela(usuarioId);

                return new Response
                {
                    Sucesso = true,
                    Mensagem = tabelaCobrancaService.Mensagem
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Sucesso = false,
                    Mensagem = $"Falha ao simular a Oportunidade. {ex.Message}"
                };
            }
        }

        [WebMethod(Description = "Integrações com o Sistema Chronos")]
        public Response IntregrarFichasChronos(int oportunidadeId, int fichaId)
        {
            try
            {
                var oportunidadeBusca = _oportunidadeDAO.ObterOportunidadePorId(oportunidadeId);

                if (oportunidadeBusca == null)
                    return new Response
                    {
                        Sucesso = false,
                        Mensagem = $"Oportunidade {oportunidadeId} não encontrada"
                    };

                if (oportunidadeBusca.TabelaId == 0)
                    return new Response
                    {
                        Sucesso = false,
                        Mensagem = $"Oportunidade {oportunidadeId} não possui nenhuma Tabela vinculada"
                    };

                var fichaBusca = _oportunidadeDAO.ObterFichaFaturamentoPorId(fichaId);

                if (fichaBusca == null)
                    return new Response
                    {
                        Sucesso = false,
                        Mensagem = $"Ficha {fichaId} não encontrada"
                    };

                if (fichaBusca.StatusFichaFaturamento != StatusFichaFaturamento.APROVADO && fichaBusca.StatusFichaFaturamento != StatusFichaFaturamento.EM_APROVACAO)
                    return new Response
                    {
                        Sucesso = false,
                        Mensagem = $"O Status atual da Ficha não permite integrações com o Chronos"
                    };

                if (!string.IsNullOrEmpty(fichaBusca.FaturadoContraDocumento))
                {
                    var faturadoContraChronos = _tabelasDAO.ConsultaParceiro(fichaBusca.FaturadoContraDocumento);

                    if (faturadoContraChronos == null)
                    {
                        var parceiro = new Parceiro
                        {
                            RazaoSocial = fichaBusca.FaturadoContraDescricao,
                            NomeFantasia = fichaBusca.FaturadoContraFantasia,
                            CNPJ = fichaBusca.FaturadoContraDocumento
                        };

                        fichaBusca.FaturadoContraId = _tabelasDAO.CadastrarParceiro(parceiro);
                    }
                    else
                    {
                        fichaBusca.FaturadoContraId = faturadoContraChronos.Id;
                    }
                }

                if (!string.IsNullOrEmpty(fichaBusca.FontePagadoraDocumento))
                {
                    var fontePagadoraChronos = _tabelasDAO.ConsultaParceiro(fichaBusca.FontePagadoraDocumento);

                    if (fontePagadoraChronos == null)
                    {
                        var parceiro = new Parceiro
                        {
                            RazaoSocial = fichaBusca.FontePagadoraDescricao,
                            NomeFantasia = fichaBusca.FontePagadoraFantasia,
                            CNPJ = fichaBusca.FontePagadoraDocumento
                        };

                        fichaBusca.FontePagadoraId = _tabelasDAO.CadastrarParceiro(parceiro);
                    }
                    else
                    {
                        fichaBusca.FontePagadoraId = fontePagadoraChronos.Id;
                    }
                }

                fichaBusca.FichaGeral = (oportunidadeBusca.ContaId == fichaBusca.ContaId);               

                var subClientes = _oportunidadeDAO.ObterSubClientesOportunidade(oportunidadeBusca.Id);

                fichaBusca.SubClientes.AddRange(subClientes);

                if (fichaBusca.RevisaoId == 0 || (fichaBusca.RevisaoId > 0 && fichaBusca.FichaGeral))
                {
                    _oportunidadeDAO.IntegrarFontePagadora(fichaBusca);                   
                }
                else
                {
                    _oportunidadeDAO.IntegrarFontePagadoraRevisada(fichaBusca);
                }               

                return new Response
                {
                    Sucesso = true,
                    Mensagem = $"Ficha de Faturamento integrada com sucesso!"
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Sucesso = true,
                    Mensagem = ex.Message
                };
            }
        }

        [WebMethod(Description = "Integrações com o Sistema Chronos")]
        public Response IntregrarAdendosChronos(int oportunidadeId, int adendoId)
        {
            string descricaoOperacao = string.Empty;

            try
            {
                var oportunidadeBusca = _oportunidadeDAO.ObterOportunidadePorId(oportunidadeId);

                if (oportunidadeBusca == null)
                    return new Response
                    {
                        Sucesso = false,
                        Mensagem = $"Oportunidade {oportunidadeId} não encontrada"
                    };

                if (oportunidadeBusca.TabelaId == 0)
                    return new Response
                    {
                        Sucesso = false,
                        Mensagem = $"Oportunidade {oportunidadeId} não possui nenhuma Tabela vinculada"
                    };

                var adendoBusca = _oportunidadeDAO.ObterAdendoPorId(adendoId);

                if (adendoBusca == null)
                    return new Response
                    {
                        Sucesso = false,
                        Mensagem = $"Adendo {adendoId} não encontrado"
                    };

                if (adendoBusca.StatusAdendo != StatusAdendo.APROVADO && adendoBusca.StatusAdendo != StatusAdendo.ENVIADO)
                    return new Response
                    {
                        Sucesso = false,
                        Mensagem = $"O Status atual do Adendo não permite integrações com o Chronos"
                    };

                if (oportunidadeBusca.TipoOperacao != TipoOperacao.RA)
                    return new Response
                    {
                        Sucesso = false,
                        Mensagem = $"Operação não disponível para Oportunidades {oportunidadeBusca.TipoOperacao.ToName()}"
                    };

                if (adendoBusca.TipoAdendo == TipoAdendo.FORMA_PAGAMENTO)
                {
                    descricaoOperacao = "Alteração de Forma de Pagamento";

                    var formaPagamentoAdendo = _oportunidadeDAO.ObterFormaPagamentoAdendoPorId(adendoBusca.Id);

                    var perfil = formaPagamentoAdendo == FormaPagamento.FATURADO ? "A" : "";

                    _tabelasDAO.AtualizarFormaPagamentoTabelaIPA(oportunidadeBusca.TabelaId,
                        ConverteFormaPagamentoIPA.FormaPagamentoIPA(formaPagamentoAdendo.ToValue()), perfil);

                    // Quando integrar de Faturado para a Vista, excluir a fonte pagadora vinculada a tabela
                    if (formaPagamentoAdendo == FormaPagamento.AVISTA)
                        _tabelasDAO.ExcluirFontePagadora(oportunidadeBusca.TabelaId);

                    _tabelasDAO.AtualizarObservacoesTabela(oportunidadeBusca.TabelaId, $" / A-{adendoId} ({formaPagamentoAdendo.ToName()})");

                    if (formaPagamentoAdendo == FormaPagamento.FATURADO)
                    {
                        var fichasFaturamento = _oportunidadeDAO.ObterFichasFaturamento(oportunidadeBusca.Id);

                        foreach (var ficha in fichasFaturamento)
                        {
                            IntregrarFichasChronos(oportunidadeBusca.Id, ficha.Id);
                        }
                    }
                }

                if (adendoBusca.TipoAdendo == TipoAdendo.INCLUSAO_SUB_CLIENTE)
                {
                    if (oportunidadeBusca.SegmentoId == Segmento.DESPACHANTE && oportunidadeBusca.TipoDePropostaId == TipoDeProposta.GERAL)
                    {
                        return new Response
                        {
                            Sucesso = false,
                            Mensagem = $"Não disponível para Segmento {oportunidadeBusca.SegmentoId.ToName()} e Tipo Proposta {oportunidadeBusca.TipoDePropostaId.ToName()}"
                        };
                    }

                    if ((oportunidadeBusca.SegmentoId == Segmento.AGENTE || oportunidadeBusca.SegmentoId == Segmento.FREIGHT_FORWARDER) && oportunidadeBusca.TipoDePropostaId == TipoDeProposta.GERAL && oportunidadeBusca.TipoServicoId == TipoServico.FCL)
                    {
                        return new Response
                        {
                            Sucesso = false,
                            Mensagem = $"Não disponível para Segmentos Agente / Freight Forward e Tipo Proposta {oportunidadeBusca.TipoDePropostaId.ToName()} FCL"
                        };
                    }

                    descricaoOperacao = "Inclusão de Sub Cliente(s)";

                    var subClientes = _oportunidadeDAO.ObterSubClientesAdendo(adendoBusca.Id, oportunidadeBusca.Id, AdendoAcao.INCLUSAO).ToList();

                    if (subClientes.Count == 0)
                        return new Response
                        {
                            Sucesso = false,
                            Mensagem = $"Nenhum Sub Cliente vinculado ao Adendo"
                        };

                    foreach (var subCliente in subClientes)
                    {
                        var parceiroBusca = _tabelasDAO.ConsultaParceiro(subCliente.Documento);

                        if (parceiroBusca == null)
                        {
                            parceiroBusca = new Parceiro();

                            var parceiro = new Parceiro
                            {
                                RazaoSocial = subCliente.Descricao,
                                NomeFantasia = subCliente?.NomeFantasia,
                                CNPJ = subCliente.Documento
                            };

                            parceiroBusca.Id = _tabelasDAO.CadastrarParceiro(parceiro);
                        }

                        _tabelasDAO.AtualizarSegmentoParceiro(parceiroBusca.Id, subCliente.Segmento);

                        switch (subCliente.Segmento)
                        {
                            case SegmentoSubCliente.IMPORTADOR:
                                if (!_tabelasDAO.ExisteParceiroGrupo(oportunidadeBusca.TabelaId, parceiroBusca.Id, "I"))
                                    _tabelasDAO.IncluirParceiroGrupo(oportunidadeBusca.TabelaId, parceiroBusca.Id, "I");

                                break;
                            case SegmentoSubCliente.DESPACHANTE:
                                if (!_tabelasDAO.ExisteParceiroGrupo(oportunidadeBusca.TabelaId, parceiroBusca.Id, "D"))
                                    _tabelasDAO.IncluirParceiroGrupo(oportunidadeBusca.TabelaId, parceiroBusca.Id, "D");

                                break;
                            case SegmentoSubCliente.COLOADER:
                                if (!_tabelasDAO.ExisteParceiroGrupo(oportunidadeBusca.TabelaId, parceiroBusca.Id, "S"))
                                    _tabelasDAO.IncluirParceiroGrupo(oportunidadeBusca.TabelaId, parceiroBusca.Id, "S");

                                break;
                            case SegmentoSubCliente.CO_COLOADER1:
                                if (!_tabelasDAO.ExisteParceiroGrupo(oportunidadeBusca.TabelaId, parceiroBusca.Id, "U"))
                                    _tabelasDAO.IncluirParceiroGrupo(oportunidadeBusca.TabelaId, parceiroBusca.Id, "U");

                                break;
                            case SegmentoSubCliente.CO_COLOADER2:
                                if (!_tabelasDAO.ExisteParceiroGrupo(oportunidadeBusca.TabelaId, parceiroBusca.Id, "B"))
                                    _tabelasDAO.IncluirParceiroGrupo(oportunidadeBusca.TabelaId, parceiroBusca.Id, "B");

                                break;
                        }

                        if (oportunidadeBusca.FormaPagamentoId == FormaPagamento.FATURADO)
                        {
                            var fichaFaturamento = _oportunidadeDAO.ObterFichasFaturamento(oportunidadeBusca.Id)
                                .Where(c => c.ContaDocumento == subCliente.Documento).FirstOrDefault();

                            if (fichaFaturamento != null)
                            {

                                var ficha = _oportunidadeDAO.ObterFichaFaturamentoPorId(fichaFaturamento.Id);

                                ficha.SegmentoSubCliente = subCliente.Segmento;

                                if (ficha.StatusFichaFaturamento != StatusFichaFaturamento.APROVADO && ficha.StatusFichaFaturamento != StatusFichaFaturamento.EM_APROVACAO)
                                    return new Response
                                    {
                                        Sucesso = false,
                                        Mensagem = $"O Status atual da Ficha não permite integrações com o Chronos"
                                    };

                                if (!string.IsNullOrEmpty(ficha.FaturadoContraDocumento))
                                {
                                    var faturadoContraChronos = _tabelasDAO.ConsultaParceiro(ficha.FaturadoContraDocumento);

                                    if (faturadoContraChronos == null)
                                    {
                                        var parceiro = new Parceiro
                                        {
                                            RazaoSocial = ficha.FaturadoContraDescricao,
                                            NomeFantasia = ficha.FaturadoContraFantasia,
                                            CNPJ = ficha.FaturadoContraDocumento
                                        };

                                        ficha.FaturadoContraId = _tabelasDAO.CadastrarParceiro(parceiro);
                                    }
                                    else
                                    {
                                        ficha.FaturadoContraId = faturadoContraChronos.Id;
                                    }
                                }

                                if (!string.IsNullOrEmpty(ficha.FontePagadoraDocumento))
                                {
                                    var fontePagadoraChronos = _tabelasDAO.ConsultaParceiro(ficha.FontePagadoraDocumento);

                                    if (fontePagadoraChronos == null)
                                    {
                                        var parceiro = new Parceiro
                                        {
                                            RazaoSocial = ficha.FontePagadoraDescricao,
                                            NomeFantasia = ficha.FontePagadoraFantasia,
                                            CNPJ = ficha.FontePagadoraDocumento
                                        };

                                        ficha.FontePagadoraId = _tabelasDAO.CadastrarParceiro(parceiro);
                                    }
                                    else
                                    {
                                        ficha.FontePagadoraId = fontePagadoraChronos.Id;
                                    }
                                }

                                var ehFichaGeral = (oportunidadeBusca.ContaId == ficha.ContaId);

                                ficha.FichaGeral = ehFichaGeral;

                                ficha.SubClientes.Add(subCliente);

                                _oportunidadeDAO.IntegrarFontePagadora(ficha);

                                //return new Response
                                //{
                                //    Sucesso = true,
                                //    Mensagem = $"Ficha de Faturamento integrada com sucesso!"
                                //};
                            }
                        }
                    }

                    _tabelasDAO.AtualizarObservacoesTabela(oportunidadeBusca.TabelaId, $" / A-{adendoId}");
                }

                if (adendoBusca.TipoAdendo == TipoAdendo.INCLUSAO_GRUPO_CNPJ)
                {
                    descricaoOperacao = "Inclusão de Grupo(s) CNPJ";

                    var gruposCnpj = _oportunidadeDAO.ObterClientesGrupoCnpjAdendo(adendoBusca.Id, oportunidadeBusca.Id, AdendoAcao.INCLUSAO).ToList();

                    if (gruposCnpj.Count == 0)
                        return new Response
                        {
                            Sucesso = false,
                            Mensagem = $"Nenhum Grupo CNPJ vinculado ao Adendo"
                        };

                    foreach (var grupoCnpj in gruposCnpj)
                    {
                        var parceiroBusca = _tabelasDAO.ConsultaParceiro(grupoCnpj.Documento);

                        if (parceiroBusca == null)
                        {
                            parceiroBusca = new Parceiro();

                            var parceiro = new Parceiro
                            {
                                RazaoSocial = grupoCnpj.Descricao,
                                NomeFantasia = grupoCnpj?.NomeFantasia,
                                CNPJ = grupoCnpj.Documento
                            };

                            parceiroBusca.Id = _tabelasDAO.CadastrarParceiro(parceiro);                            
                        }
                        
                        //_tabelasDAO.AtualizarSegmentoParceiro(parceiroBusca.Id, oportunidadeBusca.SegmentoId);
                        _tabelasDAO.AtualizarSegmentoParceiroGrupoCNPJ(parceiroBusca.Id, grupoCnpj.SegmentoConta);

                        switch (oportunidadeBusca.SegmentoId)
                        {
                            case Segmento.IMPORTADOR:
                                if (!_tabelasDAO.ExisteParceiroGrupo(oportunidadeBusca.TabelaId, parceiroBusca.Id, "I"))
                                    _tabelasDAO.IncluirParceiroGrupo(oportunidadeBusca.TabelaId, parceiroBusca.Id, "I");

                                break;
                            case Segmento.DESPACHANTE:
                                if (!_tabelasDAO.ExisteParceiroGrupo(oportunidadeBusca.TabelaId, parceiroBusca.Id, "D"))
                                    _tabelasDAO.IncluirParceiroGrupo(oportunidadeBusca.TabelaId, parceiroBusca.Id, "D");

                                break;
                            case Segmento.AGENTE:
                            case Segmento.FREIGHT_FORWARDER:
                                if (!_tabelasDAO.ExisteParceiroGrupo(oportunidadeBusca.TabelaId, parceiroBusca.Id, "C"))
                                    _tabelasDAO.IncluirParceiroGrupo(oportunidadeBusca.TabelaId, parceiroBusca.Id, "C");

                                break;
                            case Segmento.COLOADER:
                                if (!_tabelasDAO.ExisteParceiroGrupo(oportunidadeBusca.TabelaId, parceiroBusca.Id, "S"))
                                    _tabelasDAO.IncluirParceiroGrupo(oportunidadeBusca.TabelaId, parceiroBusca.Id, "S");

                                break;
                        }
                    }

                    _tabelasDAO.AtualizarObservacoesTabela(oportunidadeBusca.TabelaId, $" / A-{adendoId} (GRUPO)");
                }

                if (adendoBusca.TipoAdendo == TipoAdendo.EXCLUSAO_SUB_CLIENTE)
                {
                    descricaoOperacao = "Exclusão de Sub Cliente(s)";

                    var subClientes = _oportunidadeDAO.ObterSubClientesAdendo(adendoBusca.Id, oportunidadeBusca.Id, AdendoAcao.EXCLUSAO).ToList();

                    if (subClientes.Count == 0)
                        return new Response
                        {
                            Sucesso = false,
                            Mensagem = $"Nenhum Sub Cliente vinculado ao Adendo"
                        };

                    foreach (var subCliente in subClientes)
                    {
                        var parceiroBusca = _tabelasDAO.ConsultaParceiro(subCliente.Documento);

                        if (parceiroBusca != null)
                        {
                            switch (subCliente.Segmento)
                            {
                                case SegmentoSubCliente.IMPORTADOR:

                                    if (_tabelasDAO.ExisteOutrosSegmentosNoGrupo(oportunidadeBusca.TabelaId, parceiroBusca.Id, "I"))
                                    {
                                        _tabelasDAO.ExcluirParceiroGrupo(oportunidadeBusca.TabelaId, parceiroBusca.Id, "I");

                                        var importadorPrincipal = _tabelasDAO.ObterImportadorPrincipal(oportunidadeBusca.TabelaId);

                                        if (importadorPrincipal == parceiroBusca.Id)
                                        {
                                            importadorPrincipal = _tabelasDAO.ObterParceiroGrupo(oportunidadeBusca.TabelaId, "I");

                                            if (importadorPrincipal > 0)
                                            {
                                                _tabelasDAO.AtualizarImportadorTabela(oportunidadeBusca.TabelaId, importadorPrincipal);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return new Response
                                        {
                                            Sucesso = false,
                                            Mensagem = $"Integração não permitida. O Sub Cliente é o único Importador do grupo!"
                                        };
                                    }

                                    break;

                                case SegmentoSubCliente.DESPACHANTE:

                                    if (_tabelasDAO.ExisteOutrosSegmentosNoGrupo(oportunidadeBusca.TabelaId, parceiroBusca.Id, "D"))
                                    {
                                        _tabelasDAO.ExcluirParceiroGrupo(oportunidadeBusca.TabelaId, parceiroBusca.Id, "D");

                                        var despachantePrincipal = _tabelasDAO.ObterDespachantePrincipal(oportunidadeBusca.TabelaId);

                                        if (despachantePrincipal == parceiroBusca.Id)
                                        {
                                            despachantePrincipal = _tabelasDAO.ObterParceiroGrupo(oportunidadeBusca.TabelaId, "D");

                                            if (despachantePrincipal > 0)
                                            {
                                                _tabelasDAO.AtualizarDespachanteTabela(oportunidadeBusca.TabelaId, despachantePrincipal);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return new Response
                                        {
                                            Sucesso = false,
                                            Mensagem = $"Integração não permitida. O Sub Cliente é o único Despachante do grupo!"
                                        };
                                    }

                                    break;

                                case SegmentoSubCliente.COLOADER:

                                    if (_tabelasDAO.ExisteOutrosSegmentosNoGrupo(oportunidadeBusca.TabelaId, parceiroBusca.Id, "S"))
                                    {
                                        _tabelasDAO.ExcluirParceiroGrupo(oportunidadeBusca.TabelaId, parceiroBusca.Id, "S");

                                        var coloaderPrincipal = _tabelasDAO.ObterColoaderPrincipal(oportunidadeBusca.TabelaId);

                                        if (coloaderPrincipal == parceiroBusca.Id)
                                        {
                                            coloaderPrincipal = _tabelasDAO.ObterParceiroGrupo(oportunidadeBusca.TabelaId, "S");

                                            if (coloaderPrincipal > 0)
                                            {
                                                _tabelasDAO.AtualizarColoaderTabela(oportunidadeBusca.TabelaId, coloaderPrincipal);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return new Response
                                        {
                                            Sucesso = false,
                                            Mensagem = $"Integração não permitida. O Sub Cliente é o único Coloader do grupo!"
                                        };
                                    }

                                    break;

                                case SegmentoSubCliente.CO_COLOADER1:

                                    if (_tabelasDAO.ExisteOutrosSegmentosNoGrupo(oportunidadeBusca.TabelaId, parceiroBusca.Id, "U"))
                                    {
                                        _tabelasDAO.ExcluirParceiroGrupo(oportunidadeBusca.TabelaId, parceiroBusca.Id, "U");

                                        var cocoloaderPrincipal = _tabelasDAO.ObterCoColoaderPrincipal(oportunidadeBusca.TabelaId);

                                        if (cocoloaderPrincipal == parceiroBusca.Id)
                                        {
                                            cocoloaderPrincipal = _tabelasDAO.ObterParceiroGrupo(oportunidadeBusca.TabelaId, "U");

                                            if (cocoloaderPrincipal > 0)
                                            {
                                                _tabelasDAO.AtualizarCoColoaderTabela(oportunidadeBusca.TabelaId, cocoloaderPrincipal);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return new Response
                                        {
                                            Sucesso = false,
                                            Mensagem = $"Integração não permitida. O Sub Cliente é o único CoColoader do grupo!"
                                        };
                                    }

                                    break;

                                case SegmentoSubCliente.CO_COLOADER2:

                                    if (_tabelasDAO.ExisteOutrosSegmentosNoGrupo(oportunidadeBusca.TabelaId, parceiroBusca.Id, "B"))
                                    {
                                        _tabelasDAO.ExcluirParceiroGrupo(oportunidadeBusca.TabelaId, parceiroBusca.Id, "B");

                                        var cocoloader2Principal = _tabelasDAO.ObterCoColoader2Principal(oportunidadeBusca.TabelaId);

                                        if (cocoloader2Principal == parceiroBusca.Id)
                                        {
                                            cocoloader2Principal = _tabelasDAO.ObterParceiroGrupo(oportunidadeBusca.TabelaId, "U");

                                            if (cocoloader2Principal > 0)
                                            {
                                                _tabelasDAO.AtualizarCoColoader2Tabela(oportunidadeBusca.TabelaId, cocoloader2Principal);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return new Response
                                        {
                                            Sucesso = false,
                                            Mensagem = $"Integração não permitida. O Sub Cliente é o único CoColoader2 do grupo!"
                                        };
                                    }

                                    break;
                            }
                        }
                    }

                    _tabelasDAO.AtualizarObservacoesTabela(oportunidadeBusca.TabelaId, $" / A-{adendoId} (EXCLUSÃO)");
                }

                if (adendoBusca.TipoAdendo == TipoAdendo.EXCLUSAO_GRUPO_CNPJ)
                {
                    descricaoOperacao = "Exclusão de Grupo(s) CNPJ";

                    var gruposCnpj = _oportunidadeDAO.ObterClientesGrupoCnpjAdendo(adendoBusca.Id, oportunidadeBusca.Id, AdendoAcao.EXCLUSAO).ToList();

                    if (gruposCnpj.Count == 0)
                        return new Response
                        {
                            Sucesso = false,
                            Mensagem = $"Nenhum Grupo CNPJ vinculado ao Adendo"
                        };

                    foreach (var grupoCnpj in gruposCnpj)
                    {
                        var parceiroBusca = _tabelasDAO.ConsultaParceiro(grupoCnpj.Documento);

                        if (parceiroBusca != null)
                        {
                            switch (oportunidadeBusca.SegmentoId)
                            {
                                case Segmento.IMPORTADOR:
                                    _tabelasDAO.ExcluirParceiroGrupo(oportunidadeBusca.TabelaId, parceiroBusca.Id, "I");
                                    break;
                                case Segmento.DESPACHANTE:
                                    _tabelasDAO.ExcluirParceiroGrupo(oportunidadeBusca.TabelaId, parceiroBusca.Id, "D");
                                    break;
                                case Segmento.AGENTE:
                                    _tabelasDAO.ExcluirParceiroGrupo(oportunidadeBusca.TabelaId, parceiroBusca.Id, "C");
                                    break;
                                case Segmento.COLOADER:
                                    _tabelasDAO.ExcluirParceiroGrupo(oportunidadeBusca.TabelaId, parceiroBusca.Id, "S");
                                    break;
                            }
                        }
                    }

                    _tabelasDAO.AtualizarObservacoesTabela(oportunidadeBusca.TabelaId, $" / A-{adendoId} (GRUPO EXCLUSÃO)");
                }

                return new Response
                {
                    Sucesso = true,
                    Mensagem = $"{descricaoOperacao} realizada com sucesso!"
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Sucesso = false,
                    Mensagem = $"Falha durante a {descricaoOperacao}"
                };
            }
        }

        [WebMethod(Description = "Cancela Titulo PIX")]
        public Response CancelaTituloPix(long NumeroTitulo)
        {
            try
            {

                if (NumeroTitulo == 0)
                {
                    return new Response
                    {
                        Sucesso = false,
                        Mensagem = $"Numero do Titulo não encontrada"
                    };
                }


                var updatePix = _pagamentoPixDAO.CancelaTituloPix(NumeroTitulo);

                if ((updatePix.Substring(0, 3)) == "000")
                {
                    return new Response
                    {
                        Sucesso = true,
                        Mensagem = updatePix
                    };
                }
                else
                {
                    return new Response
                    {
                        Sucesso =false,
                        Mensagem = updatePix
                    };

                }
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Sucesso = false,
                    Mensagem = $"Falha durante a operação de Baixa"
                };
            }
        }

    
        [WebMethod(Description = "Integrações Baixa GR ")]
        public Response IntregrarBaixaChronos(long NumeroTitulo)
        {

            int Lote = 0;
            int Seq_Gr = 0;
            int BL = 0;
            int Usuario = 90;
            int contar = 0;
            bool blreefer = false;
            string SisFin = "SAP";
            int diasAdicionais = 0;
            int seq_gr = 0;
            int cod_empresa =0;
            string servico = "";
            double valor = 0;
            string titSapiens = "";

            try
            {
                if (NumeroTitulo == 0)
                {
                    return new Response
                    {
                        Sucesso = false,
                        Mensagem = $"NUmero do Titulo não encontrada"
                    };
                }



                var dadosLotes = _pagamentoPixDAO.GetListaBL(NumeroTitulo);

                foreach (var item in dadosLotes)
                {
                    Lote = Convert.ToInt32(item.LOTE);
                    Seq_Gr = item.SEQ_GR;
                    BL = item.AUTONUM;
                    cod_empresa=item.PATIO;


                        contar= _pagamentoPixDAO.Verificacalculo(Lote);
                        if (contar == 0)
                        {
                            return new Response
                            {
                                Sucesso = false,
                                Mensagem = " Lote sem cálculo pendente "
                            };
                        }

                        contar = _pagamentoPixDAO.Verificaformapagamento(Lote);
                        if (contar != 2)
                        {
                            return new Response
                            {
                                Sucesso = false,
                                Mensagem = " Lote não tem a forma de pagamento A vista "
                            };
                        }

                        contar = _pagamentoPixDAO.Verificaliberado(Lote);
                        if (contar != 1)
                        {
                            return new Response
                            {
                                Sucesso = false,
                                Mensagem = " Lote não tem liberação de calculo "
                            };
                        }


                        contar = _pagamentoPixDAO.VerificaReefer(Lote);
                        if (contar == 0)
                        {
                            blreefer = false;
                        }
                        else
                        {
                            blreefer = true;
                        }
                        


                    var dadosGRPre = _pagamentoPixDAO.obtemDadosGRPre(Lote);

                    DateTime DataFinal = dadosGRPre.Data_Final;
                    DateTime DataDoc = dadosGRPre.Data_Doc;
                    DateTime ValidadeGR = dadosGRPre.VALIDADE_GR;
                    int Periodos = dadosGRPre.PERIODOS;
                    DateTime DataBase = dadosGRPre.Data_Base;
                    int idContrato = dadosGRPre.Lista;
                    DateTime DataFiltro = dadosGRPre.DT_INICIO_CALCULO;
                    DateTime DataLiberado = dadosGRPre.dt_liberacao;
                    DateTime DataReefer = dadosGRPre.DATA_REFER;

                    if (blreefer == true)
                        {
                            if (DataReefer == null)
                            {
                                return new Response
                                {
                                    Sucesso = false,
                                    Mensagem = " Gr sem Free-Time Reefer Recalcule a GR"
                                };
                            }

                            if (DateTime.Now > DataReefer)
                            {
                                return new Response
                                {
                                    Sucesso = false,
                                    Mensagem = " Data Atual maior que o Free-Time Recalcule a GR"
                                };
                            }
                        }


              

                    var Tbh = _pagamentoPixDAO.obtemMaiorDataFinalGRPre(Lote);
                    int diaSemana = Convert.ToInt32(Tbh.Data_Final.DayOfWeek);

                    if (Tbh == null)
                    {
                        return new Response
                        {
                            Sucesso = false,
                            Mensagem = "Calculo Vencido - Favor executar o cálculo novamente"
                        };
                    }
                    string WdT = Tbh.Data_Final.ToString("dd/MM/yyyy");

                    if (_pagamentoPixDAO.EFeriado(WdT) != null)
                        WdT = Tbh.Data_Final.AddDays(1).ToString();

                    if (diaSemana == 7)
                        WdT = Tbh.Data_Final.AddDays(2).ToString();

                    if (diaSemana == 1)
                        WdT = Tbh.Data_Final.AddDays(1).ToString();

                    if (DataDoc == null)
                        DataDoc = Convert.ToDateTime(WdT);

                    if (DateTime.Now > Convert.ToDateTime(WdT) && DateTime.Now > DataDoc)
                    {
                        if (_pagamentoPixDAO.EFeriado(WdT) != null)
                        {
                                return new Response
                                {
                                    Sucesso = false,
                                    Mensagem = "Calculo Vencido - Favor executar o calculo novamente"
                                };
                        }
                    }

                        contar= _pagamentoPixDAO.verificaBLSemSaida(Lote);

                        if (contar == 0)
                        {
                            return new Response
                            {
                                Sucesso = false,
                                Mensagem = "Lote com saída , favor verificar som setor responsável"
                            };
                        }


                   if (ValidadeGR == null)
                    {
                        return new Response
                        {
                            Sucesso = false,
                            Mensagem = "Data de validade da GR inválida",
                        };
                    }

                    if (DataBase == null)
                    {
                        return new Response
                        {
                            Sucesso = false,
                            Mensagem = "Data base do cálculo inválida",
                        };
                    }

                    if (ValidadeGR < DataBase)
                    {
                        return new Response
                        {
                            Sucesso = false,
                            Mensagem = "Data de validade menor que a Data Base",
                        };
                    }

                    if (_pagamentoPixDAO.UsuarioEmProcessoDeCalculo(Lote) >0)
                    {

                        return new Response
                        {
                            Sucesso = false,
                            Mensagem = "O usuário em processo de cálculo "
                        };
                    }

                   seq_gr= _pagamentoPixDAO.obtemProximoNumGR();

                    var dadosPeriodoGr = _pagamentoPixDAO.obtemDadosPeriodoGR(Lote);
                    string wInicio = "";
                    string wFinal = "";
                     int wperiodos = 0;

                    if (dadosPeriodoGr != null)
                    {
                        wInicio = dadosPeriodoGr.INICIO.ToString("dd/MM/yyyy");
                        wFinal = dadosPeriodoGr.FINAL.ToString("dd/MM/yyyy");
                        wperiodos = dadosPeriodoGr.PERIODOS;
                    }
                    Seq_Gr = seq_gr;
                               
                    _pagamentoPixDAO.atualizaGREmServico(Lote, Seq_Gr, 90);
                    _pagamentoPixDAO.atualizaGREmDescricao(Lote, Seq_Gr);
                    _pagamentoPixDAO.atualizaGREmCNTR(Lote, Seq_Gr);
                    _pagamentoPixDAO.atualizaGREmCS(Lote, Seq_Gr);

                        var insereGr = _pagamentoPixDAO.inseregr_bl(Seq_Gr, Lote, wInicio, wFinal,NumeroTitulo);

                      //  var dadosAtualizaDadosVencimento = _pagamentoPixDAO.GetDadosAtualizaVencimento(Lote);

                 

                   var QdeLavagemCtnr = _pagamentoPixDAO.obtemQtdLavagemCNTR(Lote, Seq_Gr);

                    if (QdeLavagemCtnr != 0)
                    {
                        _pagamentoPixDAO.atualizaAMRNFCNTRLavagem(Lote, Seq_Gr);
                    }

                    _pagamentoPixDAO.atualizaServicosFixosGR(Lote, Seq_Gr);


                    _pagamentoPixDAO.atualizaPreCalculoGR(Lote);

                    #region notaIndividual 
                    

                    if (Seq_Gr == 0)
                    {
                        return new Response
                        {
                            Sucesso = false,
                            Mensagem = "Erro na geração da GR "
                        };
                    }

                    IntegracaoBaixa.notaAgrupada = false;

                    var parceiro = _pagamentoPixDAO.GetDadosFaturaGr(seq_gr);
                    int parceiroID = parceiro.PARCEIRO;
                    string loteID = parceiro.LOTE;

                    int empresa = _pagamentoPixDAO.getEmpresa(Lote);

                    string serie = _pagamentoPixDAO.Busca_Serie("NFE", empresa, "GR");
                    

                    if (serie == "")
                    {
                        return new Response
                        {
                            Sucesso = false,
                            Mensagem = "Erro na  SERIE"
                        };
                    }


                    string meioPagamento = "Pix";
                    titSapiens = _pagamentoPixDAO.obtemTituloSapiens(empresa);

                    var nota = _pagamentoPixDAO.GetDadosFaturaGr(seq_gr);

                    if (nota != null)
                    {
                        int qtdBLs = 1;
                        int qtdBlsSub = 0;

                        _pagamentoPixDAO.carrega(seq_gr);

                        if (_pagamentoPixDAO.consistenciaGR(seq_gr, parceiroID) != "")
                        {
                            return new Response
                            {
                                Sucesso = false,
                                Mensagem = _pagamentoPixDAO.consistenciaGR(seq_gr, parceiroID),
                            };
                        }

                        int fpParc = nota.FPPARC;
                        int fpGrp = nota.FPGRP;
                        int fpIpa = nota.FPIPA;
                        int fpgr = nota.fpgr;
                        string ndoc = nota.NUM_DOC;
                        string tipoDoc = nota.TIPODOC_DESCRICAO;
                        int patioGR = nota.PATIO;


                        int flagHupport = nota.flag_hubport;
                        bool primeiraHub = _pagamentoPixDAO.primeiraHub(Seq_Gr);

                        int cliente = 0;
                        string razao = "";
                        string cgc = "";
                        string cidade = "";
                        int cli_autonum = 0;

                        if (flagHupport == 0 || primeiraHub)
                        {
                            cliente = nota.CODCLI;
                            razao = nota.Razao;
                            cgc = nota.CGC;
                            cidade = nota.Cidade_Cli;
                            cli_autonum = nota.autonum_cli;
                        }
                        else
                        {
                            cliente = nota.ind_codcli;
                            razao = nota.Ind_Razao;
                            cgc = nota.Ind_CGC;
                            cidade = nota.Ind_CGC;
                            cli_autonum = nota.Ind_autonum;
                        }

                        if (cli_autonum == 0)
                        {
                            return new Response
                            {
                                Sucesso = false,
                                Mensagem = "Cadastro do cliente não encontrado !",
                            };
                        }

                        var sap_cli = _pagamentoPixDAO.Preenche_Cliente(cliente);

                        if (sap_cli == null)
                        {
                            return new Response
                            {
                                Sucesso = false,
                                Mensagem = "Cadastro de cliente não encontrado !",
                            };
                        }

                        if (cliente == 0)
                        {
                            _pagamentoPixDAO.atualizaCODCLI(cliente, cli_autonum);
                        }
                        
                        string nfe = "";
                       string dtEmissao = DateTime.Now.ToString("dd/MM/yyyy");
                        string dtVenc = DateTime.Now.ToString("dd/MM/yyyy");

                        //_pagamentoPixDAO.updateNotaById(id_nota, Usuario);

                        SapCliente sapcliente = new SapCliente();

                        sapcliente.USU_TIPLOGR = sap_cli.USU_TIPLOGR;
                        sapcliente.ENDCLI = sap_cli.ENDCLI;
                        sapcliente.NENCLI = sap_cli.NENCLI;
                        sapcliente.CPLEND = sap_cli.CPLEND;
                        sapcliente.CIDCLI = sap_cli.CIDCLI;
                        sapcliente.BAICLI = sap_cli.BAICLI;
                        sapcliente.SIGUFS = sap_cli.SIGUFS;
                        sapcliente.CEPCLI = sap_cli.CEPCLI;
                        sapcliente.ENDCOB = sap_cli.ENDCOB;
                        sapcliente.NENCOB = sap_cli.NENCOB;
                        sapcliente.CPLCOB = sap_cli.CPLCOB;
                        sapcliente.CIDCOB = sap_cli.CIDCOB;
                        sapcliente.BAICOB = sap_cli.BAICOB;
                        sapcliente.CEPCOB = sap_cli.CEPCOB;
                        sapcliente.CODCLI = sap_cli.CODCLI;
                        sapcliente.NOMCLI = sap_cli.NOMCLI;
                        sapcliente.CGCCPF = sap_cli.CGCCPF;
                        sapcliente.INSEST = sap_cli.INSEST;
                        sapcliente.IBGE = sap_cli.IBGE;
                        sapcliente.TIPCLI = sap_cli.TIPCLI;



                        //Insere Fatura Nota 

                        string NatOp = "PRESTAÇÃO DE SERVIÇOS";
                        string CodNat = "20.01";
                        int viagem = 0; //campos não usados para o tipo GR mas estão aqui como parametros para o metodo insert 
                        string Dolar = ""; //campos não usados para o tipo GR mas estão aqui como parametros para o metodo insert  
                        int autonumMin = 0;
                        int numero = 0;
                        
                        int notaAc = 0; //campos não usados para o tipo GR mas estão aqui como parametros para o metodo insert  
                        string nfeSubst = meioPagamento;
                        string clienteSAPEntrega = "";
                        int condManual = 0;
                        int fpRedex = 0;
                        int fpLTL = 0;
                        int fpOp = 0;
                        int idNotaSub = 0;
                        string conta = "";
                        string condicao = "";
                        string monta_SID_Fecha_Nota = "";
                        string monta_SID_Itens = "";
                        string comp_nota = "GR Número : " + seq_gr;
                        int nfeSubstituida = 0;
                        string Embarque = _pagamentoPixDAO.GetEmbarque(seq_gr);
                        string corpoNota = "";



                        _pagamentoPixDAO.Monta_Insert_Faturanota(
                            "GR", seq_gr, Embarque, dtEmissao, dtVenc, "", "",
                            NatOp, CodNat, ndoc, tipoDoc, viagem, Dolar, patioGR,
                            autonumMin, sapcliente, notaAc, cli_autonum,
                            Lote, parceiroID, nfeSubst, clienteSAPEntrega, fpOp,
                            fpIpa, fpGrp, fpParc, fpgr, condManual, fpRedex, fpLTL,
                            cliente, valor.ToString(), numero, cod_empresa, Usuario, serie, servico,
                            idNotaSub);

                        int id_nota = _pagamentoPixDAO.Obtem_Id_Nota(Seq_Gr, nfe);

                        if (id_nota > 0)    
                        {
                            if (_pagamentoPixDAO.geraIntegracao(servico, nfeSubstituida, dtEmissao, patioGR, "Pix", false, titSapiens, dtEmissao, conta, valor.ToString(), "Pix", sapcliente, id_nota, cod_empresa, serie, corpoNota, nfeSubstituida, seq_gr) != "")
                            {

                                var statusNota = _pagamentoPixDAO.ObtemStatusNota(id_nota);

                                if (statusNota.STATUSNFE != 0 && statusNota.STATUSNFE != 5)
                                {
                                    if (statusNota.NFE > 0 && statusNota.RPSNUM > 0)
                                    {
                                        comp_nota += comp_nota + "  ,Nota Fiscal Número : " + statusNota.NFE + " ," + " RPS Número: " + statusNota.RPSNUM;

                                        _pagamentoPixDAO.Atualiza_Doc("GR", dtEmissao, Seq_Gr, false);
                                    }
                                }

                                if (_pagamentoPixDAO.Obtem_RPSNUM(id_nota) != 0)
                                {
                                    _pagamentoPixDAO.Atualiza_Doc("GR", dtEmissao, Seq_Gr, false);
                                }

                                return new Response
                                {
                                    Sucesso = false,
                                    Mensagem = comp_nota,
                                };
                            }
                            else
                            {
                                if (_pagamentoPixDAO.Obtem_RPSNUM(id_nota) != 0)
                                {
                                    _pagamentoPixDAO.Atualiza_Doc("GR", dtEmissao, Seq_Gr, false);
                                }

                            }
                        }
                        else
                        {
                            return new Response
                            {
                                Sucesso = false,
                                Mensagem = "Erro na geração da Nota fiscal!",
                            };
                        }
                        //FIM                       




                    }
                    else 
                    {
                        return new Response
                        {
                            Sucesso = false,
                            Mensagem = "GR não localizada",
                        };
                    }                     
                    
                        



            //inicio                   

            //        rsGR = DAO.Consultar("SELECT * FROM FATURA.FATURA_GR WHERE SEQ_GR = " & seq_gr
            //If rsGR.Rows.Count > 0 Then
            //    nota = New FaturaNota
            //    nota.documento = rsGR.Rows(0)("SEQ_GR").ToString
            //    nota.parceiro = rsGR.Rows(0)("PARCEIRO").ToString
            //    nota.lote = Long.Parse(NNull(rsGR.Rows(0)("LOTE").ToString, 0))
            //    nota.substituicao = False
            //   qtdBLs = 1
            //   qtdBlsSub = 0

                
                //If Not nota.consistenciaGR() Then
                //    If nota.retornoConsistencia <> "" Then
                //     return new Response
                //     {
                //         Sucesso = false,
                //         Mensagem = nota.retornoConsistencia
                //     };
                //    END~IF
                //End If

                //If NNull(rsGR.Rows(0)("flag_hubport").ToString, 0) = 0 Or Not primeirahub(rsGR.Rows(0)("SEQ_GR").ToString) Then
                //            Cli_Codcli = NNull(rsGR.Rows(0)("Codcli").ToString, 0)
                //            Cli_Razao = NNull(rsGR.Rows(0)("Razao").ToString, 1)
                //            Cli_CGC = NNull(rsGR.Rows(0)("CGC").ToString, 0)
                //            Cli_Cidade = NNull(rsGR.Rows(0)("Cidade_cli").ToString, 0)
                //            Cli_Autonum = NNull(rsGR.Rows(0)("autonum_cli").ToString, 0)
                //        Else
                //            Cli_Codcli = NNull(rsGR.Rows(0)("Ind_Codcli").ToString, 0)
                //            Cli_Razao = NNull(rsGR.Rows(0)("Ind_Razao").ToString, 1)
                //            Cli_CGC = NNull(rsGR.Rows(0)("Ind_CGC").ToString, 0)
                //            Cli_Cidade = NNull(rsGR.Rows(0)("Ind_Cidade_cli").ToString, 0)
                //            Cli_Autonum = NNull(rsGR.Rows(0)("Ind_autonum").ToString, 0)
                //        End If


                //        If  NNull(Cli_Codcli, 0) = 0 Then
                //                 return new Response
                //                 {
                //                     Sucesso = false,
                //                     Mensagem = "Cadatro do cliente não encontrado!"
                //                 };
                //         End If
                //    sap_cli = Preenche_Cliente("GR")
                //    If sap_cli.Rows.Count = 0 Then
                //                          return new Response
                //                          {
                //                              Sucesso = false,
                //                              Mensagem = "Cadatro do cliente SAP não encontrado!"
                //                          };
                //    ENDIF
                //    If NNull(Cli_Codcli, 0) = 0 Then
                //        notaGR.atualizaCODCLI(sap_cli.Rows(0)("codcli").ToString, Cli_Autonum)
                //    End If

                        //nota.substituicao = False
                        //nota.insereFaturanota(Cli_Codcli, Cli_Razao, Cli_CGC, Cli_Cidade, Cli_Autonum, Format(CDate(Now), "dd/MM/yyyy"), Format(CDate(mskDeposito.Text), "dd/MM/yyyy"), nota.listaFatura(x).idsServFaturados, "", "", "PRESTAÇÃO DE SERVIÇOS", "20.01", rsGR.Rows(0)("NUM_DOCUMENTO").ToString, rsGR.Rows(0)("TIPODOC_DESCRICAO").ToString, rsGR.Rows(0)("PATIO").ToString, 0)
                        //If nota.idNota > 0 Then
                        //     If nota.geraIntegracao(nota.listaFatura(x).idsServFaturados, Cli_CGC, "", Format(CDate(Now), "dd/MM/yyyy"), rsGR.Rows(0)("PATIO").ToString, 
                                 
                        //         cbMeio.SelectedValue,  False, Titulo_Sapiens, mskDeposito.Text, cbDeposito.SelectedValue, txtVlL.Text, cbMeio.SelectedValue)=false Then


                        //         return new Response
                        //         {
                        //             Sucesso = false,
                        //             Mensagem = comp_nota
                        //         }; 
                        //    End If
                        //End If
               //else
               //  begin
               //            return new Response
               //             {
               //                 Sucesso = false,
               //                 Mensagem ="GR NÃO LOCALIZADA"
               //             };
               //     end
               //         ' fim '


                        
                    

                    
                    
                    #endregion

                }

                //Método para envio de email, ainda falta implemntar alguns metodos por isso as linhas abaixo estão comentadas.
                #region envio email 
                //var dados_IMG_PAG_GR = _pagamentoPixDAO.GetDadosUsuarioImagem(Lote);
                //int idUs = 0;
                //int idImagem = 0;
                //string emailUS = "";
                //int patioID = 0;
                //string descrEmpresa = "";

                //if (dados_IMG_PAG_GR != null)
                //{
                //    idUs = dados_IMG_PAG_GR.id_user;
                //    idImagem = dados_IMG_PAG_GR.AUTONUM;

                //    if (idUs < 0)
                //    {
                //        return new Response
                //        {
                //            Sucesso = false,
                //            Mensagem = "Email não encontrado",
                //        };
                //    }

                //}
                //_pagamentoPixDAO.GetUpdateTbImagem(Seq_Gr, idImagem);

                //var emailIntUser = _pagamentoPixDAO.GetDadosUserInternet(idUs);



                //if (emailIntUser != null)
                //{
                //    emailUS = emailIntUser.iusemail;
                //}

                //var Email_Recusa_Imagem_Patio = _pagamentoPixDAO.GetEmailRecusa(Lote);

                //if (Email_Recusa_Imagem_Patio != null)
                //{
                //    patioID = Email_Recusa_Imagem_Patio.PATIO;
                //}

                //var rsDirEmail = _pagamentoPixDAO.obtemDirEmail();

                //if (rsDirEmail == null)
                //{
                //    return new Response
                //    {
                //        Sucesso = false,
                //        Mensagem = "Diretório para inclusão dos anexos de E-mail não definida ",
                //    };
                //}


                //string CorpoEmail = "Prezados,<br />";

                //CorpoEmail = CorpoEmail + "Processo liberado, recibo em anexo. <br />";
                //CorpoEmail = CorpoEmail + "(Não sendo necessário a retirada) <br  /><br  />";
                //CorpoEmail = CorpoEmail + "Os pagamentos, TED, DOC ou depósito, serão considerados efetivamente pagos na data da compensação bancária da conta corrente Do terminal.<br /><br />";
                //CorpoEmail = CorpoEmail + "Att, <br />";

                //if (patioID < 5)
                //{
                //    CorpoEmail = CorpoEmail + "atendimento.op@ecoportosantos.com.br";
                //}
                //else
                //{
                //    CorpoEmail = CorpoEmail + "atendimento.ra@ecoportosantos.com.br";
                //}

                //var getTipoDoc = _pagamentoPixDAO.GetTipoDocumento(Lote);

                //if (getTipoDoc != null)
                //{
                //    descrEmpresa = "Processo Liberado - " + getTipoDoc.descr + ":" + getTipoDoc.num_documento + "- Lote: " + Lote + " - BL:  " + BL;
                //}
                //else
                //{
                //    // ver de onde virão esses dados descr and num_documento
                //}

                //var queryRPS = _pagamentoPixDAO.GetDadosFAT_GR(Seq_Gr).Count();
                //string nomeRPSPDF = "";
                //string nomeRPSPDF2 = "";
                //string nomeRPSPDF3 = "";
                //string emailTecondiNFE = "nfe@tecondi.com.br";

                //foreach (var item1 in nomeRPSPDF)
                //{
                //    for (int i = 0; i > queryRPS; i++)
                //    {
                //        if (i == 0)
                //        {
                //            nomeRPSPDF = "RPS_" + item.IDNum;
                //            // colocar o metodo para emitir a nota "emiteNotaGRRPS"
                //            nomeRPSPDF = "RPS_" + item.IDNum + " .pdf";
                //        }
                //        if (i == 1)
                //        {
                //            nomeRPSPDF2 = "RPS_" + item.IDNum;
                //            // colocar o metodo para emitir a nota "emiteNotaGRRPS"
                //            nomeRPSPDF2 = "RPS_" + item.IDNum + " .pdf";
                //        }

                //        if (i == 2) 
                //        {
                //            nomeRPSPDF3 = "RPS_" + item.IDNum;
                //            // colocar o metodo para emitir a nota "emiteNotaGRRPS"
                //            nomeRPSPDF3 = "RPS_" + item.IDNum + " .pdf";
                //        }
                //    }
                //}




                #endregion

                //Colocar este método por ultimo que o mesmo tem nas duas acoes 

                _pagamentoPixDAO.atualizaBLREGGR(Lote);
          

                return new Response
                {
                    Sucesso = true,
                    Mensagem = $"Baixa do Titulo realizada com sucesso!"
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Sucesso = false,
                    Mensagem = $"Falha durante a operação de Baixa"
                };
            }
        }
    }
}
