using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EF6 {
    using DataItem = Bug179_DataItem;

    class Bug179_DataItem {
        public int ID { get; set; }
        public string Group { get; set; }
        public int? Value { get; set; }
    }

    public class Bug179 {

        [Fact]
        public void Scenario() {
            TestDbContext.Exec(context => {
                var dbSet = context.Set<DataItem>();

                dbSet.AddRange(new[] {
                    new DataItem { Group = "A", Value = 1 },
                    new DataItem { Group = "A", Value = 3 },
                    new DataItem { Group = "A", Value = null },
                    new DataItem { Group = "B", Value = 5 },
                    new DataItem { Group = "B", Value = null },
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
