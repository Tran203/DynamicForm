using DynamicForm.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DynamicForm.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Form> Forms { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<Response> Responses { get; set; }
        public DbSet<ResponseOption> ResponseOptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure composite keys or indexes if needed
            modelBuilder.Entity<ResponseOption>()
                .HasKey(ro => new { ro.ResponseId, ro.OptionId });

            // Keep cascade from Form → Questions → Options
            modelBuilder.Entity<Question>()
                .HasOne(q => q.Form)
                .WithMany(f => f.Questions)
                .HasForeignKey(q => q.FormId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Option>()
                .HasOne(o => o.Question)
                .WithMany(q => q.Options)
                .HasForeignKey(o => o.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Keep cascade from Form → Responses (optional, but safe)
            modelBuilder.Entity<Response>()
                .HasOne(r => r.Form)
                .WithMany(f => f.Responses)
                .HasForeignKey(r => r.FormId)
                .OnDelete(DeleteBehavior.Cascade);  // OK: direct path

            // CRITICAL: NO CASCADE on Question → Response
            modelBuilder.Entity<Response>()
                .HasOne(r => r.Question)
                .WithMany(q => q.Responses)
                .HasForeignKey(r => r.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);  // or ClientSetNull if nullable

            // ResponseOptions: Composite Key
            modelBuilder.Entity<ResponseOption>()
                .HasKey(ro => new { ro.ResponseId, ro.OptionId });

            // FK: ResponseOptions → Responses (NO CASCADE)
            modelBuilder.Entity<ResponseOption>()
                .HasOne(ro => ro.Response)
                .WithMany(r => r.ResponseOptions)
                .HasForeignKey(ro => ro.ResponseId)
                .OnDelete(DeleteBehavior.Restrict);  // Critical: prevents cycle

            // FK: ResponseOptions → Options (CASCADE OK)
            modelBuilder.Entity<ResponseOption>()
                .HasOne(ro => ro.Option)
                .WithMany(o => o.ResponseOptions)
                .HasForeignKey(ro => ro.OptionId)
                .OnDelete(DeleteBehavior.Cascade);   // Safe: single path
        }
    }
}
