using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace MenulioPocMvc.Session
{
    public static class MembershipHelper
    {
        // Adapt to Contentful + NET8
        //public static UserData GetUserData(IUser user)
        //{
        //    return new UserData(Guid.NewGuid().ToString("N"))
        //    {
        //        Id = user.Id,
        //        AllowedApplications = user.AllowedSections.ToArray(),
        //        RealName = user.Name,
        //        Roles = new[] { user.UserType.Alias },
        //        StartContentNodes = new[] { user.StartContentId },
        //        StartMediaNodes = new[] { user.StartMediaId },
        //        Username = user.Username,
        //        Culture = user.Language
        //    };
        //}

        public static bool IsUserAuthenticated(HttpContext httpContext)
        {
            return httpContext.User.Identity?.IsAuthenticated ?? false;
        }

        public static Guid GetCurrentUserId(HttpContext httpContext)
        {
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : Guid.Empty;
        }

        public static async Task UserSignOutAsync(HttpContext httpContext)
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            RemoveLoginSessionCookie(httpContext);
        }

        public static async Task UserSignInAsync(HttpContext httpContext, string customerId, bool rememberMe = false)
        {
            var authTimeout = // TODO: Adapt to Contentful + NET8
                //ContentHelper.GetSiteRoot().GetPropertyValue<int>("authorisationTicketTimeout") ?? 
                30;

            var expires = GetSessionExpiry(rememberMe, authTimeout);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, customerId),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = expires
            };

            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            CreateLoginSessionCookie(httpContext, customerId);
        }

        public static bool LoginSessionExists(HttpContext httpContext)
        {
            return httpContext.Request.Cookies["LoginSession"] != null;
        }

        private static void CreateLoginSessionCookie(HttpContext httpContext, string customerId)
        {
            var sessionTimeout =
                // TODO: Adapt to Contentful + NET8
                //ContentHelper.GetSiteRoot().GetPropertyValue<int>("sessionTimeout") ??
                30;
            sessionTimeout = sessionTimeout < 1 ? 30 : sessionTimeout;

            httpContext.Response.Cookies.Append("LoginSession", customerId, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddMinutes(sessionTimeout),
                Secure = RuntimeConstants.RequireSecureCookies,
                HttpOnly = true
            });
        }

        private static void RemoveLoginSessionCookie(HttpContext httpContext)
        {
            if (LoginSessionExists(httpContext))
            {
                httpContext.Response.Cookies.Delete("LoginSession");
            }
        }

        public static void ResetLoginSessionExpiry(HttpContext httpContext)
        {
            const string cookieName = "LoginSession";

            if (httpContext.Request.Cookies.TryGetValue(cookieName, out var cookieValue))
            {
                var sessionTimeout =
                // TODO: Adapt to Contentful + NET8
                //ContentHelper.GetSiteRoot().GetPropertyValue<int>("sessionTimeout") ??
                30;
                sessionTimeout = sessionTimeout < 1 ? 30 : sessionTimeout;

                httpContext.Response.Cookies.Append(cookieName, cookieValue, new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddMinutes(sessionTimeout),
                    Secure = RuntimeConstants.RequireSecureCookies,
                    HttpOnly = true
                });
            }
        }

        private static DateTimeOffset GetSessionExpiry(bool rememberMe, int authTimeout)
        {
            var sessionTimeout =
                // TODO: Adapt to Contentful + NET8
                //ContentHelper.GetSiteRoot().GetPropertyValue<int>("sessionTimeout") ??
                30;

            sessionTimeout = sessionTimeout < 1 ? 30 : sessionTimeout;
            authTimeout = authTimeout < 1 ? 30 : authTimeout;

            return rememberMe ? DateTimeOffset.Now.AddDays(authTimeout) : DateTimeOffset.Now.AddMinutes(sessionTimeout);
        }
    }
}