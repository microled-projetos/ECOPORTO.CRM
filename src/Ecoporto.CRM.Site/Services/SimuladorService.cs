using Ecoporto.CRM.Business.DTO;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using System;

namespace Ecoporto.CRM.Site.Services
{
    public class SimuladorService
    {
        private readonly ISimuladorRepositorio _simuladorRepositorio;
        private readonly IOportunidadeRepositorio _oportunidadeRepositorio;

        public SimuladorService(ISimuladorRepositorio simuladorRepositorio, IOportunidadeRepositorio oportunidadeRepositorio)
        {
            _simuladorRepositorio = simuladorRepositorio;
            _oportunidadeRepositorio = oportunidadeRepositorio;
        }

        public void Calcular(int oportunidadeId)
        {
            var oportunidade = _oportunidadeRepositorio.ObterOportunidadePorId(oportunidadeId);

            if (oportunidade == null)
                throw new Exception("Oportunidade não encontrada");

            _simuladorRepositorio.LimparServicosSimulador(oportunidade.Identificacao);

            var clientes = _simuladorRepositorio.ObterClientesSimulador(oportunidade.Identificacao);

            _simuladorRepositorio.CadastrarTabela(
                new SimuladorDTO
                {
                    Proposta = oportunidade.Identificacao,
                    Descricao = oportunidade.Descricao,
                    Importador = clientes.Importador,
                    Despachante = clientes.Despachante,
                    Coloader = clientes.Coloader,
                    CoColoader = clientes.CoColoader,
                    CoColoader2 = clientes.CoColoader2
                });

            _simuladorRepositorio.ImportarArmazenagem(oportunidade.Identificacao, oportunidade.Id);
            _simuladorRepositorio.ImportarServicoMargem(oportunidade.Identificacao, oportunidade.Id);
            _simuladorRepositorio.ImportarServicoHubPort(oportunidade.Identificacao, oportunidade.Id);
            _simuladorRepositorio.ImportarServicoMecanicaManual(oportunidade.Identificacao, oportunidade.Id);
            _simuladorRepositorio.ImportarServicosGerais(oportunidade.Identificacao, oportunidade.Id);
            _simuladorRepositorio.ImportarServicoLiberacao(oportunidade.Identificacao, oportunidade.Id);


        }        
    }
}