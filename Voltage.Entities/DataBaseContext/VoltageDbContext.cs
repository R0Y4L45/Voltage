using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Voltage.Entities.Entity;

namespace Voltage.Entities.DataBaseContext;

public class VoltageDbContext : IdentityDbContext<User, IdentityRole, string>
{
    public DbSet<Message>? Message { get; set; }

    public VoltageDbContext() { }
    public VoltageDbContext(DbContextOptions<VoltageDbContext> optionsBuilder) : base(optionsBuilder) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder();
            string conn = builder.Configuration["ConnectionStrings:sqlConn"];
            optionsBuilder.UseSqlServer(conn);
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>().Property(_ => _.DateOfBirth).IsRequired();

        builder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany(u => u.SentMessages)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict); // You can specify the delete behavior as needed

        builder.Entity<Message>()
            .HasOne(m => m.Receiver)
            .WithMany(u => u.ReceivedMessages)
            .HasForeignKey(m => m.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(builder);
    }
}