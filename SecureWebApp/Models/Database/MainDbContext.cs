﻿using System;
using Microsoft.EntityFrameworkCore;
using SecureWebApp.Extensions.ConnectionService;

namespace SecureWebApp.Models.Database
{

    public partial class MainDbContext : DbContext
    {
        
        private readonly IConnectionService FConnectionService;

        public MainDbContext(DbContextOptions<MainDbContext> options, IConnectionService AConnectionService) : base(options)
        {
            FConnectionService = AConnectionService;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder AOptionsBuilder)
        {
            var ConnectionString = FConnectionService.GetMainDatabase();
            /// <seealso cref="https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency"/>
            AOptionsBuilder.UseSqlServer(ConnectionString, AddOptions => AddOptions.EnableRetryOnFailure());
        }

        public virtual DbSet<Cities> Cities { get; set; }
        public virtual DbSet<Countries> Countries { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<LogHistory> LogHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Cities>(entity =>
            {
                entity.Property(e => e.CityName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.Cities)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CountryId__Cities");
            });

            modelBuilder.Entity<Countries>(entity =>
            {
                entity.Property(e => e.CountryCode)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.CountryName)
                    .IsRequired()
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<LogHistory>(entity =>
            {

                entity.Property(e => e.LoggedAt).HasColumnType("datetime");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.LogHistory)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__UserId__Users");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasIndex(e => e.EmailAddr)
                    .HasName("UQ__EmailAddr__Users")
                    .IsUnique();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.EmailAddr)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.NickName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneNum)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.HasOne(d => d.City)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.CityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CityId__Users");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CountryId__Users");
            });

            OnModelCreatingPartial(modelBuilder);
        
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    }

}
