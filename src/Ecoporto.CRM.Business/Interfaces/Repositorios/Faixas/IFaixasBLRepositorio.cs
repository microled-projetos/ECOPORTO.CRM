using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface IFaixasBLRepositorio
    {
        void Cadastrar(FaixaBL faixa);
        void Atualizar(FaixaBL faixa);
        void Excluir(int id);
        IEnumerable<FaixaBL> ObterFaixasBL(int layoutId);
        FaixaBL ObterPorId(int id);
    }
}
