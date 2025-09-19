/*
PSEUDOKOD:
// Pratar med databasen. Inga regler, bara spara/hämta.
METOD Spara(bokningsobjekt)
    - Kod för att ansluta till databasen (t.ex. DbContext)
    - databas.Bookings.LäggTill(bokningsobjekt)
    - databas.SparaÄndringar()
*/

using API.Models;
using API.Entities;
using API.DbContext;

namespace API.Repositories
{
    public class BookingRepository
    {
        private readonly BookingDbContext _context;

        public BookingRepository(BookingDbContext context)
        {
            _context = context;
        }

        public void Spara(Booking booking)
        {
            // Mappa från modell till entity
            var entity = new BookingEntity
            {
                Id = booking.Id,
                UserId = booking.UserId,
                WorkoutId = booking.WorkoutId,
                StartTime = booking.StartTime
            };

                    // Här sparar du entity till databasen
                    _context.Bookings.Add(entity);
                    _context.SaveChanges();
        }
    }
}