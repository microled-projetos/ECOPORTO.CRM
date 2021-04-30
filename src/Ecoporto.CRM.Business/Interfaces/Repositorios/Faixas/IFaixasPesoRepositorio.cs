using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface IFaixasPesoRepositorio
    {      
        void Cadastrar(FaixaPeso faixa);
        void Atualizar(FaixaPeso faixa);
        void Excluir(int id);
        IEnumerable<FaixaPeso> ObterFaixasPeso(int layoutId);
        FaixaPeso ObterPorId(int id);
    }
}
