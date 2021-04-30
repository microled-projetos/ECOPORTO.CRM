using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml.Serialization;
using WsConsultaSPC.WsSPC;

namespace WsConsultaSPC
{
    /// <summary>
    /// Summary description for WsSPC
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class Ws : WebService
    {
        [WebMethod(Description = "Consulta o Serviço do SPC com os dados informados")]
        public ConsultaSpcResponse ConsultarSPC(
            string tipoPessoaDocumento, 
            string documento, 
            string usuarioLogado,
            string hash)
        {
            if (tipoPessoaDocumento != "1" && tipoPessoaDocumento != "2" && tipoPessoaDocumento != "F" && tipoPessoaDocumento != "J")
            {
                return RetornarErro("Tipo de Pessoa informado incorretamente. (1 - PJ | 2 - PF)");
            }

            if (string.IsNullOrWhiteSpace(documento))
            {
                return RetornarErro("Número do Documento (CNPJ/CPF) não informado");
            }

            if (string.IsNullOrWhiteSpace(usuarioLogado))
            {
                return RetornarErro("ID do usuário CRM não informado");
            }

            if (hash != Config.Hash)
            {
                return RetornarErro("Não autorizado (hash inválido)");
            }

            documento = documento.RemoverCaracteresEspeciaisDocumento();

            TipoPessoaResponse tipoPessoa = TipoPessoaResponse.PessoaFisica;

            if (tipoPessoaDocumento == "2" || tipoPessoaDocumento == "J")
            {
                tipoPessoa = TipoPessoaResponse.PessoaJuridica;
            }

            SPCService service = new SPCService(Parametros.WsSPCUrl, Parametros.WsSPCUsuario, Parametros.WsSPCSenha);

            ConsultaSpcResponse dadosConsulta = new ConsultaSpcResponse();
            ContaRepositorio contaRepositorio = new ContaRepositorio();
            AnaliseCreditoRepositorio analiseCreditoRepositorio = new AnaliseCreditoRepositorio();
            ParametrosRepositorio parametrosRepositorio = new ParametrosRepositorio();

            var parametros = parametrosRepositorio.ObterParametros();

            var contaBusca = contaRepositorio.ObterContaPorDocumento(documento);

            if (contaBusca == null)
                return RetornarErro($"Conta com documento {documento} é inexistente");

            var resultadoSpc = service.Consultar(tipoPessoa, documento.RemoverCaracteresEspeciaisDocumento(), Parametros.WsSPCProduto);

            if (resultadoSpc == null)
                return RetornarErro($"Sem resposta da Consulta S");

            dadosConsulta.ContaId = contaBusca.Id;
            dadosConsulta.DataConsulta = DateTime.Now;
            dadosConsulta.Protocolo = $"{resultadoSpc.protocolo.numero}-{resultadoSpc.protocolo.digito}";
            dadosConsulta.Restricao = resultadoSpc.restricao;

            if (resultadoSpc.consumidor?.consumidorpessoajuridica != null)
            {
                dadosConsulta.TipoPessoa = TipoPessoaResponse.PessoaJuridica;

                var consumidor = resultadoSpc.consumidor?.consumidorpessoajuridica;

                dadosConsulta.RazaoSocial = consumidor.razaosocial;
                dadosConsulta.Fundacao = consumidor.datafundacao;
                dadosConsulta.CNPJ = Convert.ToUInt64(consumidor.cnpj.numero).ToString(@"00\.000\.000\/0000\-00"); 
                dadosConsulta.Atividade = consumidor.atividadeeconomicaprincipal?.descricao ?? string.Empty;
                dadosConsulta.Situacao = consumidor.situacaocnpj?.descricaosituacao ?? string.Empty;

                var endereco = consumidor?.endereco;

                if (endereco != null)
                {
                    dadosConsulta.Logradouro = endereco.logradouro;
                    dadosConsulta.Bairro = endereco.bairro;
                    dadosConsulta.Cidade = endereco.cidade.nome;
                    dadosConsulta.Estado = endereco.cidade.estado.siglauf;
                    dadosConsulta.CEP = endereco.cep;
                }
            }

            if (resultadoSpc.consumidor?.consumidorpessoafisica != null)
            {
                dadosConsulta.TipoPessoa = TipoPessoaResponse.PessoaFisica;

                var consumidor = resultadoSpc.consumidor?.consumidorpessoafisica;
               
             
        
                 
            
            /// <summary>
            dadosConsulta.Nome = consumidor.nome;
                dadosConsulta.DataNascimento = consumidor.datanascimento;
                dadosConsulta.CPF = Convert.ToUInt64(consumidor.cpf.numero).ToString(@"000\.000\.000\-00");
                dadosConsulta.Situacao = consumidor.situacaocpf?.descricaosituacao ?? string.Empty;
                dadosConsulta.Nacionalidade = consumidor.nacionalidade?.nome ?? string.Empty;

                var endereco = consumidor?.endereco;

                if (endereco != null)
                {
                    dadosConsulta.Logradouro = endereco.logradouro;
                    dadosConsulta.Bairro = endereco.bairro;
                    dadosConsulta.Cidade = endereco.cidade.nome;
                    dadosConsulta.Estado = endereco.cidade.estado.siglauf;
                    dadosConsulta.CEP = endereco.cep;
                }
            }

            if (resultadoSpc.protesto?.resumo != null)
            {
                var protesto = resultadoSpc.protesto?.resumo;

                dadosConsulta.ProtestoQuantidade = protesto.quantidadetotal;
                dadosConsulta.ProtestoData = protesto.dataultimaocorrencia;
                dadosConsulta.ProtestoValorTotal = protesto.valortotal;

                dadosConsulta.Quantidade += protesto.quantidadetotal;
            }

            if (resultadoSpc.acao?.resumo != null)
            {
                var acao = resultadoSpc.acao?.resumo;

                dadosConsulta.AcaoQuantidade = acao.quantidadetotal;
                dadosConsulta.AcaoData = acao.dataultimaocorrencia;
                dadosConsulta.AcaoValorTotal = acao.valortotal;

                dadosConsulta.Quantidade += acao.quantidadetotal;
            }

            if (resultadoSpc.pendenciafinanceira?.resumo != null)
            {
                var pendencia = resultadoSpc.pendenciafinanceira?.resumo;

                dadosConsulta.PendenciaFinancQuantidade = pendencia.quantidadetotal;
                dadosConsulta.PendenciaFinancData = pendencia.dataultimaocorrencia;
                dadosConsulta.PendenciaFinancValorTotal = pendencia.valortotal;

                dadosConsulta.Quantidade += pendencia.quantidadetotal;

                for (int i = 0; i < resultadoSpc.pendenciafinanceira?.Items?.Length; i++)
                {
                    var item = (WsSPC.InsumoPendenciaFinanceira)resultadoSpc.pendenciafinanceira.Items[i];

                    var detalhe = new DetalhesPendenciaFinanceiraDTO();

                    detalhe.Titulo = item.tituloocorrencia;
                    detalhe.Avalista = item.avalista;
                    detalhe.Contrato = item.contrato;
                    detalhe.Ocorrencia = item.dataocorrencia;
                    detalhe.Filial = item.filial;
                    detalhe.Origem = item.origem;
                    detalhe.Moeda = item.moeda?.simbolo;
                    detalhe.Natureza = item.naturezaanotacao?.descricaonaturezaanotacao;
                    detalhe.Cidade = item.cidade?.nome;
                    detalhe.UF = item.cidade?.estado?.siglauf;
                    detalhe.Valor = item.valorpendencia;

                    dadosConsulta.DetalhesPendenciasFinanceiras.Add(detalhe);
                }
            }

            if (resultadoSpc.participacaofalencia?.resumo != null)
            {
                var participacaofalencia = resultadoSpc.participacaofalencia?.resumo;

                dadosConsulta.ParticipFalenciaQuantidade = participacaofalencia.quantidadetotal;
                dadosConsulta.ParticipFalenciaData = participacaofalencia.dataultimaocorrencia;
                dadosConsulta.ParticipFalenciaValorTotal = participacaofalencia.valortotal;

                dadosConsulta.Quantidade += participacaofalencia.quantidadetotal;
            }

            if (resultadoSpc.spc?.resumo != null)
            {
                var spc = resultadoSpc.spc?.resumo;

                dadosConsulta.SpcQuantidade = spc.quantidadetotal;
                dadosConsulta.SpcData = spc.dataultimaocorrencia;
                dadosConsulta.SpcValorTotal = spc.valortotal;

                dadosConsulta.Quantidade += spc.quantidadetotal;

                for (int i = 0; i < resultadoSpc.spc?.Items?.Length; i++)
                {
                    var item = (WsSPC.InsumoSPC)resultadoSpc.spc.Items[i];

                    var detalhe = new DetalhesSpcDTO();

                    detalhe.Associado = item.nomeassociado;
                    detalhe.Contrato = item.contrato;
                    detalhe.Entidade = item.nomeentidade;
                    detalhe.Inclusao = item.datainclusao;
                    detalhe.Vencimento = item.datavencimento;
                    detalhe.Valor = item.valor;

                    dadosConsulta.DetalhesSpc.Add(detalhe);
                }
            }

            if (resultadoSpc.chequesemfundovarejo?.resumo != null)
            {
                var chequeSemFundoVarejo = resultadoSpc.chequesemfundovarejo?.resumo;

                dadosConsulta.ChequeSFQuantidade = chequeSemFundoVarejo.quantidadetotal;
                dadosConsulta.ChequeSFData = chequeSemFundoVarejo.dataultimaocorrencia;
                dadosConsulta.ChequeSFValorTotal = chequeSemFundoVarejo.valortotal;

                dadosConsulta.Quantidade += chequeSemFundoVarejo.quantidadetotal;
            }

            if (resultadoSpc.ccf?.resumo != null)
            {
                var ChequeSemFundoCCF = resultadoSpc.ccf?.resumo;

                dadosConsulta.ChequeSFCCFQuantidade = ChequeSemFundoCCF.quantidadetotal;
                dadosConsulta.ChequeSFCCFData = ChequeSemFundoCCF.dataultimaocorrencia;
                dadosConsulta.ChequeSFCCFValorTotal = ChequeSemFundoCCF.valortotal;

                dadosConsulta.Quantidade += ChequeSemFundoCCF.quantidadetotal;

                for (int i = 0; i < resultadoSpc.ccf?.Items?.Length; i++)
                {
                    var item = (WsSPC.InsumoCCF)resultadoSpc.ccf.Items[i];

                    var detalhe = new CCFDetalhesDTO();

                    detalhe.Origem = item.origem;
                    detalhe.DataUltimoCheque = item.dataultimocheque;
                    detalhe.Quantidade = item.quantidade;
                    detalhe.Motivo = item.motivo?.descricao;

                    dadosConsulta.DetalhesCCF.Add(detalhe);
                }
            }

            if (resultadoSpc.chequelojista?.resumo != null)
            {
                var chequelojista = resultadoSpc.chequelojista?.resumo;

                dadosConsulta.ChequeLojistaQuantidade = chequelojista.quantidadetotal;
                dadosConsulta.ChequeLojistaData = chequelojista.dataultimaocorrencia;
                dadosConsulta.ChequeLojistaValorTotal = chequelojista.valortotal;

                dadosConsulta.Quantidade += chequelojista.quantidadetotal;

                for (int i = 0; i < resultadoSpc.chequelojista?.Items?.Length; i++)
                {
                    var item = (WsSPC.InsumoChequeLojista)resultadoSpc.chequelojista.Items[i];

                    var detalhe = new DetalhesChequeLojistaDTO();

                    detalhe.Associado = item.nomeassociado;
                    detalhe.Entidade = item.nomeentidade;
                    detalhe.Inclusao = item.datainclusao;
                    detalhe.Descricao = item.alinea?.descricao;
                    detalhe.ChequeEmissao = item.chequeinicial?.dataemissao;
                    detalhe.ChequeValor = item.chequeinicial?.valor;
                    detalhe.CidadeAssociado = item.cidadeassociado?.nome;

                    dadosConsulta.DetalhesChequesLojistas.Add(detalhe);
                }
            }

            if (resultadoSpc.chequeoutrasocorrenciassrs?.resumo != null)
            {
                var chequeOutrasOcorrencias = resultadoSpc.chequeoutrasocorrenciassrs?.resumo;

                dadosConsulta.ChequeCOOutrasQuantidade = chequeOutrasOcorrencias.quantidadetotal;
                dadosConsulta.ChequeCOOutrasData = chequeOutrasOcorrencias.dataultimaocorrencia;
                dadosConsulta.ChequeCOOutrasValorTotal = chequeOutrasOcorrencias.valortotal;

                dadosConsulta.Quantidade += chequeOutrasOcorrencias.quantidadetotal;
            }

            if (resultadoSpc.consultarealizada?.resumo != null)
            {
                var consultaRealizada = resultadoSpc.consultarealizada?.resumo;

                dadosConsulta.ConsultaRealizadaQuantidade = consultaRealizada.quantidadetotal;
                dadosConsulta.ConsultaRealizadaData = consultaRealizada.dataultimaocorrencia;
                dadosConsulta.ConsultaRealizadaValorTotal = consultaRealizada.valortotal;

                dadosConsulta.Quantidade += consultaRealizada.quantidadetotal;

                for (int i = 0; i < resultadoSpc.consultarealizada?.Items?.Length; i++)
                {
                    var item = (WsSPC.InsumoConsultaRealizada)resultadoSpc.consultarealizada.Items[i];

                    var detalhe = new ConsultaRealizadaDTO();

                    detalhe.Associado = item.nomeassociado;
                    detalhe.DataConsulta = item.dataconsulta;
                    detalhe.Entidade = item.nomeentidadeorigem;
                    detalhe.Cidade = item.origemassociado?.nome;
                    detalhe.Estado = item.origemassociado?.estado?.siglauf;

                    dadosConsulta.DetalhesConsultasRealizadas.Add(detalhe);
                }
            }

            if (resultadoSpc.alertadocumento?.resumo != null)
            {
                var alertaDocumento = resultadoSpc.alertadocumento?.resumo;

                dadosConsulta.AlertaDocQuantidade = alertaDocumento.quantidadetotal;
                dadosConsulta.AlertaDocData = alertaDocumento.dataultimaocorrencia;
                dadosConsulta.AlertaDocValorTotal = alertaDocumento.valortotal;

                dadosConsulta.Quantidade += alertaDocumento.quantidadetotal;

                for (int i = 0; i < resultadoSpc.alertadocumento?.Items?.Length; i++)
                {
                    var item = (WsSPC.InsumoAlertaDocumento)resultadoSpc.alertadocumento.Items[i];

                    var detalhe = new AlertaDocumentosDTO();

                    detalhe.Inclusao = item.datainclusao;
                    detalhe.Ocorrencia = item.dataocorrencia;
                    detalhe.Entidade = item.entidadeorigem;
                    detalhe.Motivo = item.motivo;

                    if (item.tipodocumentoalerta?.Length > 0)
                    {
                        detalhe.Tipos = string.Join(",", item.tipodocumentoalerta.Select(c => c.nome));
                    }

                    dadosConsulta.DetalhesAlertasDocumentos.Add(detalhe);
                }
            }

            if (resultadoSpc.creditoconcedido?.resumo != null)
            {
                var creditoConcedido = resultadoSpc.creditoconcedido?.resumo;

                dadosConsulta.CreditoConcQuantidade = creditoConcedido.quantidadetotal;
                dadosConsulta.CreditoConcData = creditoConcedido.dataultimaocorrencia;
                dadosConsulta.CreditoConcValorTotal = creditoConcedido.valortotal;

                dadosConsulta.Quantidade += creditoConcedido.quantidadetotal;
            }

            if (resultadoSpc.contraordem?.resumo != null)
            {
                var contraordem = resultadoSpc.contraordem?.resumo;

                dadosConsulta.ContraOrdemQuantidade = contraordem.quantidadetotal;
                dadosConsulta.ContraOrdemData = contraordem.dataultimaocorrencia;
                dadosConsulta.ContraOrdemValorTotal = contraordem.valortotal;

                dadosConsulta.Quantidade += contraordem.quantidadetotal;
            }

            if (resultadoSpc.contraordemdocumentodiferente?.resumo != null)
            {
                var contraordemDoc = resultadoSpc.contraordemdocumentodiferente?.resumo;

                dadosConsulta.ContraOrdemDFQuantidade = contraordemDoc.quantidadetotal;
                dadosConsulta.ContraOrdemDFData = contraordemDoc.dataultimaocorrencia;
                dadosConsulta.ContraOrdemDFValorTotal = contraordemDoc.valortotal;

                dadosConsulta.Quantidade += contraordemDoc.quantidadetotal;

                for (int i = 0; i < resultadoSpc.contraordemdocumentodiferente?.Items?.Length; i++)
                {
                    var item = (WsSPC.InsumoContraOrdemDocumentoDiferente)resultadoSpc.contraordemdocumentodiferente.Items[i];

                    var detalhe = new ContraOrdemDocumentoDiferenteDTO();

                    detalhe.Documento = item.documento;
                    detalhe.Inclusao = item.datainclusao;
                    detalhe.Ocorrencia = item.dataocorrencia;
                    detalhe.Origem = item.origem;
                    detalhe.Informante = item.informante;
                    detalhe.Descricao = item.motivo?.descricao;

                    dadosConsulta.DetalhesContraOrdemDocumentoDiferente.Add(detalhe);
                }
            }

            dadosConsulta.UsuarioId = Convert.ToInt32(usuarioLogado);

            bool fluxoAprovacao = true;

            var pendenciasFinanceiras = analiseCreditoRepositorio
                .ObterPendenciasFinanceiras(contaBusca.Documento.RemoverCaracteresEspeciaisDocumento());

            dadosConsulta.TotalDividaEcoporto = pendenciasFinanceiras.Sum(c => c.Valor);
            dadosConsulta.InadimplenteEcoporto = dadosConsulta.TotalDividaEcoporto > 0;

            dadosConsulta.TotalDividaSpc = ObterTotalDividaSpc(resultadoSpc);
            dadosConsulta.InadimplenteSpc = dadosConsulta.TotalDividaSpc > parametros.DividaSpc;

            if (dadosConsulta.TotalDividaSpc < parametros.DividaSpc)
            {
                dadosConsulta.Validade = DateTime.Now.AddYears(1);
            }

            if (dadosConsulta.TotalDividaSpc > parametros.DividaSpc && fluxoAprovacao == false)
            {
                dadosConsulta.Validade = DateTime.Now.AddMonths(3);
            }

            if (dadosConsulta.TotalDividaSpc > parametros.DividaSpc && fluxoAprovacao == true)
            {
                dadosConsulta.Validade = DateTime.Now.AddYears(1);
            }

            var contasRaizCnpj = contaRepositorio
                .ObterContasPorRaizDocumento(contaBusca.Documento.RemoverCaracteresEspeciaisDocumento());

           dadosConsulta.StatusAnaliseDeCredito = StatusAnaliseDeCreditoResponse.PENDENTE;
            if ((dadosConsulta.TotalDividaSpc <= parametros.DividaSpc) && (dadosConsulta.TotalDividaEcoporto==0))
            {
                dadosConsulta.StatusAnaliseDeCredito = StatusAnaliseDeCreditoResponse.APROVADO;
            }

            analiseCreditoRepositorio.GravarConsultaSpc(dadosConsulta, contasRaizCnpj);

            return dadosConsulta;
        }       

        [WebMethod(Description = "Consulta uma consulta já feita anteriormente")]
        public ConsultaSpcResponse ConsultarBancoDeDados(string documento, string hash)
        {
            ConsultaSpcResponse dadosConsulta = new ConsultaSpcResponse();
            AnaliseCreditoRepositorio analiseCreditoRepositorio = new AnaliseCreditoRepositorio();
            ContaRepositorio contaRepositorio = new ContaRepositorio();
            ParametrosRepositorio parametrosRepositorio = new ParametrosRepositorio();

            var parametros = parametrosRepositorio.ObterParametros();

            if (string.IsNullOrWhiteSpace(documento))
            {
                return RetornarErro("Número do Documento (CNPJ/CPF) não informado");
            }

            if (hash != Config.Hash)
            {
                return RetornarErro("Não autorizado (hash inválido)");
            }

            documento = documento.RemoverCaracteresEspeciaisDocumento();

            var contaBusca = contaRepositorio.ObterContaPorId(documento.ToInt() );

            if (contaBusca == null)
            {
                return RetornarErro($"Conta com documento {documento} não encontrada");
            }
           
            var analiseCreditoBusca = analiseCreditoRepositorio.ObterConsultaSpc(contaBusca.Id);

            if (analiseCreditoBusca != null)
            {
                dadosConsulta.ContaId = contaBusca.Id;
                dadosConsulta.Id = analiseCreditoBusca.Id;
                dadosConsulta.DataConsulta = analiseCreditoBusca.DataConsulta;
                dadosConsulta.Protocolo = analiseCreditoBusca.Protocolo;
                dadosConsulta.Restricao = analiseCreditoBusca.Restricao;
                dadosConsulta.TipoPessoa = analiseCreditoBusca.TipoPessoa;
                dadosConsulta.Validade = analiseCreditoBusca.Validade;
                dadosConsulta.RazaoSocial = analiseCreditoBusca.RazaoSocial;
                dadosConsulta.Fundacao = analiseCreditoBusca.Fundacao;
                dadosConsulta.CNPJ = analiseCreditoBusca.CNPJ;
                dadosConsulta.Nome = analiseCreditoBusca.Nome;
                dadosConsulta.DataNascimento = analiseCreditoBusca.DataNascimento;
                dadosConsulta.CPF = analiseCreditoBusca.CPF;
                dadosConsulta.Nacionalidade = analiseCreditoBusca.Nacionalidade;
                dadosConsulta.Atividade = analiseCreditoBusca.Atividade;
                dadosConsulta.Situacao = analiseCreditoBusca.Situacao;
                dadosConsulta.Logradouro = analiseCreditoBusca.Logradouro;
                dadosConsulta.Bairro = analiseCreditoBusca.Bairro;
                dadosConsulta.Cidade = analiseCreditoBusca.Cidade;
                dadosConsulta.Estado = analiseCreditoBusca.Estado;
                dadosConsulta.CEP = analiseCreditoBusca.CEP;

                dadosConsulta.ProtestoQuantidade = analiseCreditoBusca.ProtestoQuantidade;
                dadosConsulta.ProtestoData = analiseCreditoBusca.ProtestoData;
                dadosConsulta.ProtestoValorTotal = analiseCreditoBusca.ProtestoValorTotal;

                dadosConsulta.AcaoQuantidade = analiseCreditoBusca.AcaoQuantidade;
                dadosConsulta.AcaoData = analiseCreditoBusca.AcaoData;
                dadosConsulta.AcaoValorTotal = analiseCreditoBusca.AcaoValorTotal;
                dadosConsulta.StatusAnaliseDeCredito = analiseCreditoBusca.StatusAnaliseDeCredito;

                dadosConsulta.PendenciaFinancQuantidade = analiseCreditoBusca.PendenciaFinancQuantidade;
                dadosConsulta.PendenciaFinancData = analiseCreditoBusca.PendenciaFinancData;
                dadosConsulta.PendenciaFinancValorTotal = analiseCreditoBusca.PendenciaFinancValorTotal;

                dadosConsulta.ParticipFalenciaQuantidade = analiseCreditoBusca.ParticipFalenciaQuantidade;
                dadosConsulta.ParticipFalenciaData = analiseCreditoBusca.ParticipFalenciaData;
                dadosConsulta.ParticipFalenciaValorTotal = analiseCreditoBusca.ParticipFalenciaValorTotal;

                dadosConsulta.SpcQuantidade = analiseCreditoBusca.SpcQuantidade;
                dadosConsulta.SpcData = analiseCreditoBusca.SpcData;
                dadosConsulta.SpcValorTotal = analiseCreditoBusca.SpcValorTotal;

                dadosConsulta.ChequeSFQuantidade = analiseCreditoBusca.ChequeSFQuantidade;
                dadosConsulta.ChequeSFData = analiseCreditoBusca.ChequeSFData;
                dadosConsulta.ChequeSFValorTotal = analiseCreditoBusca.ChequeSFValorTotal;

                dadosConsulta.ChequeSFCCFQuantidade = analiseCreditoBusca.ChequeSFCCFQuantidade;
                dadosConsulta.ChequeSFCCFData = analiseCreditoBusca.ChequeSFCCFData;
                dadosConsulta.ChequeSFCCFValorTotal = analiseCreditoBusca.ChequeSFCCFValorTotal;

                dadosConsulta.ChequeLojistaQuantidade = analiseCreditoBusca.ChequeLojistaQuantidade;
                dadosConsulta.ChequeLojistaData = analiseCreditoBusca.ChequeLojistaData;
                dadosConsulta.ChequeLojistaValorTotal = analiseCreditoBusca.ChequeLojistaValorTotal;

                dadosConsulta.ChequeCOOutrasQuantidade = analiseCreditoBusca.ChequeCOOutrasQuantidade;
                dadosConsulta.ChequeCOOutrasData = analiseCreditoBusca.ChequeCOOutrasData;
                dadosConsulta.ChequeCOOutrasValorTotal = analiseCreditoBusca.ChequeCOOutrasValorTotal;

                dadosConsulta.ConsultaRealizadaQuantidade = analiseCreditoBusca.ConsultaRealizadaQuantidade;
                dadosConsulta.ConsultaRealizadaData = analiseCreditoBusca.ConsultaRealizadaData;
                dadosConsulta.ConsultaRealizadaValorTotal = analiseCreditoBusca.ConsultaRealizadaValorTotal;

                dadosConsulta.AlertaDocQuantidade = analiseCreditoBusca.AlertaDocQuantidade;
                dadosConsulta.AlertaDocData = analiseCreditoBusca.AlertaDocData;
                dadosConsulta.AlertaDocValorTotal = analiseCreditoBusca.AlertaDocValorTotal;

                dadosConsulta.CreditoConcQuantidade = analiseCreditoBusca.CreditoConcQuantidade;
                dadosConsulta.CreditoConcData = analiseCreditoBusca.CreditoConcData;
                dadosConsulta.CreditoConcValorTotal = analiseCreditoBusca.CreditoConcValorTotal;

                dadosConsulta.ContraOrdemQuantidade = analiseCreditoBusca.ContraOrdemQuantidade;
                dadosConsulta.ContraOrdemData = analiseCreditoBusca.ContraOrdemData;
                dadosConsulta.ContraOrdemValorTotal = analiseCreditoBusca.ContraOrdemValorTotal;

                dadosConsulta.ContraOrdemDFQuantidade = analiseCreditoBusca.ContraOrdemDFQuantidade;
                dadosConsulta.ContraOrdemDFData = analiseCreditoBusca.ContraOrdemDFData;
                dadosConsulta.ContraOrdemDFValorTotal = analiseCreditoBusca.ContraOrdemDFValorTotal;

                dadosConsulta.DetalhesSpc = analiseCreditoRepositorio
                    .ObterDetalhesSpc(analiseCreditoBusca.Id).ToList();

                dadosConsulta.DetalhesPendenciasFinanceiras = analiseCreditoRepositorio
                    .ObterDetalhesPendenciasFinanceiras(analiseCreditoBusca.Id).ToList();

                dadosConsulta.DetalhesChequesLojistas = analiseCreditoRepositorio
                    .ObterDetalhesChequesLojistas(analiseCreditoBusca.Id).ToList();

                dadosConsulta.DetalhesContraOrdemDocumentoDiferente = analiseCreditoRepositorio
                    .ObterDetalhesContraOrdemDocumentoDiferente(analiseCreditoBusca.Id).ToList();

                dadosConsulta.DetalhesConsultasRealizadas = analiseCreditoRepositorio
                    .ObterDetalhesHistoricoConsultas(analiseCreditoBusca.Id).ToList();

                dadosConsulta.DetalhesAlertasDocumentos = analiseCreditoRepositorio
                    .ObterDetalhesAlertasDocumentos(analiseCreditoBusca.Id).ToList();

                dadosConsulta.DetalhesCCF = analiseCreditoRepositorio
                  .ObterDetalhesCCF(analiseCreditoBusca.Id).ToList();

                var pendenciasFinanceiras = analiseCreditoRepositorio
                    .ObterPendenciasFinanceiras(contaBusca.Documento.RemoverCaracteresEspeciaisDocumento());
                if (pendenciasFinanceiras != null)
                {
                    dadosConsulta.TotalDividaEcoporto = pendenciasFinanceiras.Sum(c => c.Valor);
                }
                dadosConsulta.StatusAnaliseDeCredito = analiseCreditoBusca.StatusAnaliseDeCredito;

                dadosConsulta.TotalDividaSpc = analiseCreditoBusca.TotalDividaSpc;
                dadosConsulta.InadimplenteSpc = dadosConsulta.TotalDividaSpc > parametros.DividaSpc;
                if ((dadosConsulta.TotalDividaSpc <= parametros.DividaSpc) && (dadosConsulta.TotalDividaEcoporto == 0))

                {
                    dadosConsulta.StatusAnaliseDeCredito = StatusAnaliseDeCreditoResponse.APROVADO; 
                    }
                dadosConsulta.InadimplenteEcoporto = dadosConsulta.TotalDividaEcoporto > 0;
            }

            return dadosConsulta;
        }

        public decimal ObterTotalDividaSpc(ResultadoConsulta dadosConsulta)
        {
            var spc = dadosConsulta.spc?.resumo?.valortotal ?? 0;
            var chequelojista = dadosConsulta.chequelojista?.resumo?.valortotal ?? 0;
            var ccf = dadosConsulta.ccf?.resumo?.valortotal ?? 0;
            var protesto = dadosConsulta.protesto?.resumo?.valortotal ?? 0;
            var acao = dadosConsulta.acao?.resumo?.valortotal ?? 0;
            var pendenciafinanceira = dadosConsulta.pendenciafinanceira?.resumo?.valortotal ?? 0;
            var participacaofalencia = dadosConsulta.participacaofalencia?.resumo?.valortotal ?? 0;
            var alertadocumento = dadosConsulta.alertadocumento?.resumo?.valortotal ?? 0;
            var contraordemdocumentodiferente = dadosConsulta.contraordemdocumentodiferente?.resumo?.valortotal ?? 0;
            var contraordem = dadosConsulta.contraordem?.resumo?.valortotal ?? 0;
            var consultarealizada = dadosConsulta.consultarealizada?.resumo?.valortotal ?? 0;
            var chequesemfundovarejo = dadosConsulta.chequesemfundovarejo?.resumo?.valortotal ?? 0;
            var chequeoutrasocorrenciassrs = dadosConsulta.chequeoutrasocorrenciassrs?.resumo?.valortotal ?? 0;
            var creditoconcedido = dadosConsulta.creditoconcedido?.resumo?.valortotal ?? 0;

            return
                spc +
                chequelojista +
                ccf +
                protesto +
                acao +
                pendenciafinanceira +
                participacaofalencia +
                alertadocumento +
                contraordemdocumentodiferente +
                contraordem +
                consultarealizada +
                chequesemfundovarejo +
                chequeoutrasocorrenciassrs +
                creditoconcedido;
        }

        private static ConsultaSpcResponse RetornarErro(string mensagem)
        {
            var obj = new ResponseError
            {
                Status = "Erro",
                Mensagem = mensagem
            };

            XmlSerializer xml = new XmlSerializer(typeof(ResponseError));
            StringWriter retorno = new StringWriter();
            xml.Serialize(retorno, obj);

            HttpContext.Current.Response.Write(retorno.ToString());
            HttpContext.Current.Response.End();

            return null;
        }
    }
}
