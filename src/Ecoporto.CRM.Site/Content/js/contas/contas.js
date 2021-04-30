$('#Documento').unmask();

moment.updateLocale(
    moment.locale('pt-br'), {
        invalidDate: ''
    });

window.addEventListener('focus', function (event) {

    var id = $('#Id').val();

    if (id === undefined)
        return;

    var tabela = $('#tbContatos');

    $.get('/Contas/ObterContatosPorConta/?contaId=' + id, function (resultado) {

        var contas = resultado.dados;
        var conteudo = '';

        if (contas === null || contas === undefined)
            return;

        $('#tbContatos tbody').empty();

        contas.forEach(function (item) {

            var nome = item.Nome || '';
            var sobrenome = item.Sobrenome || '';
            var telefone = item.Telefone || '';
            var celular = item.Celular || '';
            var cargo = item.Cargo || '';
            var departamento = item.Departamento || '';
            var dataNascimento = moment(item.DataNascimento).locale('pt-br').format('L');
            var email = item.Email || '';

            var status = item.Status === 1
                ? '<span class="badge badge-success">Ativo</span>'
                : '<span class="badge badge-danger">Inativo</span>';

            conteudo = conteudo +
                '<tr id="item-contato-' + item.Id + '">' +
                '   <td>' + nome + '</td>' +
                '   <td>' + sobrenome + '</td>' +
                '   <td>' + telefone + '</td>' +
                '   <td>' + celular + '</td>' +
                '   <td>' + cargo + '</td>' +
                '   <td>' + departamento + '</td>' +
                '   <td>' + dataNascimento + '</td>' +
                '   <td>' + email + '</td>' +
                '   <td>' + status + '</td>' +
                '   <td class="campo-acao"><a href="/Contatos/Atualizar/' + item.Id + '?conta=' + item.ContaId + '" target="_blank" class="btn btn-info btn-sm btn-acao"><i class="fa fa-edit"></i></a></td>' +
                '   <td class="campo-acao"><a href="#" onclick="excluirContato(' + item.Id + ')" class="btn btn-danger btn-sm btn-acao"><i class="fa fa-trash"></i></a></td>' +
                '</tr >';
        });

        $("#tbContatos tbody").append(conteudo);
    });

});

$('#Estado').change(function () {

    $.get('/Contas/ObterCidadesPorEstado?estado=' + $(this).val(), function (resultado) {

        var select = $('#CidadeId');
        select.html('');

        $.each(resultado, function (key, value) {
            select.append('<option value=' + value.Id + '>' + value.Descricao + '</option>');
        });

    });
});

if ($('#ClassificacaoFiscal').val() === '1') {
    $('#Documento').mask('000.000.000-00');
} else if ($('#ClassificacaoFiscal').val() === '2') {
    $('#Documento').mask('00.000.000/0000-00');
} else {
    $('#Documento').unmask();
    $('#Documento').attr('disabled', 'disabled');
}

$('#ClassificacaoFiscal').change(function () {

    var tipo = $(this).val();

    $('#Documento').val('');
    $('#Documento').removeAttr('disabled', 'disabled');

    if (tipo === '1') {
        $('#Documento').mask('000.000.000-00');
    } else if (tipo === '2') {
        $('#Documento').mask('00.000.000/0000-00');
    } else {
        $('#Documento').unmask();
        $('#Documento').attr('disabled', 'disabled');
    }

});

$('#Documento').blur(function () {

    $.get('/Contas/ObterClientePorDocumento?documento=' + $(this).val(), function (resultado) {

        if (resultado !== null) {

            $('#NomeFantasia').val(resultado.NomeFantasia);
            $('#InscricaoEstadual').val(resultado.InscricaoEstadual);
            $('#Logradouro').val(resultado.Logradouro);
            $('#Numero').val(resultado.Numero);
            $('#Bairro').val(resultado.Bairro);
            $('#Estado').val(resultado.Estado);
            $('#Complemento').val(resultado.Complemento);
        }
    });
});

$('#abas a').on('click', function (e) {
    e.preventDefault();
    $(this).tab('show');
});

function excluirContato(contatoId, contaId) {

    $.get(urlBase + 'Contas/ValidarAcessoAtualizacaoExclusaoContato?contaId=' + contaId, function (resultado) {

        $('#excluir-contato-modal').data('id', contatoId).modal('show');

    }).fail(function (data) {
        if (data.statusText) {
            toastr.error(data.statusText, 'CRM');
        } else {
            toastr.error('Erro inesperado', 'CRM');
        }
    });
}

function confirmarExclusaoContato() {

    var id = $('#excluir-contato-modal').data('id');

    if (isNumero(id)) {
        $.post(urlBase + 'Contatos/Excluir/', { id: id }).done(function (data) {
            toastr.success('Contato excluído com sucesso!', 'CRM');
            $('#item-contato-' + id).remove();
        }).fail(function (data) {
            if (data.statusText) {
                toastr.error(data.statusText, 'CRM');
            } else {
                toastr.error('O registro não pode ser excluído', 'CRM');
            }
        }).always(function () {
            $('#excluir-contato-modal').data('id', '0').modal('hide');
        });
    }
}

function atualizarOportunidade(oportunidadeId, contaId) {

    var id = $('#Id').val();

    if (isNumero(oportunidadeId) && isNumero(contaId)) {
        $.get(urlBase + 'Contas/ValidarAcessoVisualizacaoOportunidade?contaId=' + contaId, function (resultado) {

            var a = document.createElement('a');

            a.target = "_blank";
            a.href = urlBase + 'Oportunidades/Atualizar/' + oportunidadeId;
            a.click();

        }).fail(function (data) {
            if (data.statusText) {
                toastr.error(data.statusText, 'CRM');
            } else {
                toastr.error('Erro inesperado', 'CRM');
            }
        });
    }
}


function excluirOportunidade(oportunidadeId, contaId) {

    $.get(urlBase + 'Contas/ValidarAcessoAtualizacaoExclusaoOportunidade?contaId=' + contaId, function (resultado) {

        $('#excluir-oportunidade-modal').data('id', oportunidadeId).modal('show');

    }).fail(function (data) {
        if (data.statusText) {
            toastr.error(data.statusText, 'CRM');
        } else {
            toastr.error('Erro inesperado', 'CRM');
        }
    });
}

function confirmarExclusaoOportunidade() {

    var id = $('#excluir-oportunidade-modal').data('id');

    if (isNumero(id)) {
        $.post(urlBase + 'Oportunidades/Excluir/', { id: id }).done(function (data) {
            toastr.success('Oportunidade excluída com sucesso!', 'CRM');
            $('#item-oportunidade-' + id).remove();
            $('#item-oportunidade-ativa-' + id).remove();
        }).fail(function (data) {
            if (data.statusText) {
                toastr.error(data.statusText, 'CRM');
            } else {
                toastr.error('O registro não pode ser excluído', 'CRM');
            }
        }).always(function () {
            $('#excluir-oportunidade-modal').data('id', '0').modal('hide');
        });
    }
}

$('#btnCadastrarContato').click(function () {

    var id = $('#Id').val();

    if (isNumero(id)) {
        $.get(urlBase + 'Contas/ValidarAcessoAtualizacaoExclusaoContato?contaId=' + id, function (resultado) {

            var a = document.createElement('a');

            a.target = "_blank";
            a.href = urlBase + 'Contatos/Cadastrar?conta=' + id;
            a.click();

        }).fail(function (data) {
            if (data.statusText) {
                toastr.error(data.statusText, 'CRM');
            } else {
                toastr.error('Erro inesperado', 'CRM');
            }
        });
    }
});

$('#btnCadastrarOportunidade').click(function () {

    var id = $('#Id').val();

    if (isNumero(id)) {
        $.get(urlBase + 'Contas/ValidarAcessoAtualizacaoExclusaoOportunidade?contaId=' + id, function (resultado) {

            var a = document.createElement('a');

            a.target = "_blank";
            a.href = urlBase + 'Oportunidades/Cadastrar?conta=' + id;
            a.click();

        }).fail(function (data) {
            if (data.statusText) {
                toastr.error(data.statusText, 'CRM');
            } else {
                toastr.error('Erro inesperado', 'CRM');
            }
        });
    }
});

function cadastrarRangeIPs() {

    $('#vincular-ips-modal')
        .modal('show');
}

function excluirVinculoIP(id) {

    if (isNumero(id)) {
        $.post(urlBase + 'Contas/ExcluirRangeIP/', { id: id }).done(function (data) {
            toastr.success('Vínculo excluído com sucesso!', 'CRM');
            $('#item-vinculo-' + id).remove();
        }).fail(function (data) {
            if (data.statusText) {
                toastr.error(data.statusText, 'CRM');
            } else {
                toastr.error('O registro não pode ser excluído', 'CRM');
            }
        })
    }
}