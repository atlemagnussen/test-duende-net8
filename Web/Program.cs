using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

const string authServerUrl = "https://localhost:6001";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddRazorPages();
builder.Services.AddControllers();
var services = builder.Services;

services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.MapInboundClaims = false; // or else sub (userId) will be translated to some long namespace
    o.Authority = authServerUrl;
    o.Audience = authServerUrl + "/resources";
    o.RequireHttpsMetadata = true;
    o.SaveToken = true;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "name",
        RoleClaimType = "role",
        ValidIssuers = [authServerUrl],
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = false
    };
});

services.AddAuthorization(options =>
{
    options.AddPolicy("admin", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("admin");
    });
});

//builder.Services.AddAuthentication()
    //.AddJwtBearer(x =>
    //{
        //x.Authority = authServer;
        // options.Audience = "https://localhost:6001/resources";
        // options.TokenValidationParameters.ValidIssuer = authServer;
        // options.TokenValidationParameters.ValidateIssuerSigningKey = false; 
        // options.TokenValidationParameters.ValidateAudience = true;
        // options.TokenValidationParameters.IgnoreTrailingSlashWhenValidatingAudience = true;
        // options.TokenValidationParameters = new 
        // {
        //     ValidateIssuer = true,
        //     ValidateIssuerSigningKey = false,
        //     ValidateAudience = true,
        //     ValidateLifetime = true,
        //     ClockSkew = TimeSpan.Zero,
        //     ValidAudience = authServer + "/resources",
        //     ValidIssuer = authServer,
        //     ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
        //         $"{authServer}/.well-known/openid-configuration",
        //         new OpenIdConnectConfigurationRetriever(),
        //         new HttpDocumentRetriever() { RequireHttps = false }
        //     )
        // };

    //});
// builder.Services.AddAuthorization(config => {
//     config.AddPolicy("auth", policyBuilder =>
//         policyBuilder.AddRequirements()
    
// });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

//app.MapRazorPages();

app.MapControllers();

app.Run();
