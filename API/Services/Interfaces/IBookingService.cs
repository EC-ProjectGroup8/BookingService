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
    /// <summary>
    /// Deletes a booking for a specific user and workout.
    /// </summary>
    /// <param name="userEmail">The email of the user whose booking is to be deleted.</param>
    /// <param name="workoutIdentifier">The string identifier of the workout to unbook.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAsync(string userEmail, string workoutIdentifier);

    /// <summary>
    /// Retrieves raw booking records directly from this service's database (Repository), 
    /// containing the WorkoutIdentifier necessary for further processing or aggregation.
    /// </summary>
    /// <param name="email">The email of the user whose booking identifiers are to be retrieved.</param>
    /// <returns>A task that represents the asynchronous operation, containing a collection of WorkoutIdDto.</returns>
    Task<IEnumerable<WorkoutIdDto>> ReturnMyBookingsFromThisAPI(string email);
}