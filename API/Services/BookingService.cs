using API.DTOs;
using API.Models;
using API.Repositories;

namespace API.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly HttpClient _httpClient;

        public BookingService(IBookingRepository bookingRepository, HttpClient httpClient)
        {
            _bookingRepository = bookingRepository;
            _httpClient = httpClient;
        }

        // Metoden måste vara 'async Task' eftersom HttpClient är asynkron
        public async Task<Booking> SkapaBokning(string userIdString, int workoutId)
        {
            if (!int.TryParse(userIdString, out int userId)) { return null; }

            // Steg 1: Anropa ert externa WorkoutService-API
            WorkoutDto workout;
            try
            {
                // URL:en till er WorkoutService bör ligga i appsettings.json
                workout = await _httpClient.GetFromJsonAsync<WorkoutDto>($"api/workouts/{workoutId}");
            }
            catch
            {
                // Anropet misslyckades eller passet hittades inte
                return null;
            }

            if (workout == null) { return null; }

            // Steg 2: Validera mot er lokala Booking-databas
            var existingBookings = await _bookingRepository.HämtaFörPassAsync(workoutId);

            if (existingBookings.Count() >= workout.Capacity) { return null; } // Passet är fullt
            if (existingBookings.Any(b => b.UserId == userId)) { return null; } // Redan bokad

            // Steg 3: Skapa och spara bokningen
            var booking = new Booking
            {
                UserId = userId,
                WorkoutId = workoutId,
                StartTime = workout.StartTime
            };

            return await _bookingRepository.SparaAsync(booking);
        }
    }
}