using System.ComponentModel.DataAnnotations;

namespace WsSimuladorCalculoTabelas.Enums
{
    public enum Regime
    {
        [Display(Name = "FCL")]
        FCL,
        [Display(Name = "LCL")]
        LCL
    }
}