﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sample.Models;

namespace Sample {

    public class Startup {

        public void ConfigureServices(IServiceCollection services) {
            services.AddMvc();

            services
                .AddLogging()
                .AddEntityFrameworkSqlServer()
                .AddDbContext<NorthwindContext>(options => options
                    .UseSqlServer("Server=.\\SQLEXPRESS; Database=Northwind; Trusted_Connection=True")
                    .ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning))
                );
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory) {
            loggerFactory.AddConsole(LogLevel.Information);
            app.UseMvc();
            app.UseStaticFiles();
        }
    }

}
