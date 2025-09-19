using API.Models;
using System.Threading.Tasks;

public interface IBookingService
{
    // Metoden måste lova att den är asynkron
    Task<Booking> SkapaBokningAsync(string userId, int workoutId);
}