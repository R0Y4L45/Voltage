using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Voltage.Entities.Models.ViewModels;

namespace Voltage.Areas.Admin.Controllers;

[Area("Admin")]
public class VoltageAdminController : Controller
{
    private readonly ILogger<VoltageAdminController> _logger;

    public VoltageAdminController(ILogger<VoltageAdminController> logger) => _logger = logger;

    public IActionResult Index() => View();

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}