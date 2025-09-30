namespace API.DTOs
{
    public class WorkoutIdDto
    {
        public int Id { get; set; }
        public string UserEmail { get; set; } = null!;
        public string WorkoutIdentifier { get; set; } = null!;


    }
}
