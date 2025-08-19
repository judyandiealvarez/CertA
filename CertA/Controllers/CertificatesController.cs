using CertA.Data;
using CertA.Models;
using CertA.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace CertA.Controllers
{
    [Authorize]
    public class CertificatesController : Controller
    {
        private readonly ICertificateService _service;
        private readonly ICertificateAuthorityService _caService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CertificatesController(
            ICertificateService service,
            ICertificateAuthorityService caService,
            UserManager<ApplicationUser> userManager)
        {
            _service = service;
            _caService = caService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                var certificates = await _service.ListAsync(userId);
                return View(certificates);
            }
            catch (Exception ex)
            {
                return View(new List<CertificateEntity>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                var cert = await _service.GetAsync(id, userId);
                if (cert == null) return NotFound();
                return View(cert);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        public IActionResult Create()
        {
            return View(new CreateCertificateVm());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCertificateVm vm)
        {
            if (!ModelState.IsValid) return View(vm);

            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                var created = await _service.CreateAsync(vm.CommonName, vm.SubjectAlternativeNames, vm.Type, userId);
                return RedirectToAction(nameof(Details), new { id = created.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Failed to create certificate. Please try again.");
                return View(vm);
            }
        }

        public async Task<IActionResult> DownloadCertificate(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                var cert = await _service.GetAsync(id, userId);
                if (cert == null) return NotFound();

                var bytes = await _service.GetCertificatePemAsync(id, userId);
                var fileName = $"{cert.CommonName.Replace(" ", "_")}_certificate.pem";
                return File(bytes, "application/x-pem-file", fileName);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> DownloadPrivateKey(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                var cert = await _service.GetAsync(id, userId);
                if (cert == null) return NotFound();

                var bytes = await _service.GetPrivateKeyPemAsync(id, userId);
                var fileName = $"{cert.CommonName.Replace(" ", "_")}_private_key.pem";
                return File(bytes, "application/x-pem-file", fileName);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> DownloadPublicKey(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                var cert = await _service.GetAsync(id, userId);
                if (cert == null) return NotFound();

                var bytes = await _service.GetPublicKeyPemAsync(id, userId);
                var fileName = $"{cert.CommonName.Replace(" ", "_")}_public_key.pem";
                return File(bytes, "application/x-pem-file", fileName);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> DownloadPfx(int id, string password = "password")
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                var cert = await _service.GetAsync(id, userId);
                if (cert == null) return NotFound();

                var bytes = await _service.GetPfxAsync(id, password, userId);
                var fileName = $"{cert.CommonName.Replace(" ", "_")}.pfx";
                return File(bytes, "application/x-pkcs12", fileName);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> Authority()
        {
            try
            {
                var ca = await _caService.GetActiveCAAsync();
                return View(ca);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> DownloadRootCA()
        {
            try
            {
                var ca = await _caService.GetActiveCAAsync();
                if (ca == null) return NotFound();

                var bytes = System.Text.Encoding.UTF8.GetBytes(ca.CertificatePem);
                var fileName = $"{ca.CommonName.Replace(" ", "_")}_Root_CA.pem";
                return File(bytes, "application/x-pem-file", fileName);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> DownloadRootCAPfx(string password = "password")
        {
            try
            {
                var ca = await _caService.GetActiveCAAsync();
                if (ca == null) return NotFound();

                // Create X509Certificate2 from CA PEM
                var certificate = X509Certificate2.CreateFromPem(ca.CertificatePem, ca.PrivateKeyPem);

                // Export as PKCS#12
                var pfxBytes = certificate.Export(X509ContentType.Pfx, password);

                var fileName = $"{ca.CommonName.Replace(" ", "_")}_Root_CA.pfx";
                return File(pfxBytes, "application/x-pkcs12", fileName);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }
    }

    public class CreateCertificateVm
    {
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(255)]
        public string CommonName { get; set; } = string.Empty;

        public string? SubjectAlternativeNames { get; set; }

        public CertificateType Type { get; set; } = CertificateType.Server;
    }
}

