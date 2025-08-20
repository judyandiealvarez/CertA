using CertA.Models;
using CertA.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CertA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICertificateService _certificateService;
        private readonly ICertificateAuthorityService _caService;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(
            ILogger<HomeController> logger,
            ICertificateService certificateService,
            ICertificateAuthorityService caService,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _certificateService = certificateService;
            _caService = caService;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = _userManager.GetUserId(User);
                var dashboardData = new DashboardViewModel
                {
                    TotalCertificates = 0,
                    ActiveCA = false,
                    RecentCertificates = new List<CertificateEntity>()
                };

                try
                {
                    // Get real certificate count
                    if (!string.IsNullOrEmpty(userId))
                    {
                        var certificates = await _certificateService.ListAsync(userId);
                        dashboardData.TotalCertificates = certificates.Count;
                        dashboardData.RecentCertificates = certificates.Take(5).ToList();
                    }

                    // Check if there's an active CA
                    var activeCA = await _caService.GetActiveCAAsync();
                    dashboardData.ActiveCA = activeCA != null;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error loading dashboard data");
                }

                return View(dashboardData);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}