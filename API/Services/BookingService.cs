using API.DTOs;
using API.Models;
using API.Repositories.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;

namespace API.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IHttpClientFactory _httpClientFactory;

        public BookingService(IBookingRepository bookingRepository, IHttpClientFactory httpClientFactory)
        {
            _bookingRepository = bookingRepository;
            _httpClientFactory = httpClientFactory;
        }

        /// <inheritdoc />
        public async Task<Booking> CreateBookingAsync(string userEmail, string workoutIdentifier)
        {
            // Create a new booking model object.
            var booking = new Booking
            {
                // Mapping over to model (items can be added here from other places in other API's)
                UserEmail = userEmail,
                WorkoutIdentifier = workoutIdentifier
            };

            // Call the repository to save the new booking and return the result.
            return await _bookingRepository.SaveAsync(booking);
        }


        public async Task<IEnumerable<BookingDetailsDto>> GetMyBookingsAsync(string email)
        {
            var result = new List<BookingDetailsDto>();


            var bookings = await _bookingRepository.GetMyBookingsAsync(email);
            var workoutIds = bookings.Select(b => b.WorkoutIdentifier).ToList();

            foreach (var workoutId in workoutIds)
            {
                var client = _httpClientFactory.CreateClient("WorkoutAPI");
                var workoutInfo = await client.GetFromJsonAsync<BookingDetailsDto>($"api/Workout/{workoutId}");
                if (workoutInfo != null)
                {
                    result.Add(workoutInfo);
                }
            }
            return result;
        }
    }
}