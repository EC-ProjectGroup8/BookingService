using API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Repositories
{
    public interface IBookingRepository
    {
        // Metoderna är nu asynkrona och returnerar en Task
        Task<Booking> SparaAsync(Booking booking);
        Task<IEnumerable<Booking>> HämtaFörPassAsync(int workoutId);
    }
}