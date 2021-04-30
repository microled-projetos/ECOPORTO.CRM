using Ecoporto.CRM.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface IFaixasVolumeRepositorio
    {
        void Cadastrar(FaixaVolume faixa);
        void Atualizar(FaixaVolume faixa);
        void Excluir(int id);
        IEnumerable<FaixaVolume> ObterFaixasVolume(int layoutId);
        FaixaVolume ObterPorId(int id);
    }
}
