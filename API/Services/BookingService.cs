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


        public async Task<IEnumerable<BookingDetailsDto>> GetMyBookingsMultiFetchAsync(string email)
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

        public async Task<IEnumerable<BookingDetailsDto>> GetMyBookingsDualFetchAsync(string email)
        {
            var result = new List<BookingDetailsDto>();
            var bookings = await _bookingRepository.GetMyBookingsAsync(email);
            var workoutIds = bookings.Select(b => b.WorkoutIdentifier).ToList();

            if (!workoutIds.Any()) return result;

            var idsParam = string.Join(",", workoutIds);

            var client = _httpClientFactory.CreateClient("WorkoutAPI");
            var workoutData = await client.GetFromJsonAsync<IEnumerable<BookingDetailsDto>>($"api/Workout/batch?ids={idsParam}");
            return workoutData ?? Enumerable.Empty<BookingDetailsDto>();

        }

        /// <inheritdoc />
        public async Task<IEnumerable<WorkoutIdDto>> ReturnMyBookingsFromThisAPI(string email)
        {
            // Denna metod är den rena interna vägen: den hämtar WorkoutIdDto direkt från databasen.
            // Denna metod ska användas för att mata det nya aggregeringsflödet.
            return await _bookingRepository.GetMyBookingsAsync(email);
        }

        /// <inheritdoc />
        public async Task DeleteAsync(string userEmail, string workoutIdentifier)
        {
            // Call the repository to delete the booking using the provided identifiers.
            // The repository will return true if a booking was deleted, otherwise false.
            var wasDeleted = await _bookingRepository.DeleteBookingAsync(userEmail, workoutIdentifier);

            // Check the boolean result from the repository.
            if (!wasDeleted)
            {
                // If false, it means no matching booking was found for that user and workout.
                // We throw an exception to inform the caller (e.g., the API Controller) that the resource does not exist.
                throw new KeyNotFoundException("Booking not found or you do not have permission to delete it.");
            }
        }


    }
}