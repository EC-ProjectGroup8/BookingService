using Xunit;
using Moq;
using API.Services;
using API.Repositories.Interfaces;
using API.Models;
using System.Threading.Tasks;

namespace TestingBookingService.Services
{
    public class BookingServiceTests
    {
        private readonly Mock<IBookingRepository> _mockRepo;
        private readonly BookingService _service;

        public BookingServiceTests()
        {
            _mockRepo = new Mock<IBookingRepository>();
            _service = new BookingService(_mockRepo.Object);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldReturnBooking_WhenCalled()
        {
            // --- Arrange ---
            var userEmail = "test.user@example.com";
            var workoutIdentifier = "strength-101"; // Changed from workoutId

            _mockRepo.Setup(r => r.SaveAsync(It.IsAny<Booking>()))
                     .ReturnsAsync((Booking b) =>
                     {
                         b.Id = 123; // Assign a test ID
                         return b;
                     });

            // --- Act ---
            // Call the service method with the new string identifier
            var result = await _service.CreateBookingAsync(userEmail, workoutIdentifier);

            // --- Assert ---
            Assert.NotNull(result);
            Assert.Equal(123, result.Id);
            // Verify that the correct string properties were set
            Assert.Equal(userEmail, result.UserEmail);
            Assert.Equal(workoutIdentifier, result.WorkoutIdentifier);
        }
    }
}