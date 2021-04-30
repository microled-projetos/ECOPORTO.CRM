function montarTabelaAuditoria() {

    var wrapper = document.querySelectorAll(".wrapper");

    wrapper.forEach(function (item) {

        var arr = [];
        var jsonStr = item.innerHTML;

        jsonStr = replaceAll(jsonStr, 'null', '""');
        jsonStr = replaceAll(jsonStr, 'False', 'Não');
        jsonStr = replaceAll(jsonStr, 'True', 'Sim');
        jsonStr = replaceAll(jsonStr, '_', ' ');

        jsonStr = replaceAll(jsonStr, 'Identificacao', 'Identificação');
        jsonStr = replaceAll(jsonStr, 'Descricao', 'Descrição');
        jsonStr = replaceAll(jsonStr, 'SucessoNegociacao', 'Sucesso Negociação');
        jsonStr = replaceAll(jsonStr, 'ClassificacaoCliente', 'Classificação Cliente');
        jsonStr = replaceAll(jsonStr, 'EstagioNegociacao', 'Estágio Negociação');
        jsonStr = replaceAll(jsonStr, 'StatusOportunidade', 'Status Oportunidade');
        jsonStr = replaceAll(jsonStr, 'MotivoPerda', 'Motivo Perda');
        jsonStr = replaceAll(jsonStr, 'TipoServico', 'Tipo Serviço');
        jsonStr = replaceAll(jsonStr, 'TipoDeProposta', 'Tipo Proposta');
        jsonStr = replaceAll(jsonStr, 'TipoNegocio', 'Tipo Negócio');
        jsonStr = replaceAll(jsonStr, 'TipoOperacaoOportunidade', 'Tipo Operação');
        jsonStr = replaceAll(jsonStr, 'FaturamentoMensalLCL', 'Fat. Mensal LCL');
        jsonStr = replaceAll(jsonStr, 'FaturamentoMensalFCL', 'Fat. Mensal FCL');
        jsonStr = replaceAll(jsonStr, 'VolumeMensal', 'Volume Mensal');
        jsonStr = replaceAll(jsonStr, 'CIFMedio', 'CIF Médio');
        jsonStr = replaceAll(jsonStr, 'PremioParceria', 'Prêmio Parceria');
        jsonStr = replaceAll(jsonStr, 'ContaId', 'Conta');
        jsonStr = replaceAll(jsonStr, 'ContatoId', 'Contato');
        jsonStr = replaceAll(jsonStr, 'EmpresaId', 'Empresa');
        jsonStr = replaceAll(jsonStr, 'MercadoriaId', 'Mercadoria');
        jsonStr = replaceAll(jsonStr, 'VendedorId', 'Vendedor');
        jsonStr = replaceAll(jsonStr, 'ImpostoId', 'Imposto');
        jsonStr = replaceAll(jsonStr, 'NAO', 'Não');
        jsonStr = replaceAll(jsonStr, 'SIM', 'Sim');
        jsonStr = replaceAll(jsonStr, 'NomeFantasia', 'Nome Fantasia');
        jsonStr = replaceAll(jsonStr, 'InscricaoEstadual', 'Inscrição Estadual');
        jsonStr = replaceAll(jsonStr, 'SituacaoCadastral', 'Situação Cadastral');
        jsonStr = replaceAll(jsonStr, 'ClassificacaoFiscal', 'Classificação Fiscal');
        jsonStr = replaceAll(jsonStr, 'TipoSolicitacao', 'Tipo Solicitação');
        jsonStr = replaceAll(jsonStr, 'StatusSolicitacao', 'Status Solicitação');
        jsonStr = replaceAll(jsonStr, 'UnidadeSolicitacao', 'Unidade Solicitação');
        jsonStr = replaceAll(jsonStr, 'AreaOcorrenciaSolicitacao', 'Área Ocorrência');
        jsonStr = replaceAll(jsonStr, 'TipoOperacao', 'Tipo Operação');
        jsonStr = replaceAll(jsonStr, 'ValorDevido', 'Valor Devido');
        jsonStr = replaceAll(jsonStr, 'ValorCobrado', 'Valor Cobrado');
        jsonStr = replaceAll(jsonStr, 'ValorCredito', 'Valor Crédito');
        jsonStr = replaceAll(jsonStr, 'HabilitaValorDevido', 'Habilita Valor Devido');
        jsonStr = replaceAll(jsonStr, 'ATIVO', 'Ativo');
        jsonStr = replaceAll(jsonStr, 'DiasFreeTime', 'Dias Free Time');
        jsonStr = replaceAll(jsonStr, 'QtdeDias', 'Qtde Dias');
        jsonStr = replaceAll(jsonStr, 'TipoValidade', 'Tipo Validade');
        jsonStr = replaceAll(jsonStr, 'RecintoAlfandegado', 'Recinto Alfandegado');

        jsonStr = replaceAll(jsonStr, 'StatusFichaFaturamento', 'Status');
        jsonStr = replaceAll(jsonStr, 'FaturadoContraId', 'Faturado Contra');
        jsonStr = replaceAll(jsonStr, 'DiasFaturamento', 'Dias Faturamento');
        jsonStr = replaceAll(jsonStr, 'DataCorte', 'Data Corte');
        jsonStr = replaceAll(jsonStr, 'CondicaoPagamento', 'Condição Pagamento');
        

        jsonStr = replaceAll(jsonStr, 'EmailFaturamento', 'Email Faturamento');
        jsonStr = replaceAll(jsonStr, 'ObservacoesFaturamento', 'Obs. Faturamento');
        jsonStr = replaceAll(jsonStr, 'AnexoFaturamentoId', 'Anexo');
        jsonStr = replaceAll(jsonStr, 'DataCorte', 'Data Corte');

        var json = JSON.parse(jsonStr);

        delete json.DataCriacao;
        delete json.UltimaAlteracao;
        delete json.SallesId;
        delete json.Valido;
        delete json.Invalido;
        delete json.CriadoPor;
        delete json.AlteradoPor;
        delete json.DataCadastro;
        delete json.IdFile;
        delete json.CondicaoPagamentoId;

        arr.push(json);

        arr.forEach(function (v) {
            delete v.Id;
        });

        item.innerHTML = '';
        item.innerHTML = ConvertJsonToTable(arr, 'jsonTable', 'table table-bordered table-sm tabela-auditoria', 'Download');
    });
}

function replaceAll(str, find, replace) {
    return str.replace(new RegExp(find, 'g'), replace);
}