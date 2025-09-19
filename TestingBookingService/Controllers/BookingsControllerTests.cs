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
        // Vi beh�ver en 'async Task' eftersom v�r controller-metod nu �r beroende av en asynkron service
        [Fact]
        public async Task CreateBooking_ShouldReturnCreated_WhenBookingIsSuccessful()
        {
            // --- Arrange (F�rberedelser) ---

            // 1. Skapa en mock av v�r service.
            var mockService = new Mock<IBookingService>();

            var bookingDto = new CreateBookingDto { WorkoutId = 1 };
            var bookingResult = new Booking { Id = 100, UserId = 1, WorkoutId = 1 };
            var userId = "1";

            // 2. St�ll in mocken: N�r SkapaBokning anropas med dessa parametrar,
            //    returnera v�rt f�rberedda bokningsobjekt.
            mockService.Setup(s => s.SkapaBokning(userId, bookingDto.WorkoutId))
                       .ReturnsAsync(bookingResult);

            // 3. Skapa en "fusk-anv�ndare" f�r att simulera inloggning.
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));

            // 4. Skapa controllern med v�r mock-service
            var controller = new BookingsController(mockService.Object);

            // 5. S�tt controllerns kontext s� att den tror att v�r fusk-anv�ndare �r inloggad.
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // --- Act (Genomf�rande) ---
            var result = await controller.CreateBooking(bookingDto);

            // --- Assert (Kontroll) ---
            Assert.IsType<CreatedAtActionResult>(result);
        }
    }
}