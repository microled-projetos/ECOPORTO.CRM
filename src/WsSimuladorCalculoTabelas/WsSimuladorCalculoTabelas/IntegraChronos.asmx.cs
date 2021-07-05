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
        int Usuario = 0;
        int contar = 0;
        bool blreefer = false;
        string SisFin = "SAP";
        int diasAdicionais = 0;
        int seq_gr = 0;
        int cod_empresa =0;

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
                    if (contar == 02)
                    {
                        blreefer = false;
                    }
                    else
                    {
                        blreefer = true;
                    }

                    contar = _pagamentoPixDAO.VerificaReefer(Lote);
                    if (contar == 02)
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

                    if (contar != 0)
                    {
                        return new Response
                        {
                            Sucesso = false,
                            Mensagem = "Lote com saída , favor verificar som setor reposnsável"
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

                if (_pagamentoPixDAO.UsuarioEmProcessoDeCalculo(Lote) != null)
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

                               
                _pagamentoPixDAO.atualizaGREmServico(Lote, Seq_Gr, 90);
                _pagamentoPixDAO.atualizaGREmDescricao(Lote, Seq_Gr);
                _pagamentoPixDAO.atualizaGREmCNTR(Lote, Seq_Gr);
                _pagamentoPixDAO.atualizaGREmCS(Lote, Seq_Gr);

                    var insereGr = _pagamentoPixDAO.inseregr_bl(Seq_Gr, Lote, wInicio, wFinal);

                  //  var dadosAtualizaDadosVencimento = _pagamentoPixDAO.GetDadosAtualizaVencimento(Lote);

                 

               var QdeLavagemCtnr = _pagamentoPixDAO.obtemQtdLavagemCNTR(Lote, Seq_Gr);

                if (QdeLavagemCtnr != 0)
                {
                    _pagamentoPixDAO.atualizaAMRNFCNTRLavagem(Lote, Seq_Gr);
                }

                _pagamentoPixDAO.atualizaServicosFixosGR(Lote, Seq_Gr);


                _pagamentoPixDAO.atualizaPreCalculoGR(Lote);


                    //Fazer o metodo de notaIndividual

                    #region notaIndividual 

                    if (Seq_Gr == 0)
                    {
                        return new Response
                        {
                            Sucesso = false,
                            Mensagem = "Erro na geração da GR "
                        };
                    }

                    //Verificar consistencias GR 

                    int grRPS = _pagamentoPixDAO.countGRRPS(Seq_Gr);

                    if (grRPS > 0)
                    {
                        return new Response
                        {
                            Sucesso = false,
                            Mensagem = "GR já possui RPS/Faturamento gerado",
                        };
                    }


                    int grDoc = _pagamentoPixDAO.countGRDOC(Seq_Gr);

                    if (grDoc > 0)
                    {
                        return new Response
                        {
                            Sucesso = false,
                            Mensagem = "O Documento da GR [" + Seq_Gr + "] está em branco",
                        };
                    }

                    int grZerada = _pagamentoPixDAO.countGRValorZerado(Seq_Gr);

                    if (grZerada > 0)
                    {
                        return new Response
                        {
                            Sucesso = false,
                            Mensagem = "Uma ou mais GRs com valor zerado!"
                        };
                    }

                    var faturaGR = _pagamentoPixDAO.GetDadosFaturaGr(Seq_Gr);

                    /*      If rsGR.Rows.Count > 0 Then
                     *      
            Zera_Nota_Emi()
            Zera_Dados_Cli_Nota()
                        If NNull(seq_gr, 0) = 0 Then
            MsgBox("Erro na  geração da GR ", vbCritical)
                    saida
        End If

        Serie_NF = Busca_Serie("NFE", NNull(Cod_Empresa, 0), "GR")
        If NNull(Serie_NF, 1) = "" Then
             MsgBox("Erro na  SERIE ", vbCritical)
                    saida
        End If
        If Nota_Pendente(Serie_NF, NNull(Cod_Empresa, 0), "GR") = True Then
             MsgBox("Erro na  SERIE ", vbCritical)
                    saida
        End If

        cbMeioPAGAMENTO='PIX';
  
        Titulo_Sapiens = cx.obtemTituloSapiens().Rows(0)(0).ToString

            
                              nota = New FaturaNota
                              nota.documento = rsGR.Rows(0)("SEQ_GR").ToString
                              nota.parceiro = rsGR.Rows(0)("PARCEIRO").ToString
                              nota.lote = Long.Parse(NNull(rsGR.Rows(0)("LOTE").ToString, 0))
                              nota.substituicao = False

                          sSql = "SELECT DISTINCT SERVICO FROM " & DAO.BancoSgipa & "TB_SERVICOS_FATURADOS WHERE SEQ_GR IN(" & seq_gr  & ") "
                          sSql = sSql & " AND (NVL(VALOR,0) + NVL(ADICIONAL,0) + NVL(DESCONTO,0) ) > 0 "
                          rsServicos = DAO.Consultar(sSql)
                          servicos = ""
                          contaItens = 0
                          contaFatura = 0
                          For idS = 0 To rsServicos.Rows.Count - 1
                              contaItens = contaItens + 1
                              If tamanhoDescrItens("GR", seq_gr, servicos & IIf(servicos <> "", ",", "") & rsServicos.Rows(idS)(0).ToString) > 2000 Then
                                  wIdServicos = ""
                                  sSql = "SELECT AUTONUM FROM " & DAO.BancoSgipa & "TB_SERVICOS_FATURADOS WHERE SEQ_GR IN(" & seq_gr & " ) "
                                  sSql = sSql & " And SERVICO IN(" & servicos & ") "
                                  rsIdServicos = DAO.Consultar(sSql)
                                  For ln = 0 To rsIdServicos.Rows.Count - 1
                                      If wIdServicos <> "" Then wIdServicos = wIdServicos & ","
                                      wIdServicos = wIdServicos & rsIdServicos.Rows(ln)(0).ToString
                                  Next

                                  notaAtu = New notasGerar
                                  notaAtu.idsServFaturados = wIdServicos
                                  notaAtu.idsServicos = servicos
                                  listaFatura.Add(notaAtu)

                                  wIdServicos = ""
                                  contaItens = 0
                                  servicos = ""

                                  servicos = rsServicos.Rows(idS)(0).ToString
                              Else
                                  If servicos <> "" Then servicos = servicos & ","
                                  servicos = servicos & rsServicos.Rows(idS)(0).ToString
                              End If
                          Next

                          If servicos <> "" Then
                              wIdServicos = ""
                              sSql = "SELECT AUTONUM FROM " & DAO.BancoSgipa & "TB_SERVICOS_FATURADOS WHERE SEQ_GR IN(" & seq_gr & " ) "
                              sSql = sSql & " And SERVICO IN(" & servicos & ") "
                              rsIdServicos = DAO.Consultar(sSql)
                              For ln = 0 To rsIdServicos.Rows.Count - 1
                                  If wIdServicos <> "" Then wIdServicos = wIdServicos & ","
                                  wIdServicos = wIdServicos & rsIdServicos.Rows(ln)(0).ToString
                              Next

                              notaAtu = New notasGerar
                              notaAtu.idsServFaturados = wIdServicos
                              notaAtu.idsServicos = servicos
                              listaFatura.Add(notaAtu)

                              wIdServicos = ""
                              contaItens = 0
                              servicos = ""
                          End If
                              If Not nota.consistenciaGR() Then
                                  If nota.retornoConsistencia <> "" Then
                                      MsgBox(nota.retornoConsistencia, vbInformation, Me.Text)
                                  End If
                                  GoTo saida
                              End If

                              For x = 0 To nota.listaFatura.Count - 1
                                 
                                      If NNull(rsGR.Rows(0)("flag_hubport").ToString, 0) = 0 Or Not primeirahub(rsGR.Rows(0)("SEQ_GR").ToString) Then
                                          Cli_Codcli = NNull(rsGR.Rows(0)("Codcli").ToString, 0)
                                          Cli_Razao = NNull(rsGR.Rows(0)("Razao").ToString, 1)
                                          Cli_CGC = NNull(rsGR.Rows(0)("CGC").ToString, 0)
                                          Cli_Cidade = NNull(rsGR.Rows(0)("Cidade_cli").ToString, 0)
                                          Cli_Autonum = NNull(rsGR.Rows(0)("autonum_cli").ToString, 0)
                                      Else
                                          Cli_Codcli = NNull(rsGR.Rows(0)("Ind_Codcli").ToString, 0)
                                          Cli_Razao = NNull(rsGR.Rows(0)("Ind_Razao").ToString, 1)
                                          Cli_CGC = NNull(rsGR.Rows(0)("Ind_CGC").ToString, 0)
                                          Cli_Cidade = NNull(rsGR.Rows(0)("Ind_Cidade_cli").ToString, 0)
                                          Cli_Autonum = NNull(rsGR.Rows(0)("Ind_autonum").ToString, 0)
                                      End If
                             
                                  If   NNull(Cli_Codcli, 0) = 0 Then
                                      MsgBox("Cadatro do cliente não encontrado!", vbInformation, Me.Text)
                                      GoTo saida
                                  End If
                                  sap_cli = Preenche_Cliente("GR")
                                  If sap_cli.Rows.Count = 0 Then
                                      Erro_Cad = True
                                  ElseIf NNull(Cli_Codcli, 0) = 0 Then
                                      notaGR.atualizaCODCLI(sap_cli.Rows(0)("codcli").ToString, Cli_Autonum)
                                  End If

                                 
                                      nota.substituicao = False
                                      nota.insereFaturanota(Cli_Codcli, Cli_Razao, Cli_CGC, Cli_Cidade, Cli_Autonum, Format(CDate(Now), "dd/MM/yyyy"), Format(CDate(mskDeposito.Text), "dd/MM/yyyy"), nota.listaFatura(x).idsServFaturados, "", "", "PRESTAÇÃO DE SERVIÇOS", "20.01", rsGR.Rows(0)("NUM_DOCUMENTO").ToString, rsGR.Rows(0)("TIPODOC_DESCRICAO").ToString, rsGR.Rows(0)("PATIO").ToString, 0)
                                      If nota.idNota > 0 Then
                                          CMD_XML(2) = Monta_Sid_FechaNota(Serie_NF, IIf(Tipo_Nota.ToUpper.Trim = "NFE", IIf(Check1.Checked = False, True, False), False), Titulo_Sapiens, mskDeposito.Text, cbDeposito.SelectedValue, txtVlL.Text, Cod_Pag)

                                          If nota.geraIntegracao(nota.listaFatura(x).idsServFaturados, Cli_CGC, "", Format(CDate(Now), "dd/MM/yyyy"), rsGR.Rows(0)("PATIO").ToString, cbMeio.SelectedValue,  , IIf(Check1.Checked = False, True, False), Titulo_Sapiens, mskDeposito.Text, cbDeposito.SelectedValue, txtVlL.Text, cbMeio.SelectedValue) = false Then
                                             MsgBox("erro na integração!", vbInformation, Me.Text)
                                      GoTo saida
                                          End If
                                    
                    FIM DA ROTINA 
                                  */


                    int fpParc = faturaGR.FPPARC;
                    int fpGrp = faturaGR.FPGRP;
                    int fpIpa = faturaGR.FPIPA;
                    int fpgr = faturaGR.fpgr;
                    string ndoc = faturaGR.NUM_DOC;
                    string tipoDoc = faturaGR.TIPODOC_DESCRICAO;
                    int patioGR = faturaGR.PATIO;


                    int flagHupport = faturaGR.flag_hubport;
                    bool primeiraHub = _pagamentoPixDAO.primeiraHub(Seq_Gr);

                    int cliente = 0;
                    string razao = "";
                    string cgc = "";
                    string cidade = "";
                    int cli_autonum = 0;


                    if (fpParc == 0 && fpGrp == 0 && fpIpa == 0 && fpgr == 0)
                    {
                        if (flagHupport == 1 || primeiraHub)
                        {
                            cliente = faturaGR.Codcli;
                            razao = faturaGR.Razao;
                            cgc = faturaGR.CGC;
                            cidade = faturaGR.Cidade_Cli;
                            cli_autonum = faturaGR.autonum_cli;
                        }
                        else
                        {
                            cliente = faturaGR.ind_codcli;
                            razao = faturaGR.Ind_Razao;
                            cgc = faturaGR.Ind_CGC;
                            cidade = faturaGR.Ind_CGC;
                            cli_autonum = faturaGR.Ind_autonum;
                        }
                    }

                    if (SisFin == "SAP" && cli_autonum == 0)
                    {
                        return new Response
                        {
                            Sucesso = false,
                            Mensagem = "Cadastro do cliente não encontrado !",
                        };
                    }

                    int empresa = _pagamentoPixDAO.getEmpresa(Lote);
                    string serie = _pagamentoPixDAO.Busca_Serie("NFE", empresa, "GR");

                    if (serie == "")
                    {
                        return new Response
                        {
                            Sucesso = false,
                            Mensagem = "Número de série não encontrado"
                        };
                    }

                    var sap_cli = "";

                    if (sap_cli == null)
                    {
                        return new Response
                        {
                            Sucesso = false,
                            Mensagem = "Cadastro de cliente não encontrado !",
                        };
                    }
                    else if (cliente == 0)
                    {
                        _pagamentoPixDAO.atualizaCODCLI(cliente, cli_autonum);
                    }
                    string nfe = "";
                    int id_nota = _pagamentoPixDAO.Obtem_Id_Nota(Seq_Gr, nfe);
                    string dtEmissao = DateTime.Now.ToString("dd/MM/YYYY");

                    _pagamentoPixDAO.updateNotaById(id_nota, Usuario);

                    var cliDados = _pagamentoPixDAO.Preenche_Cliente(cliente);

                    string USU_TIPLOGR = cliDados.USU_TIPLOGR;
                    string ENDCLI = cliDados.ENDCLI;
                    string NENCLI = cliDados.NENCLI;
                    string CPLEND = cliDados.CPLEND;
                    string CIDCLI = cliDados.CIDCLI;
                    string BAICLI = cliDados.BAICLI;
                    string SIGUFS = cliDados.SIGUFS;
                    string CEPCLI = cliDados.CEPCLI;
                    string ENDCOB = cliDados.ENDCOB;
                    string NENCOB = cliDados.NENCOB;
                    string CPLCOB = cliDados.CPLCOB;
                    string CIDCOB = cliDados.CIDCOB;
                    string BAICOB = cliDados.BAICOB;
                    string CEPCOB = cliDados.CEPCOB;
                    int CODCLI = cliDados.CODCLI;
                    string NOMCLI = cliDados.NOMCLI;
                    int CGCCPF = cliDados.CGCCPF;
                    string INSEST = cliDados.INSEST;
                    string IBGE = cliDados.IBGE;


                    //Insere Fatura Nota 

                    //_pagamentoPixDAO.Monta_Insert_Faturanota()



                    if (_pagamentoPixDAO.Obtem_RPSNUM(id_nota) != 0)
                    {
                        _pagamentoPixDAO.Atualiza_Doc("GR", dtEmissao, Seq_Gr, false);
                    }

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
