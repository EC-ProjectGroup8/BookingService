using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        Task<Booking> SparaAsync(Booking booking);
    }
}