using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Models;
using System.Collections.Generic;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface IUsuarioRepositorio
    {
        int Cadastrar(Usuario usuario);
        void Atualizar(Usuario usuario);
        IEnumerable<UsuarioDTO> ObterUsuarios();
        Usuario ObterUsuarioPorId(int id);
        void Excluir(int id);
        Usuario ObterUsuarioPorLogin(string descricao);
        Usuario ObterUsuarioPorCPF(string cpf);
        void VincularConta(int contaId, int usuarioId);
        IEnumerable<UsuarioContaDTO> ObterVinculosContas(int usuarioId);
        void ExcluirVinculoConta(int id);
        UsuarioContaDTO ObterVinculoContaPorId(int id);
        bool ExisteVinculoConta(int contaId, int usuarioId);
        void AlterarSenha(Usuario usuario);
        IEnumerable<UsuarioIntegracao> ObterUsuariosIntegracao();
    }
}
