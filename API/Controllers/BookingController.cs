using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using API.Services;
using System.Text.RegularExpressions;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        /// <summary>
        /// Creates a new booking.
        /// </summary>
        /// <param name="bookingDto">The data transfer object containing the user's email and the workout identifier.</param>
        /// <returns>
        /// A 201 Created response if successful.
        /// A 400 Bad Request response if the booking could not be created or if the email is missing.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto bookingDto)
        {
            // TODO: Add proper JWT authentication before deploying to production.
            var userEmail = bookingDto.UserEmail;

            if (string.IsNullOrEmpty(userEmail))
            {
                return BadRequest("UserEmail must be provided in the request body.");
            }

            var newBooking = await _bookingService.CreateBookingAsync(userEmail, bookingDto.WorkoutIdentifier);

            if (newBooking != null)
            {
                return CreatedAtAction(nameof(CreateBooking), new { id = newBooking.Id }, newBooking);
            }

            return BadRequest("Could not create the booking.");
        }

        [HttpGet("GetMyBookings/{email}")]
        public async Task<IActionResult> GetMyBookings(string email)
        {
            if(string.IsNullOrEmpty(email)) return BadRequest("Email must be provided.");
            if(!Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")) return BadRequest("Invalid email format.");
            var bookings = await _bookingService.GetMyBookingsDualFetchAsync(email);
            return Ok(bookings);
        }


        /// <summary>
        /// Deletes a specific booking.
        /// </summary>
        /// <param name="userEmail">The email of the user who owns the booking.</param>
        /// <param name="workoutIdentifier">The identifier of the workout to unbook.</param>
        /// <returns>
        /// A 204 No Content response if the deletion was successful.
        /// A 404 Not Found response if the booking does not exist.
        /// A 400 Bad Request response for invalid input.
        /// </returns>
        [HttpDelete("{userEmail}/{workoutIdentifier}")]
        public async Task<IActionResult> DeleteBooking(string userEmail, string workoutIdentifier)
        {
            // 1. Input Validation
            if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(workoutIdentifier))
            {
                return BadRequest("User email and workout identifier must be provided.");
            }
            if (!Regex.IsMatch(userEmail, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                return BadRequest("Invalid email format.");
            }

            try
            {
                // 2. Call the service layer to perform the deletion
                await _bookingService.DeleteAsync(userEmail, workoutIdentifier);

                // 3. Return success response
                // HTTP 204 No Content is the standard response for a successful DELETE action
                // where no response body is needed.
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                // 4. Handle the case where the booking was not found
                // This exception is thrown by our BookingService, and we translate it to a 404 Not Found.
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // 5. Handle any other unexpected errors
                // Log the exception 'ex' here for debugging
                return StatusCode(500, "An unexpected error occurred while trying to delete the booking.");
            }
        }
        /// <summary>
        /// Retrieves raw booking identifiers directly from this service's database, 
        /// intended for internal use or troubleshooting, not aggregation.
        /// </summary>
        /// <param name="email">The email of the user whose bookings are to be retrieved.</param>
        /// <returns>A 200 OK response with a list of WorkoutIdDto objects.</returns>
        [HttpGet("GetRawBookings/{email}")]
        public async Task<IActionResult> GetRawBookings(string email)
        {
            if (string.IsNullOrEmpty(email)) return BadRequest("Email must be provided.");
            if (!Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")) return BadRequest("Invalid email format.");

            // Anropar den nya metoden som hämtar data internt.
            var bookings = await _bookingService.ReturnMyBookingsFromThisAPI(email);
            return Ok(bookings);
        }
    }
}