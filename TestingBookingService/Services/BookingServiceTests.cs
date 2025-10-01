using Xunit;
using Moq;
using API.Services; // Namespace for BookingService
using API.Repositories.Interfaces; // Namespace for IBookingRepository
using API.Models; // Namespace for Booking
using System.Threading.Tasks;
using System.Net.Http; // Required for IHttpClientFactory
using System.Collections.Generic; // Required for KeyNotFoundException

namespace TestingBookingService.Services
{
    public class BookingServiceTests
    {
        private readonly Mock<IBookingRepository> _mockRepo;
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory; // Mock for the new dependency
        private readonly BookingService _service;

        // --- CONSTRUCTOR (Updated) ---
        public BookingServiceTests()
        {
            _mockRepo = new Mock<IBookingRepository>();
            _mockHttpClientFactory = new Mock<IHttpClientFactory>(); // 1. Create a mock for the factory

            // 2. Pass both mocked objects to the service constructor
            _service = new BookingService(_mockRepo.Object, _mockHttpClientFactory.Object);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldReturnBooking_WhenCalled()
        {
            // --- Arrange ---
            var userEmail = "test.user@example.com";
            var workoutIdentifier = "strength-101";

            _mockRepo.Setup(r => r.SaveAsync(It.IsAny<Booking>()))
                     .ReturnsAsync((Booking b) =>
                     {
                         b.Id = 123; // Assign a test ID
                         return b;
                     });

            // --- Act ---
            var result = await _service.CreateBookingAsync(userEmail, workoutIdentifier);

            // --- Assert ---
            Assert.NotNull(result);
            Assert.Equal(123, result.Id);
            Assert.Equal(userEmail, result.UserEmail);
            Assert.Equal(workoutIdentifier, result.WorkoutIdentifier);
        }

        // --- NEW TESTS FOR DELETEASYNC ---

        [Fact]
        public async Task DeleteAsync_ShouldCompleteSuccessfully_WhenBookingExists()
        {
            // --- Arrange ---
            var userEmail = "test@example.com";
            var workoutId = "workout-abc";

            // Setup the repository to return 'true', simulating a successful deletion.
            _mockRepo.Setup(r => r.DeleteBookingAsync(userEmail, workoutId)).ReturnsAsync(true);

            // --- Act ---
            // We call the method, and we expect it to complete without throwing an exception.
            var exception = await Record.ExceptionAsync(() => _service.DeleteAsync(userEmail, workoutId));

            // --- Assert ---
            Assert.Null(exception); // Assert that no exception was thrown.
            _mockRepo.Verify(r => r.DeleteBookingAsync(userEmail, workoutId), Times.Once); // Verify the repo method was called.
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowKeyNotFoundException_WhenBookingDoesNotExist()
        {
            // --- Arrange ---
            var userEmail = "test@example.com";
            var workoutId = "workout-abc";

            // Setup the repository to return 'false', simulating that the booking was not found.
            _mockRepo.Setup(r => r.DeleteBookingAsync(userEmail, workoutId)).ReturnsAsync(false);

            // --- Act & Assert ---
            // We assert that calling the service method throws the specific exception we expect.
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(userEmail, workoutId));
        }
    }
}