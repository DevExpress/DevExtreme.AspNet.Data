using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace DevExtreme.AspNet.Data.Tests.EFCore1 {

    partial class TestDbContext : DbContext {
        static readonly object LOCK = new object();
        static TestDbContext INSTANCE;

        private TestDbContext(DbContextOptions options)
            : base(options) {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Bug120.DataItem>();
            modelBuilder.Entity<RemoteGrouping.DataItem>();
            modelBuilder.Entity<Summary.DataItem>();
        }

        public static void Exec(Action<TestDbContext> action) {
            lock(LOCK) {
                if(INSTANCE == null) {
                    var helper = new SqlServerTestDbHelper("EFCore1");
                    helper.ResetDatabase();

                    FixStringReplaceTranslator();

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

namespace DevExtreme.AspNet.Data.Tests.EFCore1 {
    using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;
    using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.Internal;
    using System.Linq.Expressions;
    using System.Reflection;

    partial class TestDbContext {

        class FakeStringReplaceTranslator : IMethodCallTranslator {
            public Expression Translate(MethodCallExpression methodCallExpression) {
                var method = methodCallExpression.Method;
                if(method.DeclaringType == typeof(String) && method.Name == nameof(String.Replace))
                    throw new NotImplementedException();
                return null;
            }
        }

        // https://github.com/aspnet/EntityFrameworkCore/issues/8021
        static void FixStringReplaceTranslator() {
            var translatorsField = typeof(SqlServerCompositeMethodCallTranslator).GetField("_methodCallTranslators", BindingFlags.Static | BindingFlags.NonPublic);
            var translators = (IMethodCallTranslator[])translatorsField.GetValue(translatorsField);
            translators[Array.FindIndex(translators, i => i is SqlServerStringReplaceTranslator)] = new FakeStringReplaceTranslator();
        }

    }
}
