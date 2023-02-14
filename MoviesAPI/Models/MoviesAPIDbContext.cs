using Microsoft.EntityFrameworkCore;

namespace MoviesAPI.Models
{
    public class MoviesAPIDbContext : DbContext
    {
        public MoviesAPIDbContext(DbContextOptions<MoviesAPIDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rating>()
                .HasKey(c => new { c.user_id, c.movie_id });

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.User)
                .WithMany(u => u.Ratings)
                .HasForeignKey(r => r.user_id);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Movie)
                .WithMany(m => m.Ratings)
                .HasForeignKey(r => r.movie_id);
        }
    }
}
