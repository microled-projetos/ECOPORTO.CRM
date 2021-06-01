using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface IContaRepositorio
    {
        IEnumerable<ContaDTO> ObterContas(int pagina, int registrosPorPagina, string filtro, string orderBy, out int totalFiltro, int? usuarioId);
        int ObterTotalContas();
        IEnumerable<Conta> ObterContas();
        int Cadastrar(Conta conta);
        void Atualizar(Conta conta);
        Conta ObterContaPorId(int id);
        Conta ObterContaPorIdAnalise(int id);
        Conta ObterContaPorDocumento(string documento);
        void Excluir(int id);
        Conta ContaExistente(string descricao, string documento);
        IEnumerable<Conta> ObterContasPorDescricao(string descricao, int? usuarioId);
        IEnumerable<Conta> ObterContasImportadoresPorDescricao(string descricao);
        void CadastrarRangeIPS(ControleAcessoConta controle);
        void AtualizarRangeIPS(ControleAcessoConta controle);
        void ExcluirRangeIP(int id);
        IEnumerable<ControleAcessoConta> ObterVinculosIPs(int contaId);
        ControleAcessoConta ObterVinculoIPPorId(int id);   
        IEnumerable<Conta> ObterContasPorRaizDocumento(string documento);
    }
}
