using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Sample.Models;

namespace Sample {

    public class Startup {

        public void ConfigureServices(IServiceCollection services) {
            services.AddControllersWithViews();

            services
                .AddLogging()
                .AddEntityFrameworkSqlServer()
                .AddDbContext<NorthwindContext>(options => options
                    .UseSqlServer("Server=.\\SQLEXPRESS; Database=Northwind; Trusted_Connection=True")
                );
        }

        public void Configure(IApplicationBuilder app) {
            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints => {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }

}
