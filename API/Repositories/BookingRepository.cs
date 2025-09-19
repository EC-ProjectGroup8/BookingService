using API.DbContext;
using API.Entities;
using API.Models;
using API.Repositories.Interfaces;

namespace API.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly BookingDbContext _context;

        public BookingRepository(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<Booking> SparaAsync(Booking booking)
        {
            var entity = new BookingEntity
            {
                UserId = booking.UserId,
                WorkoutId = booking.WorkoutId
            };

            _context.Bookings.Add(entity);
            _context.SaveChanges();

            // Mappa tillbaka det nya Id:t till modellen som vi returnerar
            booking.Id = entity.Id;
            return booking;
        }
    }
}