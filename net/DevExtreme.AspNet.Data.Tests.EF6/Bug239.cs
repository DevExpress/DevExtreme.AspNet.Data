using DevExtreme.AspNet.Data.ResponseModel;
using System;
using System.Linq;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EF6 {
    using DataItem = Bug239_DataItem;

    class Bug239_DataItem {
        public int ID { get; set; }
        public DateTime? OrderDate { get; set; }
        public Decimal? Freight { get; set; }
    }

    public class Bug239 {

        [Fact]
        public void Scenario() {
            TestDbContext.Exec(context => {
                var dbSet = context.Set<DataItem>();

                dbSet.Add(new DataItem());
                dbSet.Add(new DataItem { OrderDate = new DateTime(2009, 9, 9), Freight = 199 });
                context.SaveChanges();

                var loadResult = DataSourceLoader.Load(dbSet, new SampleLoadOptions {
                    Group = new[] {
                        new GroupingInfo { Selector = "OrderDate", IsExpanded = false, GroupInterval = "quarter" },
                        new GroupingInfo { Selector = "Freight", IsExpanded = false, GroupInterval = "50" }
                    }
                });

                var groups = loadResult.data.Cast<Group>().ToArray();

                Assert.Null(groups[0].key);
                Assert.Null((groups[0].items[0] as Group).key);

                Assert.Equal(3, groups[1].key);
                Assert.Equal(150m, (groups[1].items[0] as Group).key);
            });
        }

    }
}
