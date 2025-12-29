using Microsoft.EntityFrameworkCore;
using RentConnect.API.RentConnect.Domain.Models;

namespace RentConnect.API.RentConnect.Infrastructure.DataAccess;

public static class DbContextExtensions
{
    public static void ConfigureJunctionTable(this ModelBuilder builder)
    {
        builder.Entity<Conversation>()
            .HasMany(c => c.Messages)
            .WithOne(m => m.Conversation)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        //builder.Entity<ChatMessage>()
        //    .HasOne(c => c.Sender)
        //    .WithMany()
        //    .HasForeignKey(c => c.SenderId)
        //    .OnDelete(DeleteBehavior.Restrict);

        //builder.Entity<ChatMessage>()
        //    .HasOne(c => c.Receiver)
        //    .WithMany()
        //    .HasForeignKey(c => c.ReceiverId)
        //    .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Favorite>()
            .HasIndex(f => new { f.UserId, f.ItemId })
            .IsUnique();

        builder.Entity<Item>()
            .HasOne(i => i.Category)
            .WithMany()
            .HasForeignKey(i => i.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Booking>()
            .HasOne(b => b.Item)
            .WithMany(i => i.Bookings)
            .HasForeignKey(b => b.ItemId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

        builder.Entity<Booking>()
            .HasOne(b => b.Renter)
            .WithMany(u => u.Bookings)
            .HasForeignKey(b => b.RenterId)
            .OnDelete(DeleteBehavior.Cascade); // Or use Restrict, SetNull, etc., as needed


        builder.Entity<Booking>()
            .HasOne(b => b.Renter)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete when user is deleted

        builder.Entity<Item>()
            .HasOne(i => i.Owner)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete when user is deleted


        // Define relationships for Product and Category through CategoryProduct
        builder.Entity<CategoryItem>()
            .HasOne(cp => cp.Category)
            .WithMany()
            .HasForeignKey(cp => cp.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<CategoryItem>()
            .HasOne(cp => cp.Item)
            .WithMany()
            .HasForeignKey(cp => cp.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        // Define relationships for Review
        builder.Entity<Review>()
            .HasOne(r => r.Item)
            .WithMany() // If Product has a collection of Reviews, replace this with .WithMany(p => p.Reviews)
            .HasForeignKey(r => r.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Review>()
            .HasOne(r => r.User)
            .WithMany() // If ApplicationUser has a collection of Reviews, replace this with .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }


}

