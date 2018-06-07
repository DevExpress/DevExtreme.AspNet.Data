using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EF6 {
    using DataItem = Bug184_DataItem;

    class Bug184_DataItem {
        public int ID { get; set; }

        // overload exists
        public Int32 Int32 { get; set; }

        // convert required
        public Byte? NullableByte { get; set; }
    }

    public class Bug184 {

        [Fact]
        public void Scenario() {
            TestDbContext.Exec(context => {
                var dbSet = context.Set<DataItem>();

                dbSet.Add(new DataItem { Int32 = 1, NullableByte = 1 });
                dbSet.Add(new DataItem { Int32 = 2, NullableByte = 2 });

                context.SaveChanges();

                var loadResult = DataSourceLoader.Load(dbSet, new SampleLoadOptions {
                    TotalSummary = new[] {
                        new SummaryInfo { SummaryType = "sum", Selector = nameof(DataItem.Int32) },
                        new SummaryInfo { SummaryType = "sum", Selector = nameof(DataItem.NullableByte) }
                    }
                });

                Assert.Equal(
                    new object[] { 3m, 3m },
                    loadResult.summary
                );

            });
        }

    }
}
