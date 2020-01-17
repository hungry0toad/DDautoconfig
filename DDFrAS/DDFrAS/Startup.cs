using Hangfire;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;
using Hangfire.Storage.SQLite;
using Microsoft.Owin;
using Owin;
using System;
using System.Diagnostics;
using System.Web;

[assembly: OwinStartup(typeof(DDFrAS.Startup))]

namespace DDFrAS
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //GlobalConfiguration.Configuration.UseSQLiteStorage(HttpContext.Current.Server.MapPath("~/App_Data/Hangfire.db"));
            JobStorage.Current = new MemoryStorage();

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            app.UseHangfireDashboard("/hangfire", new DashboardOptions()
            {
                Authorization = new[] { new MyAuthorizationFilter() }
            });
            //foreach (var config in ASselect.Getconfignotexecuted())
            //{
            //    double executetime = (config.ExDate - DateTime.Now).TotalSeconds;
            //    if (executetime > 0)
            //    {
            //        var jobid = BackgroundJob.Schedule(() => ASsshconnection.SetupConnection(config.Config_ID), TimeSpan.FromSeconds(executetime));
            //        ASInput.EditConfig(config.Config_ID, jobid);
            //    }
            //}
            //BackgroundJob.Enqueue(() => Debug.WriteLine(DateTime.Now + "Hangfire started"));
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






