using Xunit;
using Moq;
using Moq.Protected; // För att mocka HttpClient
using API.Services;
using API.Repositories;
using API.Models;
using API.DTOs;
using System.Net;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace TestingBookingService.Services
{
    public class BookingServiceTests
    {
        private readonly Mock<IBookingRepository> _mockRepo;
        private readonly Mock<HttpMessageHandler> _mockHttpHandler;
        private readonly HttpClient _httpClient;
        private readonly BookingService _service;

        // Konstruktor som körs före varje test
        public BookingServiceTests()
        {
            _mockRepo = new Mock<IBookingRepository>();
            _mockHttpHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpHandler.Object)
            {
                BaseAddress = new System.Uri("http://fake-workout-api.com")
            };

            // Skapa servicen med våra mock-objekt
            _service = new BookingService(_mockRepo.Object, _httpClient);
        }

        [Fact]
        public async Task SkapaBokning_ShouldReturnBooking_WhenSuccessful()
        {
            // --- Arrange ---
            var workoutDto = new WorkoutDto { Id = 1, Capacity = 10, StartTime = DateTime.UtcNow };
            var existingBookings = new List<Booking>(); // Inga befintliga bokningar

            // Ställ in mocken för HttpClient
            _mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(workoutDto)
                });

            // Ställ in mocken för Repository
            _mockRepo.Setup(r => r.HämtaFörPassAsync(1)).ReturnsAsync(existingBookings);
            _mockRepo.Setup(r => r.SparaAsync(It.IsAny<Booking>())).ReturnsAsync((Booking b) => b);

            // --- Act ---
            var result = await _service.SkapaBokning("1", 1);

            // --- Assert ---
            Assert.NotNull(result);
            Assert.Equal(1, result.UserId);
        }

        [Fact]
        public async Task SkapaBokning_ShouldReturnNull_WhenWorkoutIsFull()
        {
            // --- Arrange ---
            var workoutDto = new WorkoutDto { Id = 2, Capacity = 1, StartTime = DateTime.UtcNow };
            // Föreställ dig att en bokning redan finns, så passet är fullt
            var existingBookings = new List<Booking> { new Booking { UserId = 99, WorkoutId = 2 } };

            // Hela Setup-anropet måste vara med här också
            _mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(workoutDto)
                });

            _mockRepo.Setup(r => r.HämtaFörPassAsync(2)).ReturnsAsync(existingBookings);

            // --- Act ---
            var result = await _service.SkapaBokning("1", 2);

            // --- Assert ---
            Assert.Null(result); // Förväntar oss null eftersom passet är fullt
        }
    }
}