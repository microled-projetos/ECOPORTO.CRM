namespace Ecoporto.CRM.Site.Models
{
    public class OportunidadesClonarPropostaViewModel
    {
        public OportunidadesClonarPropostaViewModel()
        {
            CloneSubClientesSelecionados = new int[0];
            CloneGruposCNPJSelecionados = new int[0];
        }

        public string Descricao { get; set; }

        public int CloneOportunidadeSelecionada { get; set; }

        public int[] CloneSubClientesSelecionados { get; set; }

        public int[] CloneGruposCNPJSelecionados { get; set; }

        public int CloneContaOportunidadeSelecionada { get; set; }
    }
}

