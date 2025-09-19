/*
PSEUDOKOD:
// Innehåller bara den information vi behöver från frontend
KLASS CreateBookingDto
    EGENSKAP UserId
    EGENSKAP WorkoutId
SLUT KLASS
*/

namespace API.DTOs
{
    public class CreateBookingDto
    {
        public int UserId { get; set; }
        public int WorkoutId { get; set; }
        public DateTime StartTime { get; set; }
    }
}