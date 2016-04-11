using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class DataSourceLoaderTests {

        [Fact]
        public void TotalCount() {
            var data = new[] { 1, 3, 2 };
            var options = new SampleLoadOptions {
                Filter = new object[] { "this", "<>", 2 },
                Take = 1,
                IsCountQuery = true
            };

            Assert.Equal(2, DataSourceLoader.Load(data, options));
        }

        [Fact]
        public void Load_NoRequireTotalCount() {
            var data = new[] { 1, 3, 5, 2, 4 };
            var options = new SampleLoadOptions {
                Skip = 1,
                Take = 2,
                Filter = new object[] { "this", "<>", 2 },
                Sort = new[] {
                    new SortingInfo { Selector = "this" }
                },
                RequireTotalCount = false
            };

            Assert.Equal(new[] { 3, 4 }, DataSourceLoader.Load(data, options) as IEnumerable<int>);
        }

        [Fact]
        public void Load_RequireTotalCount() {
            var data = new[] { 1, 3, 5, 2, 4 };
            var options = new SampleLoadOptions {
                Skip = 1,
                Take = 2,
                Filter = new object[] { "this", "<>", 2 },
                Sort = new[] {
                    new SortingInfo { Selector = "this" }
                },
                RequireTotalCount = true
            };

            var result = (DataSourceLoadResult)DataSourceLoader.Load(data, options);

            Assert.Equal(4, result.totalCount);
            Assert.Equal(new[] { 3, 4 }, result.data.Cast<int>());
        }

        [Fact]
        public void Load_GroupOnly() {
            var data = new[] {
                new {
                    G1 = 1,
                    G2 = 2
                }
            };

            var result = DataSourceLoader.Load(data, new SampleLoadOptions {
                Group = new[] {
                    new GroupingInfo { Selector = "G1" },
                    new GroupingInfo { Selector = "G2" }
                }
            });

            var g1 = (result as IEnumerable<DevExtremeGroup>).First();
            var g2 = g1.items[0] as DevExtremeGroup;

            Assert.Equal(1, g1.key);
            Assert.Equal(2, g2.key);
            Assert.Same(data[0], g2.items[0]);
        }

        [Fact]
        public void Load_GroupWithOtherOptions() {
            var data = new[] {
                new { g = "g2", a = 1 },
                new { g = "g2", a = 2 }, // filtered
                new { g = "g2", a = 3 },
                new { g = "g1", a = 0 }  // skipped
            };

            var result = (DataSourceLoadResult)DataSourceLoader.Load(data, new SampleLoadOptions {
                Filter = new[] { "a", "<>", "2" },
                Sort = new[] { new SortingInfo { Selector = "a", Desc = true } },
                Group = new[] { new GroupingInfo { Selector = "g" } },
                Skip = 1,
                Take = 1,
                RequireTotalCount = true
            });

            Assert.Equal(3, result.totalCount);

            var groups = result.data.Cast<DevExtremeGroup>().ToArray();
            Assert.Equal(1, groups.Length);
            Assert.Equal(2, groups[0].items.Count);
            Assert.Same(data[2], groups[0].items[0]);
            Assert.Same(data[0], groups[0].items[1]);
        }

        [Fact]
        public void Load_TotalSummary() {
            var data = new[] { 1, 2, 3 };

            var result = (DataSourceLoadResult)DataSourceLoader.Load(data, new SampleLoadOptions {
                TotalSummary = new[] {
                    new SummaryInfo { Selector = "this", SummaryType = "min" },
                    new SummaryInfo { Selector = "this", SummaryType = "max" }
                }
            });            

            Assert.Equal(1, result.summary[0]);
            Assert.Equal(3, result.summary[1]);
        }

        [Fact]
        public void Load_GroupSummary() {
            var data = new[] {
                new { g = 1, value = 1 },
                new { g = 1, value = 2 },
                new { g = 2, value = 10 },
                new { g = 2, value = 20 }
            };

            var result = (IList<DevExtremeGroup>)DataSourceLoader.Load(data, new SampleLoadOptions {
                Group = new[] {
                    new GroupingInfo { Selector = "g" }
                },
                GroupSummary = new[] {
                    new SummaryInfo { Selector = "value", SummaryType = "sum" }
                }
            });            

            Assert.Equal(3M, result[0].summary[0]);
            Assert.Equal(30M, result[1].summary[0]);
        }

        [Fact]
        public void Load_TotalSummaryAndPaging() {
            var data = new[] { 1, 3, 5 };

            var result = (DataSourceLoadResult)DataSourceLoader.Load(data, new SampleLoadOptions {
                Skip = 1,
                Take = 1,
                TotalSummary = new[] {
                    new SummaryInfo { Selector = "this", SummaryType = "sum" }
                }
            });

            Assert.Equal(9M, result.summary[0]);
            Assert.Equal(1, result.data.Cast<object>().Count());
        }
    }

}
