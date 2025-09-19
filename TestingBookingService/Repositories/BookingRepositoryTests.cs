using Xunit;
using API.Repositories;
using API.Models;
using API.Entities;
using API.DbContext;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace TestingBookingService.Repositories
{
    public class BookingRepositoryTests
    {
        private async Task<BookingDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<BookingDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString()) // Unik databas för varje test
                .Options;
            var context = new BookingDbContext(options);
            await context.Database.EnsureCreatedAsync();
            return context;
        }

        [Fact]
        public async Task SparaAsync_ShouldAddBookingEntityToDatabase()
        {
            // --- Arrange ---
            await using var context = await GetDbContext();
            var repository = new BookingRepository(context);

            var bookingModel = new Booking
            {
                UserId = 1,
                WorkoutId = 101,
                StartTime = DateTime.UtcNow
            };

            // --- Act ---
            await repository.SparaAsync(bookingModel);

            // --- Assert ---
            var savedEntity = await context.Bookings.FirstOrDefaultAsync(b => b.WorkoutId == 101);

            Assert.NotNull(savedEntity);
            Assert.Equal(1, savedEntity.UserId);
        }

        [Fact]
        public async Task HämtaFörPassAsync_ShouldReturnCorrectBookings()
        {
            // --- Arrange ---
            await using var context = await GetDbContext();

            // Lägg in testdata direkt i databasen
            context.Bookings.AddRange(new List<BookingEntity>
            {
                new BookingEntity { WorkoutId = 202, UserId = 1 },
                new BookingEntity { WorkoutId = 202, UserId = 2 },
                new BookingEntity { WorkoutId = 303, UserId = 3 } // Annat pass
            });
            await context.SaveChangesAsync();

            var repository = new BookingRepository(context);

            // --- Act ---
            var result = await repository.HämtaFörPassAsync(202);

            // --- Assert ---
            Assert.NotNull(result);
            Assert.Equal(2, result.Count()); // Vi ska bara få tillbaka två bokningar
            Assert.True(result.All(b => b.WorkoutId == 202));
        }
    }
}