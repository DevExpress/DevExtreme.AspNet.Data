using DevExtreme.AspNet.Data.ResponseModel;
using System;
using System.Linq;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class DynamicClassTests {

        [Fact]
        public void Totals() {
            var summaryInfo = new SummaryInfo {
                Selector = "p",
                SummaryType = "sum"
            };

            var source = new[] {
                new { p = 1 },
                new { p = 2 }
            };

            var loadOptions = new SampleLoadOptions {
                GuardNulls = false,
                RemoteGrouping = true,
                TotalSummary = Enumerable
                    .Repeat(summaryInfo, 123)
                    .ToArray()
            };

            var loadResult = DataSourceLoader.Load(source, loadOptions);

            Assert.Equal(3m, loadResult.summary[122]);

            Assert.Contains(loadOptions.ExpressionLog, line =>
                line.Contains(".Select(g => new <>f__AnonymousType") &&
                line.Contains("I100 = g.Sum(obj => obj.p)")
            );
        }

        [Fact]
        public void GroupSummary() {
            var summaryInfo = new SummaryInfo {
                Selector = "p",
                SummaryType = "sum"
            };

            var loadOptions = new SampleLoadOptions {
                GuardNulls = false,
                RemoteGrouping = true,
                Group = new[] {
                    new GroupingInfo { Selector = "g", IsExpanded = false },
                },
                GroupSummary = Enumerable
                    .Repeat(summaryInfo, 123)
                    .ToArray()
            };

            var source = new[] {
                new { g = 1, p = 1 },
                new { g = 1, p = 2 },
            };

            var loadResult = DataSourceLoader.Load(source, loadOptions);
            var group = loadResult.data.Cast<Group>().First();

            Assert.Equal(3m, group.summary[122]);

            Assert.Contains(loadOptions.ExpressionLog, line =>
                line.Contains(".Select(g => new <>f__AnonymousType") &&
                line.Contains("I100 = g.Sum(obj => obj.p)")
            );
        }

        [Fact]
        public void Select() {
            var select = Enumerable.Repeat("p", 123).ToArray();

            var source = new[] {
                new { p = 1 }
            };

            var loadOptions = new SampleLoadOptions {
                GuardNulls = false,
                RemoteSelect = true,
                Select = select
            };

            var loadResult = DataSourceLoader.Load(source, loadOptions);

            Assert.Contains(loadOptions.ExpressionLog, line =>
                line.Contains(".Select(obj => new <>f__AnonymousType") &&
                line.Contains("I100 = obj.p")
            );
        }

    }

}
