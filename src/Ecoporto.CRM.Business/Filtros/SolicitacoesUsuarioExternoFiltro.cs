using Ecoporto.CRM.Business.DTO;
using System.Collections.Generic;
using System.Linq;

namespace Ecoporto.CRM.Business.Filtros
{
    public class SolicitacoesUsuarioExternoFiltro
    {
        public SolicitacoesUsuarioExternoFiltro(int usuarioId, bool acessoCancelamento, bool acessoDesconto, bool acessoProrrogacao, bool acessoRestituicao, IEnumerable<UsuarioContaDTO> cnpjVinculados)
        {
            this.UsuarioId = usuarioId;
            this.AcessoCancelamento = acessoCancelamento;
            this.AcessoDesconto = acessoDesconto;
            this.AcessoProrrogacao = acessoProrrogacao;
            this.AcessoRestituicao = acessoRestituicao;
            this.CnpjVinculados = cnpjVinculados;

            var acesso = AcessoCancelamento || AcessoDesconto || AcessoProrrogacao || AcessoRestituicao;

            if (acesso)
            {
                AcessosSolicitacao = new List<string>();

                if (AcessoCancelamento)
                    AcessosSolicitacao.Add("1");

                if (AcessoDesconto)
                    AcessosSolicitacao.Add("2");

                if (AcessoProrrogacao)
                    AcessosSolicitacao.Add("3");

                if (AcessoRestituicao)
                    AcessosSolicitacao.Add("4");
            }
        }

        public int UsuarioId { get; set; }

        public bool AcessoDesconto { get; set; }

        public bool AcessoCancelamento { get; set; }

        public bool AcessoProrrogacao { get; set; }

        public bool AcessoRestituicao { get; set; }

        public IEnumerable<UsuarioContaDTO> CnpjVinculados { get; set; }

        private List<string> AcessosSolicitacao { get; set; }

        public string FiltroAcessoSolicitacao()
        {
            if (AcessosSolicitacao.Any())
                return string.Join(",", AcessosSolicitacao);

            return string.Empty;
        }

        public string FiltroCnpjsVinculados() => string.Join(", ", CnpjVinculados.Select(x => $"'{x.ContaDocumento}'"));
    }
}
