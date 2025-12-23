using System.Net.Http.Headers;

namespace Cortex.Services
{
    public class JwtDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public JwtDelegatingHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }


        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var ctx = _httpContextAccessor.HttpContext;
            if (ctx != null && ctx.User?.Identity?.IsAuthenticated == true)
            {
                // token stored as claim "access_token"
                var token = ctx.User.FindFirst("access_token")?.Value;
                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }


            return base.SendAsync(request, cancellationToken);
        }
    }
}
