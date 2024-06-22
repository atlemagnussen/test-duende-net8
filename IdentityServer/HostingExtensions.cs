using Duende.IdentityServer.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TestDuende.IdentityServer.Data;
using TestDuende.IdentityServer.Models;
using TestDuende.IdentityServer.Services;

namespace TestDuende.IdentityServer;

internal static class HostingExtensions
{
    public static void ConfigureIdentityServer(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddTransient<ISigningCredentialStore, SignCredStore>();
        services.AddTransient<IValidationKeysStore, LocalValidationKeyStore>();

        // uncomment if you want to add a UI
        services.AddRazorPages();

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services
            .AddIdentityServer(options =>
            {
                options.KeyManagement.Enabled = false;
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/
                options.EmitStaticAudienceClaim = true;
            })
            // this adds the config data from DB (clients, resources, CORS)
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = b =>
                    b.UseSqlServer(connectionString, dbOpts => dbOpts.MigrationsAssembly(typeof(Program).Assembly.FullName));
            })
            // this adds the operational data from DB (codes, tokens, consents)
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = b =>
                    b.UseSqlServer(connectionString, dbOpts => dbOpts.MigrationsAssembly(typeof(Program).Assembly.FullName));
            })
            //.AddInMemoryIdentityResources(Config.IdentityResources)
            //.AddInMemoryApiScopes(Config.ApiScopes)
            //.AddInMemoryClients(Config.Clients)
            .AddCorsPolicyService<CustomCorsPolicy>()
            .AddAspNetIdentity<ApplicationUser>();

        services.AddAuthorization(options =>
                options.AddPolicy("admin",
                    policy => policy.RequireClaim("sub", "1"))
            );

        services.Configure<RazorPagesOptions>(options =>
            options.Conventions.AuthorizeFolder("/Admin", "admin"));

        //builder.Services.AddTransient<IdentityServer.Ef.Pages.Portal.ClientRepository>();
        services.AddTransient<ClientRepository>();
        services.AddTransient<IdentityScopeRepository>();
        services.AddTransient<ApiScopeRepository>();
        //builder.Services.AddAuthentication()
        //    .AddGoogle(options =>
        //    {
        //        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

        //        // register your IdentityServer with Google at https://console.developers.google.com
        //        // enable the Google+ API
        //        // set the redirect URI to https://localhost:5001/signin-google
        //        options.ClientId = "copy client ID from Google here";
        //        options.ClientSecret = "copy client secret from Google here";
        //    });

        //builder.Services.AddIdentityServer(options =>
        //    {
        //        // https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/api_scopes#authorization-based-on-scopes
        //        options.EmitStaticAudienceClaim = true;
        //    })
        //    .AddInMemoryIdentityResources(Config.IdentityResources)
        //    .AddInMemoryApiScopes(Config.ApiScopes)
        //    .AddInMemoryClients(Config.Clients)
        //    .AddCorsPolicyService<CustomCorsPolicy>();
    }
    
    public static WebApplication ConfigurePipeline(this WebApplication app)
    { 
        app.UseSerilogRequestLogging();
    
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // uncomment if you want to add a UI
        app.UseStaticFiles();
        app.UseRouting();
            
        app.UseIdentityServer();

        // uncomment if you want to add a UI
        app.UseAuthorization();
        app.MapRazorPages(); //.RequireAuthorization();

        return app;
    }
}
