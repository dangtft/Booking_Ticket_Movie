﻿using Booking_Movie_Tickets.Models;
using Booking_Movie_Tickets.Models.Rooms;
using Booking_Movie_Tickets.Models.Movies;
using Booking_Movie_Tickets.Models.Orders;
using Booking_Movie_Tickets.Models.Payments;
using Booking_Movie_Tickets.Models.Tickets;
using Booking_Movie_Tickets.Models.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Booking_Movie_Tickets.Data
{
    public class BookingDbContext : IdentityDbContext<User>
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }

        public DbSet<SeatStatusTracking> SeatStatusTracking { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<AgeRating> AgeRatings { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Extra> Extras { get; set; }
        public DbSet<ExtrasCategory> ExtrasCategories { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<MovieGenre> MovieGenres { get; set; }
        public DbSet<MovieMedia> MovieMedias { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<PaymentStatus> PaymentStatuses { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<SeatType> SeatTypes { get; set; }
        public DbSet<Showtime> Showtimes { get; set; }
        public DbSet<TicketStatus> TicketStatuses { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<MovieActor> MovieActors { get; set; }
        public DbSet<OrderDetailExtras> OrderDetailExtras { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ticket>()
               .HasOne(t => t.OrderDetail)
               .WithMany(od => od.Tickets)
               .HasForeignKey(t => t.OrderDetailId);

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OrderDetailExtras>()
                .HasKey(oe => new { oe.OrderDetailId, oe.ExtraId });

            modelBuilder.Entity<SeatStatusTracking>()
                .HasKey(oe => new { oe.SeatId, oe.ShowTimeId });

            modelBuilder.Entity<OrderDetailExtras>()
                .HasOne(oe => oe.OrderDetail)
                .WithMany(od => od.OrderDetailExtras)
                .HasForeignKey(oe => oe.OrderDetailId);

            modelBuilder.Entity<OrderDetailExtras>()
                .HasOne(oe => oe.Extra)
                .WithMany(e => e.OrderDetailExtras)
                .HasForeignKey(oe => oe.ExtraId);

            modelBuilder.Entity<Extra>()
                .Property(e => e.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SeatStatusTracking>()
               .HasOne(sst => sst.Seat)
               .WithMany()
               .HasForeignKey(sst => sst.SeatId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SeatStatusTracking>()
                .HasOne(sst => sst.Showtime)
                .WithMany()
                .HasForeignKey(sst => sst.ShowTimeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình khóa chính cho MovieGenre 
            modelBuilder.Entity<MovieGenre>()
                .HasKey(mg => new { mg.MovieId, mg.GenreId });
            modelBuilder.Entity<MovieActor>()
                .HasKey(mg => new { mg.MovieId, mg.ActorId });

            // Thiết lập quan hệ Movie - MovieGenre
            modelBuilder.Entity<MovieGenre>()
                .HasOne(mg => mg.Movie)
                .WithMany(m => m.MovieGenres)
                .HasForeignKey(mg => mg.MovieId);

            // Thiết lập quan hệ Genre - MovieGenre
            modelBuilder.Entity<MovieGenre>()
                .HasOne(mg => mg.Genre)
                .WithMany(g => g.MovieGenres)
                .HasForeignKey(mg => mg.GenreId);

            // Cấu hình quan hệ ChangeHistory -> User (AspNetUsers)
            modelBuilder.Entity<ChangeHistory>()
               .HasOne(ch => ch.ChangedBy)
               .WithMany()
               .HasForeignKey(ch => ch.Change_By)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
