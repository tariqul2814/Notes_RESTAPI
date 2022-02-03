using InsightRESTAPI.Model.DBModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InsightRESTAPI.Model.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }
        public DbSet<RefreshToken> RefreshToken { get; set; }
        public DbSet<Notes> Notes { get; set; }
        public DbSet<BookmarkNote> BookmarkNotes { get; set; }
        public DbSet<RegularNote> RegularNotes { get; set; }
        public DbSet<ReminderNote> ReminderNotes { get; set; }
        public DbSet<TaskNote> TaskNotes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
