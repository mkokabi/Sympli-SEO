using Microsoft.EntityFrameworkCore;
using Sympli.SEO.Common.DataTypes;
using System;
using System.Reflection;

namespace Repository
{
    public class SearchResultsContext : DbContext
    {
        public DbSet<SearchResult> SearchResults { get; set; }

        public SearchResultsContext(DbContextOptions<SearchResultsContext> dbContextOptions) :
            base(dbContextOptions)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=SOE_Database.db", options =>
            {
                options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
            });
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SearchResult>(entity =>
            {
                entity.HasKey(b => b.Id);
            });

            modelBuilder.Entity<SearchResult>()
                .ToTable("SearchResult")
                .Ignore(b => b.Keywords)
                .Ignore(b => b.Results);
        }
    }
}
