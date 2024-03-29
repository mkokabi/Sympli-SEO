﻿using Microsoft.EntityFrameworkCore;
using Repository.Model;
using System.Reflection;

namespace Repository
{
    public class SearchResultsContext : DbContext
    {
        public DbSet<SearchResult> SearchResults { get; set; }
        public DbSet<Search> Searches { get; set; }

        public SearchResultsContext(DbContextOptions<SearchResultsContext> dbContextOptions) :
            base(dbContextOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Search>()
                .ToTable("Search");

            modelBuilder.Entity<SearchResult>()
                .ToTable("SearchResult");
        }
    }
}
