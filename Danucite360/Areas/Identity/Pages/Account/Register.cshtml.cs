using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Danucite360.Web.Areas.Identity.Pages.Account;

/// <summary>Public self-registration is disabled: only seeded administrators sign in.</summary>
[AllowAnonymous]
public class RegisterModel : PageModel
{
    public IActionResult OnGet() => NotFound();

    public IActionResult OnPost() => NotFound();
}
