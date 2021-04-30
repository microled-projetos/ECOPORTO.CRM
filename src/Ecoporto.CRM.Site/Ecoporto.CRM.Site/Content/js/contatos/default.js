$("#btnPesquisarContas").click(function () {

    event.preventDefault();

    $('#pesquisa-modal-contas')
        .data('toggle', 'ContaId')
        .modal('show');
});

function selecionarConta(id, descricao) {

    var toggle = $('#pesquisa-modal-contas').data('toggle');

    $('#pesquisa-modal-contas').modal('hide');
    
    $("#" + toggle)
        .empty()
        .append($('<option>', {
            value: id,
            text: descricao
        })).focus();

    $("#ListaContas").empty();
}