using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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
            modelBuilder.Entity<RemoteGroupingStress.DataItem>();
            modelBuilder.Entity<Summary.DataItem>();
        }

        public static void Exec(Action<TestDbContext> action) {
            lock(LOCK) {
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

                action(INSTANCE);
            }
        }

    }

}
