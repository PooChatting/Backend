using Microsoft.EntityFrameworkCore;
using Poochatting.DbContext.Entities;

namespace Poochatting.DbContext
{
    public class MessageDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        private string _connectionString = "Server=(localdb)\\mssqllocaldb;Database=MessageDb;Trusted_Connection=True;";
        public DbSet<Message> Messages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(r => r.Email)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(r => r.Username)
                .IsRequired();

            modelBuilder.Entity<Role>()
                .Property(r => r.Name)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(r => r.Email)
                .IsRequired();

            modelBuilder.Entity<Message>()
                .Property(r => r.MessageText)
                .IsRequired()
                .HasMaxLength(2048);

            modelBuilder.Entity<Message>()
                .Property(r => r.ChannelId)
                .IsRequired();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
