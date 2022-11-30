$(document).ready(function () {

    habilitaFramesTela();

    $('#DescontoServicoId').prop('disabled', true);
    $('#DescontoValorDescontoNoServico').prop('readonly', true);

    $('#DescontoVencimento').prop('readonly', true);
    $('#DescontoFreeTime').prop('readonly', true);

    var statusSolicitacao = parseInt($('#StatusSolicitacao').val());

    if (statusSolicitacao !== 1 && statusSolicitacao !== 4) {
        //$("form :input:not(:file):not('#btnPesquisaSite')").attr("disabled", "disabled");
        $('#AnexoSolicitacaoId, #btnDescontoAnexar, #btnAnexosAnterior, #RecallSolicitacaoId, #MotivoRecall, #btnFecharRecall, #btnAnexar').removeAttr('disabled');
    }

    if (statusSolicitacao === 2) {
        $('#btnRecallSolicitacao').removeAttr('disabled');
    }

    var unidade = parseInt($('#UnidadeSolicitacaoId').val());
    var status = parseInt($('#StatusSolicitacao').val());

    if ((unidade === 3 || unidade === 4) && status !== 2) {
        $('#btnCadastrarDesconto').prop('disabled', false);
    }

    if ($('#chkHabilitaCampoValorDevido').is(':checked')) {
        $('#ValorDevido').prop('readonly', false);
    }
});

function habilitaFramesTela() {

    var solicitacaoId = $('#Id').val();

    if (parseInt(solicitacaoId) > 0) {

        var tipoSolicitacao = parseInt($('#TipoSolicitacao').val());
        var ocorrencia = parseInt($('#OcorrenciaId').val());

        $('#pnlCancelamentoNF').addClass('invisivel');
        $('#pnlDesconto').addClass('invisivel');
        $('#pnlProrrogacao').addClass('invisivel');
        $('#pnlRestituicao').addClass('invisivel');
        $('#pnlAlteracaoFormaPgto').addClass('invisivel');

        if (tipoSolicitacao === 1) {
            $('#pnlCancelamentoNF').removeClass('invisivel');
            obterHistoricoWorkflow(solicitacaoId, 5);
            obterCancelamentoDefault();
        }

        if (tipoSolicitacao === 2) {
            $('#pnlDesconto').removeClass('invisivel');
            obterHistoricoWorkflow(solicitacaoId, 6);
            obterDescontoDefault();
        }

        if (tipoSolicitacao === 3) {
            $('#pnlProrrogacao').removeClass('invisivel');
            obterHistoricoWorkflow(solicitacaoId, 8);
            obterProrrogacaoDefault();
        }

        if (tipoSolicitacao === 4) {
            $('#pnlRestituicao').removeClass('invisivel');
            obterHistoricoWorkflow(solicitacaoId, 7);
            obterRestituicaoDefault();
        }

        if (tipoSolicitacao === 5) {
            $('#pnlAlteracaoFormaPgto').removeClass('invisivel');
            obterHistoricoWorkflow(solicitacaoId, 10);
            obterSolicitacaoFormaPgtoDefault();
        }

        // Revisar. Id da ocorrência está fixo.
        if (tipoSolicitacao === 2 && ocorrencia === 4) {
            $('#pnlDesconto').addClass('invisivel');
            $('#pnlCancelamentoNF').removeClass('invisivel');
            $('#CancelamentoNFDataProrrogacao').prop('readonly', false);
            $('#CancelamentoNFDesconto').prop('readonly', false);

            obterCancelamentoDefault();
        }
    }
}

$('#TipoSolicitacao').change(function () {

    var solicitacaoId = $('#Id').val();

    if (parseInt(solicitacaoId) > 0) {
        habilitaFramesTela();
    }

    $.get(urlBase + 'Solicitacoes/PopularMotivosPorTipoSolicitacaoJson?tipoSolicitacao=' + $(this).val(), function (resultado) {

        var select = $('#MotivoId');

        select.empty();

        select.append('<option value=""></option>');

        $.each(resultado, function (key, value) {
            select.append('<option value=' + value.Id + '>' + value.Descricao + '</option>');
        });
    });

    $.get(urlBase + 'Solicitacoes/PopularOcorrenciasPorTipoSolicitacaoJson?tipoSolicitacao=' + $(this).val(), function (resultado) {

        var select = $('#OcorrenciaId');

        select.empty();

        select.append('<option value=""></option>');

        $.each(resultado, function (key, value) {
            select.append('<option value=' + value.Id + '>' + value.Descricao + '</option>');
        });
    });

    var tipoSolicitacao = parseInt($(this).val());

    if (tipoSolicitacao === 3 || tipoSolicitacao === 4) {
        $('#AreaOcorrenciaSolicitacao').val('0').attr('disabled', 'disabled');
    } else {
        $('#AreaOcorrenciaSolicitacao').removeAttr('disabled');
    }
});

$('#CancelamentoNFTipoPesquisaNumero').blur(function () {

    var tipoSolicitacao = $('#TipoSolicitacao').val();
    var tipoPesquisa = $('#CancelamentoNFTipoPesquisa').val();
    var termoPesquisa = $('#CancelamentoNFTipoPesquisaNumero').val();
    var tipoOperacao = parseInt($('#TipoOperacaoId').val());

    if (isNumero(tipoSolicitacao) && isNumero(tipoPesquisa)) {

        $.get(urlBase + 'Solicitacoes/ObterNotasFiscaisPorTipoSolicitacao?tipoPesquisa=' + tipoPesquisa + '&tipoOperacao=' + tipoOperacao + '&termoPesquisa=' + termoPesquisa, function (resultado) {

            var select = $('#CancelamentoNFNotaFiscalId');

            select.empty();

            if (resultado.length > 1)
                select.append('<option value=""></option>');

            $.each(resultado, function (key, value) {
                select.append('<option value=' + value.Id + '>' + value.NFE + ' - ' + value.DataEmissao + '</option>');
            });

            if (resultado.length === 1)
                obterDetalhesNotaFiscalCancelamento($('#CancelamentoNFNotaFiscalId').val());
        }).fail(function (data) {
            toastr.error(data.statusText, 'CRM');
        });
    }
});

$("#btnPesquisarContaCancelamentoNF").click(function () {

    event.preventDefault();

    $('#pesquisa-modal-contas')
        .data('toggle', 'CancelamentoNFContaId')
        .modal('show');
});

$("#btnPesquisarContaRestituicao").click(function () {

    event.preventDefault();

    $('#pesquisa-modal-contas')
        .data('toggle', 'RestituicaoFavorecidoId')
        .modal('show');
});

$("#btnPesquisarContaProrrogacao").click(function () {

    event.preventDefault();

    $('#pesquisa-modal-contas')
        .data('toggle', 'ProrrogacaoContaId')
        .modal('show');
});

$("#btnPesquisarClienteDesconto").click(function () {

    event.preventDefault();

    $('#pesquisa-modal-contas')
        .data('toggle', 'DescontoClienteId')
        .modal('show');
});

$("#btnPesquisarIndicadorDesconto").click(function () {

    event.preventDefault();

    $('#pesquisa-modal-contas')
        .data('toggle', 'DescontoIndicadorId')
        .modal('show');
});

function selecionarConta(id, descricao) {
    var toggle = $('#pesquisa-modal-contas').data('toggle');

    $('#pesquisa-modal-contas').modal('hide');

    var $controle = $("#" + toggle);

    if ($controle.is('select')) {
        $controle
            .empty()
            .append($('<option>', {
                value: id,
                text: descricao
            })).focus();
    } else {
        $controle
            .empty()
            .val(descricao);
    }

    $("#ListaContas").empty();
}

$('#CancelamentoNFNotaFiscalId').change(function () {
    obterDetalhesNotaFiscalCancelamento($(this).val());
});

function obterDetalhesNotaFiscalCancelamento(id) {

    var unidade = parseInt($('#UnidadeSolicitacaoId').val());
    var tipoOperacao = parseInt($('#TipoOperacaoId').val());

    if (isNumero(id)) {
        if (unidade !== 3 && unidade !== 4) {
            $.get(urlBase + 'Solicitacoes/ObterDetalhesNotaFiscal?nfeId=' + id + '&tipoOperacao=' + tipoOperacao, function (resultado) {

                if (resultado) {

                    $('#CancelamentoNFValorNF').empty().val(resultado.Valor);
                    $('#CancelamentoNFRazaoSocial').empty().val(resultado.Cliente);
                    $('#CancelamentoNFFormaPagamento').val('0').val(resultado.FormaPagamento);
                    $('#DescricaoCancelamentoNFFormaPagamento').empty().val(resultado.DescricaoFormaPagamento);
                    $('#CancelamentoNFDataEmissao').empty().val(resultado.DataEmissao);
                    $('#CancelamentoNFLote').empty().val(resultado.Lote);
                    $('#CancelamentoNFReserva').empty().val(resultado.Reserva);

                    calcularValorAVistaCancelamento(resultado.Lote, $('#CancelamentoNFNotaFiscalId').val(), 0);
                }
            });
        }
    }
}

$('#btnLimparCancelamentoNF').click(function () {
    limparDadosCancelamentoNF();
});

function limparDadosCancelamentoNF() {
    $("#frmSolicitacoesCancelamentoNF")[0].reset();
    $('#CancelamentoNFId').val('0');
    $('#CancelamentoNFDataEmissao').val('');
    $('#CancelamentoNFDataProrrogacao').val('');
    $('#CancelamentoNFNotaFiscalId').empty();
    $('#CancelamentoNFContaId').empty();
}

function atualizarCancelamentoNF(id) {

    event.preventDefault();

    $.get(urlBase + 'Solicitacoes/ObterDetalhesCancelamentoNF?id=' + id, function (resultado) {

        carregarCamposCancelamento(resultado);
    });
}

function obterCancelamentoDefault() {

    var solicitacaoId = $('#Id').val();

    if (isNumero(solicitacaoId)) {
        $.get(urlBase + 'Solicitacoes/ObterCancelamentoDefault?solicitacaoId=' + solicitacaoId, function (resultado) {
            carregarCamposCancelamento(resultado);
        });
    }
}

function carregarCamposCancelamento(resultado) {

    if (resultado !== null) {
        $('#CancelamentoNFId').val(resultado.Id);
        $('#CancelamentoNFTipoPesquisa').val(resultado.TipoPesquisa);
        $('#CancelamentoNFTipoPesquisaNumero').val(resultado.TipoPesquisaNumero);
        $('#CancelamentoNFNotaFiscal').val(resultado.NFE);
        $('#CancelamentoNFLote').val(resultado.Lote);
        $('#CancelamentoNFValorNF').val(resultado.ValorNF);
        $('#CancelamentoNFFormaPagamento').val(resultado.FormaPagamento);
        $('#DescricaoCancelamentoNFFormaPagamento').val(resultado.DescricaoFormaPagamento);
        $('#CancelamentoNFDataEmissao').val(resultado.DataEmissao);
        $('#CancelamentoNFRazaoSocial').val(resultado.RazaoSocial);
        $('#CancelamentoNFDesconto').val(resultado.Desconto);
        $('#CancelamentoNFValorNovaNF').val(resultado.ValorNovaNF);
        $('#CancelamentoValorAPagar').val(resultado.ValorAPagar);
        $('#CancelamentoNFDataProrrogacao').val(resultado.DataProrrogacao);

        var unidadeSolicitacao = parseInt($('#UnidadeSolicitacaoId').val());

        if (unidadeSolicitacao === 3 || unidadeSolicitacao === 4) {

            $("#CancelamentoNFContaId")
                .empty()
                .append(
                    $('<option>', {
                        value: resultado.ContaId,
                        text: resultado.ContaDescricao
                    }));
        } else {
            $('#CancelamentoNFRazaoSocial').empty().val(resultado.RazaoSocial);
        }

        $("#CancelamentoNFNotaFiscalId")
            .empty()
            .append(
                $('<option>', {
                    value: resultado.NotaFiscalId,
                    text: resultado.NFE + ' - ' + resultado.DataEmissao
                }));

        $('#CancelamentoNFNotaFiscalId').val(resultado.NotaFiscalId);
    }
}

$('#CancelamentoNFDesconto').blur(function () {

    var valorNf = parseFloat(formataMoedaCalculo($('#CancelamentoNFValorNF').val()));
    var valorDesconto = parseFloat(formataMoedaCalculo($('#CancelamentoNFDesconto').val()));

    var valorNovaNf = valorNf - valorDesconto;

    $('#CancelamentoNFValorNovaNF').val(formataMoedaPtBr(valorNovaNf));

    calcularValorAVistaCancelamento($('#CancelamentoNFLote').val(), $('#CancelamentoNFNotaFiscalId').val(), $('#CancelamentoNFDesconto').val());
});

function calcularValorAVistaCancelamento(lote, notaFiscal, desconto) {

    var tipoOperacao = parseInt($('#TipoOperacaoId').val());

    if (tipoOperacao !== 3) {

        $.get(urlBase + 'Solicitacoes/CalcularValorAVista?lote=' + lote + '&notaFiscal=' + notaFiscal + '&desconto=' + desconto, function (resultado) {

            if (resultado !== null) {
                $('#CancelamentoValorAPagar').val(formataMoedaPtBr(resultado.ValorAPagar));
                $('#CancelamentoNovoValorAPagar').val(formataMoedaPtBr(resultado.NovoValorAPagar));
            }
        });
    } else {

        var reserva = $('#CancelamentoNFReserva').val();

        $.get(urlBase + 'Solicitacoes/CalcularValorAVistaRedex?reserva=' + reserva + '&notaFiscal=' + notaFiscal + '&desconto=' + desconto, function (resultado) {

            if (resultado !== null) {
                $('#CancelamentoValorAPagar').val(formataMoedaPtBr(resultado.ValorAPagar));
                $('#CancelamentoNovoValorAPagar').val(formataMoedaPtBr(resultado.NovoValorAPagar));
            }
        });
    }
}

function calcularValorAVistaRestituicao(lote, notaFiscal, desconto) {

    var tipoOperacao = parseInt($('#TipoOperacaoId').val());

    if (tipoOperacao !== 3) {

        $.get(urlBase + 'Solicitacoes/CalcularValorAVista?lote=' + lote + '&notaFiscal=' + notaFiscal + '&desconto=' + desconto, function (resultado) {

            if (resultado !== null) {
                $('#RestituicaoValorAPagar').val(formataMoedaPtBr(resultado.ValorAPagar));
            }
        }).fail(function (data) {
            toastr.error(data.statusText, 'CRM');
        });
    } else {

        var reserva = $('#RestituicaoReserva').val();

        $.get(urlBase + 'Solicitacoes/CalcularValorAVistaRedex?reserva=' + reserva + '&notaFiscal=' + notaFiscal + '&desconto=' + desconto, function (resultado) {

            if (resultado !== null) {
                $('#RestituicaoValorAPagar').val(formataMoedaPtBr(resultado.ValorAPagar));
            }
        }).fail(function () {
            toastr.error(data.statusText, 'CRM');
        });
    }
}

function excluirCancelamentoNF(id) {

    $('#modal-exclusao-cancelamento')
        .data('id', id)
        .modal('show');
}

function confirmarExclusaoCancelamentoNF() {

    var id = $('#modal-exclusao-cancelamento').data('id');

    if (isNumero(id)) {

        $.post(urlBase + 'Solicitacoes/ExcluirCancelamentoNF/', { id: id }, function () {
            toastr.success('Registro excluído com sucesso', 'CRM');
            $('#item-cancelamento-' + id).remove();
            limparDadosCancelamentoNF();
        }).fail(function (data) {
            if (data.statusText) {
                toastr.error(data.statusText, 'CRM');
            } else {
                toastr.error('Falha ao excluir o Registro', 'CRM');
            }
        }).always(function () {
            $('#modal-exclusao-cancelamento').data('id', '0').modal('hide');
        });
    }
}

function atualizarProrrogacao(id) {

    event.preventDefault();

    $.get(urlBase + 'Solicitacoes/ObterDetalhesProrrogacao?id=' + id, function (resultado) {
        if (resultado !== null) {
            carregarCamposProrrogacao(resultado);
        }
    });
}

function obterProrrogacaoDefault() {

    var solicitacaoId = $('#Id').val();

    if (isNumero(solicitacaoId)) {
        $.get(urlBase + 'Solicitacoes/ObterProrrogacaoDefault?solicitacaoId=' + solicitacaoId, function (resultado) {
            carregarCamposProrrogacao(resultado);
        });
    }
}

function carregarCamposProrrogacao(resultado) {

    if (resultado) {

        $('#ProrrogacaoId').val(resultado.Id);
        $('#ProrrogacaoValorNF').empty().val(resultado.ValorNF);
        $('#ProrrogacaoVencimentoOriginal').empty().val(resultado.VencimentoOriginal);
        $('#ProrrogacaoDataProrrogacao').empty().val(resultado.DataProrrogacao);
        $('#ProrrogacaoNumeroProrrogacao').val(resultado.NumeroProrrogacao);
        $('#ProrrogacaoIsentarJuros').val(resultado.IsentarJuros);
        $('#ProrrogacaoValorJuros').empty().val(resultado.ValorJuros);
        $('#ProrrogacaoValorTotalComJuros').empty().val(resultado.ValorTotalComJuros);
        $('#ProrrogacaoObservacoes').empty().val(resultado.Observacoes);
        $('#ProrrogacaoNotaFiscal').empty().val(resultado.NFE);

        var unidadeSolicitacao = parseInt($('#UnidadeSolicitacaoId').val());
        var tipoOperacao = parseInt($('#TipoOperacaoId').val());

        if ((unidadeSolicitacao === 3 || unidadeSolicitacao === 4) && tipoOperacao !== 6) {

            $("#ProrrogacaoContaId")
                .empty()
                .append(
                    $('<option>', {
                        value: resultado.ContaId,
                        text: resultado.ContaDescricao
                    }));
        } else {

            $('#ProrrogacaoRazaoSocial')
                .empty()
                .val(resultado.RazaoSocial);
        }

        $("#ProrrogacaoNotaFiscalId")
            .empty()
            .append(
                $('<option>', {
                    value: resultado.NotaFiscalId,
                    text: resultado.NFE + ' - ' + resultado.DataEmissao
                }));
    }
}

function excluirProrrogacao(id) {

    $('#modal-exclusao-prorrogacao')
        .data('id', id)
        .modal('show');
}

function confirmarExclusaoProrrogacao() {

    var id = $('#modal-exclusao-prorrogacao').data('id');

    if (isNumero(id)) {

        $.post(urlBase + 'Solicitacoes/ExcluirProrrogacao/', { id: id }, function () {
            toastr.success('Registro excluído com sucesso', 'CRM');
            $('#item-prorrogacao-' + id).remove();
            limparDadosProrrogacao();
            consultarProrrogacoes();
        }).fail(function (data) {
            if (data.statusText) {
                toastr.error(data.statusText, 'CRM');
            } else {
                toastr.error('Falha ao excluir o Registro', 'CRM');
            }
        }).always(function () {
            $('#modal-exclusao-prorrogacao').data('id', '0').modal('hide');
        });
    }
}

function consultarProrrogacoes() {

    var id = $("#Id").val();

    if (isNumero(id)) {
        $.get(urlBase + 'Solicitacoes/ObterProrrogacoes/?solicitacaoId=' + id, function (data) {
            $('#_ConsultaProrrogacoes').html(data);
        });
    }
}

$('#btnLimparDadosProrrogacao').click(function () {
    limparDadosProrrogacao();
});

function limparDadosProrrogacao() {

    $("#frmSolicitacoesProrrogacao")[0].reset();

    $('#ProrrogacaoId').val('0');
    $('#ProrrogacaoNotaFiscalId').empty();
    $('#ProrrogacaoContaId').empty();
}

$('#ProrrogacaoNotaFiscal').blur(function () {

    var nfe = $("#ProrrogacaoNotaFiscal").val();

    if (isInteiro(nfe)) {
        $.get(urlBase + 'Solicitacoes/ObterNotasFiscais?nfe=' + nfe, function (resultado) {

            if (resultado.length === 0) {
                toastr.error('Numeração não existe ou foi cancelada', 'CRM');
                return;
            }

            var select = $('#ProrrogacaoNotaFiscalId');

            select.empty();

            if (resultado.length > 1)
                select.append('<option value=""></option>');

            $.each(resultado, function (key, value) {
                select.append('<option value=' + value.Id + '>' + value.NFE + ' - ' + value.DataEmissao + '</option>');
            });

            if (resultado.length === 1)
                obterDetalhesNotaFiscalProrrogacao($('#ProrrogacaoNotaFiscalId').val());
        });
    } else {
        toastr.error('Número de Nota Fiscal inválida', 'CRM');
    }
});

$('#ProrrogacaoNotaFiscalId').change(function () {
    obterDetalhesNotaFiscalProrrogacao($(this).val());
});

function obterDetalhesNotaFiscalProrrogacao(id) {

    var unidade = parseInt($('#UnidadeSolicitacaoId').val());
    var tipoOperacao = parseInt($('#TipoOperacaoId').val());

    if (isNumero(id)) {
        if (unidade !== 3 && unidade !== 4) {

            $.get(urlBase + 'Solicitacoes/ObterDetalhesNotaFiscal?nfeId=' + id + '&tipoOperacao=' + tipoOperacao, function (resultado) {
                $('#ProrrogacaoRazaoSocial').empty().val(resultado.Cliente);
                $('#ProrrogacaoValorNF').empty().val(resultado.Valor);
                $('#ProrrogacaoVencimentoOriginal').empty().val(resultado.DataVencimento);
            });
        }
    }
}

$('#ProrrogacaoIsentarJuros').change(function () {
    calculoJurosRestituicao($(this).val());
});

$('#ProrrogacaoDataProrrogacao').blur(function () {
    calculoJurosRestituicao($('#ProrrogacaoIsentarJuros').val());
});

function calculoJurosRestituicao(isento) {

    var vencimento = $('#ProrrogacaoVencimentoOriginal').val();
    var prorrogacao = $('#ProrrogacaoDataProrrogacao').val();
    var valorNF = $('#ProrrogacaoValorNF').val();

    if (parseInt(isento) === 1) {
        $.get(urlBase + 'Solicitacoes/CalcularJurosProrrogacao?vencimento=' + vencimento + '&prorrogacao=' + prorrogacao + '&valorNF=' + valorNF, function (resultado) {

            $('#ProrrogacaoValorJuros').val(resultado.juros);
            $('#ProrrogacaoValorTotalComJuros').empty().val(resultado.valorComJuros);

        }).fail(function (data) {
            toastr.error(data.statusText, 'CRM');
        });
    } else {
        $.get(urlBase + 'Solicitacoes/CalcularJurosProrrogacao?vencimento=' + vencimento + '&prorrogacao=' + prorrogacao + '&valorNF=' + valorNF, function (resultado) {

            $('#ProrrogacaoValorJuros').val(resultado.juros);
            $('#ProrrogacaoValorTotalComJuros').empty().val(resultado.valorComJuros);

        }).fail(function (data) {
            toastr.error(data.statusText, 'CRM');
        });
    }
}

$('#RestituicaoTipoPesquisaNumero').blur(function () {

    var tipoPesquisa = $('#RestituicaoTipoPesquisa').val();
    var termoPesquisa = $('#RestituicaoTipoPesquisaNumero').val();
    var tipoOperacao = parseInt($('#TipoOperacaoId').val());

    if (isNumero(tipoPesquisa)) {

        $.get(urlBase + 'Solicitacoes/ObterNotasFiscaisPorTipoSolicitacao?tipoPesquisa=' + tipoPesquisa + '&tipoOperacao=' + tipoOperacao + '&termoPesquisa=' + termoPesquisa, function (resultado) {

            var select = $('#RestituicaoNotaFiscalId');

            select.empty();

            if (resultado.length > 1)
                select.append('<option value=""></option>');

            $.each(resultado, function (key, value) {
                select.append('<option value=' + value.Id + '>' + value.NFE + ' - ' + value.DataEmissao + '</option>');
            });

            if (resultado.length === 1)
                obterDetalhesNotaFiscalRestituicao($('#RestituicaoNotaFiscalId').val());
        });
    }
});

$('#RestituicaoNotaFiscalId').change(function () {

    var nfeId = $(this).val();
    obterDetalhesNotaFiscalRestituicao(nfeId);
});

function obterDetalhesNotaFiscalRestituicao(id) {

    var unidade = parseInt($('#UnidadeSolicitacaoId').val());
    var tipoOperacao = parseInt($('#TipoOperacaoId').val());

    if (isNumero(id)) {
        if (unidade !== 3 && unidade !== 4) {
            $.get(urlBase + 'Solicitacoes/ObterDetalhesNotaFiscal?nfeId=' + id + '&tipoOperacao=' + tipoOperacao, function (resultado) {

                $('#RestituicaoValorNF').empty().val(resultado.Valor);
                $('#RestituicaoRPS').empty().val(resultado.RPS);
                $('#RestituicaoDocumento').empty().val(resultado.Documento);
                $('#RestituicaoLote').empty().val(resultado.Lote);
                $('#RestituicaoReserva').empty().val(resultado.Reserva);

                calcularValorAVistaRestituicao(resultado.Lote, $('#RestituicaoNotaFiscalId').val(), 0);
            });
        }
    }
}

function excluirRestituicao(id) {

    $('#modal-exclusao-restituicao')
        .data('id', id)
        .modal('show');
}

function confirmarExclusaoRestituicao() {

    var id = $('#modal-exclusao-restituicao').data('id');

    if (isNumero(id)) {

        $.post(urlBase + 'Solicitacoes/ExcluirRestituicao/', { id: id }, function () {
            toastr.success('Registro excluído com sucesso', 'CRM');
            $('#item-restituicao-' + id).remove();
            limparDadosRestituicao();
            consultarRestituicoes();
            atualizarResumoRestituicao();
        }).fail(function (data) {
            if (data.statusText) {
                toastr.error(data.statusText, 'CRM');
            } else {
                toastr.error('Falha ao excluir o Registro', 'CRM');
            }
        }).always(function () {
            $('#modal-exclusao-restituicao').data('id', '0').modal('hide');
        });
    }
}

function consultarRestituicoes() {

    var id = $("#Id").val();

    if (isNumero(id)) {
        $.get(urlBase + 'Solicitacoes/ObterRestituicoes/?solicitacaoId=' + id, function (data) {
            $('#_ConsultaRestituicoes').html(data);
        });
    }
}

function atualizarDetalhesRestituicao() {

    $('#ValorDevido').val('0');
    $('#ValorCobrado').val('0');
    $('#ValorCredito').val('0');
}

$('#chkHabilitaCampoValorDevido').change(function () {
    $('#ValorDevido').prop('readonly', !this.checked);
});

function atualizarRestituicao(id) {

    event.preventDefault();

    $.get(urlBase + 'Solicitacoes/ObterDetalhesRestituicao?id=' + id, function (resultado) {

        if (resultado !== null) {
            carregarCamposRestituicao(resultado);
        }
    });
}

function obterRestituicaoDefault() {

    var solicitacaoId = $('#Id').val();

    if (isNumero(solicitacaoId)) {
        $.get(urlBase + 'Solicitacoes/ObterRestituicaoDefault?solicitacaoId=' + solicitacaoId, function (resultado) {
            carregarCamposRestituicao(resultado);
        });
    }
}

function carregarCamposRestituicao(resultado) {

    if (resultado) {

        $('#RestituicaoId').empty().val(resultado.Id);
        $('#RestituicaoTipoPesquisa').val(resultado.TipoPesquisa);
        $('#RestituicaoValorNF').empty().val(resultado.ValorNF);
        $('#RestituicaoRPS').empty().val(resultado.RPS);
        $('#RestituicaoLote').empty().val(resultado.Lote);
        $('#RestituicaoDocumento').empty().val(resultado.Documento);
        $('#RestituicaoAgencia').empty().val(resultado.Agencia);
        $('#RestituicaoContaCorrente').empty().val(resultado.ContaCorrente);
        $('#RestituicaoFornecedorSAP').empty().val(resultado.FornecedorSAP);
        $('#RestituicaoDataVencimento').empty().val(resultado.DataVencimento);
        $('#RestituicaoObservacoes').empty().val(resultado.Observacoes);
        $('#RestituicaoBancoId').val(resultado.BancoId);

        if (resultado.TipoPesquisa === 2) {
            $('#RestituicaoTipoPesquisaNumero').empty().val(resultado.Lote);
        } else if (resultado.TipoPesquisa === 5) {
            $('#RestituicaoTipoPesquisaNumero').empty().val(resultado.TipoPesquisaNumero);
        } else {
            $('#RestituicaoTipoPesquisaNumero').empty().val(resultado.NFE);
        }

        $("#RestituicaoNotaFiscalId")
            .empty()
            .append(
                $('<option>', {
                    value: resultado.NotaFiscalId,
                    text: resultado.NFE + ' - ' + resultado.DataEmissao
                }));

        var tipoOperacao = parseInt($('#TipoOperacaoId').val());

        if (tipoOperacao !== 6) {

            $("#RestituicaoFavorecidoId")
                .empty()
                .append(
                    $('<option>', {
                        value: resultado.FavorecidoId,
                        text: resultado.FavorecidoDescricao
                    }));
        } else {
            $('#RestituicaoRazaoSocial').val(resultado.FavorecidoDescricao);
        }

        if (parseInt(resultado.ValorAPagar) === 0) {
            calcularValorAVistaRestituicao(resultado.Lote, $('#RestituicaoNotaFiscalId').val(), 0);
        } else {
            $('#RestituicaoValorAPagar').val(resultado.ValorAPagar);
        }
    }
}

$('#btnLimparRestituicao').click(function () {
    limparDadosRestituicao();
});

function limparDadosRestituicao() {
    $('#frmSolicitacoesRestituicao')[0].reset();
    $('#RestituicaoNotaFiscalId').val('0');
    $('#RestituicaoFavorecidoId').val('0');
    $('#RestituicaoId').val('0');
}

$('#ValorCobrado').blur(function () {

    atualizarResumoRestituicao();
});

function atualizarResumoRestituicao() {

    var solicitacaoId = $("#Id").val();

    var valorDevido = $('#ValorDevido').val();

    var valorCobrado = $('#ValorCobrado').val();

    var habilitaValorDevido = $('#chkHabilitaCampoValorDevido').is(':checked');

    if (!isMoeda(valorCobrado)) {
        toastr.error('Valor incorreto!', 'CRM');
        return;
    }

    $.post(urlBase + 'Solicitacoes/AtualizarResumoRestituicao/', { solicitacaoId: solicitacaoId, valorCobrado: valorCobrado, habilitaValorDevido: habilitaValorDevido, valorDevido: valorDevido }, function (data) {

        toastr.success('Resumo atualizado!', 'CRM');

        $('#ValorDevido').val(data.ValorDevido);
        $('#ValorCobrado').val(data.ValorCobrado);
        $('#ValorCredito').val(data.ValorCredito);

    }).fail(function (data) {
        if (data.statusText) {
            toastr.error(data.statusText, 'CRM');
        } else {
            toastr.error('Falha ao atualizar o resumo', 'CRM');
        }
    });
}

$('#DescontoTipoPesquisaNumero').blur(function () {

    var unidade = parseInt($('#UnidadeSolicitacaoId').val());
    var tipoOperacao = parseInt($('#TipoOperacaoId').val());

    if (unidade === 3 || unidade === 4 || tipoOperacao === 4) {
        return;
    }

    var tipoPesquisa = $('#TipoPesquisaSolicitacaoDesconto').val();
    var termoPesquisa = $('#DescontoTipoPesquisaNumero').val();

    limparDadosDesconto();

    $('#TipoPesquisaSolicitacaoDesconto').val(tipoPesquisa);
    $('#DescontoTipoPesquisaNumero').val(termoPesquisa);

    var url = tipoOperacao !== 3
        ? 'ObterGRS'
        : 'ObterGRSRedex';

    console.log(tipoPesquisa);
    if (isNumero(tipoPesquisa)) {

        // Se não for minuta...
        if (parseInt(tipoPesquisa) !== 4) {
            $.get(urlBase + 'Solicitacoes/' + url + '?tipoPesquisa=' + tipoPesquisa + '&termoPesquisa=' + termoPesquisa, function (resultado) {

                if (resultado.length !== 0) {
                    var select = $('#DescontoGRMinutaId');

                    select
                        .empty()
                        .append('<option></option>');

                    $.each(resultado, function (key, value) {
                        select.append('<option value=' + value.SeqGR + '>' + value.Display + '</option>');
                    });

                    if (parseInt($('#DescontoGRMinutaId').val()) === 0 || resultado.length === 1) {
                        obterDetalhesGRDesconto($('#DescontoGRMinutaId').val(), tipoOperacao);
                    }

                } else {
                    obterDetalhesGRDesconto($('#DescontoTipoPesquisaNumero').val(), tipoOperacao);
                }

            }).fail(function (data) {
                if (data.statusText) {
                    toastr.error(data.statusText, 'CRM');
                } else {
                    toastr.error('Falha ao consultar o registro', 'CRM');
                }
            });
        } else {

            $('#DescontoValor').val('');
            $('#DescontoProposta').val('');
            $('#DescontoVencimentoGR').val('');
            $('#DescontoFreeTimeGR').val('');
            $('#DescontoPeriodo').val('');
            $('#DescontoFormaPagamento').val('');
            $('#DescontoServicoId').val('0');
            $('#DescontoValorDescontoNoServico').val('');
            $('#DescontoValorDescontoFinal').val('');
            $('#DescontoTipoDesconto').val('0');
            $('#DescontoVencimento').val('');
            $('#DescontoFreeTime').val('');
            $('#DescontoValorDesconto').val('');
            $("#DescontoClienteId").empty();
            $("#DescontoIndicadorId").empty();
            $('#DescontoGRMinutaId').val('');

            $.get(urlBase + 'Solicitacoes/ObterMinuta?termoPesquisa=' + termoPesquisa, function (resultado) {

                var select = $('#DescontoGRMinutaId');

                select
                    .empty()
                    .append('<option value=' + resultado.Id + '>' + resultado.Id + '</option>');

                $('#DescontoValor').val(resultado.Valor);
                $('#DescontoClienteDescricao').val(resultado.ClienteDescricao);

                var unidadeSolicitacao = parseInt($('#UnidadeSolicitacaoId').val());

                if (unidadeSolicitacao !== 3 && unidadeSolicitacao !== 4) {

                    $("#DescontoClienteId")
                        .empty()
                        .append(
                            $('<option>', {
                                value: resultado.ClienteId,
                                text: resultado.ClienteDescricao
                            }));
                }


                $('#DescontoProposta').prop('readonly', false);

                select = $('#DescontoServicoId');

                select
                    .empty()
                    .append('<option value=""></option>');

                $.each(resultado.servicosFaturados, function (key, value) {
                    select.append('<option value=' + value.Id + '>' + value.Descricao + ' - R$ ' + formataMoedaPtBr(value.Valor) + '</option>');
                });

                $('#btnCadastrarDesconto').prop('disabled', false);

            }).fail(function (data) {
                if (data.statusText) {
                    toastr.error(data.statusText, 'CRM');
                } else {
                    toastr.error('Falha ao consultar o registro', 'CRM');
                }
            });
        }
    }
});

function obterServicosFaturamentoMinuta(minuta) {

    $.get(urlBase + 'Solicitacoes/ObterServicosFaturamentoMinuta?minuta=' + minuta, function (resultado) {

        var select = $('#DescontoServicoId');

        select
            .empty()
            .append('<option value=""></option>');

        $.each(resultado, function (key, value) {
            select.append('<option value=' + value.Id + '>' + value.Descricao + ' - ' + value.Valor + '</option>');
        });

    }).fail(function (data) {
        if (data.statusText) {
            toastr.error(data.statusText, 'CRM');
        } else {
            toastr.error('Falha ao consultar o registro', 'CRM');
        }
    });
}

$('#DescontoGRMinutaId').change(function () {

    var unidade = parseInt($('#UnidadeSolicitacaoId').val());

    if (unidade === 3 || unidade === 4) {
        return;
    }

    obterDetalhesGRDesconto($(this).val(), parseInt($('#TipoOperacaoId').val()));
});

function limparValoresDesconto() {

    $('#DescontoValorDesconto').val('0,00');
    $('#DescontoValorDescontoNoServico').val('0,00');
    $('#DescontoValorDescontoFinal').val('0,00');
    $('#DescontoValorDescontoComImposto').val('0,00');
}

$('#DescontoTipoDesconto').change(function () {

    limparValoresDesconto();
});

$('#DescontoTipoDescontoPorServico').change(function () {

    limparValoresDesconto();

    $('#DescontoServicoId').prop('disabled', !$(this).val());
    $('#DescontoValorDescontoNoServico').prop('readonly', !$(this).val());
    $('#DescontoTipoDesconto').val('0');
});

$('#DescontoTipoDescontoGeral').change(function () {

    limparValoresDesconto();

    $('#DescontoServicoId').prop('disabled', $(this).val());
    $('#DescontoServicoId').val('0');
    $('#DescontoValorDescontoNoServico').prop('readonly', $(this).val());
    $('#DescontoTipoDesconto').val('0');
});

function obterDetalhesGRDesconto(seq_gr, tipoOperacao) {

    if (isNumero(seq_gr)) {

        var url = '';
        var lote = 0;
        var bl = '';
        var reserva = '';
        var nf = '';

        var tipoPesquisa = parseInt($('#TipoPesquisaSolicitacaoDesconto').val());
        var valorPesquisa = $('#DescontoTipoPesquisaNumero').val();

        console.log('Tipo Pesquisa: ' + tipoPesquisa);
        console.log('Valor Pesquisa: ' + valorPesquisa);
        switch (tipoPesquisa) {
            case 1:
                nf = valorPesquisa;
                break;
            case 2:
                lote = valorPesquisa;
                break;
            case 3:
                seq_gr = valorPesquisa;
                break;
            case 6:
                reserva = valorPesquisa;
                break;
            case 7:
                bl = valorPesquisa;
                break;
            default:
        }

        if (tipoOperacao === 3) {
            url = urlBase + 'Solicitacoes/ObterDetalhesGRRedex/?reserva=' + reserva + '&display=' + $('#DescontoGRMinutaId option:selected').text();
        } else {
            url = urlBase + 'Solicitacoes/ObterDetalhesGR/?lote=' + lote + '&seq_gr=' + seq_gr + '&bl=' + bl + '&tipoPesquisa=' + tipoPesquisa;
        }

        $.get(url, function (resultado) {
            if (resultado) {

                if (tipoOperacao === 3) {
                    $('#DescontoReserva').val(resultado.Reserva);
                    $('#ClienteFaturamentoId').val(resultado.ClienteId);
                }

                $('#DescontoValor').empty().val(resultado.Valor);
                $('#DescontoProposta').empty().val(resultado.Proposta);
                $('#DescontoVencimentoGR').empty().val(resultado.Vencimento);
                $('#DescontoFreeTimeGR').empty().val(resultado.FreeTime);
                $('#DescontoPeriodo').empty().val(resultado.Periodos);
                $('#DescontoFormaPagamento').val(resultado.FormaPagamento);
                $('#DescontoDescricaoFormaPagamento').val(resultado.DescricaoFormaPagamento);
                $('#DescontoLote').val(resultado.Lote);
                $('#DescontoPeriodo').prop('readonly', (parseInt(resultado.Periodos) > 0));
                $('#DescontoFormaPagamento').prop('readonly', (parseInt(resultado.FormaPagamento) > 0));

                $('#DescontoValorDescontoNoServico').prop('readonly', true);

                $('#DescontoServicoId').prop('disabled', $('#DescontoTipoDescontoGeral').is(':checked'));

                var unidadeSolicitacao = $('#UnidadeSolicitacaoId').val();

                if (unidadeSolicitacao === 3 || unidadeSolicitacao === 4) {

                    $("#DescontoClienteId")
                        .empty()
                        .append(
                            $('<option>', {
                                value: resultado.ClienteId,
                                text: resultado.ClienteDescricao
                            }));

                    $("#DescontoIndicadorId")
                        .empty()
                        .append(
                            $('<option>', {
                                value: resultado.IndicadorId,
                                text: resultado.IndicadorDescricao
                            }));

                } else {

                    $('#DescontoClienteId').empty().val(resultado.ClienteId);
                    $('#DescontoClienteDescricao').empty().val(resultado.ClienteDescricao);

                    $('#DescontoIndicadorId').empty().val(resultado.IndicadorId);
                    $('#DescontoIndicadorDescricao').empty().val(resultado.IndicadorDescricao);
                }

                if (resultado.servicosFaturados) {

                    var select = $('#DescontoServicoId').empty();

                    if (resultado.servicosFaturados.length > 1) {
                        select.append('<option value=""></option>');
                    }

                    $.each(resultado.servicosFaturados, function (key, value) {
                        select.append('<option value=' + value.Id + '>' + value.Descricao + ' - R$ ' + formataMoedaPtBr(value.Valor) + '</option>');
                    });
                }

                $('#btnCadastrarDesconto').prop('disabled', false);

            }
        }).fail(function (data) {
            if (data.statusText) {
                toastr.error(data.statusText, 'CRM');
            } else {
                toastr.error('Falha ao consultar o registro', 'CRM');
            }
        });
    }
}

function obterServicosFaturados(lote, seqGr) {

    var tipoPesquisa = $('#DescontoGRMinutaId').val();

    $.get(urlBase + 'Solicitacoes/ObterServicosFaturamento?lote=' + lote + '&seqGr=' + seqGr, function (resultado) {

        var select = $('#DescontoServicoId');

        select.empty();

        if (resultado.length > 1)
            select.append('<option value=""></option>');

        $.each(resultado, function (key, value) {
            select.append('<option value=' + value.Id + '>' + value.Descricao + ' - ' + value.Valor + '</option>');
        });
    }).fail(function (data) {
        if (data.statusText) {
            toastr.error(data.statusText, 'CRM');
        } else {
            toastr.error('Falha ao consultar o registro', 'CRM');
        }
    });
}

function obterServicosFaturadosRedex(reserva, seqGr) {

    var display = $('#DescontoGRMinutaId option:selected').text();

    $.get(urlBase + 'Solicitacoes/ObterServicosFaturamentoRedex?reserva=' + reserva + '&seqGr=' + seqGr + '&display=' + display, function (resultado) {

        var select = $('#DescontoServicoId');

        select.empty();

        if (resultado.length > 1)
            select.append('<option value=""></option>');

        $.each(resultado, function (key, value) {
            select.append('<option value=' + value.Id + '>' + value.Descricao + ' - ' + value.Valor + '</option>');
        });
    }).fail(function (data) {
        if (data.statusText) {
            toastr.error(data.statusText, 'CRM');
        } else {
            toastr.error('Falha ao consultar o registro', 'CRM');
        }
    });
}

$('#btnLimparDadosDesconto').click(function () {
    limparValoresCamposDesconto();
});

function limparDadosDesconto() {
    $("#frmSolicitacoesDescontos")[0].reset();
    $('#DescontoId').val('0');
    $('#DescontoClienteId').empty();
    $('#DescontoIndicadorId').empty();
    $('#DescontoGRMinutaId').empty();
}

function limparValoresCamposDesconto() {

    $('#DescontoId').val('0');
    $('#DescontoServicoId').val('0');
    $('#DescontoFreeTime').val('');
    $('#DescontoVencimento').val('');
    $('#DescontoTipoDesconto').val('0');
    $('#DescontoValorDesconto').val('');
    $('#DescontoValorDescontoNoServico').val('');
    $('#DescontoValorDescontoFinal').val('');
    $('#DescontoValorDescontoComImposto').val('');

    $("#DescontoTipoDescontoPorServico").prop("checked", false).trigger("change");
    $("#DescontoTipoDescontoGeral").prop("checked", true).trigger("change");

    var isRedex = parseInt($('#TipoOperacaoId').val()) === 3;

    if (isRedex) {
        obterServicosFaturadosRedex($('#DescontoReserva').val(), $('#DescontoGRMinutaId').val());
    } else {
        if (parseInt($('#TipoPesquisaSolicitacaoDesconto').val()) !== 4) {
            obterServicosFaturados($('#DescontoLote').val(), $('#DescontoGRMinutaId').val());
        } else {
            obterServicosFaturamentoMinuta($('#DescontoGRMinutaId').val());
        }
    }

    $('#btnCadastrarDesconto').prop('disabled', false);
}

function atualizarDesconto(id) {

    event.preventDefault();

    var tipoOperacao = parseInt($('#TipoOperacaoId').val());

    $.get(urlBase + 'Solicitacoes/ObterDetalhesDesconto?id=' + id + '&tipoOperacao=' + tipoOperacao, function (resultado) {

        if (resultado !== null) {
            carregarCamposDesconto(resultado);
        }
    });
}

function obterDescontoDefault() {

    var solicitacaoId = $('#Id').val();

    if (isNumero(solicitacaoId)) {
        $.get(urlBase + 'Solicitacoes/ObterDescontoDefault?solicitacaoId=' + solicitacaoId, function (resultado) {
            carregarCamposDesconto(resultado);
        });
    }
}

function carregarCamposDesconto(resultado) {

    if (resultado) {

        if (resultado.PorServico) {
            $("#DescontoTipoDescontoPorServico")
                .prop("checked", true)
                .trigger("change");
        } else {
            $("#DescontoTipoDescontoGeral")
                .prop("checked", true)
                .trigger("change");
        }

        var tipoOperacao = parseInt($('#TipoOperacaoId').val());

        if (tipoOperacao === 3) {
            $('#DescontoReserva').empty().val(resultado.Reserva);
            $('#ClienteFaturamentoId').empty().val(resultado.ClienteFaturamentoId);
        }

        $('#TipoPesquisaSolicitacaoDesconto').val(resultado.TipoPesquisa);
        $('#DescontoId').empty().val(resultado.Id);
        $('#DescontoValor').empty().val(resultado.ValorGR);
        $('#DescontoProposta').empty().val(resultado.Proposta);
        $('#DescontoVencimentoGR').empty().val(resultado.VencimentoGR);
        $('#DescontoFreeTimeGR').empty().val(resultado.FreeTimeGR);
        $('#DescontoPeriodo').empty().val(resultado.Periodo);
        $('#DescontoLote').empty().val(resultado.Lote);
        $('#DescontoGRMinutaId').empty().val(resultado.MinutaGRId);
        $('#DescontoReserva').val(resultado.Reserva);
        $('#DescontoTipoDesconto').val(resultado.TipoDesconto);
        $('#DescontoValorDesconto').val(resultado.Desconto);
        $('#DescontoValorDescontoNoServico').val(resultado.DescontoNoServico);
        $('#DescontoValorDescontoFinal').val(resultado.DescontoFinal);
        $('#DescontoVencimento').val(resultado.Vencimento);
        $('#DescontoFreeTime').val(resultado.FreeTime);
        $('#DescontoValorDescontoComImposto').val(resultado.DescontoComImposto);

        if (resultado.ServicoFaturadoId > 0) {
            $('#DescontoServicoId')
                .empty()
                .append($('<option>', {
                    value: resultado.ServicoFaturadoId,
                    text: resultado.ServicoDescricao + ' R$ ' + resultado.ServicoValor
                }));
        }

        if (tipoOperacao !== 3) {
            $("#DescontoGRMinutaId")
                .empty()
                .append(
                    $('<option>', {
                        value: resultado.MinutaGRId,
                        text: resultado.MinutaGRId
                    }));
        } else {
            $("#DescontoGRMinutaId")
                .empty()
                .append(
                    $('<option>', {
                        value: resultado.MinutaGRId,
                        text: resultado.MinutaGRId + ' - ' + resultado.ClienteFaturamentoDescricao + ' (' + resultado.ClienteFaturamentoId + ')'
                    }));
        }

        if (resultado.TipoPesquisa === 2) {
            $('#DescontoTipoPesquisaNumero').empty().val(resultado.Lote);
        } else if (resultado.TipoPesquisa === 3) {
            $('#DescontoTipoPesquisaNumero').empty().val(resultado.MinutaGRId);
        } else if (resultado.TipoPesquisa === 4) {
            $('#DescontoTipoPesquisaNumero').empty().val(resultado.Minuta);
        } else {
            $('#DescontoTipoPesquisaNumero').val(resultado.TipoPesquisaNumero);
        }

        var unidadeSolicitacao = parseInt($('#UnidadeSolicitacaoId').val());

        if (resultado.Minuta > 0) {

            $("#DescontoGRMinutaId")
                .empty()
                .append(
                    $('<option>', {
                        value: resultado.Minuta,
                        text: resultado.Minuta
                    }));

            $('#TipoPesquisaSolicitacaoDesconto').val('3');
        }

        if (unidadeSolicitacao === 3 || unidadeSolicitacao === 4) {

            if (tipoOperacao !== 6) {

                $("#DescontoClienteId")
                    .empty()
                    .append(
                        $('<option>', {
                            value: resultado.ClienteId,
                            text: resultado.ClienteDescricao
                        }));
            } else {
                $('#DescontoRazaoSocial').val(resultado.RazaoSocial);
            }

            $("#DescontoIndicadorId")
                .empty()
                .append(
                    $('<option>', {
                        value: resultado.IndicadorId,
                        text: resultado.IndicadorDescricao
                    }));

            $('#DescontoFormaPagamento').val(resultado.FormaPagamento);

        } else {
            $('#DescontoClienteId').empty().val(resultado.ClienteId);
            $('#DescontoClienteDescricao').empty().val(resultado.ClienteDescricao);

            $('#DescontoIndicadorId').empty().val(resultado.IndicadorId);
            $('#DescontoIndicadorDescricao').empty().val(resultado.IndicadorDescricao);

            $('#DescontoDescricaoFormaPagamento').val(resultado.DescricaoFormaPagamento);
        }

        var status = parseInt($('#StatusSolicitacao').val());

        if (status !== 2) {
            $('#btnCadastrarDesconto').prop('disabled', false);
        }
    }
}

$('#DescontoValorDesconto').blur(function () {

    $('#DescontoValorDescontoNoServico').val('0,00');
    $('#DescontoValorDescontoFinal').val('0,00');
    $('#DescontoValorDescontoComImposto').val('0,00');

    var valor = formataMoedaCalculo($('#DescontoValor').val());
    var valorDesconto = formataMoedaCalculo($('#DescontoValorDesconto').val());

    var tipoDesconto = parseInt($('#DescontoTipoDesconto').val());

    var porServico = $('#DescontoTipoDescontoPorServico').is(':checked');

    var resultado = 0;

    if (porServico) {

        var id = $('#DescontoServicoId').val();

        var isRedex = parseInt($('#TipoOperacaoId').val()) === 3;

        if (isNumero(id)) {
            calcularDescontoPorServico(id, isRedex);
        }
    } else {

        if (tipoDesconto === 1) {
            resultado = valor - valorDesconto;
        } else if (tipoDesconto === 2) {

            resultado = (valor / 100) * valorDesconto;
            resultado = valor - resultado;

            var diferenca = valor - resultado;

            $('#DescontoValorDescontoComImposto').val(formataMoedaPtBr(diferenca));
        }
    }

    $('#DescontoValorDescontoFinal').val(formataMoedaPtBr(resultado));

});

function calcularDescontoPorServico(id, isRedex) {

    var valorDesconto = $('#DescontoValorDesconto').val();
    var tipoDesconto = $('#DescontoTipoDesconto').val();

    var lote = $('#DescontoLote').val();
    var reserva = $('#DescontoReserva').val();
    var seqGr = $('#DescontoGRMinutaId').val();
    var tabelaId = $('#DescontoProposta').val();
    var solicitacaoId = $('#Id').val();

    $('.label-calculando').removeClass('invisivel');

    var url = '';

    var tipoPesquisa = $('#TipoPesquisaSolicitacaoDesconto').val();

    if (!isRedex) {
        if (parseInt(tipoPesquisa) !== 4) {
            url = urlBase + 'Solicitacoes/CalcularDescontoPorServico?servicoFaturamentoId=' + id + '&tipoDesconto=' + tipoDesconto + '&desconto=' + valorDesconto + '&lote=' + lote + '&seqGr=' + seqGr + '&tabelaId=' + tabelaId + '&solicitacaoId=' + solicitacaoId;
        } else {
            url = urlBase + 'Solicitacoes/CalcularDescontoMinutaPorServico?minuta=' + seqGr + '&tipoDesconto=' + tipoDesconto + '&servico=' + id + '&desconto=' + valorDesconto + '&solicitacaoId=' + solicitacaoId;
        }
    } else {
        var display = $('#DescontoGRMinutaId option:selected').text();
        url = urlBase + 'Solicitacoes/CalcularDescontoRedex?servicoFaturamentoId=' + id + '&tipoDesconto=' + tipoDesconto + '&desconto=' + valorDesconto + '&reserva=' + reserva + '&seqGr=' + seqGr + '&tabelaId=' + tabelaId + '&solicitacaoId=' + solicitacaoId + '&display=' + display;
    }

    $.get(url, function (resultado) {
        $('#DescontoValorDescontoNoServico').val(resultado.ValorDescontoNoServico);
        $('#DescontoValorDescontoFinal').val(resultado.ValorFinal);
        $('#DescontoValorDescontoComImposto').val(resultado.DescontoComImposto);
    }).fail(function (data) {
        if (data.statusText) {
            toastr.error(data.statusText, 'CRM');
        } else {
            toastr.error('Falha ao consultar o registro', 'CRM');
        }
    }).always(function () {
        $('.label-calculando').addClass('invisivel');
    });
}

$('#chkHabilitaDescontoVencimento').change(function () {

    if (this.checked) {
        $('#DescontoVencimento').prop('readonly', false);
    } else {
        $('#DescontoVencimento').css('background-color', '#FFFFFF').val('').prop('readonly', true);
    }
});

$('#chkHabilitaDescontoFreeTime').change(function () {

    if (this.checked) {
        $('#DescontoFreeTime').prop('readonly', false);
    } else {
        $('#DescontoFreeTime').css('background-color', '#FFFFFF').val('').prop('readonly', true);
    }
});

$('#btnEnviarParaAprovacao').click(function () {

    var solicitacaoId = $("#Id").val();

    if (!isNumero(solicitacaoId)) {
        toastr.error('Solicitação Comercial não informada', 'CRM');
        return;
    }

    var tipoSolicitacao = $('#TipoSolicitacao').val();

    $('#btnEnviarParaAprovacao')
        .html('<i class="fa fa-spinner fa-spin"></i> aguarde...')
        .prop('disabled', true);

    $.post(urlBase + 'Solicitacoes/EnviarParaAprovacao/', { tipoSolicitacao: tipoSolicitacao, solicitacaoId: solicitacaoId }, function (data) {

        toastr.success('Solicitação enviada para aprovação!', 'CRM');
        console.log(typeof tipoSolicitacao);
        setTimeout(function () {
            if (data) {
                document.location.href = decodeURIComponent(data.RedirectUrl);
            }
        }, 2000);

    }).fail(function (data) {
        if (data.statusText) {
            toastr.error(data.statusText, 'CRM');
        } else {
            toastr.error('Falha ao enviar a Solicitação para Aprovação', 'CRM');
        }
    }).always(function () {
        $("#btnEnviarParaAprovacao")
            .html('Enviar para Aprovação')
            .prop('disabled', false);
    });
    var statusAtualizado = document.getElementById("statusLimit").innerHTML;
    statusAtualizado = "Em Aprovação";
});

function obterHistoricoWorkflow(id, idProcesso) {

    if (!isNumero(id)) {
        toastr.error('Solicitação Comercial não informada', 'CRM');
        return;
    }

    if (!isNumero(idProcesso)) {
        toastr.error('Id do processo não informado', 'CRM');
        return;
    }

    $.get(urlBase + 'Solicitacoes/ObterHistoricoWorkflow/?id=' + id + '&idProcesso=' + idProcesso, function (data) {
        console.log(idProcesso);
        switch (idProcesso) {
            case 5:
                $('#pnlInfoHistoricoWorkflowCancelamento').html(data);
                break;
            case 6:
                $('#pnlInfoHistoricoWorkflowDesconto').html(data);
                break;
            case 7:
                $('#pnlInfoHistoricoWorkflowRestituicao').html(data);
                break;
            case 8:
                $('#pnlInfoHistoricoWorkflowProrrogacao').html(data);
                break;
            case 10:
                $('#pnlInfoHistoricoWorkflowAlteracaoFormaPgto').html(data);
                break;
            default:
        }

    }).fail(function (data) {
        toastr.error(data.statusText, 'CRM');
    });
}

solicitacoesMensagemSucesso = function (data) {

    toastr.success('Informações atualizadas com sucesso!', 'CRM');

    var tipoSolicitacao = parseInt($('#TipoSolicitacao').val());

    if (tipoSolicitacao === 1) {
        limparDadosCancelamentoNF();
    }

    if (tipoSolicitacao === 2) {
        limparValoresCamposDesconto();
    }

    if (tipoSolicitacao === 3) {
        limparDadosProrrogacao();
    }

    if (tipoSolicitacao === 4) {
        limparDadosRestituicao();
        atualizarResumoRestituicao();
    }

    setTimeout(function () {
        if (data) {
            if (data.hash) {

                window.location.hash = "#" + data.hash;
                location.reload();

                return;
            }

            if (data.RedirectUrl)
                document.location.href = decodeURIComponent(data.RedirectUrl);
        }
    }, 1000);
}

function confirmarExclusaoDesconto() {

    var id = $('#modal-exclusao-desconto').data('id');

    if (isNumero(id)) {

        $.post(urlBase + 'Solicitacoes/ExcluirDesconto/', { id: id }, function () {
            toastr.success('Registro excluído com sucesso', 'CRM');
            $('#item-desconto-' + id).remove();
            limparValoresCamposDesconto();
        }).fail(function (data) {
            if (data.statusText) {
                toastr.error(data.statusText, 'CRM');
            } else {
                toastr.error('Falha ao excluir o Registro', 'CRM');
            }
        }).always(function () {
            $('#modal-exclusao-desconto').data('id', '0').modal('hide');
        });
    }
}

function excluirDesconto(id) {

    $('#modal-exclusao-desconto')
        .data('id', id)
        .modal('show');
}

function excluirAnexo(id, idArquivo) {

    if (isNumero(id)) {

        $.post(urlBase + 'Solicitacoes/ExcluirAnexo/', { id: id, idArquivo: idArquivo }, function () {
            toastr.success('Anexo excluído com sucesso', 'CRM');
            $('#item-anexo-' + id).remove();
        }).fail(function (data) {
            if (data.statusText) {
                toastr.error(data.statusText, 'CRM');
            } else {
                toastr.error('Falha ao excluir o Anexo', 'CRM');
            }
        });
    }
}

var solicitacoesMensagemErro = function (xhr, status) {

    var msg = $('#msgErro');
    msg.html('');

    if (xhr !== null && xhr.responseText !== '') {

        toastr.error('Falha ao atualizar a Solicitação. Verifique mensagens.', 'CRM');

        msg.removeClass('invisivel');

        var resultado = JSON.parse(xhr.responseText);

        var mensagens = resultado.erros.map(function (erro) {
            return '<li>' + erro.ErrorMessage + '</li>'
        });

        msg.html(mensagens);

        setTimeout(function () {
            msg.addClass('invisivel');
        }, 9000);
    } else {
        toastr.error(xhr.statusText, 'CRM');
    }
}

$("#btnRecallSolicitacao").click(function () {

    event.preventDefault();

    $('#RecallSolicitacaoId').val($('#Id').val());

    $('#modal-motivo-recall').modal('show');
});

$('#MotivoRecall').keyup(function () {

    if ($('#MotivoRecall').val().length > 0) {
        $('#btnConfirmaRecall').prop('disabled', false);
    } else {
        $('#btnConfirmaRecall').prop('disabled', true);
    }
});

$('#btnAnexar').click(function () {

    $(this).html('<i class="fa fa-spinner fa-spin"></i> aguarde...')
        .addClass('disabled');
});

recallSucesso = function (data) {

    if (data) {

        toastr.success('Recall aprovado', 'CRM');

        if (data.Processo === 1) {
            $('#modal-motivo-recall').modal('hide');
        } else if (data.Processo === 2) {
            $('#modal-motivo-recall-fichas').modal('hide');
            window.location.hash = "#fichaDeFaturamento";
        } else if (data.Processo === 3) {
            $('#modal-motivo-recall-premios').modal('hide');
            window.location.hash = "#premioDeParceria";
        } else if (data.Processo === 4) {
            $('#modal-motivo-recall-adendos').modal('hide');
            window.location.hash = "#adendos";
        }

        setTimeout(function () {
            location.reload();
        }, 2000);
    }
};

recallErro = function (xhr, status) {

    if (xhr.statusText)
        toastr.error(xhr.statusText, 'CRM');
};

$('#AlteracaoFormaPagamentoTipoPesquisaNumero').blur(function () {

    var tipoPesquisa = $('#AlteracaoFormaPagamentoTipoPesquisa').val();
    var termoPesquisa = $('#AlteracaoFormaPagamentoTipoPesquisaNumero').val();

    if (isNumero(tipoPesquisa)) {

        $.get(urlBase + 'Solicitacoes/ObterGRsFaturadas?tipoPesquisa=' + tipoPesquisa + '&termoPesquisa=' + termoPesquisa, function (resultado) {

            var select = $('#AlteracaoFormaPagamentoGrId');

            select.empty();

            if (resultado.length > 1)
                select.append('<option value=""></option>');

            $.each(resultado, function (key, value) {
                select.append('<option value=' + value.SeqGR + '>' + value.SeqGR + '</option>');
            });

            if (resultado.length === 1) {
                obterDetalhesAlteracaoFormaPagamento(tipoPesquisa, parseInt(termoPesquisa), $('#AlteracaoFormaPagamentoGrId').val());
            }
        });
    }
});

$('#AlteracaoFormaPagamentoGrId').change(function () {

    var tipoPesquisa = $('#AlteracaoFormaPagamentoTipoPesquisa').val();
    var termoPesquisa = $('#AlteracaoFormaPagamentoTipoPesquisaNumero').val();

    obterDetalhesAlteracaoFormaPagamento(tipoPesquisa, parseInt(termoPesquisa), $(this).val());
});

function obterDetalhesAlteracaoFormaPagamento(tipoPesquisa, lote, seq_gr) {

    if (isNumero(lote) && isNumero(seq_gr)) {
        $.get(urlBase + 'Solicitacoes/ObterDetalhesGR?tipoPesquisa=' + tipoPesquisa + '&lote=' + lote + '&seq_gr=' + seq_gr, function (resultado) {

            if (resultado) {

                $('#AlteracaoFormaPagamentoValor').empty().val(resultado.Valor);
                $('#AlteracaoFormaPagamentoLote').empty().val(resultado.Lote);
                $('#AlteracaoFormaPagamentoProposta').val('0').val(resultado.Proposta);
                $('#AlteracaoFormaPagamentoFreeTimeGR').empty().val(resultado.FreeTime);
                $('#AlteracaoFormaPagamentoPeriodo').empty().val(resultado.Periodos);
                $('#AlteracaoFormaPagamentoCliente').empty().val(resultado.ClienteDescricao);
                $('#AlteracaoFormaPagamentoIndicador').empty().val(resultado.IndicadorDescricao);
            }
        });
    }
}

$("#btnPesquisarFatContraFormaPgto").click(function () {

    event.preventDefault();

    $('#pesquisa-modal-contas')
        .data('toggle', 'AlteracaoFormaPagamentoFaturadoContraId')
        .modal('show');
});

function limparDadosAlteracaoFormaPgto() {

    $("#frmSolicitacoesAlteracaoFormaPgto")[0].reset();
    $('#AlteracaoFormaPagamentoId').val('0');
    $('#AlteracaoFormaPagamentoTipoPesquisa').val('0');
    $('#AlteracaoFormaPagamentoTipoPesquisaNumero').empty();
    $('#AlteracaoFormaPagamentoGrId').val('0');
    $('#AlteracaoFormaPagamentoValor').empty();
    $('#AlteracaoFormaPagamentoLote').empty();
    $('#AlteracaoFormaPagamentoProposta').empty();
    $('#AlteracaoFormaPagamentoFreeTimeGR').empty();
    $('#AlteracaoFormaPagamentoPeriodo').empty();
    $('#AlteracaoFormaPagamentoCliente').empty();
    $('#AlteracaoFormaPagamentoIndicador').empty();
    $('#AlteracaoFormaPagamentoFaturadoContraId').empty();
    $('#AlteracaoFormaPagamentoEncaminharPara').empty();
    $('#AlteracaoFormaPagamentoCondicaoPagamentoId').val('0');
}

function atualizarSolicitacaoFormaPgto(id) {

    event.preventDefault();

    $.get(urlBase + 'Solicitacoes/ObterDetalhesAlteracaoFormaPgto?id=' + id, function (resultado) {

        carregarCamposSolicitacaoFormaPgto(resultado);
    });
}

function excluirSolicitacaoFormaPgto(id) {

    $('#modal-exclusao-alteracao-formapgto')
        .data('id', id)
        .modal('show');
}

function confirmarExclusaoSolicitacaoFormaPgto() {

    var id = $('#modal-exclusao-alteracao-formapgto').data('id');

    if (isNumero(id)) {

        $.post(urlBase + 'Solicitacoes/ExcluirSolicitacaoFormaPgto/', { id: id }, function () {
            toastr.success('Registro excluído com sucesso', 'CRM');
            $('#item-formapgto-' + id).remove();
            limparDadosAlteracaoFormaPgto();
        }).fail(function (data) {
            if (data.statusText) {
                toastr.error(data.statusText, 'CRM');
            } else {
                toastr.error('Falha ao excluir o Registro', 'CRM');
            }
        }).always(function () {
            $('#modal-exclusao-alteracao-formapgto').data('id', '0').modal('hide');
        });
    }
}

function obterSolicitacaoFormaPgtoDefault() {

    var solicitacaoId = $('#Id').val();

    if (isNumero(solicitacaoId)) {
        $.get(urlBase + 'Solicitacoes/ObterSolicitacaoFormaPgtoDefault?solicitacaoId=' + solicitacaoId, function (resultado) {
            carregarCamposSolicitacaoFormaPgto(resultado);
        });
    }
}

function carregarCamposSolicitacaoFormaPgto(resultado) {

    if (resultado !== null) {

        $('#AlteracaoFormaPagamentoId').val(resultado.Id);
        $('#AlteracaoFormaPagamentoTipoPesquisa').val(resultado.TipoPesquisa);
        $('#AlteracaoFormaPagamentoTipoPesquisaNumero').val(resultado.TipoPesquisaNumero);
        $('#AlteracaoFormaPagamentoValor').val(resultado.Valor);
        $('#AlteracaoFormaPagamentoLote').val(resultado.Lote);
        $('#AlteracaoFormaPagamentoProposta').val(resultado.Proposta);
        $('#AlteracaoFormaPagamentoFreeTimeGR').val(resultado.FreeTime);
        $('#AlteracaoFormaPagamentoPeriodo').val(resultado.Periodo);
        $('#AlteracaoFormaPagamentoCliente').val(resultado.Cliente);
        $('#AlteracaoFormaPagamentoIndicador').val(resultado.Indicador);
        $('#AlteracaoFormaPagamentoEncaminharPara').val(resultado.EmailNota);
        $("#AlteracaoFormaPagamentoCondicaoPagamentoId").val(resultado.CondicaoPagamentoId);

        $("#AlteracaoFormaPagamentoFaturadoContraId")
            .empty()
            .append(
                $('<option>', {
                    value: resultado.FaturadoContraId,
                    text: resultado.FaturadoContraDescricao
                }));

        $("#AlteracaoFormaPagamentoGrId")
            .empty()
            .append(
                $('<option>', {
                    value: resultado.Gr,
                    text: resultado.Gr
                }));
    }
}

window.addEventListener("submit", function (e) {

    var form = e.target;
    if (form.getAttribute("enctype") === "multipart/form-data") {
        if (form.dataset.ajax) {
            e.preventDefault();
            e.stopImmediatePropagation();
            var xhr = new XMLHttpRequest();
            xhr.open(form.method, form.action);
            xhr.onreadystatechange = function () {
                if (xhr.readyState == 4) {
                    if (xhr.status == 200) {

                        solicitacoesMensagemSucesso();

                        if (form.dataset.ajaxUpdate) {
                            var updateTarget = document.querySelector(form.dataset.ajaxUpdate);
                            if (updateTarget) {
                                updateTarget.innerHTML = xhr.responseText;
                            }
                        }
                    } else if (xhr.status == 400) {
                        solicitacoesMensagemErro(xhr, '');
                    }

                    if (form.id === 'frmSolicitacoesAnexos') {
                        $("#btnAnexar").removeClass('disabled')
                            .html('Salvar');
                    }
                }
            };
            xhr.send(new FormData(form));
        }
    }
}, true);