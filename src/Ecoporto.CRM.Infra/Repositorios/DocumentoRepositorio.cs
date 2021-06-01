using Dapper;
using Ecoporto.CRM.Business.Interfaces.Repositorios;
using Ecoporto.CRM.Business.Models;
using Ecoporto.CRM.Infra.Configuracao;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace Ecoporto.CRM.Infra.Repositorios
{
    public class DocumentoRepositorio : IDocumentoRepositorio
    {
        public IEnumerable<Documento> ObterTiposDocumentos()
        {           
            MemoryCache cache = MemoryCache.Default;

            var documentos = cache["Documento.ObterTiposDocumentos"] as IEnumerable<Documento>;

            if (documentos == null)
            {
                using (OracleConnection con = new OracleConnection(Config.StringConexao()))
                {
                    documentos = con.Query<Documento>(@"SELECT Code As Id, Descr As Descricao FROM SGIPA.TB_TIPOS_DOCUMENTOS ORDER BY DESCR");
                }

                cache["Documento.ObterTiposDocumentos"] = documentos;
            }

            return documentos;
        }
    }
}
