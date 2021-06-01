$('#abas a').on('click', function(e) {
    e.preventDefault();
    $(this).tab('show');
    corrigirSelects();
});

$("#btnPesquisarContas").click(function() {

    event.preventDefault();

    $('#pesquisa-modal-contas')
        .data('toggle', 'ContaId')
        .modal('show');
});

function selecionarConta(id, descricao, vendedorId) {

    alert('Estou aqui sebastiao foi no default');

    var toggle = $('#pesquisa-modal-contas').data('toggle');

    var subCliente = false;

    if (toggle !== 'ContaId') {
        subCliente = true;
    }

    if (isNumero(vendedorId)) {
        $('#VendedorId').val(vendedorId);
    }

    $.get(urlBase + 'Oportunidades/ValidarEquipeConta?contaId=' + id + '&subCliente=' + subCliente, function(resultado) {

        $('#pesquisa-modal-contas').modal('hide');

        $("#" + toggle)
            .empty()
            .append($('<option>', {
                value: id,
                text: descricao
            })).focus();

        $("#ListaContas").empty();

    }).fail(function(data) {
        if (data.statusText) {
            toastr.error(data.statusText, 'CRM');
        } else {
            toastr.error('Falha ao selecionar a Conta', 'CRM');
        }
    });
}

$("#ContaId").focusin(function() {
    consultarContatos($(this).val());
});

function consultarContatos(contaId) {

    if (isNumero(contaId)) {
        $.get(urlBase + 'Contatos/ObterContatosPorConta?contaId=' + contaId, function(resultado) {

            var select = $('#ContatoId');

            select.empty();

            select.append('<option value=""></option>');

            $.each(resultado, function(key, value) {
                select.append('<option value=' + value.Id + '>' + value.NomeCompleto + '</option>');
            });
        });
    }
}

$("#btnPesquisarOportunidades").click(function() {
    event.preventDefault();
    $('#pesquisa-modal-oportunidades').modal('show');
});

function selecionarOportunidade(id, descricao, proposta) {

    $('#pesquisa-modal-oportunidades').modal('hide');

    $('#RevisaoId')
        .empty()
        .append($('<option>', {
            value: id,
            text: descricao
        }));

    $('#lnkRevisaoProposta')
        .html(proposta);

    $("#ListaOportunidades").empty();
}

$("#SucessoNegociacao").change(function() {

    var tipo = $(this).val();

    $("#Probabilidade").val('0');
    $("#EstagioNegociacao").val('0');

    $("#MotivoPerda").val('0');
    $("#MotivoPerda").attr('disabled', 'disabled');

    switch (tipo) {
        case '1':
            $("#Probabilidade").val('20');
            $("#EstagioNegociacao").val('1');
            break;
        case '2':
            $("#Probabilidade").val('50');
            $("#EstagioNegociacao").val('2');
            break;
        case '3':
            $("#Probabilidade").val('90');
            $("#EstagioNegociacao").val('3');
            break;
        case '4':

            toastr.error('Não é permitido escolher Sucesso de Negociação Ganho para atualização manual de Oportunidade', 'CRM');

            $("#Probabilidade").val('100');
            $("#EstagioNegociacao").val('4');

            $(this).val(0);

            break;
        case '5':
            $("#Probabilidade").val('0');
            $("#EstagioNegociacao").val('5');
            $("#MotivoPerda").removeAttr('disabled');
            break;
        default:
    }
});

oportunidadeMensagemSucesso = function(resultado) {

    toastr.success('Informações atualizadas com sucesso!', 'CRM');

    setTimeout(function () {
        if (resultado) {
            if (resultado.hash) {

                window.location.hash = "#" + resultado.hash;
                location.reload();

                return;
            }

            if (resultado.RedirectUrl)
                document.location.href = decodeURIComponent(resultado.RedirectUrl);
        }
    }, 1000);
}

var oportunidadeCadastrada = function(data) {

    toastr.success('Oportunidade cadastrada com sucesso!', 'CRM');

    if (data) {
        document.location.href = decodeURIComponent(data.RedirectUrl);
    }
}

var oportunidadeMensagemErro = function(xhr, status) {

    if (xhr !== null && xhr.responseText !== '') {

        toastr.error('Falha ao atualizar a Oportunidade. Verifique mensagens.', 'CRM');

        var msg = $('#msgErro');

        msg.html('');
        msg.removeClass('invisivel');

        var resultado = JSON.parse(xhr.responseText);

        var mensagens = resultado.erros.map(function (erro) {
            return '<li>' + erro.ErrorMessage + '</li>';
        });

        msg.html(mensagens);

        $('#btnAtualizarOportunidade')
            .html('<i class="fa fa-save"></i> Salvar')
            .removeClass('disabled');

        setTimeout(function() {
            msg.addClass('invisivel');
        }, 9000);
    } else {
        toastr.error(xhr.statusText, 'CRM');
    }
}

$("#btnClonarOportunidade").click(function() {

    $('#pnlOportunidadeSubClientes').addClass('invisivel');

    $(document).ready(function() {
        $('#tbClonarOportunidade').DataTable({
            destroy: true,
            "bLengthChange": false,
            "bServerSide": true,
            "sAjaxSource": urlBase + "Oportunidades/Consultar",
            "bProcessing": true,
            "pagingType": "full_numbers",
            "searchDelay": 1000,
            "pageLength": 10,
            "aoColumns":
                [
                    {
                        "sName": "Identificacao",
                        "mData": "Identificacao"
                    },
                    {
                        "sName": "Descricao",
                        "render": function(data, type, row) {
                            if (row.Descricao) {
                                return row.Descricao.length > 40 ?
                                    row.Descricao.substr(0, 40) + '…' :
                                    row.Descricao;
                            }

                        },
                        "defaultContent": ""
                    },
                    {
                        "sName": "StatusOportunidade",
                        "mData": "StatusOportunidade"
                    },
                    {
                        "sName": "SucessoNegociacao",
                        "mData": "SucessoNegociacao"
                    },
                    {
                        "sName": "TipoServico",
                        "mData": "TipoServico"
                    },
                    {
                        "sName": "TabelaId",
                        "mData": "TabelaId"
                    },
                    {
                        "data": "DataInicio",
                        "mData": "DataInicio"
                    },
                    {
                        "data": "DataTermino",
                        "mData": "DataTermino"
                    },
                    {
                        "render": function(data, type, row) {
                            return '<a href="#" onclick="selecionarOportunidadeClone(' + row.Id + ')"><i class="far fa-check-circle"></i></a>';
                        },
                        "className": "text-center"
                    },
                    {
                        "render": function(data, type, row) {
                            return '<a href="/Oportunidades/Atualizar/' + row.Id + '" target="_blank"><i class="fa fa-search"></i></a>';
                        },
                        "className": "text-center"
                    }
                ],
            "language": {
                "url": urlBase + "Content/plugins/datatables/language/pt-br.json"
            }
        });
    });

    $('#clonar-oportunidade-modal').modal('show');

});

$("#SelTodosSubClientes").click(function() {
    if (this.checked) {
        $('.chk-subcliente-clone').each(function() {
            this.checked = true;
        });
    } else {
        $('.chk-subcliente-clone').each(function() {
            this.checked = false;
        });
    }
});

$("#SelTodosGruposCNPJ").click(function() {
    if (this.checked) {
        $('.chk-clienteGrupo-clone').each(function() {
            this.checked = true;
        });
    } else {
        $('.chk-clienteGrupo-clone').each(function() {
            this.checked = false;
        });
    }
});

function selecionarOportunidadeClone(id) {

    event.preventDefault();

    $('#CloneOportunidadeSelecionada').val(id);

    $('#tbClonarOportunidade > tbody  > tr').removeClass('linha-selecionada');

    $('#item-proposta-clone-' + id).addClass('linha-selecionada');

    $('#pnlOportunidadeSubClientes').removeClass('invisivel');
    $('#pnlOportunidadeGruposCNPJ').removeClass('invisivel');
    $('#pnlOportunidadeContaPrincipal').removeClass('invisivel');

    $('#tbClonarOportunidadeSubClientes').DataTable({
        destroy: true,
        "bInfo": false,
        "bLengthChange": false,
        "bFilter": false,
        "bPaginate": false,
        "bSort": true,
        "ajax": {
            "url": urlBase + "Oportunidades/ConsultarSubClientesJson/?oportunidadeId=" + id,
            "dataSrc": "dados"
        },
        "createdRow": function(row, data, dataIndex) {
            $(row).attr('id', 'item-subCliente-clone-' + data.Id);
        },
        "columns": [
            {
                "render": function(data, type, full, meta) {
                    return '<input type="checkbox" name="CloneSubClientesSelecionados" id="CloneSubClientesSelecionados" class="chk-subcliente-clone" value="' + full.Id + '" checked />';
                },
                "width": "20px"
            },
            { "data": "Id" },
            { "data": "Descricao" }
        ],
        "language": {
            "url": urlBase + "Content/plugins/datatables/language/pt-br.json"
        }
    });

    $('#tbClonarOportunidadeGruposCNPJ').DataTable({
        destroy: true,
        "bInfo": false,
        "bLengthChange": false,
        "bFilter": false,
        "bPaginate": false,
        "bSort": true,
        "ajax": {
            "url": urlBase + "Oportunidades/ConsultarClientesGrupoCNPJJson/?oportunidadeId=" + id,
            "dataSrc": "dados"
        },
        "createdRow": function(row, data, dataIndex) {
            $(row).attr('id', 'item-grupoCnpj-clone-' + data.Id);
        },
        "columns": [
            {
                "render": function(data, type, full, meta) {
                    return '<input type="checkbox" name="CloneGruposCNPJSelecionados" id="CloneGruposCNPJSelecionados" class="chk-clienteGrupo-clone" value="' + full.Id + '" checked />';
                },
                "width": "20px"
            },
            { "data": "Id" },
            { "data": "Descricao" }
        ],
        "language": {
            "url": urlBase + "Content/plugins/datatables/language/pt-br.json"
        }
    });

    $('#tbClonarOportunidadeConta').DataTable({
        destroy: true,
        "bInfo": false,
        "bLengthChange": false,
        "bFilter": false,
        "bPaginate": false,
        "bSort": false,
        "ajax": {
            "url": urlBase + "Oportunidades/ConsultarContaDaOportunidadeJson/?oportunidadeId=" + id,
            "dataSrc": "dados",
        },
        "createdRow": function(row, data, dataIndex) {
            $(row).attr('id', 'item-conta-clone-' + data.Id);
        },
        "columns": [
            {
                "render": function(data, type, full, meta) {
                    return '<input type="checkbox" name="CloneContaOportunidadeSelecionada" id="CloneContaOportunidadeSelecionada" value="' + full.Id + '" />';
                },
                "width": "20px"
            },
            { "data": "Id" },
            { "data": "Descricao" }
        ],
        "language": {
            "url": urlBase + "Content/plugins/datatables/language/pt-br.json"
        }
    });

    $('#btnConfirmarClonagem').prop("disabled", false);
}

function oportunidadeClonadaMensagemSucesso(data) {

    if (data) {
        if (isNumero(data.novaOportunidade)) {

            toastr.success('Oportunidade clonada com sucesso!', 'CRM');

            $('#btnIrParaOportunidadeClonada').attr('href', urlBase + 'Oportunidades/Atualizar/' + data.novaOportunidade);

            $('#btnConfirmarClonagem').addClass('invisivel');
            $('#btnIrParaOportunidadeClonada').removeClass('invisivel');
        }
    }
}

$('#frmOportunidadesInformacoesIniciais').submit(function() {

    $('#btnSalvarOportunidade').prop('disabled', true);

    setTimeout(function() {
        $('#btnSalvarOportunidade').prop('disabled', false);
    }, 4000);    
});

$(document).ready(function() {

    var hash = window.location.hash;
    hash && $('[href="' + hash + '"]').tab('show');

    var form = document.getElementById("frmClonarOportunidade");

    if (form) {

        document.getElementById("frmClonarOportunidade").addEventListener("submit", function() {
            $('#btnConfirmarClonagem').prop("disabled", true);
        }, false);
    }

    $('#ClientePropostaSelecionadoId').select2();
});

function corrigirSelects() {

    $('#ClientePropostaSelecionadoId')
        .select2({ width: '100%' });
}