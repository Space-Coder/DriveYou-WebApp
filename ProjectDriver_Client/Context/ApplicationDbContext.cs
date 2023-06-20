using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using DriveYOU_WebClient.Models;

namespace DriveYOU_WebClient.Context
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<ScheduledTripsModel> ScheduledTrips { get; set; }
        public DbSet<EndedTripsModel> EndedTrips { get; set; }
        public DbSet<UserReviewModel> UserReviews { get; set; }
        public DbSet<SubscribedOnTripsModel> SubscribedOnTrips { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            //Constructor
            // Database.EnsureDeleted();
            // Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Number).IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email).IsUnique();
        }
    }
}
