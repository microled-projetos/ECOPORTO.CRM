using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface ICargoRepositorio
    {
        int Cadastrar(Cargo cargo);
        void Atualizar(Cargo cargo);
        IEnumerable<Cargo> ObterCargos();
        Cargo ObterCargoPorId(int id);
        void Excluir(int id);
        IEnumerable<Cargo> ObterCargoPorDescricao(string descricao);
    }
}
