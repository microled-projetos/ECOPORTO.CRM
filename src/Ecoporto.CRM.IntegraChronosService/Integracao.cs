using System;
using System.Configuration;
using System.ServiceProcess;
using System.Timers;
using Ecoporto.CRM.IntegraChronosService.Enums;
using NLog;

namespace Ecoporto.CRM.IntegraChronosService
{
    public partial class Integracao : ServiceBase
    {
        private readonly OportunidadeDAO _oportunidadeDAO = new OportunidadeDAO();
        private readonly FilaDAO _filaDAO = new FilaDAO();
        private readonly ModeloDAO _modeloDAO = new ModeloDAO();

        private Timer timer;

        Logger logger = LogManager.GetCurrentClassLogger();

        public Integracao()
        {
            InitializeComponent();
        }

        public void Setup()
        {
            var tempoTimer = ConfigurationManager.AppSettings["TempoTimer"].ToString();

            this.timer = new Timer(Convert.ToDouble(tempoTimer))
            {
                AutoReset = true
            };

            this.timer.Elapsed += Run;
            this.timer.Start();
        }

        protected override void OnStart(string[] args)
        {
            logger.Info("Serviço iniciado");

            Setup();
        }

        public void Run(object sender, ElapsedEventArgs e)
        {
            var fila = _filaDAO.ObterFilaIntegracao();

            foreach (var item in fila)
            {
                try
                {
                    switch (item.Tipo_Processo)
                    {
                        case 1:
                            ExportarTabelas(item);
                            break;
                        case 4:
                            Adendo(item);
                            break;
                        case 9:
                            CancelamentoOportunidade(item);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    logger.Info(ex.Message);

                    _filaDAO.AtualizarFila(item.Id, Status.Erro);
                }
            }
        }

        private void ExportarTabelas(Fila itemFila)
        {
            var oportunidadeBusca = _oportunidadeDAO.ObterOportunidadePorId(itemFila.Id_Processo);

            if (oportunidadeBusca == null)
                throw new Exception("Oportunidade não encontrada");

            if (oportunidadeBusca.ModeloId == null)
                throw new Exception("Nenhum Modelo vinculado na Oportunidade");

            var modeloBusca = _modeloDAO.ObterModeloPorId(oportunidadeBusca.ModeloId.Value);

            if (modeloBusca == null)
                throw new Exception("Modelo inexistente");

            if (modeloBusca.IntegraChronos == false)
                throw new Exception("Modelo não configurado para integração");

            using (var wsIntegraChronos = new WsIntegraChronos.IntegraChronos())
            {
                var response = new WsIntegraChronos.Response();

                using (var ws = new WsIntegraChronos.IntegraChronos())
                {
                    response = ws.ExportarTabelas(oportunidadeBusca.Id, 234);

                    if (response.Sucesso == false)
                        throw new Exception(response.Mensagem);

                    _filaDAO.AtualizarFila(itemFila.Id, Status.Executada);

                    logger.Info($"Tabela {response.TabelaId} criada com sucesso");
                }
            }
        }

        public void Adendo(Fila itemFila)
        {
            using (var ws = new WsIntegraChronos.IntegraChronos())
            {
                var adendoBusca = _oportunidadeDAO.ObterAdendoPorId(itemFila.Id_Processo);

                if (adendoBusca == null)
                    throw new Exception($"Adendo {itemFila.Id_Processo} não encontrado");

                var response = ws.IntregrarAdendosChronos(adendoBusca.OportunidadeId, adendoBusca.Id);

                if (response.Sucesso == false)
                    throw new Exception(response.Mensagem);

                _filaDAO.AtualizarFila(itemFila.Id, Status.Executada);

                logger.Info(response.Mensagem);
            }
        }

        public void CancelamentoOportunidade(Fila itemFila)
        {
            using (var ws = new WsIntegraChronos.IntegraChronos())
            {
                var oportunidadeBusca = _oportunidadeDAO.ObterOportunidadePorId(itemFila.Id_Processo);

                if (oportunidadeBusca == null)
                    throw new Exception("Oportunidade não encontrada");

                var response = ws.CancelarTabela(oportunidadeBusca.Id);

                if (response.Sucesso == false)
                    throw new Exception(response.Mensagem);

                _filaDAO.AtualizarFila(itemFila.Id, Status.Executada);

                logger.Info(response.Mensagem);
            }
        }

        protected override void OnStop()
        {
            logger.Info("Serviço parado...");
        }
    }
}
