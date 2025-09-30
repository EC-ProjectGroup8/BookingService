using API.DbContext;
using API.DTOs;
using API.Entities;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<WorkoutIdDto>> GetMyBookingsAsync(string email)
        {
            var bookings = await _context.Bookings
                .Where(b => b.UserEmail == email)
                .Select(b => new WorkoutIdDto
                {
                    Id = b.Id,
                    UserEmail = b.UserEmail,
                    WorkoutIdentifier = b.WorkoutIdentifier
                })
                .ToListAsync();
            return bookings;
        }
    }
}