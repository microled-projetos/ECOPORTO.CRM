using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface IMercadoriaRepositorio
    {
        int Cadastrar(Mercadoria mercadoria);
        void Atualizar(Mercadoria mercadoria);
        IEnumerable<Mercadoria> ObterMercadorias();
        Mercadoria ObterMercadoriaPorId(int id);
        void Excluir(int id);
        IEnumerable<Mercadoria> ObterMercadoriaPorDescricao(string descricao);
    }
}
