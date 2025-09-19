using Xunit;
using Moq;
using API.Controllers;
using API.Services;
using API.DTOs;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace TestingBookingService.Controllers
{
    public class BookingsControllerTests
    {
        [Fact]
        public async Task CreateBooking_ShouldReturnCreated_WhenBookingIsSuccessful()
        {
            // --- Arrange ---
            var mockService = new Mock<IBookingService>();
            var bookingDto = new CreateBookingDto { WorkoutId = 1 };
            var bookingResult = new Booking { Id = 100, UserId = 1, WorkoutId = 1 };
            var userId = "1";

            // Mocken måste vara asynkron för att matcha servicen
            mockService.Setup(s => s.SkapaBokningAsync(userId, bookingDto.WorkoutId))
                       .ReturnsAsync(bookingResult); // Använd ReturnsAsync

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));

            var controller = new BookingsController(mockService.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext() { User = user }
                }
            };

            // --- Act ---
            var result = await controller.CreateBooking(bookingDto);

            // --- Assert ---
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedBooking = Assert.IsType<Booking>(createdResult.Value);
            Assert.Equal(100, returnedBooking.Id);
        }
    }
}