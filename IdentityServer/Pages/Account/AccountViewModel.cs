namespace TestDuende.IdentityServer.Pages.Account;

public class AccountViewModel
{
    public bool IsLoggedIn { get; set; }
    public string? UserName { get; set; }
    public Dictionary<string, string>? Claims { get; set; }
}