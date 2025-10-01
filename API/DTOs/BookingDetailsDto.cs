namespace API.DTOs
{
    public class BookingDetailsDto
    {
        public string Title { get; set; } = null!;
        public string Location { get; set; } = null!;

        public DateTime StartTime { get; set; }

        public string Instructor { get; set; } = null!;
    }
}
