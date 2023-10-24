//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore;
//
//namespace Voltage.Entities.DataBaseContext;
//
//public class VoltageDbContext : IdentityDbContext
//{
//    public VoltageDbContext()
//    {
//    }
//
//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//    {
//        if(!optionsBuilder.IsConfigured)
//        {
//            WebApplicationBuilder builder = WebApplication.CreateBuilder();
//            optionsBuilder.UseSqlServer(builder.Configuration["ConnectionStrings:default"]);
//        }
//
//        base.OnConfiguring(optionsBuilder);
//    }
//}
