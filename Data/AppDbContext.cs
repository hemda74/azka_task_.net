using Microsoft.EntityFrameworkCore;
using task_amwag.Models;

namespace task_amwag.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Leave> Leaves { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Name)
                .IsUnique(); 

            modelBuilder.Entity<Leave>()
                .HasOne(l => l.Employee)
                .WithMany(e => e.Leaves)
                .HasForeignKey(l => l.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
