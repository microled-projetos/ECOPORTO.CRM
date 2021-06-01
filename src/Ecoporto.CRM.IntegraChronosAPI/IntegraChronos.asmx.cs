using Ecoporto.CRM.IntegraChronosAPI.Data;
using Ecoporto.CRM.IntegraChronosAPI.Extensions;
using Ecoporto.CRM.IntegraChronosAPI.Models;
using Ecoporto.CRM.IntegraChronosAPI.Requests;
using Ecoporto.CRM.IntegraChronosAPI.Responses;
using Ecoporto.CRM.IntegraChronosAPI.Security;
using System;
using System.Linq;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace Ecoporto.CRM.IntegraChronosAPI
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]

    public class IntegraChronos : WebService
    {
        private readonly FilaRepository _filaRepository = new FilaRepository();

        public Usuario Credenciais;

        [WebMethod]
        [SoapHeader("Credenciais")]
        public CadastrarResponse Cadastrar(CadastrarRequest request)
        {
            if (!Autenticar(Credenciais))
            {
                return RetornaErroCadastro(null, "Falha durante a autenticação. Verifique credenciais.");
            }

            try
            {
                var model = new Processo(
                    request.Id_Processo,
                    request.Tipo_Processo,
                    request.Id_Workflow,
                    request.Id_Etapa,
                    request.Acao);

                if (!model.ValidationResult.IsValid)
                {
                    return RetornaErroCadastro(model, "Parâmetros incorretos. Verifique mensagens de erros");
                }

                var existeProcesso = _filaRepository.ConsultarPorProcesso(model)
                    .GetAwaiter()
                    .GetResult();

                if (existeProcesso != null)
                {
                    return RetornaErroCadastro(model, $"Já existe um outro processo com protocolo nº {existeProcesso.Id} pendente");
                }

                var processoBusca = _filaRepository.ExisteProcesso(model);

                if (processoBusca == null)
                {
                    return RetornaErroCadastro(model, $"Processo ID {model.Id_Processo} não encontrado");                    
                }

                var protocolo = _filaRepository.Cadastrar(model)
                    .GetAwaiter()
                    .GetResult();

                return new CadastrarResponse
                {
                    Sucesso = true,
                    Mensagem = "Operação realizada com sucesso!",
                    Protocolo = protocolo.ToString()
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        [WebMethod]
        [SoapHeader("Credenciais")]
        public ConsultaResponse Consultar(string protocolo)
        {
            if (!Autenticar(Credenciais))
            {
                return RetornaErroConsulta("Falha durante a autenticação. Verifique credenciais.");
            }

            try
            {
                if (!Int32.TryParse(protocolo, out int resultado))
                {
                    return RetornaErroConsulta("Número de protocolo inválido.");
                }

                var processoBusca = _filaRepository.Consultar(protocolo.ToInt())
                    .GetAwaiter()
                    .GetResult();

                if (processoBusca == null)
                {
                    return RetornaErroConsulta($"Nenhum processo encontrado com o protocolo nº {protocolo}");
                }

                return new ConsultaResponse
                {
                    Sucesso = true,
                    Protocolo = protocolo,
                    Status = processoBusca.Status.ToValue(),
                    StatusDescricao = processoBusca.Status.ToName(),
                    Motivo = processoBusca.Motivo
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static CadastrarResponse RetornaErroCadastro(Processo model, string mensagem)
        {
            return new CadastrarResponse
            {
                Sucesso = false,
                Mensagem = mensagem,
                Erros =  model?.ValidationResult.Errors.Select(c => c.ErrorMessage).ToArray()
            };
        }

        private static ConsultaResponse RetornaErroConsulta(string mensagem)
        {
            return new ConsultaResponse
            {
                Sucesso = false,
                Mensagem = mensagem
            };
        }

        private bool Autenticar(Usuario usuario)
        {
            if (usuario == null)
                return false;

            return usuario.Login == Config.SecretUserString() && usuario.Senha == Config.SecretKeyString();
        }
    }
}
