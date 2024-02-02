using Microsoft.EntityFrameworkCore;
using MOWebAPI.Models;

namespace MOWebAPI.Data
{
    public class MedicalOfficeContext : DbContext
    {
        public MedicalOfficeContext(DbContextOptions<MedicalOfficeContext> options)
            : base(options)
        {

        }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //Add a unique index to the OHIP Number
            modelBuilder.Entity<Patient>()
            .HasIndex(p => p.OHIP)
            .IsUnique();

            modelBuilder.Entity<Doctor>()
                .HasMany(p => p.Patients)
                .WithOne(d => d.Doctor)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
