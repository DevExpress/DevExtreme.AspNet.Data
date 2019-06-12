using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Tests.EFCore2 {

    class TestDbContext : DbContext {
        static TestDbContext INSTANCE;

        private TestDbContext(DbContextOptions options)
            : base(options) {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<RemoteGrouping.DataItem>();
            modelBuilder.Entity<RemoteGroupingStress.DataItem>();
            modelBuilder.Entity<Summary.DataItem>();
            modelBuilder.Entity<Bug326.Entity>();
            modelBuilder.Entity<PaginateViaPrimaryKey.DataItem>().HasKey("K1", "K2");
        }

        public static async Task ExecAsync(Func<TestDbContext, Task> action) {
            if(INSTANCE == null) {
                var helper = new SqlServerTestDbHelper("EFCore2");
                helper.ResetDatabase();

                var options = new DbContextOptionsBuilder()
                    .UseSqlServer(helper.ConnectionString)
                    .ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning))
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
