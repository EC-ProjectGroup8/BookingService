using API.DbContext;
using API.Entities;
using API.Models;
using API.Repositories.Interfaces;
using System.Threading.Tasks;

namespace API.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly BookingDbContext _context;

        public BookingRepository(BookingDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<Booking> SaveAsync(Booking booking)
        {
            // Create a new database entity from the incoming model.
            var entity = new BookingEntity
            {
                UserEmail = booking.UserEmail,
                WorkoutIdentifier = booking.WorkoutIdentifier
            };

            // Stage the new entity for insertion into the database.
            _context.Bookings.Add(entity);

            // Asynchronously save all changes to the database.
            await _context.SaveChangesAsync();

            // Map the database-generated Id back to the model.
            booking.Id = entity.Id;

            // Return the updated model, now including the new Id.
            return booking;
        }
    }
}