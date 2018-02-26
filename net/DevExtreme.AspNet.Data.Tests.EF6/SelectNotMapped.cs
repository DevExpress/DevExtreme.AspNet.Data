using System;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EF6 {

    class SelectNotMapped_DataItem {
        public int ID { get; set; }

        [NotMapped]
        public string NotMapped {
            get { return "NotMapped"; }
        }
    }

    partial class TestDbContext {
        public DbSet<SelectNotMapped_DataItem> SelectNotMapped_Data { get; set; }
    }

    public class SelectNotMapped {

        [Fact]
        public void Scenario() {
            TestDbContext.Exec(context => {
                var dbSet = context.SelectNotMapped_Data;

                dbSet.Add(new SelectNotMapped_DataItem { ID = 1 });
                context.SaveChanges();

                var loadResult = DataSourceLoader.Load(dbSet, new SampleLoadOptions {
                    Select = new[] { "NotMapped" },
                    RemoteSelect = false
                });

                var item = loadResult.data.Cast<IDictionary>().First();

                Assert.Single(item.Keys);
                Assert.Equal("NotMapped", item["NotMapped"]);
            });
        }

    }
}
