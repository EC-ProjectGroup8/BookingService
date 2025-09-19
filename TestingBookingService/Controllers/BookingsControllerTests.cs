using Xunit;
using Moq; // Importera Moq
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
        // Vi behöver en 'async Task' eftersom vår controller-metod nu är beroende av en asynkron service
        [Fact]
        public async Task CreateBooking_ShouldReturnCreated_WhenBookingIsSuccessful()
        {
            // --- Arrange (Förberedelser) ---

            // 1. Skapa en mock av vår service.
            var mockService = new Mock<IBookingService>();

            var bookingDto = new CreateBookingDto { WorkoutId = 1 };
            var bookingResult = new Booking { Id = 100, UserId = 1, WorkoutId = 1 };
            var userId = "1";

            // 2. Ställ in mocken: När SkapaBokning anropas med dessa parametrar,
            //    returnera vårt förberedda bokningsobjekt.
            mockService.Setup(s => s.SkapaBokning(userId, bookingDto.WorkoutId))
                       .ReturnsAsync(bookingResult);

            // 3. Skapa en "fusk-användare" för att simulera inloggning.
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));

            // 4. Skapa controllern med vår mock-service
            var controller = new BookingsController(mockService.Object);

            // 5. Sätt controllerns kontext så att den tror att vår fusk-användare är inloggad.
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // --- Act (Genomförande) ---
            var result = await controller.CreateBooking(bookingDto);

            // --- Assert (Kontroll) ---
            Assert.IsType<CreatedAtActionResult>(result);
        }
    }
}