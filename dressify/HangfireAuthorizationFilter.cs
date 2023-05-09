using Hangfire.Dashboard;
using System.Security.Claims;

namespace dressify
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            //// Check if the user is authenticated
            //var user = context.GetHttpContext().User;
            //if (!user.Identity.IsAuthenticated)
            //{
            //    return false;
            //}

            //// Check if the user is authorized
            //var roles = user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
            //if (!roles.Any(role => role == "HangfireAdmin"))
            //{
            //    return false;
            //}

            //return true;
            return true;
        }
    }
}
