using Microsoft.EntityFrameworkCore;
using Sample.Models;

namespace Sample {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services
                .AddControllersWithViews()
                //.AddRazorRuntimeCompilation()
#if NET6_0
                .AddJsonOptions(options => {
                    options.JsonSerializerOptions.Converters.Add(new Net6DateOnlyJsonConverter());
                    options.JsonSerializerOptions.Converters.Add(new Net6TimeOnlyJsonConverter());
                })
#endif
                ;
            builder.Services
                .AddLogging()
                .AddEntityFrameworkSqlServer()
                .AddDbContext<NorthwindContext>(options => options
                    //.UseSqlServer("Server=.\\SQLEXPRESS; Database=Northwind; Trusted_Connection=True")
                    .UseSqlServer(builder.Configuration.GetConnectionString("NorthwindConnectionString"))
                );

            var app = builder.Build();
            app.UseStaticFiles();
            app.UseRouting();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.Run();
        }
    }
}
