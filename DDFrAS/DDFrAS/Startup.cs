using System;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using Owin;
using Hangfire.SqlServer;
using System.Diagnostics;

[assembly: OwinStartup(typeof(DDFrAS.Startup))]

namespace DDFrAS
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            JobStorage.Current = new MemoryStorage();
            
            app.UseHangfireDashboard();
            app.UseHangfireServer();

            app.UseHangfireDashboard("/hangfire", new DashboardOptions()
            {
                Authorization = new[] { new MyAuthorizationFilter() }
            });

            BackgroundJob.Enqueue(() => Debug.WriteLine("sup"));
        }

        public class MyAuthorizationFilter : IDashboardAuthorizationFilter
        {
            public bool Authorize(DashboardContext context)
            {
                var httpContext = context.GetHttpContext();

                // Allow all authenticated users to see the Dashboard (potentially dangerous).
                return httpContext.User.Identity.IsAuthenticated;

            }
        }
    }
}

 
    
        

    
