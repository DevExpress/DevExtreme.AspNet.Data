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

            var result = DataSourceLoader.Load(data, options) as IDictionary;
            Assert.NotNull(result);

            Assert.Equal(4, result["totalCount"]);
            Assert.Equal(new[] { 3, 4 }, result["data"] as IEnumerable<int>);
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

            var result = (IDictionary)DataSourceLoader.Load(data, new SampleLoadOptions {
                Filter = new[] { "a", "<>", "2" },
                Sort = new[] { new SortingInfo { Selector = "a", Desc = true } },
                Group = new[] { new GroupingInfo { Selector = "g" } },
                Skip = 1,
                Take = 1,
                RequireTotalCount = true
            });

            Assert.Equal(3, result["totalCount"]);

            var groups = (result["data"] as IEnumerable<DevExtremeGroup>).ToArray();
            Assert.Equal(1, groups.Length);
            Assert.Equal(2, groups[0].items.Count);
            Assert.Same(data[2], groups[0].items[0]);
            Assert.Same(data[0], groups[0].items[1]);
        }
    }

}
