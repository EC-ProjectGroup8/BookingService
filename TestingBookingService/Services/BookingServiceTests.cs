using Xunit;
using Moq;
using API.Services;
using API.Repositories.Interfaces; // Anv�nd interface f�r repository
using API.Models;

namespace TestingBookingService.Services
{
    public class BookingServiceTests
    {
        private readonly Mock<IBookingRepository> _mockRepo;
        private readonly BookingService _service;

        // Konstruktorn �r nu mycket enklare
        public BookingServiceTests()
        {
            _mockRepo = new Mock<IBookingRepository>();
            _service = new BookingService(_mockRepo.Object);
        }

        [Fact]
        public async Task SkapaBokning_ShouldReturnBooking_WhenSuccessful()
        {
            // --- Arrange ---
            var userId = "1";
            var workoutId = 101;

            // H�r simulerar vi en asynkron operation
            _mockRepo.Setup(r => r.SparaAsync(It.IsAny<Booking>()))
                     .ReturnsAsync((Booking b) =>
                     {
                         b.Id = 123;
                         return b;
                     });

            // --- Act ---
            // Anropa metoden med 'await'
            var result = await _service.SkapaBokningAsync(userId, workoutId);

            // --- Assert ---
            Assert.NotNull(result);
            Assert.Equal(123, result.Id);
            Assert.Equal(1, result.UserId);
        }

        [Fact]
        public async Task SkapaBokning_ShouldReturnNull_WhenUserIdIsInvalid()
        {
            // --- Arrange ---
            var invalidUserId = "abc";
            var workoutId = 101;

            // --- Act ---
            // Anv�nd await f�r att v�nta p� det faktiska resultatet
            var result = await _service.SkapaBokningAsync(invalidUserId, workoutId);

            // --- Assert ---
            Assert.Null(result);
        }
    }
}