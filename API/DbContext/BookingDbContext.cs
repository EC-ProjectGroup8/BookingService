using Microsoft.EntityFrameworkCore;
using API.Entities;

namespace API.DbContext
{
    public class BookingDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> options)
            : base(options) { }

        public DbSet<BookingEntity> Bookings { get; set; }
    }
}