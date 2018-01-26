using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EF6 {

    class Bug179_DataItem {
        [Key]
        public int ID { get; set; }
        public string Group { get; set; }
        public int? Value { get; set; }
    }

    partial class TestDbContext {
        public DbSet<Bug179_DataItem> Bug179_Data { get; set; }
    }

    public class Bug179 {

        [Fact]
        public void Scenario() {
            TestDbContext.Exec(context => {
                var dbSet = context.Bug179_Data;

                dbSet.AddRange(new[] {
                    new Bug179_DataItem { ID = 1, Group = "A", Value = 1 },
                    new Bug179_DataItem { ID = 2, Group = "A", Value = 3 },
                    new Bug179_DataItem { ID = 3, Group = "A", Value = null },
                    new Bug179_DataItem { ID = 4, Group = "B", Value = 5 },
                    new Bug179_DataItem { ID = 5, Group = "B", Value = null },
                });

                context.SaveChanges();

                var loadResult = DataSourceLoader.Load(dbSet, new SampleLoadOptions {
                    Group = new[] {
                        new GroupingInfo { Selector = "Group", IsExpanded = false }
                    },
                    GroupSummary = new[] {
                        new SummaryInfo { SummaryType = "avg", Selector = "Value" },
                        new SummaryInfo { SummaryType = "count" },
                        new SummaryInfo { SummaryType = "sum", Selector = "Value" }
                    },
                    TotalSummary = new[] {
                        new SummaryInfo { SummaryType = "sum", Selector = "Value" },
                        new SummaryInfo { SummaryType = "count" },
                        new SummaryInfo { SummaryType = "avg", Selector = "Value" }
                    }
                });

                var loadResultGroups = loadResult.data.Cast<ResponseModel.Group>().ToArray();

                var referenceGroups = dbSet
                    .GroupBy(i => i.Group)
                    .OrderBy(g => g.Key)
                    .Select(g => new {
                        Avg = g.Average(i => i.Value),
                        Count = g.Count(),
                        Sum = g.Sum(i => i.Value)
                    })
                    .ToArray();

                for(var g = 0; g < 2; g++) {
                    Assert.Equal((decimal)referenceGroups[g].Avg, loadResultGroups[g].summary[0]);
                    Assert.Equal(referenceGroups[g].Count, loadResultGroups[g].summary[1]);
                    Assert.Equal((decimal)referenceGroups[g].Sum, loadResultGroups[g].summary[2]);
                }

                Assert.Equal((decimal)dbSet.Sum(i => i.Value), loadResult.summary[0]);
                Assert.Equal(dbSet.Count(), loadResult.summary[1]);
                Assert.Equal((decimal)dbSet.Average(i => i.Value), loadResult.summary[2]);
            });
        }

    }


}
