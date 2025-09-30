using API.DTOs;
using API.Models;

namespace API.Repositories.Interfaces
{
    /// <summary>
    /// Defines the contract for repository operations related to bookings.
    /// </summary>
    public interface IBookingRepository
    {
        /// <summary>
        /// Defines the method signature for saving a new booking.
        /// </summary>
        /// <param name="booking">The booking model object to be saved.</param>
        /// <returns>A task that represents the asynchronous operation, containing the saved booking model with its new Id.</returns>
        Task<Booking> SaveAsync(Booking booking);
        Task<IEnumerable<WorkoutIdDto>> GetMyBookingsAsync(string email);
    }
}