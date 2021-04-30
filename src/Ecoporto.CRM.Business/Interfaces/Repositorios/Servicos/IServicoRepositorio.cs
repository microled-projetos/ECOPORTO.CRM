using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface IServicoRepositorio
    {
        int Cadastrar(Servico servico);
        void Atualizar(Servico servico);
        void Excluir(int id);
        IEnumerable<Servico> ObterServicos();
        Servico ObterServicoPorId(int id);
        IEnumerable<ServicoFaturamento> ObterServicosVinculados(int servicoId);
        Servico ObterServicoPorDescricao(string descricao, int? id = 0);
        IEnumerable<Servico> ObterServicosPorDescricao(string descricao);
    }
}
