using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ORM;

public partial class Test1Context : DbContext
{
    public Test1Context()
    {
    }

    public Test1Context(DbContextOptions<Test1Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Facility> Facilities { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=exercises;Username=postgres;Password=123");
        optionsBuilder.LogTo(message => System.Diagnostics.Debug.WriteLine(message));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Bookid).HasName("bookings_pk");

            entity.ToTable("bookings");

            entity.HasIndex(e => new { e.Facid, e.Memid }, "bookings.facid_memid");

            entity.HasIndex(e => new { e.Facid, e.Starttime }, "bookings.facid_starttime");

            entity.HasIndex(e => new { e.Memid, e.Facid }, "bookings.memid_facid");

            entity.HasIndex(e => new { e.Memid, e.Starttime }, "bookings.memid_starttime");

            entity.HasIndex(e => e.Starttime, "bookings.starttime");

            entity.Property(e => e.Bookid)
                .ValueGeneratedNever()
                .HasColumnName("bookid");
            entity.Property(e => e.Facid).HasColumnName("facid");
            entity.Property(e => e.Memid).HasColumnName("memid");
            entity.Property(e => e.Slots).HasColumnName("slots");
            entity.Property(e => e.Starttime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("starttime");

            entity.HasOne(d => d.Fac).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.Facid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_bookings_facid");

            entity.HasOne(d => d.Mem).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.Memid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_bookings_memid");
        });

        modelBuilder.Entity<Facility>(entity =>
        {
            entity.HasKey(e => e.Facid).HasName("facilities_pk");

            entity.ToTable("facilities");

            entity.Property(e => e.Facid)
                .ValueGeneratedNever()
                .HasColumnName("facid");
            entity.Property(e => e.Guestcost).HasColumnName("guestcost");
            entity.Property(e => e.Initialoutlay).HasColumnName("initialoutlay");
            entity.Property(e => e.Membercost).HasColumnName("membercost");
            entity.Property(e => e.Monthlymaintenance).HasColumnName("monthlymaintenance");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.Memid).HasName("members_pk");

            entity.ToTable("members");

            entity.HasIndex(e => e.Joindate, "members.joindate");

            entity.HasIndex(e => e.Recommendedby, "members.recommendedby");

            entity.Property(e => e.Memid)
                .ValueGeneratedNever()
                .HasColumnName("memid");
            entity.Property(e => e.Address)
                .HasMaxLength(300)
                .HasColumnName("address");
            entity.Property(e => e.Firstname)
                .HasMaxLength(200)
                .HasColumnName("firstname");
            entity.Property(e => e.Joindate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("joindate");
            entity.Property(e => e.Recommendedby).HasColumnName("recommendedby");
            entity.Property(e => e.Surname)
                .HasMaxLength(200)
                .HasColumnName("surname");
            entity.Property(e => e.Telephone)
                .HasMaxLength(20)
                .HasColumnName("telephone");
            entity.Property(e => e.Zipcode).HasColumnName("zipcode");

            entity.HasOne(d => d.RecommendedbyNavigation).WithMany(p => p.InverseRecommendedbyNavigation)
                .HasForeignKey(d => d.Recommendedby)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_members_recommendedby");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
