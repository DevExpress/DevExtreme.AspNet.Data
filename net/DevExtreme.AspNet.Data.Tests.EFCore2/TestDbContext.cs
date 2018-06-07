using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace DevExtreme.AspNet.Data.Tests.EFCore2 {

    class TestDbContext : DbContext {
        static readonly object LOCK = new object();
        static TestDbContext INSTANCE;

        private TestDbContext(DbContextOptions options)
            : base(options) {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<RemoteGrouping.DataItem>();
        }

        public static void Exec(Action<TestDbContext> action) {
            lock(LOCK) {
                if(INSTANCE == null) {
                    var helper = new SqlServerTestDbHelper("EFCore2");
                    helper.ResetDatabase();

                    var options = new DbContextOptionsBuilder()
                        .UseSqlServer(helper.ConnectionString)
                        .Options;

                    INSTANCE = new TestDbContext(options);
                    INSTANCE.Database.EnsureCreated();
                }

                action(INSTANCE);
            }
        }

    }

}
