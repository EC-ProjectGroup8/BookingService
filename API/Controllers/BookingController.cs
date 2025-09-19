using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using API.Services;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly BookingService _bookingService;

        public BookingsController(BookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost]
        public IActionResult CreateBooking([FromBody] CreateBookingDto bookingDto)
        {
            var lyckades = _bookingService.SkapaBokning(
                bookingDto.UserId,
                bookingDto.WorkoutId,
                bookingDto.StartTime
            );

            if (lyckades)
            {
                return CreatedAtAction(nameof(CreateBooking), new { id = 0 }, bookingDto);
            }

            return BadRequest("Kunde inte skapa bokningen.");
        }
    }
}