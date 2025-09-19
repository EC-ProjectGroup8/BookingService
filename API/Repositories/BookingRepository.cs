using API.Models;
using API.Entities;
using API.DbContext;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<Booking> SparaAsync(Booking booking)
        {
            // Steg 1: Mappa från Booking (modell) till BookingEntity
            var entity = new BookingEntity
            {
                UserId = booking.UserId,
                WorkoutId = booking.WorkoutId,
                StartTime = booking.StartTime
            };

            await _context.Bookings.AddAsync(entity);
            await _context.SaveChangesAsync();

            // Steg 2: Mappa tillbaka till Booking (för att få med det nya Id:t)
            booking.Id = entity.Id;
            return booking;
        }

        public async Task<IEnumerable<Booking>> HämtaFörPassAsync(int workoutId)
        {
            return await _context.Bookings
                .Where(b => b.WorkoutId == workoutId)
                // Steg 3: Mappa varje BookingEntity till en Booking-modell
                .Select(entity => new Booking
                {
                    Id = entity.Id,
                    UserId = entity.Id,
                    WorkoutId = entity.WorkoutId,
                    StartTime = entity.StartTime
                })
                .ToListAsync();
        }
    }
}