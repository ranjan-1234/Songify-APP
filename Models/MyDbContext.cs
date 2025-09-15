using Microsoft.EntityFrameworkCore;

namespace Singer.Models
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        public DbSet<SongDto> Songs { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SongDto>(entity =>
            {
                entity.HasKey(e => e.SongId);

                entity.Property(e => e.FileName)
                      .HasMaxLength(255)  // varchar(255)
                      .IsRequired();

                entity.Property(e => e.FilePath)
                      .HasMaxLength(500)  // varchar(500)
                      .IsRequired();

                entity.Property(e => e.UploadedAt)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserId).IsRequired();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.Property(e => e.Email)
                      .HasMaxLength(255)
                      .IsRequired();

                entity.Property(e => e.PasswordHash)
                      .IsRequired();

                entity.Property(e => e.PasswordSalt)
                      .IsRequired();

                entity.Property(e => e.CreatedAt)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
        }
    }
}
