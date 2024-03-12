using Microsoft.EntityFrameworkCore;
using Task1.Database.Entities;

namespace Task1.Database
{
    public class Task1Context: DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<GameTransactions> GameTransactions { get; set; }
        public virtual DbSet<MatchHistory> MatchHistory { get; set; }

        public Task1Context(DbContextOptions<Task1Context> options) : base(options) 
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
