using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sample.Models;

namespace Sample {

    public class Startup {

        public void ConfigureServices(IServiceCollection services) {
            services.AddMvc();

            services
                .AddLogging()
                .AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<NorthwindContext>();

        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory) {
            DevExtreme.AspNet.Data.Helpers.Compat.EF3361 = true;
            loggerFactory.AddConsole(LogLevel.Information);
            app.UseMvc();
            app.UseStaticFiles();
        }
    }

}
