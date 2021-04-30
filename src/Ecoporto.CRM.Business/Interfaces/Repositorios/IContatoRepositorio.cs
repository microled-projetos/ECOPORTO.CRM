using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface IContatoRepositorio
    {
        int Cadastrar(Contato contato);
        void Atualizar(Contato contato);
        IEnumerable<Contato> ObterContatosPorConta(int contaId);
        IEnumerable<Contato> ObterTodosContatos();
        IEnumerable<Contato> ObterContatosPorDescricao(string descricao, int? usuarioId);
        IEnumerable<ContatoDTO> ObterContatosEContaPorDescricao(string descricao, int? usuarioId);
        Contato ObterContatoPorId(int id);
        void Excluir(int id);
    }
}
