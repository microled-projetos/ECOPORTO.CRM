namespace Ecoporto.CRM.Site.Models
{
    public class OportunidadesViewModel
    {
        public OportunidadesViewModel()
        {
            OportunidadesInformacoesIniciaisViewModel = new OportunidadesInformacoesIniciaisViewModel();
            OportunidadesFichaFaturamentoViewModel = new OportunidadesFichaFaturamentoViewModel();
            OportunidadesViewModelProposta = new OportunidadesPropostaViewModel();
            OportunidadesSimuladorViewModel = new SimuladorPropostaViewModel();
            OportunidadesAnexosNotasViewModel = new OportunidadesAnexosNotasViewModel();
            OportunidadesPremioParceriaViewModel = new OportunidadesPremioParceriaViewModel();
            OportunidadesClonarPropostaViewModel = new OportunidadesClonarPropostaViewModel();
            OportunidadesAdendosViewModel = new OportunidadesAdendosViewModel();
            OportunidadesIntegracaoChronosViewModel = new OportunidadesIntegracaoChronosViewModel();
        }

        public int Id { get; set; }

        public OportunidadesInformacoesIniciaisViewModel OportunidadesInformacoesIniciaisViewModel { get; set; }

        public OportunidadesFichaFaturamentoViewModel OportunidadesFichaFaturamentoViewModel { get; set; }

        public OportunidadesPropostaViewModel OportunidadesViewModelProposta { get; set; }

        public SimuladorPropostaViewModel OportunidadesSimuladorViewModel { get; set; }

        public OportunidadesAnexosNotasViewModel OportunidadesAnexosNotasViewModel { get; set; }

        public OportunidadesPremioParceriaViewModel OportunidadesPremioParceriaViewModel { get; set; }

        public OportunidadesClonarPropostaViewModel OportunidadesClonarPropostaViewModel { get; set; }

        public OportunidadesAdendosViewModel OportunidadesAdendosViewModel { get; set; }

        public OportunidadesIntegracaoChronosViewModel OportunidadesIntegracaoChronosViewModel { get; set; }
    }
}