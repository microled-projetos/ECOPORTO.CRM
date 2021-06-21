window.addEventListener("DOMContentLoaded", function (event) {

    var id = $("#Id").val();

    if (parseInt(id) > 0) {

        consultarSubClientes();
        consultarClientesGrupoCNPJ();

        $('#tbAdendosExclusaoSubCliente').DataTable({
            "bInfo": false,
            "bLengthChange": false,
            "bFilter": false,
            "bPaginate": false,
            "bSort": true,
            "language": {
                "url": urlBase + "Content/plugins/datatables/language/pt-br.json"
            }
        });

        if (parseInt($('#SimuladorPropostaId').val()) === 0) {

            $('#SimuladorPropostaPeriodos').val('1');
            $('#SimuladorPropostaNumeroLotes').val('1');
            $('#SimuladorPropostaVolumeM3').val('5,00');
            $('#SimuladorPropostaPeso').val('5,00');
        }

        var campoLotes = $('#Lote');
        var campoLotesValores = $('#Lote').val();
        
        if (campoLotesValores != null) {
            var lotes = campoLotesValores.split(',') || [];

            campoLotes.tagsinput('removeAll');

            if (lotes.length > 0) {
                lotes.forEach(function (item) {
                    campoLotes.tagsinput('add', item);
                });
            }
        }        

        var campoBls = $('#BL');
        var campoBlsValores = $('#BL').val();

        if (campoBlsValores != null) {
            var bls = campoBlsValores.split(',') || [];

            campoBls.tagsinput('removeAll');

            if (bls.length > 0) {
                bls.forEach(function (item) {
                    campoBls.tagsinput('add', item);
                });
            }
        }       

        $('#txtLotesInformados').tagsinput('items');

        var campoLotesProposta = $('.bootstrap-tagsinput input');
        campoLotesProposta.prop('readonly', false);

        $('#txtLotesInformados').on('beforeItemAdd', function (event) {
            var tag = event.item;

            if (!event.options || !event.options.preventPost) {
                $.get(urlBase + 'Oportunidades/ValidarLoteProposta?lote=' + tag, function (response) {
                    $('#btnSelecionarLoteProposta').prop('disabled', false);
                }).fail(function (data) {

                    $('#txtLotesInformados').tagsinput('remove', tag, { preventPost: true });

                    if (data.statusText) {
                        toastr.error(data.statusText, 'CRM');
                    } else {
                        toastr.error('Falha ao incluir o Lote', 'CRM');
                    }
                });
            }
        });
      
        $('#Lote').on('beforeItemRemove', function (event) {
            var tag = event.item;

            $.get(urlBase + 'Oportunidades/ObterBLsPorId?lote=' + tag, function (bl) {
                console.log(bl);
                $('#BL').tagsinput('remove', bl);
            }).fail(function (data) {

                if (data.statusText) {
                    toastr.error(data.statusText, 'CRM');
                } else {
                    toastr.error('Falha ao incluir o Lote', 'CRM');
                }
            });
        });             
    }

    $('.controle-input-tags').prop('readonly', true);

    var imposto = $('#ImpostoId').val();
    console.log('imposto ', imposto);
    if (parseInt(imposto) !== 3) {
        $('#lnkImpostosExcecao').addClass('invisivel');
    } else {
        $('#lnkImpostosExcecao').removeClass('invisivel');
    }

});

$('#ImpostoId').change(function () {
    var imposto = $('#ImpostoId').val();
    if (parseInt(imposto) !== 3) {
        $('#lnkImpostosExcecao').addClass('invisivel');
    } else {
        $('#lnkImpostosExcecao').removeClass('invisivel');
    }
});

function selecionarLoteProposta() {

    var lotes = $('#txtLotesInformados').val();

    if (lotes === '') {

        toastr.error('Nenhum lote informado', 'CRM');
        return;
    }

    $.get(urlBase + 'Oportunidades/ObterLotesProposta?lotes=' + lotes, function (resultado) {

        if (resultado) {

            var lotesAntigos = $('#Lote').val();
            var blsAntigos = $('#BL').val();
            
            if (lotesAntigos !== '') {
                var lotesArr = lotesAntigos.split(',');
                if (lotesArr.length > 0) {
                    lotesArr.forEach(function (item) {
                        resultado.lotesLista.push(item);
                    });                    
                }                
            }

            //if (blsAntigos !== '') {
            //    resultado.blsLista.push(blsAntigos);
            //}
            if (blsAntigos !== '') {
                var blArr = blsAntigos.split(',');
                if (blArr.length > 0) {
                    blArr.forEach(function (item) {
                        resultado.blsLista.push(item);
                    });
                }
            }

            var campoLotes = $('#Lote');
            campoLotes.tagsinput('removeAll');

            resultado.lotesLista.forEach(function (item) {
                campoLotes.tagsinput('add', item);
            });

            var campoBls = $('#BL');
            campoBls.tagsinput('removeAll');

            resultado.blsLista.forEach(function (item) {
                campoBls.tagsinput('add', item);
            });

            $('#btnSelecionarLoteProposta').prop('disabled', true);

            $('#pesquisa-lotes-proposta').modal('hide');
        }
    });
}

$('#pesquisa-lotes-proposta').on('hidden.bs.modal', function () {
    $('.controle-input-tags').prop('readonly', true);
});

$('#EntregaEletronica').change(function () {
    if (this.checked) {
        $('#EmailFaturamento').prop('readonly', false);

        if ($('#EntregaManual').is(':checked')) {
            $('#EntregaManual').prop('checked', false);
        }
    } else {
        $('#EmailFaturamento').val('');
        $('#EmailFaturamento').prop('readonly', true);
    }
});

$('#EntregaManual').change(function () {
    if (this.checked) {

        if ($('#EntregaEletronica').is(':checked')) {
            $('#EntregaEletronica').prop('checked', false);
            $('#EmailFaturamento').prop('readonly', true);
        }
    }
});

$('#frmOportunidadesInformacoesIniciais').submit(function () {

    $('#btnAtualizarOportunidade')
        .html('<i class="fa fa-spinner fa-spin"></i> aguarde...')
        .addClass('disabled');
});

$("#btnPesquisarContatos").click(function () {

    event.preventDefault();

    $('#pesquisa-modal-contatos')
        .data('toggle', 'PremioParceriaContatoId')
        .modal('show');
});

function selecionarContato(id, descricao) {

    event.preventDefault();

    var toggle = $('#pesquisa-modal-contatos').data('toggle');

    $('#pesquisa-modal-contatos').modal('hide');

    $("#" + toggle)
        .empty()
        .append($('<option>', {
            value: id,
            text: descricao
        })).focus();

    $("#ListaContatos").empty();
}

$("#btnPesquisarSubCliente").click(function () {

    event.preventDefault();

    $('#pesquisa-modal-contas')
        .data('toggle', 'SubClienteContaId')
        .modal('show');
});

$("#btnPesquisarClienteGrupoCNPJ").click(function () {

    event.preventDefault();

    $('#pesquisa-modal-contas')
        .data('toggle', 'GrupoCNPJContaId')
        .modal('show');
});

$("#btnPesquisarClienteFaturadoContra").click(function () {

    event.preventDefault();

    $('#pesquisa-modal-contas')
        .data('toggle', 'FaturadoContraId')
        .modal('show');
});

$("#btnPesquisarClienteFontePagadora").click(function () {

    event.preventDefault();

    $('#pesquisa-modal-contas')
        .data('toggle', 'FontePagadoraId')
        .modal('show');
});

window.addEventListener('focus', function (event) {

    var ref_this = $('.nav-tabs .active');

    var abaId = ref_this.attr('id');

    if (abaId === 'proposta-tab') {
        atualizarPreview();
    }
});

$("#btnPesquisarPremioReferencia").click(function () {

    event.preventDefault();

    $('#pesquisa-modal-premios')
        .data('toggle', 'PremioReferenciaId')
        .modal('show');
});

function selecionarPremioReferencia(id, descricao) {

    var toggle = $('#pesquisa-modal-premios').data('toggle');

    $('#pesquisa-modal-premios').modal('hide');

    $("#" + toggle)
        .empty()
        .append($('<option>', {
            value: id,
            text: descricao
        })).focus();

    $.get(urlBase + 'Oportunidades/ObterDetalhesPremioParceria/' + id, function (resultado) {

        if (!isNumero($('#Favorecido1').val())) {
            $("#Favorecido1")
                .empty()
                .append($('<option>', {
                    value: resultado.Favorecido1,
                    text: Strings.orEmpty(resultado.DescricaoFavorecido1) + ' ' + Strings.orEmpty(resultado.DocumentoFavorecido1)
                }));
        }

        if (!isNumero($('#Favorecido2').val())) {
            $("#Favorecido2")
                .empty()
                .append($('<option>', {
                    value: resultado.Favorecido2,
                    text: Strings.orEmpty(resultado.DescricaoFavorecido2) + ' ' + Strings.orEmpty(resultado.DocumentoFavorecido2)
                }));
        }

        if (!isNumero($('#Favorecido3').val())) {
            $("#Favorecido3")
                .empty()
                .append($('<option>', {
                    value: resultado.Favorecido3,
                    text: Strings.orEmpty(resultado.DescricaoFavorecido3) + ' ' + Strings.orEmpty(resultado.DocumentoFavorecido3)
                }));
        }

        if (!isNumero($('#PremioParceriaContatoId').val())) {
            $("#PremioParceriaContatoId")
                .empty()
                .append($('<option>', {
                    value: resultado.ContatoId, text: resultado.DescricaoContato
                }));
        }

        if (!isNumero($('#TipoServicoPremioParceria').val())) {

            $("#TipoServicoPremioParceria")
                .val(resultado.TipoServicoPremioParceria);

            consultarModalidades(resultado.TipoServicoPremioParceria);

            var $selectModalidadesSelecionadas = $("#ModalidadesSelecionadas");
            $selectModalidadesSelecionadas.empty();

            resultado.ModalidadesSelecionadas.forEach(function (item) {
                $selectModalidadesSelecionadas
                    .append($('<option>', {
                        value: item.Id, text: item.Descricao
                    }));
            });
        }
    });

    $("#ListaPremios").empty();
}

$("#btnPesquisarParametrosSimulador").click(function () {

    event.preventDefault();

    $('#pesquisa-modal-simulador')
        .data('toggle', 'ParametroSimuladorId')
        .modal('show');
});

function selecionarParametroSimulador(id, descricao) {

    event.preventDefault();

    var toggle = $('#pesquisa-modal-simulador').data('toggle');

    $('#pesquisa-modal-simulador').modal('hide');

    $("#" + toggle)
        .empty()
        .append($('<option>', {
            value: id,
            text: descricao
        })).focus();

    $("#ListaParametros").empty();
}

// ===============

var confirmaMudancaModelo = function (callback) {

    $("#btn-confirm").on("click", function () {
        $("#confirm-modal").modal('show');
    });

    $("#modal-btn-sim").on("click", function () {
        callback(true);
        $("#confirm-modal").modal('hide');
    });

    $("#modal-btn-nao").on("click", function () {
        callback(false);
        $("#confirm-modal").modal('hide');
    });
};

confirmaMudancaModelo(function (confirmou) {
    if (confirmou) {
        obterInformacoesModelo();
    }
});

// ==========

$('#ModeloId').click(function () {

    $(this).data('val', $(this).val());
});

$('#ModeloId').change(function () {

    var id = $('#Id').val();
    var modelo = $(this).val();

    var valorAnterior = $(this).data('val');

    $.get(urlBase + 'Oportunidades/ExisteModeloNaOportunidade/' + id, function (resultado) {

        if (parseInt(resultado) === 0) {
            obterInformacoesModelo();
        } else {
            if (valorAnterior !== '') {
                if (modelo != valorAnterior) {

                    var modalConfirm = $('#confirm-modal');
                    modalConfirm.find('#modal-mensagem').text('Atenção: Já existe um modelo salvo na proposta. Ao selecionar um Modelo diferente, todas as alterações feitas no modelo anterior serão perdidas. Confirma a alteração?');
                    modalConfirm.modal('show');
                }
            }
        }
    });
});

function obterInformacoesModelo() {

    var id = $('#Id').val();
    var modelo = $('#ModeloId').val();

    if (isNumero(modelo)) {

        $.get(urlBase + 'Modelos/ObterModeloPorId?id=' + modelo, function (resultado) {

            if (resultado) {

                $('#QtdeDias').empty().val(resultado.QtdeDias);
                $('#DiasFreeTime').empty().val(resultado.DiasFreeTime);
                $('#Validade').empty().val(resultado.Validade);
                $('#TipoValidade').val(resultado.TipoValidade);
                $('#ImpostoId').val(resultado.ImpostoId);
                $('#ParametroLote').val(resultado.ParametroLote);
                $('#ParametroBL').val(resultado.ParametroBL);
                $('#ParametroConteiner').val(resultado.ParametroConteiner);
                $('#ParametroIdTabela').val(resultado.ParametroIdTabela);
                $('#DesovaParcial').empty().val(formataMoedaPtBr(resultado.DesovaParcial));
                $('#FatorCP').empty().val(formataMoedaPtBr(resultado.FatorCP));
                $('#PosicIsento').empty().val(resultado.PosicIsento);
                $('#Acordo').prop('checked', resultado.Acordo);
                $('#HubPort').prop('checked', resultado.HubPort);
                $('#CobrancaEspecial').prop('checked', resultado.CobrancaEspecial);

                $('#Lote').empty();
                $('#BL').empty();
                $('#TabelaReferencia').val('');

                $('#loteProposta').addClass('invisivel');
                $('#blProposta').addClass('invisivel');
                $('#conteinerProposta').addClass('invisivel');
                $('#tabelaReferenciaProposta').addClass('invisivel');

                if (resultado.ParametroLote) {
                    $('#loteProposta').removeClass('invisivel');
                }

                if (resultado.ParametroBL) {
                    $('#blProposta').removeClass('invisivel');
                }

                if (resultado.ParametroConteiner) {
                    $('#conteinerProposta').removeClass('invisivel');
                }

                if (resultado.ParametroIdTabela) {
                    $('#tabelaReferenciaProposta').removeClass('invisivel');
                }

                $.get(urlBase + 'Oportunidades/ExisteModeloNaOportunidade/' + id, function (dados) {

                    if (parseInt(dados) === 0) {

                        if (parseInt($('#VendedorId').val()) === 0) {
                            $('#VendedorId').val(resultado.VendedorId);
                        }

                        $('#FormaPagamento').val(resultado.FormaPagamento);
                    }
                });
            }
        });
    }
}

$("#TipoOperacao").change(function () {

    var tipo = $(this).val();

    $('#ModeloId').html('');

    if (isNumero(tipo)) {
        $.get(urlBase + 'Modelos/ObterModelosPorTipoOperacao?tipo=' + tipo, function (resultado) {

            var select = $('#ModeloId');

            select.append('<option value="">Escolha o Modelo</option>');

            $.each(resultado, function (key, value) {
                select.append('<option value=' + value.Id + '>' + value.Descricao + '</option>');
            });
        });
    }
});

function atualizarPreview() {

    var id = $("#Id").val();

    if (!isNumero(id)) {
        toastr.error('Oportunidade não informada', 'CRM');
        return;
    }

    $.get(urlBase + 'Oportunidades/MontaPreview/' + id, function (data) {
        $('#preview').html(data);
    }).done(function () {

        var statusOportunidade = parseInt($('#StatusOportunidade').val());

        if (statusOportunidade === 0 || statusOportunidade === 4) {

            $('.tbPreview tr').dblclick(function () {
                var id = $(this).data('id');
                atualizarLinhaLayout(id);
            });

            $('.LinhaCondicoesIniciais').dblclick(function () {
                var id = $(this).data('id');
                atualizarLinhaLayout(id);
            });

            $('.LinhaCondicoesGerais').dblclick(function () {
                var id = $(this).data('id');
                atualizarLinhaLayout(id);
            });
        }
    });
}

$("#btnImportarLayout").click(function () {

    var id = $("#Id").val();
    var modeloId = $("#ModeloId").val();

    if (!isNumero(id)) {
        toastr.error('Oportunidade não informada', 'CRM');
        return;
    }

    if (!isNumero(modeloId)) {
        toastr.error('Selecione o Modelo', 'CRM');
        return;
    }

    if (isNumero(id)) {
        $.post(urlBase + 'Oportunidades/ImportarModeloLayout/', { id: id, modeloId: modeloId }, function () {
            toastr.success('Layout importado com sucesso', 'CRM');
            atualizarPreview();
        }).fail(function (data) {
            if (data.statusText) {
                toastr.error(data.statusText, 'CRM');
            } else {
                toastr.error('Falha ao importar o layout', 'CRM');
            }
        });
    }
});

$("#btnSalvarDataCancelamento").click(function () {

    var id = $("#Id").val();
    var dataCancelamento = $("#OportunidadesInformacoesIniciaisViewModel_DataCancelamentoOportunidade").val();

    if (!isNumero(id)) {
        toastr.error('Oportunidade não informada', 'CRM');
        return;
    }

    if (dataCancelamento === '') {
        toastr.error('Informe a Data de Cancelamento', 'CRM');
        return;
    }

    if (isNumero(id)) {
        $.post(urlBase + 'Oportunidades/AtualizarDataCancelamento/', { id: id, dataCancelamento: dataCancelamento }, function () {
            toastr.success('Data de Cancelamento atualizada com sucesso!', 'CRM');
        }).fail(function (data) {
            if (data.statusText) {
                toastr.error(data.statusText, 'CRM');
            } else {
                toastr.error('Falha ao atualizar a Data de Cancelamento', 'CRM');
            }
        });
    }
});

$('#FormaPagamento').change(function () {

    var formaPagamento = $(this).val();

    $('#QtdeDias').removeAttr('disabled');

    if (formaPagamento == 1) {
        $('#QtdeDias')
            .val('0')
            .attr('disabled', 'disabled');
    }
});

function atualizarLinhaLayout(id) {

    var a = document.createElement('a');
    var oportunidadeId = $('#Id').val();

    a.target = "_blank";
    a.href = urlBase + 'Layouts/Atualizar/' + id + '?proposta=true&oportunidadeId=' + oportunidadeId;
    a.click();
}

$("#btnAdicionarSubCliente").click(function () {

    event.preventDefault();

    var oportunidadeId = $("#Id").val();
    var segmento = $("#SubClienteSegmento").val();
    var contaId = $("#SubClienteContaId").val();
    var contaDescricao = $("#SubClienteContaId option:selected").text();

    if (!isNumero(oportunidadeId)) {
        toastr.error('Oportunidade não informada', 'CRM');
        return;
    }

    if (!isNumero(segmento)) {
        toastr.error('Selecione um Segmento na lista', 'CRM');
        return;
    }

    if (!isNumero(contaId)) {
        toastr.error('Selecione uma Conta na lista', 'CRM');
        return;
    }

    $("#btnAdicionarSubCliente")
        .html('<i class="fa fa-spinner fa-spin"></i> aguarde...')
        .addClass('disabled');

    $.post(urlBase + 'Oportunidades/IncluirSubCliente/', { segmento: segmento, contaId: contaId, oportunidadeId: oportunidadeId }, function () {

        toastr.success('Sub Cliente incluído com sucesso', 'CRM');

        consultarSubClientes();

        $("#btnAdicionarSubCliente")
            .html('<i class="fa fa-edit"></i> Adicionar')
            .removeClass('disabled');

        $("#SubClienteContaId").empty();

    }).fail(function (data) {
        if (data.statusText) {
            toastr.error(data.statusText, 'CRM');
        } else {
            toastr.error('Falha ao incluir o Sub Cliente', 'CRM');
        }
    });
});

function excluirSubCliente(id) {

    event.preventDefault();

    if (isNumero(id)) {

        $.post(urlBase + 'Oportunidades/ExcluirSubCliente/', { id: id }, function () {
            toastr.success('Sub Cliente excluído com sucesso', 'CRM');
            consultarSubClientes();
        }).fail(function (data) {
            if (data.statusText) {
                toastr.error(data.statusText, 'CRM');
            } else {
                toastr.error('Falha ao excluir o Sub Cliente', 'CRM');
            }
        });
    }
}

function consultarSubClientes() {

    var id = $("#Id").val();

    if (!isNumero(id)) {
        toastr.error('Oportunidade não informada', 'CRM');
        return;
    }

    $('#tbSubClientes').DataTable({
        destroy: true,
        "pageLength": 10,
        "bAutoWidth": false,
        "bLengthChange": false,
        "ajax": {
            "url": urlBase + "Oportunidades/ConsultarSubClientesJson?oportunidadeId=" + id,
            "dataSrc": "dados",
        },
        "createdRow": function (row, data, dataIndex) {
            $(row).attr('id', 'item-subcliente-' + data.Id);
        },
        "columns": [
            { "data": "Descricao" },
            { "data": "Documento" },
            { "data": "Segmento" },
            { "data": "CriadoPor" },
            { "data": "DataCriacao" },
            {
                "data": "excluir",
                sortable: false,
                "render": function (data, type, row) {
                    if (toBoolean(podeExcluirSubClientes)) {
                        return '<a href="#" onclick="excluirSubCliente(' + row.Id + ')" class="btn btn-danger btn-sm btn-acao"><i class="fa fa-trash"></i>&nbsp;Excluir</a>';
                    } else {
                        return '';
                    }
                }
            }
        ],
        "language": {
            "url": urlBase + "Content/plugins/datatables/language/pt-br.json"
        }
    });
}

$("#btnAdicionarClienteGrupoCNPJ").click(function () {

    var oportunidadeId = $("#Id").val();
    var contaId = $("#GrupoCNPJContaId").val();

    if (!isNumero(oportunidadeId)) {
        toastr.error('Oportunidade não informada', 'CRM');
        return;
    }

    if (!isNumero(contaId)) {
        toastr.error('Selecione uma Conta na lista', 'CRM');
        return;
    }

    $(this).html('<i class="fa fa-spinner fa-spin"></i> aguarde...')
        .addClass('disabled');

    $.post(urlBase + 'Oportunidades/IncluirClienteGrupoCNPJ/', { contaId: contaId, oportunidadeId: oportunidadeId }, function () {

        toastr.success('Cliente incluído com sucesso', 'CRM');

        $("#GrupoCNPJContaId").empty();

        consultarClientesGrupoCNPJ();

        $("#btnAdicionarClienteGrupoCNPJ").removeClass('disabled')
            .html('<i class="fa fa-edit"></i> Adicionar');

    }).fail(function (data) {
        if (data.statusText) {
            toastr.error(data.statusText, 'CRM');
        } else {
            toastr.error('Falha ao incluir o Sub Cliente', 'CRM');
        }
    });
});

function excluirClienteGrupoCNPJ(id) {

    if (isNumero(id)) {

        $.post(urlBase + 'Oportunidades/ExcluirClienteGrupoCNPJ/', { id: id }, function () {
            toastr.success('Sub Cliente excluído com sucesso', 'CRM');
            consultarClientesGrupoCNPJ();
        }).fail(function (data) {
            if (data.statusText) {
                toastr.error(data.statusText, 'CRM');
            } else {
                toastr.error('Falha ao excluir o Sub Cliente', 'CRM');
            }
        });
    }
}

function consultarClientesGrupoCNPJ() {

    var id = $("#Id").val();

    if (!isNumero(id)) {
        toastr.error('Oportunidade não informada', 'CRM');
        return;
    }

    $('#tbClientesGrupoCNPJ').DataTable({
        destroy: true,
        "pageLength": 10,
        "bAutoWidth": false,
        "bLengthChange": false,
        "ajax": {
            "url": urlBase + "Oportunidades/ConsultarClientesGrupoCNPJJson?oportunidadeId=" + id,
            "dataSrc": "dados",
        },
        "createdRow": function (row, data, dataIndex) {
            $(row).attr('id', 'item-subcliente-' + data.Id);
        },
        "columns": [
            { "data": "Descricao" },
            { "data": "Documento" },
            { "data": "CriadoPor" },
            { "data": "DataCriacao" },
            {
                "data": "excluir",
                sortable: false,
                "render": function (data, type, row) {
                    if (toBoolean(podeExcluirClientesGrupoCnpj)) {
                        return '<a href="#" onclick="excluirClienteGrupoCNPJ(' + row.Id + ')" class="btn btn-danger btn-sm btn-acao"><i class="fa fa-trash"></i>&nbsp;Excluir</a>';
                    } else {
                        return '';
                    }
                }
            }
        ],
        "language": {
            "url": urlBase + "Content/plugins/datatables/language/pt-br.json"
        }
    });
}

function abrirModalDiasSemana() {
    event.preventDefault();
    $('#pesquisa-modal-diassemana').modal('show');
}

function selecionarDiaSemana() {

    var select = $("#cbDiaSemana option:selected");

    var diaSemana = select.val();
    var diaSemanaTexto = select.text();

    var campo = $('#DiasSemana');

    if (isNumero(diaSemana)) {

        campo.tagsinput({
            itemValue: 'id',
            itemText: 'text',
        });

        campo.tagsinput('add', { id: diaSemana, text: diaSemanaTexto });
    }
}

function abrirModalDias() {
    event.preventDefault();
    $('#pesquisa-modal-dias').modal('show');
}

function selecionarDia() {

    var select = $("#cbDia option:selected");

    var dia = select.val();
    var diaTexto = select.text();

    var campo = $('#DiasFaturamento');

    if (isNumero(dia)) {

        campo.tagsinput({
            itemValue: 'id',
            itemText: 'text',
        });

        campo.tagsinput('add', { id: dia, text: diaTexto });
    }
}

function abrirModalDiasCondicaoPgto() {
    event.preventDefault();
    $('#pesquisa-modal-dias-cond-pgto').modal('show');
}

function selecionarDiaCondicaoPgto() {

    var select = $("#cbDiaCondicaoPgto option:selected");

    var dia = select.val();
    var diaTexto = select.text();

    var campo = $('#CondicaoPagamentoPorDia');

    if (isNumero(dia)) {

        campo.tagsinput({
            itemValue: 'id',
            itemText: 'text',
        });

        campo.tagsinput('add', { id: dia, text: diaTexto });
    }
}

function abrirModalDiasSemanaCondicaoPgto() {
    event.preventDefault();
    $('#pesquisa-modal-dias-semana-cond-pgto').modal('show');
}

function selecionarDiaSemanaCondicaoPgto() {

    var select = $("#cbDiaSemanaCondicaoPgto option:selected");

    var dia = select.val();
    var diaTexto = select.text();

    var campo = $('#CondicaoPagamentoPorDiaSemana');

    if (isNumero(dia)) {

        campo.tagsinput({
            itemValue: 'id',
            itemText: 'text',
        });

        campo.tagsinput('add', { id: dia, text: diaTexto });
    }
}

$("#btnInfoProximo").click(function () {
    event.preventDefault();
    $('[href="#proposta"]').tab('show');
    atualizarPreview();
});

$("#btnPropostaAnterior").click(function () {
    event.preventDefault();
    $('[href="#informacoesIniciais"]').tab('show');
});

$("#btnPropostaProximo").click(function () {
    event.preventDefault();
    $('[href="#fichaDeFaturamento"]').tab('show');
});

$("#btnFichaAnterior").click(function () {
    event.preventDefault();
    $('[href="#proposta"]').tab('show');
    atualizarPreview();
});

$("#btnFichaProximo").click(function () {
    event.preventDefault();
    $('[href="#anexos"]').tab('show');
    consultarAnexos();
});

$("#btnAnexosAnterior").click(function () {
    event.preventDefault();
    $('[href="#fichaDeFaturamento"]').tab('show');
});

$("#btnAnexosProximo").click(function () {
    event.preventDefault();
    obterPremioParceriaAtivo();
    $('[href="#premioDeParceria"]').tab('show');
});

$("#btnPremioParceriaAnterior").click(function () {
    event.preventDefault();
    $('[href="#anexos"]').tab('show');
    consultarAnexos();
});

$("#btnPremioParceriaProximo").click(function () {
    event.preventDefault();
    $('[href="#adendos"]').tab('show');
});

$('#anexos-tab').click(function () {
    consultarAnexos();
});

$('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {

    var aba = e.target.id;

    if (aba === 'proposta-tab') {
        atualizarPreview();
    }

    if (aba === 'premioDeParceria-tab') {
        obterPremioParceriaAtivo();
    }
})

function cancelarOportunidade(id) {

    event.preventDefault();

    $.get(urlBase + 'Oportunidades/ExisteCancelamento/' + $('#Id').val(), function (resultado) {

        if (resultado) {
            if (resultado.toLowerCase() === 'true') {

                $("#btnGerarPDFCancelamento").removeClass('invisivel');
                $("#btnConfirmarCancelamentoOportunidade").addClass('invisivel');
                $('#cancelar-oportunidade-modal').data('id', id).modal('show');

                return;
            }
        }

    });

    $("#btnConfirmarCancelamentoOportunidade").removeClass('invisivel');
    $("#btnGerarPDFCancelamento").addClass('invisivel');
    $('#cancelar-oportunidade-modal').data('id', id).modal('show');
}

function confirmarCancelamentoOportunidade() {

    event.preventDefault();

    var dataCancelamento = $('#OportunidadesInformacoesIniciaisViewModel_DataCancelamentoOportunidade').val();

    $('#msgErroDataCancelamento').addClass('invisivel');

    if (dataCancelamento === '') {
        $('#msgErroDataCancelamento').removeClass('invisivel');
        return;
    }

    var id = $('#cancelar-oportunidade-modal').data('id');

    $('#btnConfirmarCancelamentoOportunidade')
        .html('<i class="fa fa-spinner fa-spin"></i> aguarde...')
        .addClass('disabled');

    if (isNumero(id)) {
        $.post(urlBase + 'Oportunidades/CancelarOportunidade/', {
            id: id, dataCancelamento: dataCancelamento
        }, function () {

            toastr.success('Oportunidade cancelada com sucesso', 'CRM');

            $("#btnConfirmarCancelamentoOportunidade").addClass('invisivel');
            $("#btnGerarPDFCancelamento").removeClass('invisivel');
            $('#StatusOportunidade').val(2);
            $('#btnEnviarOportunidadeParaAprovacao').prop('disabled', false);
            $('#btnRecallOportunidade').prop('disabled', true);
            $('#OportunidadesInformacoesIniciaisViewModel_PermiteAlterarDataCancelamento').val('true');

        }).fail(function (data) {
            if (data.statusText) {
                toastr.error(data.statusText, 'CRM');
            } else {
                toastr.error('Falha ao cancelar a oportunidade', 'CRM');
            }
        }).always(function () {
            $('#btnConfirmarCancelamentoOportunidade')
                .html('Estou ciente e confirmo o Cancelamento')
                .removeClass('disabled');
        });
    }
}

$('#btnGerarProposta').click(function () {

    event.preventDefault();

    var id = $('#Id').val();

    $(this).html('<i class="fa fa-spinner fa-spin"></i> aguarde...')
        .addClass('disabled');

    if (isNumero(id)) {

        $.get(urlBase + 'Oportunidades/ExisteConcomitancia?oportunidadeId=' + id, function (concomitancia) {

            if (concomitancia) {
                if (concomitancia.Existe) {

                    $("#msgPropostasConcomitantes").html(concomitancia.Mensagem);

                    if (concomitancia.Bloqueia) {

                        $('#imgIconeConcomitancia')
                            .attr('src', urlBase + 'content/img/icone-integracao-erro.png');
                    } else {

                        $('#imgIconeConcomitancia')
                            .attr('src', urlBase + 'content/img/icone-integracao.png');

                        $('#btnIrParaOportunidadeConcomitante')
                            .removeClass('invisivel')
                            .attr('href', concomitancia.RedirectUrl);
                    }

                    $('#btnAtualizarOportunidade')
                        .html('<i class="fa fa-save"></i>&nbsp;&nbsp;Salvar')
                        .removeClass('disabled');

                    $('#concomitancia-modal').modal('show');

                } else {

                    $.get(urlBase + 'Oportunidades/ExisteLayoutNaProposta?oportunidadeId=' + id, function (resultado) {
                        $('#pnlGeracaoProposta').removeClass('invisivel');
                        $('#pnlDownloadProposta').addClass('invisivel');
                        $("#pnlGeracaoPropostaFooter").removeClass('invisivel');
                        $('#confirmar-proposta-modal').modal('show');
                    }).fail(function (data) {
                        toastr.error(data.statusText, 'CRM');
                    }).always(function () {
                        $("#btnGerarProposta").removeClass('disabled')
                            .html('Gerar Proposta');
                    });
                }
            }

        }).fail(function (data) {
            toastr.error(data.statusText, 'CRM');
        }).always(function () {
            $("#btnGerarProposta").removeClass('disabled')
                .html('Gerar Proposta');
        });
    }
});

$('#btnConfirmarGeracaoProposta').click(function () {

    $('#confirmar-proposta-modal').modal('hide');
});

function gerarProposta() {

    var id = $('#Id').val();

    $('#btnConfirmaGerarProposta').html('<i class="fa fa-spinner fa-spin"></i> aguarde, sua proposta está sendo gerada...')
        .addClass('disabled');

    $.post(urlBase + 'Oportunidades/GerarProposta/', { id: id }, function (resultado) {

        $('#pnlGeracaoProposta').addClass('invisivel');
        $('#pnlDownloadProposta').removeClass('invisivel');
        $("#pnlGeracaoPropostaFooter").addClass('invisivel');

        $("#pnlDownloadProposta").html(resultado);

    }).fail(function (data) {
        if (data.statusText) {
            toastr.error(data.statusText, 'CRM');
        } else {
            toastr.error('Falha ao gerar a proposta', 'CRM');
        }
    }).always(function () {

        $('#btnConfirmaGerarProposta').html('Confirmo a Geração da Proposta')
            .removeClass('disabled');
    });
}

function excluirAnexo(id, idArquivo) {

    if (isNumero(id)) {

        $.post(urlBase + 'Oportunidades/ExcluirAnexo/', { id: id, idArquivo: idArquivo }, function () {
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

function excluirNota(id) {

    if (isNumero(id)) {

        $.post(urlBase + 'Oportunidades/ExcluirNota/', { id: id }, function () {
            toastr.success('Nota excluída com sucesso', 'CRM');
            $('#item-nota-' + id).remove();
        }).fail(function (data) {
            if (data.statusText) {
                toastr.error(data.statusText, 'CRM');
            } else {
                toastr.error('Falha ao excluir a Nota', 'CRM');
            }
        });
    }
}

$('#btnLimparDadosNota').click(function () {
    $('#frmOportunidadesNotas')[0].reset();
    $("#NotaId").val('0');
});

$('#btnPesquisarNotas').click(function () {

    var id = $('#Id').val();
    var descricao = $('#txtPesquisarNota').val();

    $.get(urlBase + 'Oportunidades/ObterNotasPorDescricao/?descricao=' + descricao + '&oportunidadeId=' + id, function (resultado) {

        if (resultado) {
            $('#tbNotas').html(resultado);
        }
    });
});

function visualizarNota(id) {

    if (isNumero(id)) {

        $.get(urlBase + 'Oportunidades/ObterDetalhesNota/' + id, function (resultado) {

            $("#NotaId").val(resultado.Id);
            $("#Nota").val(resultado.Nota);
            $("#NotaDescricao").val(resultado.Descricao);
        });
    }
}

$('#btnPesquisarFavorecido1').click(function () {

    event.preventDefault();

    $('#pesquisa-modal-contas')
        .data('toggle', 'Favorecido1')
        .modal('show');
});

$('#btnPesquisarFavorecido2').click(function () {

    event.preventDefault();

    $('#pesquisa-modal-contas')
        .data('toggle', 'Favorecido2')
        .modal('show');
});

$('#btnPesquisarFavorecido3').click(function () {

    event.preventDefault();

    $('#pesquisa-modal-contas')
        .data('toggle', 'Favorecido3')
        .modal('show');
});

$("#TipoServicoPremioParceria").change(function () {
    consultarModalidades($(this).val());
});

function consultarModalidades(tipo) {

    if (tipo) {

        $.get(urlBase + 'Oportunidades/ObterModalidadesPremioParceria/?tipoServico=' + tipo, function (resultado) {

            $("#Modalidades").empty();

            $.each(resultado.modalidades, function (key, value) {
                $("#Modalidades").append('<option value=' + value.Id + '>' + value.Descricao + '</option>');
            });

            if (parseInt(tipo) === 1 || parseInt(tipo) === 2) {
                $("#pnlModalidades").removeClass('invisivel');
            } else {
                $("#pnlModalidades").addClass('invisivel');
            }
        });
    }
}

$('#btnSalvarPremioParceria').click(function () {

    $('#ModalidadesSelecionadas option').prop('selected', 'selected');

    $(this).html('<i class="fa fa-spinner fa-spin"></i> aguarde...')
        .addClass('disabled');
});

premioParceriaMensagemSucesso = function () {

    toastr.success('Informações atualizadas com sucesso', 'CRM');

    $("#btnSalvarPremioParceria").removeClass('disabled')
        .html('Salvar');
}

$('#RevisarPremioParceria').click(function () {

    var id = $('#Id').val();

    $('#confirmar-revisaoPremio-modal')
        .data('id', id)
        .modal('show');
});

function confirmarRevisaoPremio() {

    var id = $('#confirmar-revisaoPremio-modal').data('id');

    if (isNumero(id)) {
        $.get(urlBase + 'Oportunidades/ObterPremioParceriaAtivo/?oportunidadeId=' + id, function (resultado) {
            if (resultado) {
                atualizarPremioParceria(resultado.Id);
                $('#PremioRevisaoId').val(resultado.Id);
            } else {
                toastr.error('Nenhum Prêmio ativo encontrado na Oportunidade', 'CRM');
            }
        });
    }

    $('#confirmar-revisaoPremio-modal')
        .data('id', '0')
        .modal('hide');
}

$('#btnCancelarPremioParceria').click(function () {

    var id = $('#PremioParceriaId').val();

    $('#confirmar-cancelamentoPremio-modal')
        .data('id', id)
        .modal('show');
});

function confirmarCancelamentoPremio() {

    var id = $('#confirmar-cancelamentoPremio-modal').data('id');

    if (isNumero(id)) {
        $.post(urlBase + 'Oportunidades/CancelarPremioParceria/', { id: id }, function () {
            toastr.success('Prêmio Parceria cancelado com sucesso', 'CRM');
            $('#item-status-premio-' + id).html('Cancelado');
            $('#btn-enviar-premio-' + id).removeClass('disabled');
        }).fail(function (data) {
            if (data.statusText) {
                toastr.error(data.statusText, 'CRM');
            } else {
                toastr.error('Falha ao cancelar o Prêmio Parceria', 'CRM');
            }
        });
    }

    $('#confirmar-cancelamentoPremio-modal')
        .data('id', '0')
        .modal('hide');
}

function obterPremioParceriaAtivo() {

    var id = $('#Id').val();

    if (isNumero(id)) {
        $.get(urlBase + 'Oportunidades/ObterPremioParceriaAtivo/?oportunidadeId=' + id, function (resultado) {
            if (resultado)
                atualizarPremioParceria(resultado.Id);
        });
    }
}

$('#btnAdicionarModalidade').click(function (e) {

    e.preventDefault();
    var modalidade = $('#Modalidades option:selected');

    if (modalidade.length > 0)
        $('#ModalidadesSelecionadas').append($(modalidade).clone());
});

$('#btnRemoverModalidade').click(function (e) {

    e.preventDefault();
    var modalidade = $('#ModalidadesSelecionadas option:selected');

    if (modalidade.length > 0)
        $(modalidade).remove();
});

function excluirPremioParceria(id) {

    if (isNumero(id)) {

        $.post(urlBase + 'Oportunidades/ExcluirPremioParceria/', { id: id }, function () {
            toastr.success('Prêmio Parceria excluído com sucesso', 'CRM');
            $('#item-premio-' + id).remove();
        }).fail(function (data) {
            if (data.statusText) {
                toastr.error(data.statusText, 'CRM');
            } else {
                toastr.error('Falha ao excluir o Prêmio Parceria', 'CRM');
            }
        });
    }
}

function atualizarPremioParceria(id) {

    event.preventDefault();

    if (isNumero(id)) {

        limparPremioParceria();

        $.get(urlBase + 'Oportunidades/ObterDetalhesPremioParceria/' + id, function (resultado) {

            $("#PremioParceriaId").val(resultado.Id);
            $("#PremioParceriaOportunidadeId").val(resultado.OportunidadeId);
            $("#Observacoes").val(resultado.Observacoes);
            $("#UrlPremio").val(resultado.UrlPremio);
            $("#DataUrlPremio").val(resultado.DataUrlPremio);
            $("#EmailFavorecido1").val(resultado.EmailFavorecido1);
            $("#EmailFavorecido2").val(resultado.EmailFavorecido2);
            $("#EmailFavorecido3").val(resultado.EmailFavorecido3);

            $("#Favorecido1")
                .empty()
                .append($('<option>', {
                    value: resultado.Favorecido1,
                    text: Strings.orEmpty(resultado.DescricaoFavorecido1) + ' ' + Strings.orEmpty(resultado.DocumentoFavorecido1)
                }));

            $("#Favorecido2")
                .empty()
                .append($('<option>', {
                    value: resultado.Favorecido2,
                    text: Strings.orEmpty(resultado.DescricaoFavorecido2) + ' ' + Strings.orEmpty(resultado.DocumentoFavorecido2)
                }));

            $("#Favorecido3")
                .empty()
                .append($('<option>', {
                    value: resultado.Favorecido3,
                    text: Strings.orEmpty(resultado.DescricaoFavorecido3) + ' ' + Strings.orEmpty(resultado.DocumentoFavorecido3)
                }));

            $("#StatusPremioParceria").val(0).val(resultado.StatusPremioParceria);
            $("#TipoServicoPremioParceria").val(0).val(resultado.TipoServicoPremioParceria);
            $("#Instrucao").val(0).val(resultado.Instrucao);
            $("#PremioParceriaContatoId").empty().append($('<option>', { value: resultado.ContatoId, text: resultado.DescricaoContato }));
            $('#PremioReferenciaId').empty();

            if (resultado.PremioReferenciaId && isNumero(resultado.PremioReferenciaId)) {
                if (resultado.PremioReferenciaId > 0) {
                    $('#PremioReferenciaId')
                        .append($('<option>', {
                            value: resultado.PremioReferenciaId,
                            text: 'P-' + resultado.PremioReferenciaId
                        }));
                }
            }

            $('#lnkPremioAnexo').addClass('invisivel');

            if (resultado.IdFile) {
                if (resultado.IdFile !== '') {
                    $('#lnkPremioAnexo')
                        .attr('href', urlBase + 'Anexos/Download/' + resultado.IdFile)
                        .removeClass('invisivel');
                }
            }

            if (resultado.UrlPremio !== '') {
                $('#lnkUrlPremio')
                    .removeClass('invisivel')
                    .attr('href', resultado.UrlPremio);
            } else {
                $('#lnkUrlPremio')
                    .addClass('invisivel')
                    .removeAttr('href');
            }

            consultarModalidades(resultado.TipoServicoPremioParceria);

            var $selectModalidadesSelecionadas = $("#ModalidadesSelecionadas");
            $selectModalidadesSelecionadas.empty();

            resultado.ModalidadesSelecionadas.forEach(function (item) {
                $selectModalidadesSelecionadas.append($('<option>', { value: item.Id, text: item.Descricao }));
            });

            if (resultado.StatusOportunidade === 1 && resultado.StatusPremioParceria === 3)
                $('#btnCancelarPremioParceria').removeAttr('disabled');
        });
    }
}

function limparPremioParceria() {

    $("#frmOportunidadesPremiosParceria")[0].reset();

    $('#Modalidades').empty();
    $('#ModalidadesSelecionadas').empty();

    $("#Favorecido1").empty();
    $("#Favorecido2").empty();
    $("#Favorecido3").empty();

    $("#PremioParceriaId").val(0);
    $('#lnkPremioAnexo').addClass('invisivel');
    $('#btnCancelarPremioParceria').attr('disabled', true);
    $('#PremioRevisaoId').val(0);
}

function consultarAnexos() {

    var id = $("#Id").val();

    if (isNumero(id)) {
        $.get(urlBase + 'Oportunidades/ObterAnexosOportunidade/?oportunidadeId=' + id, function (data) {
            $('#ListaAnexos').html(data);
        });
    }
}

$('#TipoAdendo').change(function () {

    var tipo = $(this).val();

    $('#pnlAdendosVendedor, #pnlAdendosExclusaoSubClientes, #pnlAdendosExclusaoGrupoCNPJ, #pnlAdendosFormaPagamento, #pnlAdendosInclusaoSubClientes, #pnlAdendosInclusaoGrupoCNPJ').addClass('invisivel');

    if (isNumero(tipo)) {
        switch (tipo) {
            case '1':
                $('#pnlAdendosVendedor').removeClass('invisivel');
                break;
            case '2':
                $('#pnlAdendosFormaPagamento').removeClass('invisivel');
                break;
            case '3':
                $('#pnlAdendosInclusaoSubClientes').removeClass('invisivel');
                $('#tbAdendosInclusaoSubCliente > tbody').empty();
                break;
            case '4':
                $('#pnlAdendosExclusaoSubClientes').removeClass('invisivel');
                break;
            case '5':
                $('#pnlAdendosInclusaoGrupoCNPJ').removeClass('invisivel');
                $('#tbAdendosInclusaoClientesGrupo > tbody').empty();
                break;
            case '6':
                $('#pnlAdendosExclusaoGrupoCNPJ').removeClass('invisivel');
                break;
        }
    }
});

$('#btnAdendoPesquisarSubCliente').click(function () {

    event.preventDefault();

    $('#pesquisa-modal-contas')
        .data('toggle', 'AdendoSubClienteId')
        .modal('show');
});

$('#btnAdendoPesquisarGrupoCNPJ').click(function () {

    event.preventDefault();

    $('#pesquisa-modal-contas')
        .data('toggle', 'AdendoClienteGrupoCNPJId')
        .modal('show');
});

function validarOportunidadeConcomitante() {

    $.get(urlBase + 'Oportunidades/ValidarOportunidadeConcomitante?oportunidadeId=' + $('#Id').val(), function (concomitancia) {

        if (concomitancia) {

            if (concomitancia.Existe) {

                $("#msgPropostasConcomitantes").html(concomitancia.Mensagem + 'Não é possível seguir com o envio para aprovação');

                if (concomitancia.Bloqueia) {

                    $('#imgIconeConcomitancia')
                        .attr('src', urlBase + 'content/img/icone-integracao-erro.png');
                } else {

                    toastr.success('Enviado para aprovação!', 'CRM');

                    $('#imgIconeConcomitancia')
                        .attr('src', urlBase + 'content/img/icone-integracao.png');

                    $('#btnIrParaOportunidadeConcomitante')
                        .removeClass('invisivel')
                        .attr('href', concomitancia.RedirectUrl);
                }

                $('#concomitancia-modal').modal('show');

            }
        }
    });
}

$('#btnEnviarOportunidadeParaAprovacao').click(function () {

    event.preventDefault();

    var id = $("#Id").val();

    $('#btnAtualizarOportunidade').prop('disabled', true);

    $('#btnEnviarOportunidadeParaAprovacao')
        .html('<i class="fa fa-spinner fa-spin"></i> aguarde...')
        .prop('disabled', true);

    $.get(urlBase + 'Oportunidades/ValidarOportunidadeConcomitante?oportunidadeId=' + $('#Id').val(), function (concomitancia) {

        if (concomitancia) {

            if (concomitancia.Existe) {

                $("#msgPropostasConcomitantes").html(concomitancia.Mensagem + 'Não é possível seguir com o envio para aprovação');

                if (concomitancia.Bloqueia) {

                    $('#imgIconeConcomitancia')
                        .attr('src', urlBase + 'content/img/icone-integracao-erro.png');
                } else {

                    toastr.success('Enviado para aprovação!', 'CRM');

                    $('#imgIconeConcomitancia')
                        .attr('src', urlBase + 'content/img/icone-integracao.png');

                    $('#btnIrParaOportunidadeConcomitante')
                        .removeClass('invisivel')
                        .attr('href', concomitancia.RedirectUrl);
                }

                $('#btnAtualizarOportunidade').prop('disabled', false);

                $("#btnEnviarOportunidadeParaAprovacao")
                    .html('Enviar para Aprovação')
                    .prop('disabled', false);

                $('#concomitancia-modal').modal('show');

            } else {

                $.post(urlBase + 'Oportunidades/EnviarOportunidadeParaAprovacao/', { id: id }, function (resultado) {

                    if (resultado) {

                        toastr.success('Enviado para aprovação!', 'CRM');

                        if (resultado.RedirectUrl !== '') {

                            setTimeout(function () {
                                if (resultado) {
                                    document.location.href = decodeURIComponent(resultado.RedirectUrl);
                                }
                            }, 2000);
                        }
                    }

                }).fail(function (data) {
                    if (data.statusText) {
                        toastr.error(data.statusText, 'CRM');
                    } else {
                        toastr.error('Falha ao enviar a Oportunidade para aprovação', 'CRM');
                    }

                    $('#btnAtualizarOportunidade').prop('disabled', false);

                    $("#btnEnviarOportunidadeParaAprovacao")
                        .html('Enviar para Aprovação')
                        .prop('disabled', false);
                }).always(function () {
                    $("#btnEnviarOportunidadeParaAprovacao")
                        .html('Enviar para Aprovação');
                });
            }
        }
    });

});

$('#btnSalvarAdendoFormaPgto').click(function () {

    $(this).html('<i class="fa fa-spinner fa-spin"></i> aguarde...')
        .addClass('disabled');
});

$('#btnAdendoExclusaoSubCliente').click(function () {

    $(this).html('<i class="fa fa-spinner fa-spin"></i> aguarde...')
        .addClass('disabled');
});

$('#btnAdendoExclusaoGrupoCnpj').click(function () {

    $(this).html('<i class="fa fa-spinner fa-spin"></i> aguarde...')
        .addClass('disabled');
});

function adicionarClienteFicha() {

    var select = $("#ClientePropostaSelecionadoId option:selected");

    var clientePropostaSelecionadoId = select.val();
    var clientePropostaSelecionadoTexto = select.text();

    var campo = $('#ClientesPropostaSelecionados');

    if (isNumero(clientePropostaSelecionadoId)) {

        campo.tagsinput({
            itemValue: 'id',
            itemText: 'text',
        });

        campo.tagsinput('add', { id: clientePropostaSelecionadoId, text: clientePropostaSelecionadoTexto });
    }
}


fichasFaturamentoMensagemSucesso = function () {

    toastr.success('Informações atualizadas com sucesso', 'CRM');

    $("#btnCadastrarFichaFaturamento").removeClass('disabled')
        .html('Salvar');
}

function enviarFichaFaturamentoParaAprovacao(id) {

    event.preventDefault();

    if (isNumero(id)) {

        $('#btn-enviar-ficha-' + id)
            .html('<i class="fa fa-spinner fa-spin"></i> aguarde...')
            .addClass('disabled');

        $.post(urlBase + 'Oportunidades/EnviarFichaFaturamentoParaAprovacao/', { id: id }, function () {
            toastr.success('Enviado para aprovação!', 'CRM');
            $('#item-status-ficha-' + id).html('Em aprovação');
            $('#btn-enviar-ficha-' + id).addClass('disabled');
            $('#btn-recall-ficha-' + id).removeClass('disabled');
        }).fail(function (data) {
            if (data.statusText) {
                toastr.error(data.statusText, 'CRM');
            } else {
                toastr.error('Falha ao enviar a Oportunidade para aprovação', 'CRM');
            }
        }).always(function () {
            $('#btn-enviar-ficha-' + id).html('<i class="fa fa-check"></i> Enviar para Aprovação');
        });
    }
}

var diasSemanaArr = ['Domingo', 'Segunda-Feira', 'Terça-Feira', 'Quarta-Feira', 'Quinta-Feira', 'Sexta-Feira', 'Sábado'];

function atualizarFichaFaturamento(id) {

    limparFichasFaturamento();

    carregarDetalhesFichaFaturamento(id);
}

function carregarDetalhesFichaFaturamento(id) {

    event.preventDefault();

    $.get(urlBase + 'Oportunidades/PermiteAlterarCamposFicha?oportunidadeId=' + $('#Id').val() + '&fichaId=' + id, function (resultado) {

        $('#tbFichasFaturamento > tbody  > tr').removeClass('linha-selecionada');

        $('#item-ficha-' + id).addClass('linha-selecionada');

        if (isNumero(id)) {
            $.get(urlBase + 'Oportunidades/ObterDetalhesFichaFaturamento/' + id, function (resultado) {

                $('#FichaFaturamentoId').val(resultado.Id);
                $("#DiasFaturamento").val(resultado.DiasFaturamento);
                $("#DataCorte").val(resultado.DataCorte);
                $("#EmailFaturamento").val(resultado.EmailFaturamento);
                $("#CondicaoPagamentoFaturamentoId").val(resultado.CondicaoPagamentoId);
                $("#ObservacoesFaturamento").val(resultado.ObservacoesFaturamento);

                $('#DiaUtil').prop('checked', resultado.DiaUtil);
                $('#UltimoDiaDoMes').prop('checked', resultado.UltimoDiaDoMes);
                $('#EntregaEletronica').prop('checked', resultado.EntregaEletronica);
                $('#EntregaManual').prop('checked', resultado.EntregaManual);

                if (resultado.EntregaManualSedex) {
                    $('#CorreiosSedex').prop('checked', true);
                } else {
                    $('#CorreiosComum').prop('checked', true);
                }

                if ($('#FaturadoContraId').find("option:contains('" + resultado.FaturadoContraId + "')").length) {

                    $("#FaturadoContraId").val(resultado.FaturadoContraId);
                } else {
                    if (parseInt(resultado.FaturadoContraId) > 0) {

                        $("#FaturadoContraId")
                            .empty()
                            .append(
                                $('<option>', {
                                    value: resultado.FaturadoContraId,
                                    text: resultado.FaturadoContra + ' (' + resultado.FaturadoContraDocumento + ')'
                                }));
                    }
                }

                if ($('#FontePagadoraId').find("option:contains('" + resultado.FontePagadoraId + "')").length) {

                    $("#FontePagadoraId").val(resultado.FontePagadoraId);
                } else {

                    if (parseInt(resultado.FontePagadoraId) > 0) {

                        $("#FontePagadoraId")
                            .empty()
                            .append(
                                $('<option>', {
                                    value: resultado.FontePagadoraId,
                                    text: resultado.FontePagadora + ' (' + resultado.FontePagadoraDocumento + ')'
                                }));
                    }
                }

                carregarSubClientesFichasFaturamento();

                if (parseInt(resultado.ContaId) > 0) {

                    if ($('#ClientePropostaSelecionadoId').find("option:contains('" + resultado.ContaId + "')").length) {

                        $('#ClientePropostaSelecionadoId').select2('val', [resultado.ContaId]);
                    } else {

                        $('#ClientePropostaSelecionadoId')
                            .empty()
                            .append(
                                $('<option>', {
                                    value: resultado.ContaId,
                                    text: resultado.Cliente + ' (' + resultado.ClienteDocumento + ')'
                                }));

                        $('#ClientePropostaSelecionadoId').select2('val', [resultado.ContaId]);
                    }
                }

                var campoDiasSemana = $('#DiasSemana');
                var valorDiasSemana = resultado.DiasSemanaLiterais;

                if (valorDiasSemana !== null) {

                    var diasSemana = valorDiasSemana.split(',');

                    campoDiasSemana.tagsinput({
                        itemValue: 'id',
                        itemText: 'text'
                    });

                    campoDiasSemana.tagsinput('removeAll');

                    diasSemana.forEach(function (item) {
                        campoDiasSemana.tagsinput('add', { id: item, text: diasSemanaArr[parseInt(item - 1)] });
                    });
                }

                var campoDiasFaturamento = $('#DiasFaturamento');
                var valorDiasFaturamento = resultado.DiasFaturamento;

                if (valorDiasFaturamento !== null) {

                    var diasFaturamento = valorDiasFaturamento.split(',');

                    campoDiasFaturamento.tagsinput({
                        itemValue: 'id',
                        itemText: 'text'
                    });

                    campoDiasFaturamento.tagsinput('removeAll');

                    diasFaturamento.forEach(function (item) {
                        campoDiasFaturamento.tagsinput('add', { id: item, text: item });
                    });
                }

                var campoCondicaoPgtoDias = $('#CondicaoPagamentoPorDia');
                var condicaoPgtoDias = resultado.CondicaoPagamentoPorDia;

                if (condicaoPgtoDias !== null) {

                    var diasPgto = condicaoPgtoDias.split(',');

                    campoCondicaoPgtoDias.tagsinput({
                        itemValue: 'id',
                        itemText: 'text'
                    });

                    campoCondicaoPgtoDias.tagsinput('removeAll');

                    diasPgto.forEach(function (item) {
                        campoCondicaoPgtoDias.tagsinput('add', { id: item, text: item });
                    });
                }

                var camposCondicaoPgtoDiasSemana = $('#CondicaoPagamentoPorDiaSemana');
                var condicaoPgtoDiasSemana = resultado.CondicaoPagamentoPorDiaSemana;

                if (condicaoPgtoDiasSemana !== null) {

                    var diasSemanaPgto = condicaoPgtoDiasSemana.split(',');

                    camposCondicaoPgtoDiasSemana.tagsinput({
                        itemValue: 'id',
                        itemText: 'text'
                    });

                    camposCondicaoPgtoDiasSemana.tagsinput('removeAll');

                    diasSemanaPgto.forEach(function (item) {
                        camposCondicaoPgtoDiasSemana.tagsinput('add', { id: item, text: diasSemanaArr[parseInt(item - 1)] });
                    });
                }

                $('#lnkFichaAnexo').addClass('invisivel');

                if (resultado.IdFile) {
                    if (resultado.IdFile !== '') {
                        $('#lnkFichaAnexo')
                            .attr('href', urlBase + 'Anexos/Download/' + resultado.IdFile)
                            .removeClass('invisivel');
                    }
                }
            });
        }

        if (!resultado.permiteAlteracoes) {
            if (!resultado.admin) {

                $('#frmOportunidadesFichaFaturamento :input')
                    .not(':input[type=button]')
                    .prop('readonly', true);

                resultado.campos.forEach(function (item) {

                    $('#' + item.Campo.capitalize()).prop('readonly', !item.Permissao);

                    if (item.Campo === 'permiteSalvarFichas ' && item.Permissao === false) {
                        $('#btnCadastrarFichaFaturamento').addClass('disabled');
                    }

                    if (item.Campo === 'revisarFichaFaturamento' && item.Permissao === false) {
                        $('#RevisarFichaFaturamento').addClass('disabled');
                    }

                    if (item.Campo === 'novaFichaFaturamento ' && item.Permissao === false) {
                        $('#btnNovaFichaFaturamento').addClass('disabled');
                    }

                    if (item.Campo === 'diasSemana' && item.Permissao === false) {
                        $('#btnAdicionarDiasSemanaFaturamento').addClass('link-desabilitado');
                    }

                    if (item.Campo === 'diasFaturamento' && item.Permissao === false) {
                        $('#btnAdicionarDiasFaturamento').addClass('link-desabilitado');
                    }

                    if (item.Campo === 'condicaoPagamentoPorDia' && item.Permissao === false) {
                        $('#btnAdicionarDiaCondicaoPgto').addClass('link-desabilitado');
                    }

                    if (item.Campo === 'diaUtil' && item.Permissao === false) {
                        $('#DiaUtil').prop('disabled', false);
                        $('#DiaUtil').addClass('checkbox-readonly');
                    }

                    if (item.Campo === 'entregaEletronica' && item.Permissao === false) {
                        $('#EntregaEletronica').prop('disabled', false);
                        $('#EntregaEletronica').addClass('checkbox-readonly');
                    }

                    if (item.Campo === 'entregaManual' && item.Permissao === false) {
                        $('#EntregaManual').prop('disabled', false);
                        $('#EntregaManual').addClass('checkbox-readonly');
                    }

                    if (item.Campo === 'correiosComum' && item.Permissao === false) {
                        $('#CorreiosComum').prop('disabled', false);
                        $('#CorreiosComum').addClass('checkbox-readonly');

                        $('#CorreiosSedex').prop('disabled', false);
                        $('#CorreiosSedex').addClass('checkbox-readonly');
                    }
                });

            }
        }
    });
}

$("#btnNovaFichaFaturamento").click(function () {

    $('#frmOportunidadesFichaFaturamento :input')
        .prop('disabled', false);

    $('#frmOportunidadesFichaFaturamento a').not('.btn-atualiza-ficha').removeClass('disabled');

    limparFichasFaturamento();
    carregarSubClientesFichasFaturamento();
});

function limparFichasFaturamento() {

    $("#ClientePropostaSelecionadoId").empty();
    $('#ClientePropostaSelecionadoId').val([]).change();

    $('#FichaRevisaoId').val('0');
    $("#FichaFaturamentoId").val('0');
    $('#EmailFaturamento').val('');
    $("#FaturadoContraId").prop('selectedIndex', -1);
    $('#FontePagadoraId').prop('selectedIndex', -1);
    $("#DataCorte").val('');
    $("#ObservacoesFaturamento").val('');
    $('#CondicaoPagamentoFaturamentoId').val('0');
    $('#lnkFichaAnexo').addClass('invisivel');
    $('#AnexoFaturamento').val('');

    $('#EntregaEletronica').prop('checked', false);
    $('#EntregaManual').prop('checked', false);
    $('#EmailFaturamento').prop('readonly', false);

    var tagsClientesPropostaSelecionados = $('#ClientesPropostaSelecionados');

    tagsClientesPropostaSelecionados.tagsinput({
        itemValue: 'id',
        itemText: 'text'
    });

    tagsClientesPropostaSelecionados.tagsinput('removeAll');

    var tagsDiasSemana = $('#DiasSemana');

    tagsDiasSemana.tagsinput({
        itemValue: 'id',
        itemText: 'text'
    });

    tagsDiasSemana.tagsinput('removeAll');

    var tagsDiasFaturamento = $('#DiasFaturamento');

    tagsDiasFaturamento.tagsinput({
        itemValue: 'id',
        itemText: 'text'
    });

    tagsDiasFaturamento.tagsinput('removeAll');

    var tagsCondicaoPagamentoPorDia = $('#CondicaoPagamentoPorDia');

    tagsCondicaoPagamentoPorDia.tagsinput({
        itemValue: 'id',
        itemText: 'text'
    });

    tagsCondicaoPagamentoPorDia.tagsinput('removeAll');

    var tagsCondicaoPagamentoPorDiaSemana = $('#CondicaoPagamentoPorDiaSemana');

    tagsCondicaoPagamentoPorDiaSemana.tagsinput({
        itemValue: 'id',
        itemText: 'text'
    });

    tagsCondicaoPagamentoPorDiaSemana.tagsinput('removeAll');
}

function carregarSubClientesFichasFaturamento() {

    $.get(urlBase + 'Oportunidades/ConsultarSubClientesFichaFaturamentoJson?oportunidadeId=' + $('#Id').val(), function (resultado) {

        $.each(resultado.dados, function (key, value) {
            $("#ClientePropostaSelecionadoId").append('<option value=' + value.ContaId + '>' + value.Descricao + ' (' + value.Documento + ')' + '</option>');
        });
    });
}

function excluirFichaFaturamento(id) {

    if (isNumero(id)) {

        $.post(urlBase + 'Oportunidades/ExcluirFichaFaturamento/', { id: id }, function () {
            toastr.success('Ficha de Faturamento excluída com sucesso', 'CRM');
            $('#item-ficha-' + id).remove();
        }).fail(function (data) {
            if (data.statusText) {
                toastr.error(data.statusText, 'CRM');
            } else {
                toastr.error('Falha ao excluir a Ficha Faturamento', 'CRM');
            }
        });
    }
}

function enviarPremioParceriaParaAprovacao(id) {

    event.preventDefault();

    if (isNumero(id)) {

        $('#btn-enviar-premio-' + id)
            .html('<i class="fa fa-spinner fa-spin"></i> aguarde...')
            .addClass('disabled');

        $.post(urlBase + 'Oportunidades/EnviarPremioParceriaParaAprovacao/', { id: id }, function () {
            toastr.success('Enviado para aprovação!', 'CRM');
            $('#item-status-premio-' + id).html('Em aprovação');
            $('#btn-enviar-premio-' + id).addClass('disabled');
            $('#btn-recall-premio-' + id).removeClass('disabled');
        }).fail(function (data) {
            if (data.statusText) {
                toastr.error(data.statusText, 'CRM');
            } else {
                toastr.error('Falha ao enviar a Oportunidade para aprovação', 'CRM');
            }
        }).always(function () {
            $('#btn-enviar-premio-' + id).html('<i class="fa fa-check"></i> Enviar para Aprovação');
        });
    }
}

function excluirAdendo(id) {

    if (isNumero(id)) {
        $.post(urlBase + 'Oportunidades/ExcluirAdendo/', { id: id }, function () {
            toastr.success('Adendo excluído com sucesso', 'CRM');
            $('#item-adendo-' + id).remove();

            setTimeout(function () {
                location.hash = '#adendos';
                location.reload();
            }, 1000);

        }).fail(function (data) {
            if (data.statusText) {
                toastr.error(data.statusText, 'CRM');
            } else {
                toastr.error('Falha ao excluir o Adendo', 'CRM');
            }
        });
    }
}

function enviarAdendoParaAprovacao(id) {

    event.preventDefault();

    $('#btn-enviar-adendo-' + id)
        .html('<i class="fa fa-spinner fa-spin"></i> aguarde...')
        .addClass('disabled');

    if (isNumero(id)) {
        $.post(urlBase + 'Oportunidades/EnviarAdendoParaAprovacao/', { id: id }, function (resultado) {

            if (resultado) {
                if (resultado.Bloqueia) {

                    $("#msgPropostasConcomitantes").html(resultado.Mensagem);

                    $('#imgIconeConcomitancia')
                        .attr('src', urlBase + 'content/img/icone-integracao-erro.png');

                    $('#concomitancia-modal').modal('show');

                    $('#btn-enviar-adendo-' + id)
                        .html('<i class="fa fa-check"></i>&nbsp;Enviar para Aprovação')
                        .removeClass('disabled');

                    return;
                }

                toastr.success(resultado.Mensagem, 'CRM');

                $('#item-status-adendo-' + id).html('Enviado');
                $('#btn-enviar-adendo-' + id).addClass('disabled');
                $('#btn-recall-adendo-' + id).removeClass('disabled');

                $('#btn-enviar-adendo-' + id)
                    .addClass('disabled')
                    .html('<i class="fa fa-check"></i> Enviar para Aprovação');

                console.log(resultado.HabilitaAbaFichas);

                if (resultado.HabilitaAbaFichas) {
                    setTimeout(function () {
                        location.hash = '#adendos';
                        location.reload();
                    }, 1000);
                }
            }

        }).fail(function (data) {
            if (data.statusText) {
                toastr.error(data.statusText, 'CRM');
            } else {
                toastr.error('Falha ao enviar o Adendo para aprovação', 'CRM');
            }

            $('#btn-enviar-adendo-' + id)
                .removeClass('disabled')
                .html('<i class="fa fa-check"></i> Enviar para Aprovação');

        });
    }
}

$('#btnFiltroAdendos').click(function () {
    $('#pnlFiltroAdendos').toggle("slow");
});

$('#btnPesquisarAdendos').click(function () {

    var id = $('#filtroAdendoId').val();
    var descricaoCliente = $('#filtroAdendoCliente').val();
    var oportunidadeId = $('#AdendoOportunidadeId').val();

    $.post(urlBase + 'Oportunidades/FiltrarAdendos/', { id: id, oportunidadeId: oportunidadeId, descricaoCliente: descricaoCliente }, function (resultado) {
        $('#ListaAdendos').html(resultado)
    }).fail(function (data) {
        if (data.statusText) {
            toastr.error(data.statusText, 'CRM');
        } else {
            toastr.error('Falha ao consultar os adendos', 'CRM');
        }
    });
});

function obterHistoricoWorkflow(id, idProcesso) {

    if (!isNumero(id)) {
        toastr.error('Oportunidade não informada', 'CRM');
        return;
    }

    if (!isNumero(idProcesso)) {
        toastr.error('Id do processo não informado', 'CRM');
        return;
    }

    $.get(urlBase + 'Oportunidades/ObterHistoricoWorkflow/?id=' + id + '&idProcesso=' + idProcesso, function (data) {

        switch (idProcesso) {
            case 1:
                $('#pnlInfoHistoricoWorkflow').html(data);
                break;
            case 2:
                $('#pnlFichasHistoricoWorkflow').hide();
                $('#pnlFichasHistoricoWorkflow').html(data);
                $('#pnlFichasHistoricoWorkflow').show('slow');
                break;
            case 3:
                $('#pnlPremiosHistoricoWorkflow').hide();
                $('#pnlPremiosHistoricoWorkflow').html(data);
                $('#pnlPremiosHistoricoWorkflow').show('slow');
                break;
            case 4:
                $('#pnlAdendosHistoricoWorkflow').hide();
                $('#pnlAdendosHistoricoWorkflow').html(data);
                $('#pnlAdendosHistoricoWorkflow').show('slow');
                break;
            default:
        }

    }).fail(function (data) {
        toastr.error(data.statusText, 'CRM');
    });
}

function integrarAdendoChronos(id) {

    if (!isNumero(id)) {
        toastr.error('Adendo não informado', 'CRM');
        return;
    }

    $('#btnIntegraAdendoChronos').html('<i class="fa fa-spinner fa-spin"></i> aguarde...')
        .addClass('disabled');

    $.post(urlBase + 'Oportunidades/IntegraAdendoChronos', { id: id }, function (resultado) {

        $('#pnlIntegracaoChronos').html(resultado);

        $('#integracao-chronos-modal').modal('show');

    }).fail(function (data) {
        toastr.error(data.statusText, 'CRM');
    }).always(function () {

        $("#btnIntegraAdendoChronos")
            .html('<i class="fa fa-cogs"></i> Integra Chronos')
            .removeClass('disabled');
    });
}

function integrarFichaChronos(id) {

    if (!isNumero(id)) {
        toastr.error('Adendo não informado', 'CRM');
        return;
    }

    $('#btnIntegraFichaChronos-' + id).html('<i class="fa fa-spinner fa-spin"></i> aguarde...')
        .addClass('disabled');

    $.post(urlBase + 'Oportunidades/IntegraFichaChronos', { id: id }, function (resultado) {

        $('#pnlIntegracaoChronos').html(resultado);

        $('#integracao-chronos-modal').modal('show');

    }).fail(function (data) {
        toastr.error(data.statusText, 'CRM');
    }).always(function () {

        $('#btnIntegraFichaChronos-' + id)
            .html('<i class="fa fa-cogs"></i> Integra Chronos')
            .removeClass('disabled');
    });
}

function atualizarDataTermino() {

    var validade = $('#Validade').val();

    var dataAtual = moment().locale('pt-br').format("L");
    var data = moment(dataAtual, "DD/MM/YYYY");

    var tipo = $('#TipoValidade').val();

    var acordo = $('#Acordo').is(':checked');

    if (tipo === '1') {
        if (parseInt(validade) === 365) {
            dataAtual = moment(data.format("YYYY-MM-DD"))
                .add(1, 'year')
                .add(-1, 'days')
                .format("DD/MM/YYYY");
        } else {
            dataAtual = moment(data.format("YYYY-MM-DD"))
                .add(validade, 'days')
                .format("DD/MM/YYYY");
        }
    } else if (tipo === '2') {
        if (parseInt(validade) === 12) {
            dataAtual = moment(data.format("YYYY-MM-DD"))
                .add(1, 'year')
                .add(-1, 'days')
                .format("DD/MM/YYYY");
        } else {
            dataAtual = moment(data.format("YYYY-MM-DD"))
                .add(validade, 'month')
                .format("DD/MM/YYYY");
        }
    } else {
        dataAtual = moment(data.format("YYYY-MM-DD"))
            .add(validade, 'year')
            .add(-1, 'days')
            .format("DD/MM/YYYY");
    }

    if (!moment($('#DataInicio').val()).isValid()) {
        $('#DataInicio').val(data.format("DD/MM/YYYY"));
    }

    if (!acordo) {
        $('#DataTermino').val(dataAtual);
    }
}

function validarInclusaoSubCliente(oportunidadeId, segmento, subClienteId) {


}

$('#btnAdicionarSubClienteAdendo').click(function () {

    event.preventDefault();

    var segmento = $('#AdendoSegmento option:selected');
    var subCliente = $('#AdendoSubClienteId option:selected');

    var oportunidadeId = $('#Id').val();
    var segmentoId = segmento.val();
    var segmentoNome = segmento.text();
    var subClienteId = subCliente.val();
    var subClienteNome = subCliente.text();

    if (isNumero(segmentoId) && isNumero(subClienteId)) {

        if (parseInt(segmentoId) === 0) {
            toastr.error('Informe o segmento do Sub Cliente!', 'CRM - Adendos');
            return;
        }

        var parametros = '?oportunidadeId=' + oportunidadeId + '&segmento=' + segmentoId + '&subClienteId=' + subClienteId;

        $.get(urlBase + 'Oportunidades/ValidarInclusaoSubCliente' + parametros, function (resultado) {

            console.log('validou: ');
            $("#btnAdicionarSubClienteAdendo")
                .html('<i class="fa fa-spinner fa-spin"></i> aguarde...')
                .addClass('disabled');

            var registro = subClienteId + ':' + segmentoId;

            var linha =
                '<tr id="adendo-subcliente-' + subClienteId + '">' +
                '   <td><input type="checkbox" name="AdendosSubClientesInclusao" id="AdendosSubClientesInclusao" value="' + registro + '" checked /></td>' +
                '   <td>' + segmentoNome + '</td>' +
                '   <td>' + subClienteNome + '</td>' +
                '   <td><img src="' + urlBase + 'Content/img/excluir.png" onclick="excluirLinhaAdendoSubCliente(' + subClienteId + ')" /></td>' +
                '</tr>';

            $('#tbAdendosInclusaoSubCliente > tbody:last-child').append(linha);
            $('#AdendoSubClienteId').empty();

            $("#btnAdicionarSubClienteAdendo")
                .html('<i class="fa fa-edit"></i> Adicionar')
                .removeClass('disabled');

        }).fail(function (data) {

            toastr.error(data.statusText, 'CRM');

            return false;
        });

    }
});

function excluirLinhaAdendoSubCliente(id) {

    if (isNumero(id))
        $('#adendo-subcliente-' + id).remove();
}

$('#btnAdicionarClienteGrupoAdendo').click(function () {

    var subClienteGrupo = $('#AdendoClienteGrupoCNPJId option:selected');

    var clienteGrupoId = subClienteGrupo.val();
    var clienteGrupoNome = subClienteGrupo.text();

    if (isNumero(clienteGrupoId)) {

        var linha =
            '<tr id="adendo-clientegrupo-' + clienteGrupoId + '">' +
            '   <td><input type="checkbox" name="AdendosClientesGrupoInclusao" id="AdendosClientesGrupoInclusao_' + clienteGrupoId + '"" value="' + clienteGrupoId + '" checked /></td>' +
            '   <td>' + clienteGrupoNome + '</td>' +
            '   <td><img src="' + urlBase + 'Content/img/excluir.png" onclick="excluirLinhaAdendoClienteGrupo(' + clienteGrupoId + ')" /></td>' +
            '</tr>';

        $('#tbAdendosInclusaoClientesGrupo > tbody:last-child').append(linha);
        $('#AdendoClienteGrupoCNPJId').empty();
    }
});

function excluirLinhaAdendoClienteGrupo(id) {

    if (isNumero(id))
        $('#adendo-clientegrupo-' + id).remove();
}

function ObterAdendosOportunidade(oportunidadeId) {

    $.get(urlBase + 'Oportunidades/ObterAdendosOportunidade?oportunidadeId=' + oportunidadeId, function (adendos) {

        $('#ListaAdendos').html(adendos);

    }).fail(function (data) {
        toastr.error(data.statusText, 'CRM');
    });

}

$("#btnSalvarAdendoSubCliente").click(function () {

    $(this).html('<i class="fa fa-spinner fa-spin"></i> aguarde...')
        .addClass('disabled');

});

adendosMensagemSucesso = function (data) {

    if (data.response) {

        var adendoObj = JSON.parse(data.response);

        if (adendoObj) {

            $.get(urlBase + 'Oportunidades/ValidarConcomitanciaAdendosCliente?oportunidadeId=' + adendoObj.OportunidadeId + '&adendoId=' + adendoObj.AdendoId, function (concomitancia) {

                if (concomitancia.Existe) {

                    $('#imgIconeConcomitancia')
                        .attr('src', urlBase + 'content/img/icone-integracao.png');

                    $("#msgPropostasConcomitantes").html(concomitancia.Mensagem);

                    $('#concomitancia-modal').modal('show');
                }

            }).fail(function (data) {
                toastr.error(data.statusText, 'CRM');
            }).always(function () {

                toastr.success('Informações atualizadas com sucesso!', 'CRM - Adendos');

                ObterAdendosOportunidade(adendoObj.OportunidadeId);

                $('#TipoAdendo').val('0');
                $('#tbAdendosInclusaoSubCliente > tbody').empty();
                $('#tbAdendosInclusaoClientesGrupo > tbody').empty();
                $('#pnlAdendosVendedor, #pnlAdendosExclusaoSubClientes, #pnlAdendosExclusaoGrupoCNPJ, #pnlAdendosFormaPagamento, #pnlAdendosInclusaoSubClientes, #pnlAdendosInclusaoGrupoCNPJ').addClass('invisivel');

                $("#btnSalvarAdendoSubCliente").html('Salvar')
                    .removeClass('disabled');

                console.log('reload: ' + adendoObj.HabilitaAbaFichas);

                if (adendoObj.HabilitaAbaFichas) {
                    setTimeout(function () {
                        location.hash = '#adendos';
                        location.reload();
                    }, 1000);
                }
            });
        }
    }

};

adendosAtualizacaoMensagemSucesso = function (data) {

    toastr.success('Informações atualizadas com sucesso!', 'CRM - Adendos');
}

$('#btnAnexarArquivo').click(function () {

    $(this).html('<i class="fa fa-spinner fa-spin"></i> aguarde...')
        .addClass('disabled');
});

anexosAtualizacaoMensagemSucesso = function (data) {

    toastr.success('Arquivo anexado com sucesso!', 'CRM - Anexos');
    $('#AnexosLoader').hide();
}

$("#btnRecallOportunidade").click(function () {

    event.preventDefault();

    $('#RecallOportunidadeId').val($('#Id').val());

    $('#modal-motivo-recall').modal('show');
});

$('#MotivoRecallOportunidade').keyup(function () {

    if ($('#MotivoRecallOportunidade').val().length > 0) {
        $('#btnConfirmaRecallOportunidade').removeAttr('disabled');
    } else {
        $('#btnConfirmaRecallOportunidade').attr('disabled', 'disabled');
    }
});

function recallFichaFaturamento(id) {

    event.preventDefault();

    $('#RecallFichasId').val(id);
    $('#RecallFichasOportunidadeId').val($('#Id').val());

    $('#modal-motivo-recall-fichas').modal('show');
}

$('#MotivoRecallFichas').keyup(function () {

    if ($('#MotivoRecallFichas').val().length > 0) {
        $('#btnConfirmaRecallFichas').removeAttr('disabled');
    } else {
        $('#btnConfirmaRecallFichas').attr('disabled', 'disabled');
    }
});

function recallPremioParceria(id) {

    event.preventDefault();

    $('#RecallPremioId').val(id);
    $('#RecallPremiosOportunidadeId').val($('#Id').val());

    $('#modal-motivo-recall-premios').modal('show');
}

$('#MotivoRecallPremios').keyup(function () {

    if ($('#MotivoRecallPremios').val().length > 0) {
        $('#btnConfirmaRecallPremios').removeAttr('disabled');
    } else {
        $('#btnConfirmaRecallPremios').attr('disabled', 'disabled');
    }
});

function recallPremioAdendo(id) {

    event.preventDefault();

    $('#RecallAdendoId').val(id);
    $('#RecallAdendosOportunidadeId').val($('#Id').val());

    $('#modal-motivo-recall-adendos').modal('show');
}

$('#MotivoRecallAdendos').keyup(function () {

    if ($('#MotivoRecallAdendos').val().length > 0) {
        $('#btnConfirmaRecallAdendos').removeAttr('disabled');
    } else {
        $('#btnConfirmaRecallAdendos').attr('disabled', 'disabled');
    }
});

$('#btnExportarTabelas').click(function () {

    var id = $('#Id').val();
    var lotes = $('#Lote').val();

    $('#confirmar-exportacaoTabela-modal')
        .data('id', id)
        .data('lotes', lotes)
        .modal('show');
});

function confirmarExportacaoTabela() {

    var id = $('#confirmar-exportacaoTabela-modal').data('id');
    var lotes = $('#confirmar-exportacaoTabela-modal').data('lotes');
    console.log(lotes);

    if (parseInt(id) > 0) {

        $("#btnConfirmarExportacaoTabela")
            .html('<i class="fa fa-spinner fa-spin"></i> aguarde...')
            .addClass('disabled');

        $.post(urlBase + 'Oportunidades/ExportarTabelas/', { id: id, lotes: lotes }, function (resultado) {
            $('#pnlTabelaGerada').html(resultado);
            $('#confirmar-exportacaoTabela-modal').modal('hide');
            $('#tabela-gerada-modal').modal('show');
        }).fail(function (data) {
            if (data.statusText) {
                toastr.error(data.statusText, 'CRM');
            } else {
                toastr.error('Falha ao gerar a Tabela de Cobranças', 'CRM');
            }
        }).always(function () {

            $("#btnConfirmarExportacaoTabela")
                .html('Estou ciente e confirmo a Exportação')
                .removeClass('disabled');
        });
    }
}

function exportarTabelaCancelada() {

    var id = $('#Id').val();

    $('#confirmar-cancelamento-tabela-modal')
        .data('id', id)
        .modal('show');
}

function confirmarExportacaoTabelaCancelada() {

    var id = $('#confirmar-cancelamento-tabela-modal').data('id');

    if (parseInt(id) > 0) {

        $("#btnConfirmarExportacaoTabelaCancelada")
            .html('<i class="fa fa-spinner fa-spin"></i> aguarde...')
            .addClass('disabled');

        $.post(urlBase + 'Oportunidades/ExportarTabelaCancelada/', { id: id }, function (resultado) {
            $('#pnlTabelaCancelada').html(resultado);
            $('#confirmar-cancelamento-tabela-modal').modal('hide');
            $('#tabela-cancelada-modal').modal('show');
        }).fail(function (data) {
            if (data.statusText) {
                toastr.error(data.statusText, 'CRM');
            } else {
                toastr.error('Falha ao cancelar a Tabela de Cobrança', 'CRM');
            }
        }).always(function () {

            $("#btnConfirmarExportacaoTabelaCancelada")
                .html('Estou ciente e confirmo o Cancelamento')
                .removeClass('disabled');
        });
    }
}


function abrirAuditoriaFichaFaturamento(id) {

    $('#auditoria-modal')
        .modal('show');

    $.get(urlBase + 'Auditoria/ObterLogsFichasFaturamento?oportunidadeId=' + id, function (resultado) {

        if (resultado) {

            $('#modal-tabela').html(resultado);

            var wrapper = document.querySelectorAll(".wrapper-logs-fichas");

            var jsonAnterior;

            wrapper.forEach(function (item) {

                var arr = [];
                var jsonStr = item.innerHTML;

                var json = JSON.parse(jsonStr);

                delete json.FaturadoContraId;
                delete json.IdFile;
                delete json.CondicaoPagamentoId;
                delete json.OportunidadeId;
                delete json.StatusOportunidade;
                delete json.ContaId;

                if ($(item).hasClass('wrapper-logs-fichas-update')) {

                    var jsonTemp = jsonCopy(json);

                    if (jsonAnterior != null) {

                        for (var key in json) {
                            if (json.hasOwnProperty(key)) {
                                if (json[key] !== jsonAnterior[key]) {

                                    if (jsonTemp[key] == null) {
                                        jsonTemp[key] = '<span class="campo-log-vermelho"></span>';
                                    } else {
                                        if (jsonTemp[key] != null) {
                                            if (jsonTemp[key].trim() != '') {
                                                jsonTemp[key] = '<span class="campo-log-verde">' + jsonTemp[key] + '</span>';
                                            } else {
                                                jsonTemp[key] = '<span class="campo-log-vermelho">' + jsonTemp[key] + '</span>';
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    jsonStr = JSON.stringify(jsonTemp);

                } else {
                    jsonStr = JSON.stringify(json);
                }

                jsonStr = replaceAll(jsonStr, 'null', '""');
                jsonStr = replaceAll(jsonStr, 'False', 'Não');
                jsonStr = replaceAll(jsonStr, 'True', 'Sim');
                jsonStr = replaceAll(jsonStr, '_', ' ');

                jsonStr = replaceAll(jsonStr, 'ContaId', 'Conta');
                jsonStr = replaceAll(jsonStr, 'NAO', 'Não');
                jsonStr = replaceAll(jsonStr, 'SIM', 'Sim');

                jsonStr = replaceAll(jsonStr, 'StatusFichaFaturamento', 'Status');
                jsonStr = replaceAll(jsonStr, 'FaturadoContra', 'Faturado Contra');
                jsonStr = replaceAll(jsonStr, 'DiasFaturamento', 'Dias Faturamento');
                jsonStr = replaceAll(jsonStr, 'DataCorte', 'Data Corte');
                jsonStr = replaceAll(jsonStr, 'CondicaoPagamento', 'Condição Pagamento');
                jsonStr = replaceAll(jsonStr, 'EmailFaturamento', 'Email Faturamento');
                jsonStr = replaceAll(jsonStr, 'ObservacoesFaturamento', 'Obs. Faturamento');
                jsonStr = replaceAll(jsonStr, 'AnexoFaturamentoId', 'Anexo');
                jsonStr = replaceAll(jsonStr, 'DataCorte', 'Data Corte');
                jsonStr = replaceAll(jsonStr, 'StatusDescricao', 'Status');

                arr.push(JSON.parse(jsonStr));
                jsonAnterior = json;

                arr.forEach(function (v) {
                    delete v.Id;
                });

                item.innerHTML = '';
                item.innerHTML = ConvertJsonToTable(arr, 'jsonTable', 'table table-bordered table-sm tabela-auditoria', 'Download');
            });

            $('#jsonTable tr td')
                .has("span.campo-log-verde")
                .addClass('campo-log-verde');

            $('#jsonTable tr td')
                .has("span.campo-log-vermelho")
                .addClass('campo-log-vermelho');
        }
    }).done(function () {
        montarTabelaAuditoria();
    });
}

function abrirAuditoriaPremiosParceria(id) {

    $('#auditoria-modal')
        .modal('show');

    $.get(urlBase + 'Auditoria/ObterLogsPremiosParceria?oportunidadeId=' + id, function (resultado) {

        if (resultado) {

            $('#modal-tabela').html(resultado);

            var wrapper = document.querySelectorAll(".wrapper-logs-premios");

            var jsonAnterior;

            wrapper.forEach(function (item) {

                var arr = [];
                var jsonStr = item.innerHTML;

                var json = JSON.parse(jsonStr);

                delete json.OportunidadeId;
                delete json.ContaId;
                delete json.ContatoId;
                delete json.Favorecido1;
                delete json.Favorecido2;
                delete json.Favorecido3;
                delete json.AnexoId;
                delete json.CriadoPor;
                delete json.DataCadastro;
                delete json.OportunidadeDescricao;
                delete json.StatusPremioParceria;
                delete json.StatusOportunidade;
                delete json.DocumentoFavorecido1;
                delete json.DocumentoFavorecido2;
                delete json.DocumentoFavorecido3;
                delete json.PremioReferenciaId;
                delete json.Instrucao;
                delete json.TipoOperacao;
                delete json.TipoServicoPremioParceria;

                if ($(item).hasClass('wrapper-logs-premios-update')) {

                    var jsonTemp = jsonCopy(json);

                    if (jsonAnterior != null) {

                        for (var key in json) {

                            if (json.hasOwnProperty(key)) {

                                if (json[key] !== jsonAnterior[key]) {

                                    if (jsonTemp[key] == null) {
                                        jsonTemp[key] = '<span class="campo-log-vermelho"></span>';
                                    } else {
                                        if (jsonTemp[key] != null) {
                                            if (jsonTemp[key].trim() != '') {
                                                jsonTemp[key] = '<span class="campo-log-verde">' + jsonTemp[key] + '</span>';
                                            } else {
                                                jsonTemp[key] = '<span class="campo-log-vermelho">' + jsonTemp[key] + '</span>';
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    jsonStr = JSON.stringify(jsonTemp);
                } else {
                    jsonStr = JSON.stringify(json);
                }

                jsonStr = replaceAll(jsonStr, 'null', '""');
                jsonStr = replaceAll(jsonStr, 'False', 'Não');
                jsonStr = replaceAll(jsonStr, 'True', 'Sim');
                jsonStr = replaceAll(jsonStr, '_', ' ');

                jsonStr = replaceAll(jsonStr, 'DescricaoContato', 'Contato');
                jsonStr = replaceAll(jsonStr, 'PremioReferenciaDescricao', 'Prêmio Ref.');
                jsonStr = replaceAll(jsonStr, 'NAO', 'Não');
                jsonStr = replaceAll(jsonStr, 'SIM', 'Sim');

                jsonStr = replaceAll(jsonStr, 'DescricaoStatusPremioParceria', 'Status');

                jsonStr = replaceAll(jsonStr, 'TipoServicoPremioParceria', 'Tipo Serviço');
                jsonStr = replaceAll(jsonStr, 'DescricaoServicoPremioParceria', 'Serviço');
                jsonStr = replaceAll(jsonStr, 'DescricaoFavorecido1', 'Favorecido 1');
                jsonStr = replaceAll(jsonStr, 'DescricaoFavorecido2', 'Favorecido 2');
                jsonStr = replaceAll(jsonStr, 'DescricaoFavorecido3', 'Favorecido 3');
                jsonStr = replaceAll(jsonStr, 'EmailFavorecido1', 'Email Favorecido 1');
                jsonStr = replaceAll(jsonStr, 'EmailFavorecido2', 'Email Favorecido 2');
                jsonStr = replaceAll(jsonStr, 'EmailFavorecido3', 'Email Favorecido 3');
                jsonStr = replaceAll(jsonStr, 'DescricaoInstrucao', 'Instrução');
                jsonStr = replaceAll(jsonStr, 'Observacoes', 'Observações');
                jsonStr = replaceAll(jsonStr, 'UrlPremio', 'Url Prêmio');
                jsonStr = replaceAll(jsonStr, 'DataUrlPremio', 'Data Url Prêmio');
                jsonStr = replaceAll(jsonStr, 'DescricaoTipoOperacao', 'Tipo Operação');

                arr.push(JSON.parse(jsonStr));
                jsonAnterior = json;

                arr.forEach(function (v) {
                    delete v.Id;
                });

                item.innerHTML = '';
                item.innerHTML = ConvertJsonToTable(arr, 'jsonTable', 'table table-bordered table-sm tabela-auditoria', 'Download');

            });

            $('#jsonTable tr td')
                .has("span.campo-log-verde")
                .addClass('campo-log-verde');

            $('#jsonTable tr td')
                .has("span.campo-log-vermelho")
                .addClass('campo-log-vermelho');
        }
    }).done(function () {
        montarTabelaAuditoria();
    });
}

function abrirAuditoriaAdendos(id) {

    $('#auditoria-modal')
        .modal('show');

    $.get(urlBase + 'Auditoria/ObterLogsAdendos?oportunidadeId=' + id, function (resultado) {

        if (resultado) {

            $('#modal-tabela').html(resultado);

            var wrapper = document.querySelectorAll(".wrapper-logs-adendos");

            var jsonAnterior;

            wrapper.forEach(function (item) {

                var arr = [];
                var jsonStr = item.innerHTML;

                var json = JSON.parse(jsonStr);

                delete json.OportunidadeDescricao;

                if ($(item).hasClass('wrapper-logs-adendos-update')) {

                    var jsonTemp = jsonCopy(json);

                    if (jsonAnterior != null) {

                        for (var key in json) {

                            if (json.hasOwnProperty(key)) {

                                if (json[key] !== jsonAnterior[key]) {

                                    if (jsonTemp[key] != null) {
                                        if (jsonTemp[key].trim() != '') {
                                            jsonTemp[key] = '<span class="campo-log-verde">' + jsonTemp[key] + '</span>';
                                        } else {
                                            jsonTemp[key] = '<span class="campo-log-vermelho">' + jsonTemp[key] + '</span>';
                                        }
                                    }
                                }
                            }
                        }
                    }

                    jsonStr = JSON.stringify(jsonTemp);

                } else {
                    jsonStr = JSON.stringify(json);
                }

                jsonStr = replaceAll(jsonStr, 'null', '""');
                jsonStr = replaceAll(jsonStr, 'False', 'Não');
                jsonStr = replaceAll(jsonStr, 'True', 'Sim');
                jsonStr = replaceAll(jsonStr, '_', ' ');
                jsonStr = replaceAll(jsonStr, 'TipoAdendo', 'Tipo');
                jsonStr = replaceAll(jsonStr, 'StatusAdendo', 'Status');
                jsonStr = replaceAll(jsonStr, 'FormaPagamento', 'Forma Pagamento');
                jsonStr = replaceAll(jsonStr, 'SubClientesInclusao', 'Sub Clientes (inclusão)');
                jsonStr = replaceAll(jsonStr, 'SubClientesExclusao', 'Sub Clientes (exclusão)');
                jsonStr = replaceAll(jsonStr, 'GruposCnpjInclusao', 'Grupos CNPJ (inclusão)');
                jsonStr = replaceAll(jsonStr, 'GruposCnpjExclusao', 'grupos CNPJ (exclusão)');
                jsonStr = replaceAll(jsonStr, 'CriadoPor', 'Criado Por');
                jsonStr = replaceAll(jsonStr, '#', '<br />');

                arr.push(JSON.parse(jsonStr));
                jsonAnterior = json;

                arr.forEach(function (v) {
                    delete v.Id;
                });

                item.innerHTML = '';
                item.innerHTML = ConvertJsonToTable(arr, 'jsonTable', 'table table-bordered table-sm tabela-auditoria', 'Download');
            });

            $('#jsonTable tr td')
                .has("span.campo-log-verde")
                .addClass('campo-log-verde');

            $('#jsonTable tr td')
                .has("span.campo-log-vermelho")
                .addClass('campo-log-vermelho');
        }
    }).done(function () {
        montarTabelaAuditoria();
    });
}

function abrirAuditoriaProposta(id) {

    $('#auditoria-modal')
        .modal('show');

    $.get(urlBase + 'Auditoria/ObterLogsProposta?oportunidadeId=' + id, function (resultado) {

        if (resultado) {

            $('#modal-tabela').html(resultado);

            var wrapper = document.querySelectorAll(".wrapper-logs-proposta");

            var jsonAnterior;

            wrapper.forEach(function (item) {

                var arr = [];
                var jsonStr = item.innerHTML;

                var json = JSON.parse(jsonStr);

                delete json.SallesId;
                delete json.Valido;
                delete json.Invalido;
                delete json.UltimaAlteracao;
                delete json.Cancelado;
                delete json.UltimaAlteracao;
                delete json.AlteradoPor;
                delete json.DataCriacao;
                delete json.CriadoPor;
                delete json.PremioParceria;
                delete json.CIFMedio;
                delete json.VolumeMensal;
                delete json.FaturamentoMensalFCL;
                delete json.FaturamentoMensalLCL;
                delete json.Observacao;
                delete json.MercadoriaId;
                delete json.TipoOperacaoOportunidade;
                delete json.TipoNegocio;
                delete json.TipoDeProposta;
                delete json.TipoServico;
                delete json.MotivoPerda;
                delete json.StatusOportunidade;
                delete json.EstagioNegociacao;
                delete json.Segmento;
                delete json.ClassificacaoCliente;
                delete json.SucessoNegociacao;
                delete json.Probabilidade;
                delete json.ContatoId;
                delete json.Descricao;
                delete json.Aprovada;
                delete json.EmpresaId;
                delete json.ContaId;
                delete json.Identificacao;
                delete json.ConsultaTabela;

                if ($(item).hasClass('wrapper-logs-proposta-update')) {

                    var jsonTemp = jsonCopy(json);

                    if (jsonAnterior != null) {

                        for (var key in json) {

                            if (json.hasOwnProperty(key)) {

                                if (json[key] !== jsonAnterior[key]) {

                                    if (jsonTemp[key] != null) {
                                        if (jsonTemp[key].trim() != '') {
                                            jsonTemp[key] = '<span class="campo-log-verde">' + jsonTemp[key] + '</span>';
                                        } else {
                                            jsonTemp[key] = '<span class="campo-log-vermelho">' + jsonTemp[key] + '</span>';
                                        }
                                    }
                                }
                            }
                        }
                    }

                    jsonStr = JSON.stringify(jsonTemp);

                } else {
                    jsonStr = JSON.stringify(json);
                }

                jsonStr = replaceAll(jsonStr, 'DescricaoTipoOperacao', 'Tipo Operação');
                jsonStr = replaceAll(jsonStr, 'DataInicio', 'Início');
                jsonStr = replaceAll(jsonStr, 'DataTermino', 'Término');
                jsonStr = replaceAll(jsonStr, 'ModeloDescricao', 'Modelo');
                jsonStr = replaceAll(jsonStr, 'FormaPagamento', 'Forma Pagamento');
                jsonStr = replaceAll(jsonStr, 'DiasFreeTime', 'Dias Free Time');
                jsonStr = replaceAll(jsonStr, 'QtdeDias', 'Qtde. Dias');
                jsonStr = replaceAll(jsonStr, 'TipoValidade', 'Tipo Validade');
                jsonStr = replaceAll(jsonStr, 'VendedorDescricao', 'Vendedor');
                jsonStr = replaceAll(jsonStr, 'ImpostoDescricao', 'Imposto');
                jsonStr = replaceAll(jsonStr, 'ImpostoDescricao', 'Imposto');

                arr.push(JSON.parse(jsonStr));
                jsonAnterior = json;

                arr.forEach(function (v) {
                    delete v.Id;
                });

                item.innerHTML = '';
                item.innerHTML = ConvertJsonToTable(arr, 'jsonTable', 'table table-bordered table-sm tabela-auditoria', 'Download');
            });

            $('#jsonTable tr td')
                .has("span.campo-log-verde")
                .addClass('campo-log-verde');

            $('#jsonTable tr td')
                .has("span.campo-log-vermelho")
                .addClass('campo-log-vermelho');
        }
    }).done(function () {
        montarTabelaAuditoria();
    });
}

function abrirAuditoriaAnexos(id) {

    $('#auditoria-modal')
        .modal('show');

    $.get(urlBase + 'Auditoria/ObterLogsAnexos?oportunidadeId=' + id, function (resultado) {

        if (resultado) {

            $('#modal-tabela').html(resultado);

            var wrapper = document.querySelectorAll(".wrapper-logs-anexos");

            var jsonAnterior;

            wrapper.forEach(function (item) {

                var arr = [];
                var jsonStr = item.innerHTML;

                var json = JSON.parse(jsonStr);

                delete json.IdProcesso;
                delete json.IdFile;
                delete json.TipoAnexo;

                if ($(item).hasClass('wrapper-logs-anexos-update')) {

                    var jsonTemp = jsonCopy(json);

                    if (jsonAnterior != null) {

                        for (var key in json) {

                            if (json.hasOwnProperty(key)) {

                                if (json[key] !== jsonAnterior[key]) {

                                    if (jsonTemp[key] != null) {
                                        if (jsonTemp[key].trim() != '') {
                                            jsonTemp[key] = '<span class="campo-log-verde">' + jsonTemp[key] + '</span>';
                                        } else {
                                            jsonTemp[key] = '<span class="campo-log-vermelho">' + jsonTemp[key] + '</span>';
                                        }
                                    }
                                }
                            }
                        }
                    }

                    jsonStr = JSON.stringify(jsonTemp);

                } else {
                    jsonStr = JSON.stringify(json);
                }

                jsonStr = replaceAll(jsonStr, 'DataCadastro', 'Data Cadastro');
                jsonStr = replaceAll(jsonStr, 'CriadoPor', 'Criado Por');
                jsonStr = replaceAll(jsonStr, 'TipoAnexoDescricao', 'Tipo Anexo');

                arr.push(JSON.parse(jsonStr));
                jsonAnterior = json;

                arr.forEach(function (v) {
                    delete v.Id;
                });

                item.innerHTML = '';
                item.innerHTML = ConvertJsonToTable(arr, 'jsonTable', 'table table-bordered table-sm tabela-auditoria', 'Download');
            });

            $('#jsonTable tr td')
                .has("span.campo-log-verde")
                .addClass('campo-log-verde');

            $('#jsonTable tr td')
                .has("span.campo-log-vermelho")
                .addClass('campo-log-vermelho');
        }
    }).done(function () {
        montarTabelaAuditoria();
    });
}

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

$("#btnClonarProposta").click(function () {

    event.preventDefault();

    $(document).ready(function () {
        $('#tbClonarProposta').DataTable({
            destroy: true,
            "bLengthChange": false,
            "bServerSide": true,
            "sAjaxSource": urlBase + "Oportunidades/Consultar",
            "bProcessing": true,
            "pagingType": "full_numbers",
            "searchDelay": 1000,
            "pageLength": 10,
            "createdRow": function (row, data, dataIndex) {
                $(row).attr('id', 'item-proposta-clone-' + data.Id);
            },
            "aoColumns":
                [
                    {
                        "sName": "Identificacao",
                        "mData": "Identificacao"
                    },
                    {
                        "sName": "Descricao",
                        "render": function (data, type, row) {
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
                        "render": function (data, type, row) {
                            return '<a href="#" onclick="selecionarPropostaClone(' + row.Id + ')"><i class="far fa-check-circle"></i></a>';
                        },
                        "className": "text-center"
                    },
                    {
                        "render": function (data, type, row) {
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

    $('#clonar-proposta-modal').modal('show');

});

function selecionarPropostaClone(id) {

    event.preventDefault();

    $('#CloneOportunidadeSelecionada').val(id);

    $('#tbClonarProposta > tbody  > tr').removeClass('linha-selecionada');

    $('#item-proposta-clone-' + id).addClass('linha-selecionada');

    $('#btnConfirmarClonagemProposta').prop("disabled", false);
}

$('#btnConfirmarClonagemProposta').click(function () {

    event.preventDefault();

    var origem = $('#CloneOportunidadeSelecionada').val();
    var destino = $('#Id').val();

    if (isNumero(origem) && isNumero(destino)) {
        $.post(urlBase + 'Oportunidades/ClonarProposta/', { oportunidadeOrigem: origem, oportunidadeDestino: destino }, function () {
            toastr.success('Proposta clonada com sucesso', 'CRM');
            //atualizarPreview();

            setTimeout(function () {
                location.hash = '#proposta';
                location.reload();
            }, 1000);

        }).fail(function (data) {
            if (data.statusText) {
                toastr.error(data.statusText, 'CRM');
            } else {
                toastr.error('Falha ao clonar a Proposta', 'CRM');
            }
        });
    }
});

simuladorMensagemSucesso = function (resultado) {

    toastr.success('Parâmetros Simulador atualizados com sucesso', 'CRM');
};

function simularOportunidade(oportunidadeId, id, modeloSimuladorId) {

    event.preventDefault();

    $('#lnkSimular-' + id)
        .html('<i class="fa fa-spinner fa-spin"></i> aguarde...')
        .prop('disabled', true);

    $.post(urlBase + 'SimuladorProposta/SimularOportunidade/', { simuladorOportunidadeId: oportunidadeId, simuladorParametroId: id, modeloSimuladorId: modeloSimuladorId }, function (resultado) {

        $('#AbaSimuladorResultado').html(resultado);

    }).fail(function (data) {
        if (data.statusText) {
            toastr.error(data.statusText, 'CRM');
        } else {
            toastr.error('Falha ao simular a Proposta', 'CRM');
        }
    }).always(function () {
        $('#lnkSimular-' + id)
            .html('<i class="fa fa-check">  Simular</i>')
            .prop('disabled', false);
    });
}

function excluirParametroSimulador(id) {

    event.preventDefault();

    $('#lnkExcluir-' + id)
        .html('<i class="fa fa-spinner fa-spin"></i> aguarde...')
        .prop('disabled', true);

    $.post(urlBase + 'SimuladorProposta/ExcluirParametroSimulador/', { id: id }, function () {

        $('#item-parametro-simulador-' + id).remove();

    }).fail(function (data) {
        if (data.statusText) {
            toastr.error(data.statusText, 'CRM');
        } else {
            toastr.error('Falha ao excluir o parâmetro do simulador', 'CRM');
        }
    }).always(function () {
        $('#lnkExcluir-' + id)
            .html('<i class="fa fa-check">  Excluir</i>')
            .prop('disabled', false);
    });
}

$('#RevisarFichaFaturamento').click(function () {

    var id = $('#Id').val();

    if (isNumero(id)) {
        $.get(urlBase + 'Oportunidades/ObterFichaFaturamentoAtiva/?oportunidadeId=' + id, function (resultado) {
            if (resultado) {

                if (resultado.multipos) {

                    $('#pnlMultiplasFichas').removeClass('invisivel');

                    $('#ddlFichasAtivas').empty();

                    resultado.fichasAtivas.forEach(function (item) {
                        $('#ddlFichasAtivas')
                            .append($('<option>', {
                                value: item.Id, text: 'F-' + item.Id + ' | ' + item.Cliente
                            }));
                    });
                } else {

                    var fichaId = resultado.fichasAtivas.Id;

                    if (isNumero(fichaId)) {

                        limparFichasFaturamento();

                        carregarDetalhesFichaFaturamento(fichaId);

                        $('#FichaRevisaoId').val(fichaId);
                    }
                }
            }

            $('#confirmar-revisaoFicha-modal')
                .data('id', id)
                .modal('show');

        }).fail(function (data) {
            if (data.statusText) {
                toastr.error(data.statusText, 'CRM');
            } else {
                toastr.error('Falha ao importar o layout', 'CRM');
            }
        });
    }
});

function confirmarRevisaoFicha() {

    $('#confirmar-revisaoFicha-modal')
        .data('id', '0')
        .modal('hide');

    var valor = $('#ddlFichasAtivas').val();

    if (isNumero(valor)) {

        limparFichasFaturamento();
        carregarDetalhesFichaFaturamento(valor);

        $('#FichaRevisaoId').val(valor);
    }
}

var oportunidadeMensagemAdendoErro = function (xhr, status) {

    if (xhr !== null && xhr.responseText !== '') {

        try {
            var retorno = JSON.parse(xhr.responseText);

            if (retorno) {
                if (retorno.BloqueiaInclusaoSubCliente) {

                    if (parseInt(retorno.SubClienteId) > 0) {

                        $('#ClientePropostaSelecionadoId')
                            .empty()
                            .append(
                                $('<option>', {
                                    value: retorno.SubClienteId,
                                    text: retorno.DescricaoSubCliente + ' (' + retorno.DocumentoSubCliente + ')'
                                }));

                        $('#ClientePropostaSelecionadoId').select2('val', [retorno.SubClienteId]);

                        $('[href="#fichaDeFaturamento"]').tab('show');

                        toastr.info('É necessário cadastrar a ficha de faturamento para continuar', 'CRM');
                    }

                }
            }
        } catch (e) {
            console.log('Json inválido');
        }
    } else {
        toastr.error(xhr.statusText, 'CRM');
    }
}

window.addEventListener("submit", function (e) {

    var form = e.target;

    if (form.getAttribute("enctype") === "multipart/form-data") {

        if (form.id === 'frmOportunidadesFichaFaturamento') {

            $("#btnCadastrarFichaFaturamento")
                .html('<i class="fa fa-spinner fa-spin"></i> aguarde...')
                .prop('disabled', true)
        }

        if (form.dataset.ajax) {
            e.preventDefault();
            e.stopImmediatePropagation();
            var xhr = new XMLHttpRequest();
            xhr.open(form.method, form.action);
            xhr.onreadystatechange = function () {
                if (xhr.readyState == 4) {
                    if (xhr.status == 200) {

                        if (form.id === 'frmOportunidadesAdendos') {
                            adendosMensagemSucesso(xhr);
                        }

                        if (form.id === 'frmAtualizacaoOportunidadesAdendos') {
                            adendosAtualizacaoMensagemSucesso();
                        }

                        if (form.id === 'frmOportunidadesAnexos') {
                            anexosAtualizacaoMensagemSucesso();
                        }

                        if (form.id === 'frmOportunidadesFichaFaturamento') {
                            fichasFaturamentoMensagemSucesso();
                        }

                        if (form.id === 'frmOportunidadesPremiosParceria') {
                            premioParceriaMensagemSucesso();
                        }

                        if (form.dataset.ajaxUpdate) {
                            var updateTarget = document.querySelector(form.dataset.ajaxUpdate);
                            if (updateTarget) {
                                updateTarget.innerHTML = xhr.responseText;
                            }
                        }
                    } else if (xhr.status == 400) {
                        if (form.id === 'frmOportunidadesAdendos' || form.id === 'frmAtualizacaoOportunidadesAdendos') {
                            oportunidadeMensagemAdendoErro(xhr, '');
                        } else {
                            oportunidadeMensagemErro(xhr, '');
                        }
                    }

                    if (form.id === 'frmOportunidadesFichaFaturamento') {
                        $("#btnCadastrarFichaFaturamento").prop('disabled', false)
                            .html('Salvar');
                    }

                    if (form.id === 'frmOportunidadesAnexos') {
                        $("#btnAnexarArquivo").removeClass('disabled')
                            .html('Salvar');
                    }

                    if (form.id === 'frmOportunidadesAdendos' || form.id === 'frmAtualizacaoOportunidadesAdendos') {

                        $("#btnSalvarAdendoFormaPgto").removeClass('disabled')
                            .html('Salvar');

                        $("#btnAdendoExclusaoSubCliente").removeClass('disabled')
                            .html('Salvar');

                        $("#btnAdendoExclusaoGrupoCnpj").removeClass('disabled')
                            .html('Salvar');

                        $("#btnSalvarAdendoSubCliente").removeClass('disabled')
                            .html('Salvar');

                    }

                    if (form.id === 'frmOportunidadesPremiosParceria') {

                        $("#btnSalvarPremioParceria").removeClass('disabled')
                            .html('Salvar');
                    }
                }
            };
            xhr.send(new FormData(form));
        }
    }
}, true);

$('#Lote').on('keypress keyup blur', function (event) {
    $(this).val($(this).val().replace(/[^0-9\,]/g, ''));
});

function selecionaLotesProposta() {
    $('#pesquisa-lotes-proposta').modal('show');
    $('.controle-input-tags').prop('readonly', false);
}

