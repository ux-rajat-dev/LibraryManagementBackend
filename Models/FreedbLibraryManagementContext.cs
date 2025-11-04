using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Models
{
public partial class FreedbLibraryManagementContext : DbContext
{
public FreedbLibraryManagementContext()
{
}


    public FreedbLibraryManagementContext(DbContextOptions<FreedbLibraryManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }
    public virtual DbSet<Book> Books { get; set; }
    public virtual DbSet<Borrowtransaction> Borrowtransactions { get; set; }
    public virtual DbSet<Genre> Genres { get; set; }
    public virtual DbSet<Refreshtoken> Refreshtokens { get; set; }
    public virtual DbSet<Review> Reviews { get; set; }
    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Read from configuration (Program.cs already passes connection string)
            var connString = "Host=dpg-d41lfnbipnbc73fg69kg-a.oregon-postgres.render.com;Port=5432;Database=librarymanagementhero;Username=uxrajatdev;Password=PSUCmxdRu8n6lzTPQ5SkPt95JjF0mA0p;SSL Mode=Require;Trust Server Certificate=true;";
            optionsBuilder.UseNpgsql(connString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
        entity.HasKey(e => e.AuthorId);
        entity.ToTable("authors");
    
        entity.Property(e => e.AuthorId).HasColumnName("authorid");
        entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(10000);
        entity.Property(e => e.Bio).HasColumnName("bio");
        });


        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.BookId);
            entity.ToTable("books");

            entity.Property(e => e.BookId).HasColumnName("bookid");
            entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(200);
            entity.Property(e => e.Isbn).HasColumnName("isbn").HasMaxLength(20);
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.CoverImageUrl).HasColumnName("coverimageurl");
            entity.Property(e => e.TotalCopies).HasColumnName("totalcopies").HasDefaultValue(1);
            entity.Property(e => e.AvailableCopies).HasColumnName("availablecopies").HasDefaultValue(1);
            entity.Property(e => e.AuthorId).HasColumnName("authorid");
            entity.Property(e => e.GenreId).HasColumnName("genreid");

            entity.HasOne(d => d.Author)
                .WithMany(p => p.Books)
                .HasForeignKey(d => d.AuthorId);
        
            entity.HasOne(d => d.Genre)
                .WithMany(p => p.Books)
                .HasForeignKey(d => d.GenreId);
        });


        modelBuilder.Entity<Borrowtransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId);
            entity.ToTable("borrowtransactions");

            entity.Property(e => e.TransactionId).HasColumnName("transactionid");
            entity.Property(e => e.BookId).HasColumnName("bookid");
            entity.Property(e => e.UserId).HasColumnName("userid");
            entity.Property(e => e.FineAmount).HasPrecision(10, 2).HasDefaultValue(0.00m);

            entity.Property(e => e.BorrowDate).HasColumnName("borrowdate"); // if exists
            entity.Property(e => e.DueDate).HasColumnName("duedate");       // <-- add this mapping
            entity.Property(e => e.ReturnDate).HasColumnName("returndate"); // if exists

            entity.HasOne(d => d.Book)
                  .WithMany(p => p.Borrowtransactions)
                  .HasForeignKey(d => d.BookId);

            entity.HasOne(d => d.User)
                  .WithMany(p => p.Borrowtransactions)
                  .HasForeignKey(d => d.UserId);

            entity.Property(e => e.FineAmount)
            .HasColumnName("fineamount")
            .HasPrecision(10, 2)
            .HasDefaultValue(0.00m);
            
            entity.Property(e => e.Status)
          .HasColumnName("status");
        });


        modelBuilder.Entity<Genre>(entity =>
{
    entity.HasKey(e => e.GenreId);
    entity.ToTable("genres");

    entity.Property(e => e.GenreId).HasColumnName("genreid");
    entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100);
});


        modelBuilder.Entity<Refreshtoken>(entity =>
        {
            entity.HasKey(e => e.TokenId);
            entity.ToTable("refreshtokens");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsRevoked).HasDefaultValue(false);

            entity.HasOne(d => d.User)
                .WithMany(p => p.Refreshtokens)
                .HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId);
            entity.ToTable("reviews");
            entity.Property(e => e.ReviewDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Book)
                .WithMany(p => p.Reviews)
                .HasForeignKey(d => d.BookId);

            entity.HasOne(d => d.User)
                .WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<User>(entity =>
{
    entity.HasKey(e => e.UserId);
    entity.ToTable("users");

    entity.Property(e => e.UserId).HasColumnName("userid"); 
    entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(100);
    entity.Property(e => e.FullName).HasColumnName("fullname").HasMaxLength(100);
    entity.Property(e => e.Role).HasColumnName("role").HasDefaultValue("user");
    entity.Property(e => e.CreatedAt).HasColumnName("createdat").HasDefaultValueSql("CURRENT_TIMESTAMP");
    entity.Property(e => e.PasswordHash).HasColumnName("passwordhash");
});

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}


}
