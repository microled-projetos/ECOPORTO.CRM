using Ecoporto.CRM.Business.Models;

namespace Ecoporto.CRM.Business.Interfaces.Repositorios
{
    public interface IBookingRepositorio
    {
        Booking ObterBookingPorReserva(string reserva);
    }
}
