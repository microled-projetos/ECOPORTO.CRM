namespace Ecoporto.CRM.Business.Models
{
    public class OportunidadeNotas : Entidade<OportunidadeNotas>
    {
        public OportunidadeNotas()
        {

        }

        public OportunidadeNotas(int oportunidadeId, string nota, string descricao, int criadoPor)
        {
            OportunidadeId = oportunidadeId;
            Nota = nota;
            Descricao = descricao;
            CriadoPor = criadoPor;
        }

        public int OportunidadeId { get; set; }

        public string Nota { get; set; }

        public string Descricao { get; set; }

        public int CriadoPor { get; set; }

        public void Alterar(OportunidadeNotas oportunidadeNota)
        {
            Nota = oportunidadeNota.Nota;
            Descricao = oportunidadeNota.Descricao;
        }

        public override void Validar()
        {
            ValidationResult = Validate(this);
        }
    }
}
