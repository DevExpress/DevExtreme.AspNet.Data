using Microsoft.EntityFrameworkCore;
using Sample.Models;

namespace Sample {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();
            builder.Services
                .AddLogging()
                .AddEntityFrameworkSqlServer()
                .AddDbContext<NorthwindContext>(options => options
                    //.UseSqlServer("Server=.\\SQLEXPRESS; Database=Northwind; Trusted_Connection=True")
                    .UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB; Database=Northwind; Integrated Security=True; MultipleActiveResultSets=True; App=EntityFramework")
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
