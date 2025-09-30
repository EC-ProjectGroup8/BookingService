using API.DTOs;
using API.Models;
using System.Threading.Tasks;

/// <summary>
/// Defines the contract for business logic operations related to bookings.
/// </summary>
public interface IBookingService
{
    /// <summary>
    /// Defines the method for creating and saving a new booking.
    /// </summary>
    /// <param name="userEmail">The email of the user making the booking.</param>
    /// <param name="workoutIdentifier">The string identifier of the workout to book.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains the created booking object.
    /// </returns>
    Task<Booking> CreateBookingAsync(string userEmail, string workoutIdentifier);
    Task<IEnumerable<BookingDetailsDto>> GetMyBookingsAsync(string email); 
}