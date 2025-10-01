using API.DTOs;
using API.Entities;
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


        /// <summary>
        /// Deletes a booking directly from the database based on the user's email and the workout's string identifier.
        /// </summary>
        /// <param name="userEmail">The email of the user who made the booking.</param>
        /// <param name="workoutIdentifier">The string identifier of the workout to unbook.</param>
        /// <returns>A task that represents the asynchronous operation, containing true if a booking was found and deleted; otherwise, false.</returns>
        Task<bool> DeleteBookingAsync(string userEmail, string workoutIdentifier);

 
    }
}