using API.Models;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IBookingService
    {
        Task<Booking> SkapaBokning(string userId, int workoutId);
    }
}