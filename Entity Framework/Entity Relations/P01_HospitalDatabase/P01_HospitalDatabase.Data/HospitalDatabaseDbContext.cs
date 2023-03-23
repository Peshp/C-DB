using Microsoft.EntityFrameworkCore;
using P01_HospitalDatabase.Data.Models;

namespace P01_HospitalDatabase.Data
{
    public class HospitalDatabaseDbContext : DbContext
    {
        public HospitalDatabaseDbContext(DbContextOptions options) 
            :base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasKey(p => p.PatiantId);

                entity.Property(p => p.FirstName).HasMaxLength(50).IsUnicode(true);
                entity.Property(p => p.LastName).HasMaxLength(50).IsUnicode(true);
                entity.Property(p => p.Address).HasMaxLength(250).IsUnicode(true);
                entity.Property(p => p.Email).HasMaxLength(80).IsUnicode(true);
            });

            modelBuilder.Entity<Visitation>(entity =>
            {
                entity.HasKey(v => v.VisitationId);

                entity.Property(v => v.Comments).HasMaxLength(250).IsUnicode(true);
            });

            modelBuilder.Entity<Diagnose>(entity =>
            {
                entity.HasKey(d => d.DiagnoseId);

                entity.Property(d => d.Name).HasMaxLength(50).IsUnicode(true);
                entity.Property(d => d.Comments).HasMaxLength(250).IsUnicode(true);
            });

            modelBuilder.Entity<Medicament>(entity =>
            {
                entity.HasKey(m => m.MedicamentId);

                entity.Property(m => m.Name).HasMaxLength(50).IsUnicode(true);
            });
        }
    }
}