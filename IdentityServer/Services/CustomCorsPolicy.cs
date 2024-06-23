using Duende.IdentityServer.Services;

namespace TestDuende.IdentityServer.Services
{
    public class CustomCorsPolicy : ICorsPolicyService
    {
        private readonly ILogger<CustomCorsPolicy> _logger;
        public CustomCorsPolicy(ILogger<CustomCorsPolicy> logger)
        {
            _logger = logger;
        }
        public Task<bool> IsOriginAllowedAsync(string origin)
        {
            var uri = new Uri(origin);
            _logger.LogInformation($"uri.Host={uri.Host}");

            if (uri.Host.EndsWith("localhost", StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(true);

            if (uri.Host.EndsWith("jwt.io",  StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(true);
            return Task.FromResult(false);
        }
    }
}
