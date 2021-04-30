namespace Ecoporto.CRM.Business.Interfaces.Servicos
{
    public interface IControleAcessoService
    {
        void LogarTentativaAcesso(int usuarioId, bool externo, bool sucesso, string mensagem, string ip);
    }
}
