using System;
using System.Data.Entity;
using System.Linq;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EF6 {

    class Bug240_DataItem {
        public int ID { get; set; }
        public DateTime? Date { get; set; }
        public string Text { get; set; }
    }

    partial class TestDbContext {
        public DbSet<Bug240_DataItem> Bug240_Data { get; set; }
    }

    public class Bug240 {

        [Fact]
        public void Scenario() {
            TestDbContext.Exec(context => {
                var dbSet = context.Bug240_Data;

                dbSet.Add(new Bug240_DataItem());
                context.SaveChanges();

                var loadResult = DataSourceLoader.Load(dbSet, new SampleLoadOptions {
                    Select = new[] { "Date.Year", "Text.Length" }
                });

                var items = loadResult.data.Cast<dynamic>().ToArray();

                Assert.Null(items[0].Date.Year);
                Assert.Null(items[0].Text.Length);
            });
        }

    }

}
