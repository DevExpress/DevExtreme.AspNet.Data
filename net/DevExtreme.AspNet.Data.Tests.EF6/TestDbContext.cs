using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Tests.EF6 {

    class TestDbContext : DbContext {
        static TestDbContext INSTANCE;

        private TestDbContext(string connectionString)
            : base(connectionString) {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            // NOTE cannot use inner classes (e.g. Bug112.DataItem) because of
            // https://github.com/aspnet/EntityFramework6/issues/362

            modelBuilder.Entity<Bug112_DataItem>();
            modelBuilder.Entity<Bug179_DataItem>();
            modelBuilder.Entity<Bug184_DataItem>();
            modelBuilder.Entity<Bug235_DataItem>();
            modelBuilder.Entity<Bug239_DataItem>();
            modelBuilder.Entity<Bug240_DataItem>();

            modelBuilder.Entity<T640117_ParentItem>();
            modelBuilder.Entity<T640117_ChildItem>();

            modelBuilder.Entity<SelectNotMapped_DataItem>();
            modelBuilder.Entity<RemoteGroupingStress_DataItem>();
            modelBuilder.Entity<Summary_DataItem>();
            modelBuilder.Entity<PaginateViaPrimaryKey_DataItem>().HasKey(i => new { i.K1, i.K2 });
            modelBuilder.Entity<Async_DataItem>();
            modelBuilder.Entity<ExpandLinqSumType_DataItem>();
            modelBuilder.Entity<RemoteGroupCount_DataItem>();
        }

        public static async Task ExecAsync(Func<TestDbContext, Task> action) {
            if(INSTANCE == null) {
                var helper = new SqlServerTestDbHelper("EF6");
                helper.ResetDatabase();

                INSTANCE = new TestDbContext(helper.ConnectionString);
                INSTANCE.Database.CreateIfNotExists();
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
