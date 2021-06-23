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

        public IntegraChronos()
        {
            _tabelasDAO = new TabelasDAO(false);
            _oportunidadeDAO = new OportunidadeDAO();
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
    }
}
