using Microsoft.EntityFrameworkCore;
using Voltage.Entities.Entity;

namespace Voltage.Entities.Models.ViewModels;

public static class StaticSaver
{
    public static DbContextOptionsBuilder? builder;
}
