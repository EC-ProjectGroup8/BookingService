namespace API.Entities
{
    public class BookingEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int WorkoutId { get; set; }
    }
}