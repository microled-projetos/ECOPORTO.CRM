using Ecoporto.CRM.Business.Enums;

namespace Ecoporto.CRM.Business.Models
{
    public class ServicoTipoOperacao : Entidade<ServicoTipoOperacao>
    {     
        public TipoOperacao TipoOperacao { get; set; }

        public override void Validar()
        {
            ValidationResult = Validate(this);
        }
    }
}
