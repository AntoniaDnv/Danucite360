using System.ComponentModel.DataAnnotations;
using Danucite360.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Danucite360.Web.Areas.Identity.Pages.Account;

/// <summary>Bulgarian admin login; overrides the default Identity UI page with the same sign-in logic.</summary>
[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly ILogger<LoginModel> logger;

    public LoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger)
    {
        this.signInManager = signInManager;
        this.logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ReturnUrl { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Въведете имейл")]
        [EmailAddress(ErrorMessage = "Невалиден имейл")]
        [Display(Name = "Имейл")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Въведете парола")]
        [DataType(DataType.Password)]
        [Display(Name = "Парола")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Запомни ме")]
        public bool RememberMe { get; set; }
    }

    public void OnGet(string? returnUrl = null) => ReturnUrl = returnUrl;

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        // Only allow local redirects after login (no open redirect).
        returnUrl = returnUrl != null && Url.IsLocalUrl(returnUrl) ? returnUrl : Url.Content("~/");
        ReturnUrl = returnUrl;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            logger.LogInformation("Admin login succeeded.");
            return LocalRedirect(returnUrl);
        }

        if (result.IsLockedOut)
        {
            logger.LogWarning("Account locked out after failed logins.");
            ModelState.AddModelError(string.Empty, "Профилът е временно заключен след няколко неуспешни опита. Опитайте отново по-късно.");
            return Page();
        }

        ModelState.AddModelError(string.Empty, "Грешен имейл или парола.");
        return Page();
    }
}
