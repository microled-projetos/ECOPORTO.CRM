using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Enums;
using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface IPremioParceriaRepositorio
    {
        int Cadastrar(OportunidadePremioParceria premioParceria);
        void Atualizar(OportunidadePremioParceria premioParceria);
        IEnumerable<PremioParceriaDTO> ObterPremiosParceriaPorOportunidade(int oportunidadeId);
        OportunidadePremioParceria ObterPremioParceriaPorId(int id);
        IEnumerable<OportunidadePremioParceria> ObterPremiosParceriaPorStatus(StatusPremioParceria statusPremioParceria);
        IEnumerable<OportunidadePremioParceriaModalidade> ObterModalidades(int premioParceriaId);
        void Excluir(int id);
        void CancelarPremioParceria(int id);
        void AtualizarStatusPremioParceria(StatusPremioParceria status, int id);
        IEnumerable<PremioParceriaDTO> ObterPremiosParceriaPorDescricao(string descricao);
        PremioParceriaDetalhesDTO ObterDetalhesPremioParceria(int id);
        bool ExistePremioParceria(OportunidadePremioParceria premioParceria);
        void AtualizarCancelamento(bool cancelado);
    }
}
