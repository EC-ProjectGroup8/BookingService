using Xunit;
using API.Repositories;
using API.Models;
using API.Entities;
using API.DbContext;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace TestingBookingService.Repositories
{
    public class BookingRepositoryTests
    {
        /// <summary>
        /// Helper method to create a fresh in-memory database for each test.
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
            var bookingModel = new Booking
            {
                UserEmail = "test.user@example.com",
                WorkoutIdentifier = "strength-101"
            };

            // --- Act ---
            await repository.SaveAsync(bookingModel);

            // --- Assert ---
            var savedEntity = await context.Bookings.FirstOrDefaultAsync(b => b.WorkoutIdentifier == "strength-101");
            Assert.NotNull(savedEntity);
            Assert.Equal("test.user@example.com", savedEntity.UserEmail);
        }

        // --- NYA TESTER FÖR DELETEBOOKINGASYNC ---

        [Fact]
        public async Task DeleteBookingAsync_ShouldReturnTrueAndRemoveBooking_WhenBookingExists()
        {
            // --- Arrange ---
            // Get a fresh database and repository for this test
            await using var context = GetDbContext();
            var repository = new BookingRepository(context);

            // Add a booking entity to the database so we have something to delete
            var bookingEntity = new BookingEntity { UserEmail = "delete.me@example.com", WorkoutIdentifier = "workout-to-delete" };
            context.Bookings.Add(bookingEntity);
            await context.SaveChangesAsync();

            // Verify it was added correctly before we act
            Assert.Equal(1, await context.Bookings.CountAsync());

            // --- Act ---
            // Call the method under test with the correct identifiers
            var result = await repository.DeleteBookingAsync("delete.me@example.com", "workout-to-delete");

            // --- Assert ---
            // The method should return true indicating success
            Assert.True(result);
            // The booking should no longer be in the database
            Assert.Equal(0, await context.Bookings.CountAsync());
        }

        [Fact]
        public async Task DeleteBookingAsync_ShouldReturnFalse_WhenBookingDoesNotExist()
        {
            // --- Arrange ---
            await using var context = GetDbContext();
            var repository = new BookingRepository(context);

            // (Optional) Add a different booking to ensure we don't get a false positive from an empty DB
            var otherBooking = new BookingEntity { UserEmail = "other.user@example.com", WorkoutIdentifier = "other-workout" };
            context.Bookings.Add(otherBooking);
            await context.SaveChangesAsync();

            // --- Act ---
            // Attempt to delete a booking that does not exist
            var result = await repository.DeleteBookingAsync("non.existent@example.com", "non-existent-workout");

            // --- Assert ---
            // The method should return false indicating failure
            Assert.False(result);
            // The number of items in the database should be unchanged
            Assert.Equal(1, await context.Bookings.CountAsync());
        }
    }
}