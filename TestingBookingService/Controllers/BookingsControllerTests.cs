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
            // Create a mock of the booking service.
            var mockService = new Mock<IBookingService>();

            // Updated DTO to use a string identifier for the workout.
            var bookingDto = new CreateBookingDto { WorkoutIdentifier = "strength-class-101" };

            // Updated the expected user identifier to be an email.
            var userEmail = "test.user@example.com";

            // Updated the booking result to match the new Booking model.
            var bookingResult = new Booking
            {
                Id = 100,
                UserEmail = userEmail,
                WorkoutIdentifier = bookingDto.WorkoutIdentifier
            };

            // Set up the mock service to expect the new string parameters.
            mockService.Setup(s => s.CreateBookingAsync(userEmail, bookingDto.WorkoutIdentifier))
                       .ReturnsAsync(bookingResult);

            // Create a mock user principal with an Email claim instead of NameIdentifier.
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, userEmail)
            }, "mock"));

            // Instantiate the controller with the mock service and context.
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

            // Verify the properties of the returned booking.
            Assert.Equal(100, returnedBooking.Id);
            Assert.Equal(userEmail, returnedBooking.UserEmail);
        }
    }
}