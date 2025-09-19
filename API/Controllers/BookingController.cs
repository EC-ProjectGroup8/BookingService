using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using API.DTOs;
using API.Services; // Här finns IBookingService

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Skyddar alla metoder i denna controller
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto bookingDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Token innehåller inget användar-ID.");
            }

            // Vi förväntar oss hela bokningsobjektet tillbaka, inte bara true/false
            var nyBokning = _bookingService.SkapaBokning(userId, bookingDto.WorkoutId);

            if (nyBokning != null)
            {
                // Skicka tillbaka ett 201 Created-svar med den nya bokningen
                return CreatedAtAction(nameof(CreateBooking), new { id = nyBokning.Id }, nyBokning);
            }

            // Om servicen returnerade null, misslyckades bokningen
            return BadRequest("Kunde inte skapa bokningen.");
        }
    }
}