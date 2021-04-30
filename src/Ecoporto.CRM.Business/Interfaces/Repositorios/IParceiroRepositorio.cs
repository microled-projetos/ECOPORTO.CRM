using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface IParceiroRepositorio
    {
        Parceiro ObterParceiroPorId(int id);
        Parceiro ObterParceiroPorDocumento(string documento);
        Parceiro ObterDetalhesImportadorPorCnpj(string cnpj);
        Parceiro ObterDetalhesExportadorPorCnpj(string cnpj);
        IEnumerable<Parceiro> ObterArmadoresPorDescricao(string descricao);
    }
}
