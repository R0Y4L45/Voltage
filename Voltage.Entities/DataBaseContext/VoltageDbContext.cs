 using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Voltage.Entities.Entity;

namespace Voltage.Entities.DataBaseContext;

public class VoltageDbContext : IdentityDbContext<User, IdentityRole, string>
{
    public VoltageDbContext() { }
    public VoltageDbContext(DbContextOptions<VoltageDbContext> optionsBuilder) : base(optionsBuilder) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder();
            string conn = builder.Configuration["ConnectionStrings:sqlConn2"];
            optionsBuilder.UseSqlServer(conn);
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>().Property(_ => _.DateOfBirth).IsRequired();
        base.OnModelCreating(builder);  
    }
}