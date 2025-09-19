using API.Models;
using API.Repositories;
using API.Repositories.Interfaces;

namespace API.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        // Borta: HttpClient
        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<Booking> SkapaBokningAsync(string userIdString, int workoutId)
        {
            // Konvertera userId från string (token) till int
            if (!int.TryParse(userIdString, out int userId))
            {
                return null;
            }

            // Notera: Vi kan inte längre validera mot kapacitet eller dubbelbokningar
            // eftersom denna service inte har den informationen.

            // Skapa bokningsobjektet
            var booking = new Booking
            {
                UserId = userId,
                WorkoutId = workoutId
                // Vi har ingen StartTime att sätta här längre
            };

            // Spara och returnera
            return await _bookingRepository.SparaAsync(booking);

        }
    }
}