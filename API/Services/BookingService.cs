/*
PSEUDOKOD:
// Hjärnan. Innehåller all affärslogik.
METOD SkapaBokning(userId, workoutId)
    - Validering och regler
    - användare = Hämta användare från databasen via UserRepository
    - pass = Hämta pass från databasen via WorkoutRepository
    - OM användare FINNS INTE ELLER pass FINNS INTE
        - RETURNERA misslyckat("Användare eller pass hittades inte")
    - Skapa ett nytt bokningsobjekt (Model/Entity)
    - Anropa BookingRepository.Spara(nyBokning)
    - RETURNERA lyckat
*/

using API.Models;
using API.Repositories;

namespace API.Services
{
    public class BookingService
    {
        private readonly BookingRepository _bookingRepository;

        // Repositories injectas via konstruktorn
        public BookingService(BookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public bool SkapaBokning(int userId, int workoutId, DateTime startTime)
        {
            // Enkel validering
            if (userId <= 0 || workoutId <= 0)
                return false;

            var booking = new Booking
            {
                UserId = userId,
                WorkoutId = workoutId,
                StartTime = startTime
            };

            try
            {
                _bookingRepository.Spara(booking);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}