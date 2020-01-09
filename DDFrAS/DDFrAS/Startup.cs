using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;

namespace DDFrAS
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHangfire(x => x.UseSqlServerStorage("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\frede\\DDautoconfig\\DDFrAS\\DDFrAS\\App_Data\\DDFrAS.mdf;Integrated Security=True;MultipleActiveResultSets=True;Connect Timeout=30;Application Name=EntityFramework"));
            services.AddHangfireServer();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseHangfireDashboard();
        }
    

    }
}