using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface IFaixasCIFRepositorio
    {    
        void Cadastrar(FaixaCIF faixa);
        void Atualizar(FaixaCIF faixa);
        void Excluir(int id);
        IEnumerable<FaixaCIF> ObterFaixasCIF(int layoutId);
        FaixaCIF ObterPorId(int id);
    }
}
