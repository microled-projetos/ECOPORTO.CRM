$('#abas a').on('click', function (e) {
    var modelo = $("#ModeloId").val();
    atualizarPreview(modelo);
});

window.addEventListener("DOMContentLoaded", function (event) {

    var id = $("#Id").val();

    if (id !== undefined) {

        tipoCargaArmazenagem($('#TipoCargaArmazenagem').val());
        tipoCargaArmazenagemCIF($('#TipoCargaArmazenagemCIF').val());        
        tipoCargaArmazenagemAllIn($("#TipoCargaArmazenagemAllIn").val());
        tipoCargaArmazenagemMinimo($("#TipoCargaArmazenagemMinimo").val());
        tipoCagraMinimoGerais($("#TipoCargaMinimoGerais").val());
        tipoCargaServLib($("#TipoCargaServLib").val());
        tipoCargaServGerais($("#TipoCargaServGerais").val());
        tipoCargaPeriodoPadrao($('#TipoCargaPeriodoPadrao').val());
        tipoCargaServMecManual($('#TipoCargaServMecManual').val());
    }

    obterCamposDaOportunidade();
});

$("#TipoRegistro").change(function () {
    montarCampos();
});

document.addEventListener('dragstart', function (event) {

    var linha = event.target.getAttribute('data-linha');
    var campo = event.target.getAttribute('data-campo');

    if (linha && campo) {
        event.dataTransfer.setData('text/plain', '{' + linha + ':' + campo + '} ');
    } else {
        if (event.target !== null) {
            event.dataTransfer.setData('text/plain', campo);
        }
    }
});

window.addEventListener("focus", function (event) {

    var modelo = $('#ModeloId').val();

    if (isNumero(modelo)) {
        consultarLayouts(modelo);
    }
});

function tipoCargaArmazenagem(modo) {

    if (modo === 1) {
        $('#pnlArmCntr20, #pnlArmCntr40').removeClass('invisivel');
        $('#pnlArmCarga').addClass('invisivel');
    } else if (modo === 2) {
        $('#pnlArmCntr20, #pnlArmCntr40').addClass('invisivel');
        $('#pnlArmCarga').removeClass('invisivel');
    }
}

$('#TipoCargaArmazenagem').change(function () {
    tipoCargaArmazenagem(parseInt($(this).val()));
});

function tipoCargaArmazenagemCIF(modo) {

    if (modo === 1) {
        $('#pnlArmCifCntr20, #pnlArmCifCntr40').removeClass('invisivel');
        $('#pnlArmCifCarga').addClass('invisivel');
    } else if (modo === 2) {
        $('#pnlArmCifCntr20, #pnlArmCifCntr40').addClass('invisivel');
        $('#pnlArmCifCarga').removeClass('invisivel');
    }
}

$('#TipoCargaArmazenagemCIF').change(function () {
    tipoCargaArmazenagemCIF(parseInt($(this).val()));
});

function tipoCargaArmazenagemAllIn(modo) {
    if (modo === 1) {
        $('#pnlArmAllInCntr20, #pnlArmAllInCntr40').removeClass('invisivel');
        $('#pnlArmAllInCarga').addClass('invisivel');
    } else if (modo === 2) {
        $('#pnlArmAllInCntr20, #pnlArmAllInCntr40').addClass('invisivel');
        $('#pnlArmAllInCarga').removeClass('invisivel');
    }
}

$("#TipoCargaArmazenagemAllIn").change(function () {
    tipoCargaArmazenagemAllIn(parseInt($(this).val()));
});

function tipoCargaArmazenagemMinimo(modo) {
    if (modo == 1) {
        $('#pnlArmMinCntr20, #pnlArmMinCntr40').removeClass('invisivel');
        $('#pnlArmMinCarga, #pnlLimiteBls').addClass('invisivel');
        $('#pnlArmMinLinRef').removeClass('col-md-1').addClass('col-md-3');
    } else if (modo == 2) {
        $('#pnlArmMinCntr20, #pnlArmMinCntr40').addClass('invisivel');
        $('#pnlArmMinCarga, #pnlLimiteBls').removeClass('invisivel');
        $('#pnlArmMinLinRef').removeClass('col-md-3').addClass('col-md-1');
    }
}
$("#TipoCargaArmazenagemMinimo").change(function () {
    tipoCargaArmazenagemMinimo($(this).val());
});

function tipoCargaArmazenagemCifMinimo(modo) {
    if (modo == 1) {
        $('#pnlArmCifMinCarga, #pnlArmCifMinLimiteBls').addClass('invisivel');
        $('#pnlArmCifMinLinRef').removeClass('col-md-1').addClass('col-md-2');
        $('#pnlArmCifMinDescrValor').removeClass('col-sm-4').addClass('col-sm-5');
    } else if (modo == 2) {
        $('#pnlArmCifMinCarga, #pnlArmCifMinLimiteBls').removeClass('invisivel');
        $('#pnlArmCifMinLinRef').removeClass('col-md-2').addClass('col-md-1');
        $('#pnlArmCifMinDescrValor').removeClass('col-sm-5').addClass('col-sm-4');
    }
}

$("#TipoCargaArmazenagemMinimoCIF").change(function () {
    tipoCargaArmazenagemCifMinimo($(this).val());
});

function tipoCagraMinimoGerais(modo) {
    if (modo == 1) {
        $('#pnlMinGeraisCntr20, #pnlMinGeraisCntr40').removeClass('invisivel');
        $('#pnlMinGeraisCarga').addClass('invisivel');
    } else if (modo == 2) {
        $('#pnlMinGeraisCntr20, #pnlMinGeraisCntr40').addClass('invisivel');
        $('#pnlMinGeraisCarga').removeClass('invisivel');
    }
}

$("#TipoCargaMinimoGerais").change(function () {
    tipoCagraMinimoGerais($(this).val());
});

function tipoCargaServLib(modo) {
    if (modo === 1) {
        $('#pnlServLibCntr20, #pnlServLibCntr40').removeClass('invisivel');
        $('#pnlServLibCarga').addClass('invisivel');
    } else if (modo === 2) {
        $('#pnlServLibCntr20, #pnlServLibCntr40').addClass('invisivel');
        $('#pnlServLibCarga').removeClass('invisivel');
    }
}
$("#TipoCargaServLib").change(function () {
    tipoCargaServLib(parseInt($(this).val()));
});

function tipoCargaServGerais(modo) {

    var opcao = parseInt(modo);

    if (opcao === 1) {

        $('#pnlGeraisCntr20, #pnlGeraisCntr40').removeClass('invisivel');
        $('#pnlGeraisCarga').addClass('invisivel');
    }

    if (opcao === 2) {

        $('#pnlGeraisCntr20, #pnlGeraisCntr40').addClass('invisivel');
        $('#pnlGeraisCarga').removeClass('invisivel');
    }
}
$("#TipoCargaServGerais").change(function () {
    tipoCargaServGerais(parseInt($(this).val()));
});

function tipoCargaPeriodoPadrao(modo) {

    var opcao = parseInt(modo);

    if (opcao === 1) {

        $('#pnlPerPadraoCntr20, #pnlPerPadraoCntr40').removeClass('invisivel');
        $('#pnlPerPadraoCarga').addClass('invisivel');
    }

    if (opcao === 2) {

        $('#pnlPerPadraoCntr20, #pnlPerPadraoCntr40').addClass('invisivel');
        $('#pnlPerPadraoCarga').removeClass('invisivel');
    }
}

$("#TipoCargaPeriodoPadrao").change(function () {
    tipoCargaPeriodoPadrao(parseInt($(this).val()));
});

function tipoCargaServMecManual(modo) {

    var opcao = parseInt(modo);

    if (opcao === 1) {

        $('#pnlMecManualCntr20, #pnlMecManualCntr40').removeClass('invisivel');
        $('#pnlMecManualCarga').addClass('invisivel');
    }

    if (opcao === 2) {

        $('#pnlMecManualCntr20, #pnlMecManualCntr40').addClass('invisivel');
        $('#pnlMecManualCarga').removeClass('invisivel');
    }
}

$("#TipoCargaServMecManual").change(function () {
    tipoCargaServMecManual(parseInt($(this).val()));
});

function consultarLayouts(modeloId) {

    $.get(urlBase + 'Layouts/ConsultarLayouts?modeloId=' + modeloId, function (data) {
        $('#RelacaoLayouts').html(data);
    });
}

$("#ModeloId").change(function () {

    var modelo = $(this).val();

    $('#pnlCabecalho').hide();

    if (modelo !== "") {
        $('#pnlCabecalho').show('fast');
    }

    if (isNumero(modelo)) {

        consultarLayouts(modelo);

        $.get(urlBase + 'Layouts/ObterUltimaLinha?modeloId=' + modelo, function (linha) {
            $('#Linha').val(linha);
        });
    }

    $('#TipoRegistro').val(3);
    montarCampos();
});

function escondeTodos() {
    $('#pnlCondicaoInicial,#pnlArm,#pnlArmMinimo,#pnlArmCIF,#pnlArmMinimoCIF,#pnlArmAllIn,#pnlServicoParaMargem,#pnlMinimoPorMargem,#pnlServMecanicoManual,' +
        '#pnlMinMecanicoManual,#pnlServicoLiberacao,#pnlServicoHubPort,#pnlGerais,#pnlMinimoGerais,#pnlCondicaoGeral,#pnlPeriodoPadrao').hide();
}

function escondeServico() {
    $('#pnlDescricao').removeClass('col-md-5').addClass('col-md-8');
    $('#pnlServico').hide();
}

function exibeServico() {
    $('#pnlDescricao').removeClass('col-md-8').addClass('col-md-5');
    $('#pnlServico').show('fast');
}

function atualizarPreview(modeloId) {

    $('#preview').html('');

    $.get(urlBase + 'Layouts/MontaLayout?modeloId=' + modeloId + '&ocultar=true', function (data) {
        $('#preview').html(data);
    });
}

function montarCampos() {

    escondeTodos();
    escondeServico();

    var tipo = $("#TipoRegistro").val();

    var titulo = $("#TipoRegistro option:selected");
    $('#label-registro').text(titulo.text());

    switch (tipo) {
        case "1":
            $('#pnlCondicaoInicial').show('fast');
            break;
        case "7":
            $('#pnlArm').show('fast');
            exibeServico();
            break;
        case "8":
            $('#pnlArmMinimo').show('fast');
            exibeServico();
            break;
        case "9":
            $('#pnlArmAllIn').show('fast');
            exibeServico();
            break;
        case "10":
            $('#pnlServicoParaMargem').show('fast');
            exibeServico();
            break;
        case "11":
            $('#pnlMinimoPorMargem').show('fast');
            exibeServico();
            break;
        case "12":
            $('#pnlServMecanicoManual').show('fast');
            exibeServico();
            break;
        case "13":
            $('#pnlMinMecanicoManual').show('fast');
            break;
        case "14":
            $('#pnlServicoLiberacao').show('fast');
            exibeServico();
            break;
        case "15":
            $('#pnlServicoHubPort').show('fast');
            exibeServico();
            break;
        case "16":
            $('#pnlGerais').show('fast');
            exibeServico();
            break;
        case "17":
            $('#pnlMinimoGerais').show('fast');
            break;
        case "18":
            $('#pnlCondicaoGeral').show('fast');
            break;
        case "19":
            $('#pnlPeriodoPadrao').show('fast');
            exibeServico();
            break;
        case "21":
            $('#pnlArmCIF').show('fast');
            exibeServico();
            break;
        case "22":
            $('#pnlArmMinimoCIF').show('fast');
            exibeServico();
            break;

        default:
    }

    $('#btnSalvar').removeAttr('disabled');
}

var ckEditorToolbar = [
    { name: 'clipboard', groups: ['clipboard', 'undo'], items: ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'Undo', 'Redo'] },
    { name: 'editing', groups: ['find', 'selection', 'spellchecker'], items: ['Find', 'Replace', '-', 'SelectAll'] },
    { name: 'basicstyles', groups: ['basicstyles', 'cleanup'], items: ['Bold', 'Italic', 'Underline', 'Strike'] },
    { name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi'], items: ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'Blockquote', 'CreateDiv', '-', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'] },
    { name: 'insert', items: ['Image', 'Table'] },
    { name: 'styles', items: ['Styles', 'Format', 'Font', 'FontSize'] },
    { name: 'colors', items: ['TextColor', 'BGColor'] }
];

CKEDITOR.replace('CondicoesIniciais', {
    toolbar: ckEditorToolbar,
    height: 600,
    on: {
        insertElement: function (e) {
            var el = $(e.data.$);

            if (el.is('table')) {
                el.addClass('ck_table').removeAttr('cellpadding').removeAttr('cellspacing');
            }
        },
        instanceReady: function (ev) {
            // Output paragraphs as <p>Text</p>.
            this.dataProcessor.writer.setRules('p', {
                indent: false,
                breakBeforeOpen: true,
                breakAfterOpen: false,
                breakBeforeClose: false,
                breakAfterClose: true
            });
        }
    }
});

CKEDITOR.replace('CondicoesGerais', {
    toolbar: ckEditorToolbar,
    height: 600
});

var LayoutCadastrado = function () {

    var msg = $('#msgSucesso');
    msg.removeClass('invisivel');

    resetaValores();

    var modelo = $("#ModeloId").val();

    setTimeout(function () {
        msg.addClass('invisivel');
    }, 3000);
}

var LayoutAtualizado = function () {

    var msg = $('#msgSucesso');
    msg.removeClass('invisivel');

    setTimeout(function () {
        msg.addClass('invisivel');
    }, 3000);
}

var LayoutComErros = function (xhr, status) {

    var msg = $('#msgErro');

    msg.html('');
    msg.removeClass('invisivel');

    var resultado = JSON.parse(xhr.responseText);

    var mensagens = resultado.erros.map(function (erro) {
        return '<li>' + erro.ErrorMessage + '</li>'
    })

    msg.html(mensagens);

    setTimeout(function () {
        msg.addClass('invisivel');
    }, 6000);
}

$('#msgErro').click(function () {
    $(this).addClass('invisivel');
});

function resetaValores() {

    escondeTodos();

    if (sessionStorage.getItem('faixasBl')) {
        sessionStorage.removeItem('faixasBl');
    }

    if (sessionStorage.getItem('faixasCif')) {
        sessionStorage.removeItem('faixasCif');
    }

    if (sessionStorage.getItem('faixasPeso')) {
        sessionStorage.removeItem('faixasPeso');
    }

    var modelo = $('#ModeloId').val();
    var linha = $('#Linha').val();

    $('#frmLayout')[0].reset();

    $('#ModeloId').val(modelo);

    if (isNumero(linha)) {
        $('#Linha').val(parseInt(linha) + 1);
    }

    $('#pnlArmCntr20, #pnlArmCntr40').removeClass('invisivel');
    $('#pnlArmCarga').addClass('invisivel');

    $('#pnlArmMinCntr20, #pnlArmMinCntr40').removeClass('invisivel');
    $('#pnlArmMinCarga').addClass('invisivel');

    $('#pnlArmAllInCntr20, #pnlArmAllInCntr40').removeClass('invisivel');
    $('#pnlArmAllInCarga').addClass('invisivel');

    $('#pnlGeraisCntr20, #pnlGeraisCntr40').removeClass('invisivel');
    $('#pnlGeraisCarga').addClass('invisivel');

    $('#pnlMinGeraisCntr20, #pnlMinGeraisCntr40').removeClass('invisivel');
    $('#pnlMinGeraisCarga').addClass('invisivel');

    $('#pnlServLibCntr20, #pnlServLibCntr40').removeClass('invisivel');
    $('#pnlServLibCarga').addClass('invisivel');

    $('#pnlPerPadraoCntr20, #pnlPerPadraoCntr40').removeClass('invisivel');
    $('#pnlPerPadraoCarga').addClass('invisivel');

    $('#pnlArmMinimoFaixas').addClass('invisivel');
    $('#pnlArmAllInFaixas').addClass('invisivel');
    $('#pnlServicoParaMargemFaixas').addClass('invisivel');

    $('#cbTipo').focus();
}

$("#btnAdicionarParametro").click(function () {

    event.preventDefault();

    var linha = $('#txtLinhaParametros').val();
    var campoValor = $('#cbCamposParametros').val();
    var campoTexto = $('#cbCamposParametros option:selected').text();

    if (isNumero(linha) && campoValor) {
        CKEDITOR.instances['CondicoesGerais'].insertText('{' + linha + ':' + campoValor + '} ');
    }
});

$('#btnPropostaValorExtenso').click(function () {

    if ($(this).data('campo')) {
        CKEDITOR.instances['CondicoesGerais'].insertText($(this).data('campo'));
    }
});

$("#txtLinhaParametros").blur(function () {

    var linha = $('#txtLinhaParametros').val();
    var modeloId = $('#ModeloId').val();

    if (isNumero(linha)) {
        $.get(urlBase + 'Layouts/ObterTipoRegistroPorLinha?linha=' + linha + '&modeloId=' + modeloId, function (tipo) {

            if (tipo) {
                $.get(urlBase + 'Layouts/ObterCamposLayout?tipo=' + tipo, function (resultado) {

                    var select = $('#cbCamposParametros');

                    select.html('');
                    select.append('<option value="">Escolha o Campo</option>');

                    $.each(resultado, function (i, item) {
                        select.append('<option value=' + item.Key + '>' + item.Value + '</option>');
                    });
                });
            }
        });
    }
});

function obterCamposDaOportunidade() {

    $.get(urlBase + 'Layouts/ObterCamposOportunidade', function (resultado) {

        var select = $('#cbCamposOportunidade');

        select.html('');
        select.append('<option value="">Escolha o Campo</option>');

        $.each(resultado, function (i, item) {

            select.append('<option value="' + item.Key + '">' + item.Value + '</option>');
        });
    });
}

$("#btnAdicionarParametroOportunidade").click(function () {

    event.preventDefault();

    var campoValor = $('#cbCamposOportunidade').val();
    var campoTexto = $('#cbCamposOportunidade option:selected').text();

    if (campoValor) {
        CKEDITOR.instances['CondicoesIniciais'].insertText('{' + campoValor + '} ');
    }
});

$('#btnOportunidadeValorExtenso').click(function () {

    if ($(this).data('campo')) {
        CKEDITOR.instances['CondicoesIniciais'].insertText($(this).data('campo'));
    }
});

function excluirLinha(id) {
    $('#modal-mensagem').text('Confirma a exclusão da Linha?');
    $('#del-modal').data('id', id).modal('show');
}

function confirmarExclusao() {

    var linha = $('#del-modal').data('id');
    var modelo = $('#ModeloId').val();

    $.post(urlBase + 'Layouts/ExcluirLinha?linha=' + linha + '&modelo=' + modelo)
        .done(function (data) {
            toastr.success('Linha excluída com sucesso!', 'CRM');
            consultarLayouts(modelo);
        }).fail(function (data) {
            if (data.statusText) {
                toastr.error(data.statusText, 'CRM');
            } else {
                toastr.error('O registro não pode ser excluído', 'CRM');
            }
        }).always(function () {
            $('#del-modal').data('id', '0').modal('hide');
        });
}

$('#btnCadastrarFaixasPeso').click(function () {

    var id = $("#Id").val();

    if (isNumero(id)) {

        $.get(urlBase + 'FaixasPeso/Consultar/?layoutId=' + id, function (resultado) {

            $("#ListaFaixasPeso").html(resultado);

            $('#FaixaPesoLayoutId').val(id);
            $('#faixas-peso-modal').modal('show');

        });
    }
});

function faixaPesoCadastrada() {

    toastr.success('Faixa cadastrada com sucesso!', 'Faixas Peso');

    $('#FaixasPesoValorInicial').val('');
    $('#FaixasPesoValorFinal').val('');
    $('#FaixasPesoPreco').val('');
}

function faixaPesoComErros(data) {
    if (data.statusText) {
        toastr.error(data.statusText, 'Faixas Peso');
    } else {
        toastr.error('Falha ao incluir o registro', 'Faixas Peso');
    }
}

function excluirFaixaPeso(id) {

    if (isNumero(id)) {

        $.post(urlBase + 'FaixasPeso/Excluir/' + id, function (resultado) {
            toastr.success('Faixa excluída com sucesso!', 'Faixas Peso');
            $("#ListaFaixasPeso").html(resultado);
        }).fail(function (data) {
            if (data.statusText) {
                toastr.error(data.statusText, 'CRM');
            } else {
                toastr.error('Falha ao excluir a Faixa', 'Faixas Peso');
            }
        });
    }
}

$('#btnCadastrarFaixasCIF').click(function () {

    var id = $("#Id").val();

    if (isNumero(id)) {

        $.get(urlBase + 'FaixasCIF/Consultar/?layoutId=' + id, function (resultado) {

            $("#ListaFaixasCIF").html(resultado);

            $('#FaixaCIFLayoutId').val(id);
            $('#faixas-cif-modal').modal('show');
        });
    }
});

function faixaCIFCadastrada() {

    toastr.success('Faixa cadastrada com sucesso!', 'Faixas CIF');

    $('#FaixasCIFMinimo').val('');
    $('#FaixasCIFMaximo').val('');
    $('#FaixasCIFMargem').val('');
    $('#FaixasCIFValor20').val('');
    $('#FaixasCIFValor40').val('');
    $('#FaixasCIFDescricao').val('');
}

function faixaCIFComErros(data) {
    if (data.statusText) {
        toastr.error(data.statusText, 'Faixas CIF');
    } else {
        toastr.error('Falha ao incluir o registro', 'Faixas CIF');
    }
}

function excluirFaixaCIF(id) {

    if (isNumero(id)) {

        $.post(urlBase + 'FaixasCIF/Excluir/' + id, function (resultado) {
            toastr.success('Faixa excluída com sucesso!', 'Faixas CIF');
            $("#ListaFaixasCIF").html(resultado);
        }).fail(function (data) {
            if (data.statusText) {
                toastr.error(data.statusText, 'Faixas CIF');
            } else {
                toastr.error('Falha ao excluir a Faixa', 'Faixas CIF');
            }
        });
    }
}

$('#btnCadastrarFaixasBL').click(function () {

    var id = $("#Id").val();

    if (isNumero(id)) {

        $.get(urlBase + 'FaixasBL/Consultar/?layoutId=' + id, function (resultado) {

            $("#ListaFaixasBL").html(resultado);

            $('#FaixaBLLayoutId').val(id);
            $('#faixas-bl-modal').modal('show');
        });
    }
});

function faixaBLCadastrada() {

    toastr.success('Faixa cadastrada com sucesso!', 'Faixas BL');

    $('#FaixasBLMinimo').val('');
    $('#FaixasBLMaximo').val('');
    $('#FaixasBLValorMinimo').val('');
}

function faixaBLComErros(data) {
    if (data.statusText) {
        toastr.error(data.statusText, 'Faixas BL');
    } else {
        toastr.error('Falha ao incluir o registro', 'Faixas BL');
    }
}

function excluirFaixaBL(id) {

    if (isNumero(id)) {

        $.post(urlBase + 'FaixasBL/Excluir/' + id, function (resultado) {
            toastr.success('Faixa excluída com sucesso!', 'Faixas BL');
            $("#ListaFaixasBL").html(resultado);
        }).fail(function (data) {
            if (data.statusText) {
                toastr.error(data.statusText, 'Faixas BL');
            } else {
                toastr.error('Falha ao excluir a Faixa', 'Faixas BL');
            }
        });
    }
}

$('#btnCadastrarFaixasVolume').click(function () {

    var id = $("#Id").val();

    if (isNumero(id)) {

        $.get(urlBase + 'FaixasVolume/Consultar/?layoutId=' + id, function (resultado) {

            $("#ListaFaixasVolume").html(resultado);

            $('#FaixaVolumeLayoutId').val(id);
            $('#faixas-volume-modal').modal('show');
        });
    }
});

function faixaVolumeCadastrada() {

    toastr.success('Faixa cadastrada com sucesso!', 'Faixas Volume');

    $('#FaixasVolumeValorInicial').val('');
    $('#FaixasVolumeValorFinal').val('');
    $('#FaixasVolumePreco').val('');
}

function faixaVolumeComErros(data) {
    if (data.statusText) {
        toastr.error(data.statusText, 'Faixas Volume');
    } else {
        toastr.error('Falha ao incluir o registro', 'Faixas Volume');
    }
}

function excluirFaixaVolume(id) {

    if (isNumero(id)) {

        $.post(urlBase + 'FaixasVolume/Excluir/' + id, function (resultado) {
            toastr.success('Faixa excluída com sucesso!', 'Faixas Volume');
            $("#ListaFaixasVolume").html(resultado);
        }).fail(function (data) {
            if (data.statusText) {
                toastr.error(data.statusText, 'Faixas Volume');
            } else {
                toastr.error('Falha ao excluir a Faixa', 'Faixas Volume');
            }
        });
    }
}
