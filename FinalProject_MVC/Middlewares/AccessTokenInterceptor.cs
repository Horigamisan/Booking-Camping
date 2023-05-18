using System.Net.Http.Headers;

namespace FinalProject_MVC.Middlewares
{
    public class AccessTokenInterceptor : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccessTokenInterceptor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Lấy Access Token từ Cookie
            //string accessToken = _httpContextAccessor.HttpContext.Request.Cookies["access_token"];

            // Lấy Access Token từ Cookie claims
            string? accessToken = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "access_token")?.Value;

            // Nếu Access Token hợp lệ, thêm vào header Authorization
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            // Gọi tiếp theo
            return await base.SendAsync(request, cancellationToken);
        }
    }

}
