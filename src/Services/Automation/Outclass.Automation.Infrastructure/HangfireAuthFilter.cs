using Hangfire.Dashboard;

namespace Outclass.Automation.Infrastructure;

public class HangfireAuthFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context) => true; // In production, validate JWT
}
