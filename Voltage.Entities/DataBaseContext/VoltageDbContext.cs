using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Voltage.Entities.Entity;

namespace Voltage.Entities.DataBaseContext;

public class VoltageDbContext : IdentityDbContext<User, IdentityRole, string>
{
    public DbSet<Message>? Message { get; set; }
    public DbSet<FriendList>? FriendList { get; set; }
    public DbSet<Notification>? Notification { get; set; }
    public DbSet<UsersFriendListResult>? UsersFriendListResults { get; set; }

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
        builder.Entity<UsersFriendListResult>().HasNoKey();
        
        builder.Entity<User>().Property(_ => _.DateOfBirth).IsRequired();

        builder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany(u => u.SentMessages)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.Entity<Message>()
            .HasOne(m => m.Receiver)
            .WithMany(u => u.ReceivedMessages)
            .HasForeignKey(m => m.ReceiverId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<FriendList>()
            .HasOne(f => f.Sender)
            .WithMany(u => u.SenderRequest)
            .HasForeignKey(f => f.SenderId)
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.Entity<FriendList>()
            .HasOne(f => f.Receiver)
            .WithMany(u => u.ReceiverRequest)
            .HasForeignKey(f => f.ReceiverId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(builder);
    }
}