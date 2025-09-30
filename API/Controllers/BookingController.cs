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
            var bookings = await _bookingService.GetMyBookingsAsync(email);
            return Ok(bookings);
        }
    }
}