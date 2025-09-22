using API.Models;
using API.Repositories.Interfaces;
using System.Threading.Tasks;

namespace API.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
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
    }
}