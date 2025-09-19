using Xunit;
using API.Repositories;
using API.Models;
using API.Entities;
using API.DbContext;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace TestingBookingService.Repositories
{
    public class BookingRepositoryTests
    {
        private BookingDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<BookingDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            var context = new BookingDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        // Borta: async Task
        public async Task Spara_ShouldAddBookingToDatabase()
        {
            // --- Arrange ---
            using var context = GetDbContext();
            var repository = new BookingRepository(context);

            var bookingModel = new Booking
            {
                UserId = 1,
                WorkoutId = 101
                // Borta: StartTime
            };

            // --- Act ---
            await repository.SparaAsync(bookingModel);

            // --- Assert ---
            var savedEntity = context.Bookings.FirstOrDefault(b => b.WorkoutId == 101);

            Assert.NotNull(savedEntity);
            Assert.Equal(1, savedEntity.UserId);
        }

        // Vi tar bort testet för HämtaFörPassAsync eftersom den metoden inte
        // längre behövs av vår nya, enklare BookingService.
    }
}