using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Tests.EFCore {

    class TestDbContext : DbContext {
        static TestDbContext INSTANCE;

        private TestDbContext(DbContextOptions options)
            : base(options) {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<RemoteGrouping.DataItem>();
#if !EFCORE1
            modelBuilder.Entity<RemoteGroupingStress.DataItem>();
#endif
            modelBuilder.Entity<Summary.DataItem>();
            modelBuilder.Entity<Bug120.DataItem>();
            modelBuilder.Entity<Bug326.Entity>();
            modelBuilder.Entity<PaginateViaPrimaryKey.DataItem>().HasKey("K1", "K2");
        }

        public static async Task ExecAsync(Func<TestDbContext, Task> action) {
            if(INSTANCE == null) {
                var efVersion = typeof(DbContext).Assembly.GetName().Version.Major;
                var helper = new SqlServerTestDbHelper("EFCore" + efVersion);
                helper.ResetDatabase();

                var options = new DbContextOptionsBuilder()
                    .UseSqlServer(helper.ConnectionString)
#if !EFCORE1
                    .ConfigureWarnings(warnings => warnings.Throw(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.QueryClientEvaluationWarning))
#endif
                    .Options;

                INSTANCE = new TestDbContext(options);
                INSTANCE.Database.EnsureCreated();
            }

            await action(INSTANCE);
        }

        public static async Task ExecAsync(Action<TestDbContext> action) {
            await ExecAsync(context => {
                action(context);
                return Task.CompletedTask;
            });
        }

    }

}
