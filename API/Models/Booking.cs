/*
PSEUDOKOD:
// Representerar hur en bokning ser ut i databasen
KLASS Booking
    EGENSKAP Id (primärnyckel)
    EGENSKAP UserId (främmande nyckel till User)
    EGENSKAP WorkoutId (främmande nyckel till Workout)
    EGENSKAP BookingDate
SLUT KLASS
*/

namespace API.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int WorkoutId { get; set; }
        public DateTime StartTime { get; set; }
    }
}