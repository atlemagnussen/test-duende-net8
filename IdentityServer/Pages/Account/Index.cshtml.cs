using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestDuende.IdentityServer.Models;

namespace TestDuende.IdentityServer.Pages.Account;

[SecurityHeaders]
[AllowAnonymous]
public class Index : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<Index> _logger;

    public AccountViewModel View { get; set; } = default!;
        
        
    public Index(UserManager<ApplicationUser> userManager, ILogger<Index> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        View = new AccountViewModel { IsLoggedIn = false };
        
        if (User.Identity is not null && User.Identity.IsAuthenticated)
        {
            View.IsLoggedIn = true;
            View.UserName = User.Identity.Name;
            View.Claims = new Dictionary<string, string>();

            var claims = User.Claims.ToList();
            _logger.LogInformation($"Claims count = {claims.Count}");

            if (claims.Count > 0) {
                foreach (var claim in claims)
                    View.Claims.Add(claim.Type, claim.Value);
            }
        }

        return Page();
    }

}
