using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Interfaces.Servicos;
using Ecoporto.CRM.Infra.Busca;
using Ecoporto.CRM.Infra.Repositorios;
using Ecoporto.CRM.Infra.Services;
using Ecoporto.CRM.Site.Services;
using NLog;
using System;
using Unity;
using Unity.Injection;

namespace Ecoporto.CRM.Site
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => container.Value;
        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // Repositórios

            container.RegisterType<IHubPortRepositorio, HubPortRepositorio>();
            container.RegisterType<IImpostoRepositorio, ImpostoRepositorio>();
            container.RegisterType<IModeloRepositorio, ModeloRepositorio>();
            container.RegisterType<IServicoFaturamentoRepositorio, ServicoFaturamentoRepositorio>();
            container.RegisterType<IServicoRepositorio, ServicoRepositorio>();
            container.RegisterType<IVendedorRepositorio, VendedorRepositorio>();
            container.RegisterType<IFaixasBLRepositorio, FaixasBLRepositorio>();
            container.RegisterType<IFaixasCIFRepositorio, FaixasCIFRepositorio>();
            container.RegisterType<IFaixasPesoRepositorio, FaixasPesoRepositorio>();
            container.RegisterType<IFaixasVolumeRepositorio, FaixasVolumeRepositorio>();
            container.RegisterType<IContatoRepositorio, ContatoRepositorio>();
            container.RegisterType<ICidadeRepositorio, CidadeRepositorio>();
            container.RegisterType<IPaisRepositorio, PaisRepositorio>();
            container.RegisterType<IContaRepositorio, ContaRepositorio>();
            container.RegisterType<IParceiroRepositorio, ParceiroRepositorio>();
            container.RegisterType<IMercadoriaRepositorio, MercadoriaRepositorio>();
            container.RegisterType<IOportunidadeRepositorio, OportunidadeRepositorio>();
            container.RegisterType<ICondicaoPagamentoFaturaRepositorio, CondicaoPagamentoFaturaRepositorio>();
            container.RegisterType<IPremioParceriaRepositorio, PremioParceriaRepositorio>();            
            container.RegisterType<ICargoRepositorio, CargoRepositorio>();
            container.RegisterType<IUsuarioRepositorio, UsuarioRepositorio>();
            container.RegisterType<IAuditoriaRepositorio, AuditoriaRepositorio>();
            container.RegisterType<IControleAcessoRepositorio, ControleAcessoRepositorio>();
            container.RegisterType<ISolicitacoesRepositorio, SolicitacoesRepositorio>();
            container.RegisterType<INotaFiscalRepositorio, NotaFiscalRepositorio>();
            container.RegisterType<IParametrosRepositorio, ParametrosRepositorio>();
            container.RegisterType<IBancoRepositorio, BancoRepositorio>();
            container.RegisterType<IGRRepositorio, GRRepositorio>();
            container.RegisterType<IMinutaRepositorio, MinutaRepositorio>();
            container.RegisterType<IWorkflowRepositorio, WorkflowRepositorio>();
            container.RegisterType<IAnexoRepositorio, AnexoRepositorio>();
            container.RegisterType<IEquipeVendedorRepositorio, EquipeVendedorRepositorio>();
            container.RegisterType<IEquipeContaRepositorio, EquipeContaRepositorio>();
            container.RegisterType<IEquipeOportunidadeRepositorio, EquipeOportunidadeRepositorio>();
            container.RegisterType<IEmpresaRepositorio, EmpresaRepositorio>();
            container.RegisterType<IBookingRepositorio, BookingRepositorio>();
            container.RegisterType<ISimuladorRepositorio, SimuladorRepositorio>();
            container.RegisterType<ILoteRepositorio, LoteRepositorio>();
            container.RegisterType<IMotivosRepositorio, MotivosRepositorio>();
            container.RegisterType<IOcorrenciasRepositorio, OcorrenciasRepositorio>();
            container.RegisterType<IDocumentoRepositorio, DocumentoRepositorio>();
            container.RegisterType<ILocalAtracacaoRepositorio, LocalAtracacaoRepositorio>();
            container.RegisterType<IGrupoAtracacaoRepositorio, GrupoAtracacaoRepositorio>();
            container.RegisterType<IModeloSimuladorRepositorio, ModeloSimuladorRepositorio>();
            container.RegisterType<ISimuladorPropostaRepositorio, SimuladorPropostaRepositorio>();
            container.RegisterType<IMargemRepositorio, MargemRepositorio>();
            container.RegisterType<ITabelasRepositorio, TabelasRepositorio>();
            container.RegisterType<IImpostosExcecaoRepositorio, ImpostosExcecaoRepositorio>();
            container.RegisterType<IAnaliseCreditoRepositorio, AnaliseCreditoRepositorio>();

            container.RegisterType<ILayoutRepositorio, LayoutRepositorio>(new InjectionConstructor(false));
            container.RegisterType<ILayoutPropostaRepositorio, LayoutRepositorio>(new InjectionConstructor(true));

            // Busca e Logs

            container.RegisterType<IBusca, BuscaInterna>();
            container.RegisterType<ILogger>(new InjectionFactory(l => LogManager.GetCurrentClassLogger()));

            // Serviços

            container.RegisterType<IOportunidadeService, OportunidadeService>();
            container.RegisterType<IEquipesService, EquipesService>();
            container.RegisterType<IControleAcessoService, ControleAcessoService>();
            container.RegisterType<IRelogioService, RelogioService>();
            container.RegisterType<IAmbienteOracleService, AmbienteOracleService>();
            container.RegisterType<IConcomitanciaTabelaService, ConcomitanciaTabelaService>();
        }
    }
}