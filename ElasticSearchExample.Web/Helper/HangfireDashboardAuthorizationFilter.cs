using ElasticSearchExample.Data.DAL;
using Hangfire.Annotations;
using Hangfire.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ElasticSearchExample.Web.Helper
{
    public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        { 
            var httpContent = context.GetHttpContext();
            var userRole = httpContent.User.FindFirst(ClaimTypes.Role)?.Value;
            return userRole != null ? true : false;
            throw new NotImplementedException();
        }
    }
}
