using Xunit;
using API.Repositories;
using API.Models;
using API.Entities;
using API.DbContext;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks; // Added for async operations

namespace TestingBookingService.Repositories
{
    public class BookingRepositoryTests
    {
        /// <summary>
        /// Helper method to create a fresh in-memory database for each test.
        /// Using a new GUID for the database name ensures that tests are isolated.
        /// </summary>
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
        public async Task SaveAsync_ShouldAddBookingToDatabase()
        {
            // --- Arrange ---
            await using var context = GetDbContext();
            var repository = new BookingRepository(context);

            // Create the booking model with the new string properties.
            var bookingModel = new Booking
            {
                UserEmail = "test.user@example.com",
                WorkoutIdentifier = "strength-101"
            };

            // --- Act ---
            // Call the method under test.
            await repository.SaveAsync(bookingModel);

            // --- Assert ---
            // Query the database using the new string identifier to verify the entity was saved.
            var savedEntity = await context.Bookings.FirstOrDefaultAsync(b => b.WorkoutIdentifier == "strength-101");

            // Check that an entity was found.
            Assert.NotNull(savedEntity);
            // Verify that the data was saved correctly.
            Assert.Equal("test.user@example.com", savedEntity.UserEmail);
        }

        // The test for GetByWorkoutAsync (HämtaFörPassAsync) has been removed,
        // as the method is no longer needed by the simplified BookingService.
    }
}