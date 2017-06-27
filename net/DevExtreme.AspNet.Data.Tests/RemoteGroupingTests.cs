using DevExtreme.AspNet.Data.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class RemoteGroupingTests {

        [Fact]
        public void GroupingAndSummary() {
            /*

                G1=1
                    G2=1
                        A=1 B=1
                        A=2 B=3

                        count=2 sum(A)=3 avg(A)=1.5

                    G2=2
                        A=3 B=5

                        count=1 sum(A)=3 avg(A)=3

                    count=3 sum(A)=6 avg(A)=2

                G2=2
                    G1=1
                        A=4 B=7
                        A=5 B=9

                        count=2 sum(A)=9 avg(A)=4.5

                    G1=2
                        A=6 B=11

                        count=1 sum(A)=6 avg(A)=6

                    count=3 sum(A)=15, avg(A)=5

                count=6 sum(B)=36 avg(B)=6


            */


            var data = new[] {
                new { G1 = 1, G2 = 1, A = 1, B = 1 },
                new { G1 = 1, G2 = 1, A = 2, B = 3 },
                new { G1 = 1, G2 = 2, A = 3, B = 5 },
                new { G1 = 2, G2 = 1, A = 4, B = 7 },
                new { G1 = 2, G2 = 1, A = 5, B = 9 },
                new { G1 = 2, G2 = 2, A = 6, B = 11 },
            };

            var loadOptions = new SampleLoadOptions {
                RemoteGrouping = true,
                RequireTotalCount = true,
                Group = new[] {
                    new GroupingInfo { Selector = "G1", IsExpanded = false },
                    new GroupingInfo { Selector = "G2", IsExpanded = false }
                },
                GroupSummary = new[] {
                    new SummaryInfo { SummaryType = "count" },
                    new SummaryInfo { Selector = "A", SummaryType = "sum" },
                    new SummaryInfo { Selector = "A", SummaryType = "avg" }
                },
                TotalSummary = new[] {
                    new SummaryInfo { SummaryType = "count" },
                    new SummaryInfo { Selector = "B", SummaryType = "sum" },
                    new SummaryInfo { Selector = "B", SummaryType = "avg" },
                }
            };

            var result = DataSourceLoader.Load(data, loadOptions);

            Assert.Equal(1, loadOptions.ExpressionLog.Count);
            Assert.Contains("AnonType`2(I0 = obj.G1, I1 = obj.G2)", loadOptions.ExpressionLog[0]);

            Assert.Equal(new object[] { 6, 36M, 6M }, result.summary);
            Assert.Equal(6, result.totalCount);
            Assert.Equal(-1, result.groupCount);

            var groups = result.data.Cast<Group>().ToArray();

            var g1 = groups[0];
            var g2 = groups[1];
            var g11 = g1.items[0] as Group;
            var g12 = g1.items[1] as Group;
            var g21 = g2.items[0] as Group;
            var g22 = g2.items[1] as Group;

            Assert.Equal(1, g1.key);
            Assert.Equal(1, g11.key);
            Assert.Equal(2, g12.key);
            Assert.Equal(2, g2.key);
            Assert.Equal(1, g21.key);
            Assert.Equal(2, g22.key);

            Assert.Equal(new object[] { 3, 6M, 2M }, g1.summary);
            Assert.Equal(new object[] { 2, 3M, 1.5M }, g11.summary);
            Assert.Equal(new object[] { 1, 3M, 3M }, g12.summary);
            Assert.Equal(new object[] { 3, 15M, 5M }, g2.summary);
            Assert.Equal(new object[] { 2, 9M, 4.5M }, g21.summary);
            Assert.Equal(new object[] { 1, 6M, 6M }, g22.summary);

            Assert.Equal(2, g11.count);
            Assert.Equal(1, g12.count);
            Assert.Equal(2, g21.count);
            Assert.Equal(1, g22.count);

            Assert.Null(g11.items);
            Assert.Null(g12.items);
            Assert.Null(g21.items);
            Assert.Null(g22.items);
        }

        [Fact]
        public void TotalSummaryOnly() {
            var data = new[] {
                new { a = 1 },
                new { a = 2 },
                new { a = 3 }
            };

            var loadOptions = new SampleLoadOptions {
                RemoteGrouping = true,
                RequireTotalCount = true,
                Filter = new[] { "a", "<>", "2" },
                TotalSummary = new[] {
                    new SummaryInfo { Selector = "a", SummaryType = "sum" }
                },
                Skip = 1,
                Take = 1
            };

            var result = DataSourceLoader.Load(data, loadOptions);

            Assert.Equal(2, loadOptions.ExpressionLog.Count);

            // 1 - load paged data
            Assert.Contains("Skip", loadOptions.ExpressionLog[0]);
            Assert.Contains("Take", loadOptions.ExpressionLog[0]);

            // 2 - load summaries
            Assert.Contains("AnonType()", loadOptions.ExpressionLog[1]);

            Assert.Equal(4M, result.summary[0]);
            Assert.Equal(1, result.data.Cast<object>().Count());
            Assert.Equal(2, result.totalCount);
        }

        [Fact]
        public void GroupSummaryAndTotalCount() {
            var exprList = new List<string>();

            var result = DataSourceLoader.Load(
                new[] {
                    new { g = 1, v = 1 },
                    new { g = 1, v = 9 }
                },
                new SampleLoadOptions {
                    ExpressionWatcher = x => exprList.Add(x.ToString()),
                    RequireTotalCount = true,
                    RemoteGrouping = true,
                    Group = new[] {
                        new GroupingInfo { Selector = "g", IsExpanded = false }
                    },
                    GroupSummary = new[] {
                        new SummaryInfo { Selector = "v", SummaryType="sum" }
                    }
                }
            );

            Assert.Equal(10M, result.data.Cast<Group>().First().summary[0]);
            Assert.Null(result.summary);
            Assert.Equal(2, result.totalCount);
        }

        [Fact]
        public void NotUsedIfTotalCountOnly() {
            var data = new[] {
                new { a = 1 },
                new { a = 2 },
                new { a = 3 }
            };

            var loadOptions = new SampleLoadOptions {
                RemoteGrouping = true,
                RequireTotalCount = true,
                Filter = new[] { "a", "<>", "2" },
                Skip = 1,
                Take = 1
            };

            var result = DataSourceLoader.Load(data, loadOptions);

            Assert.False(loadOptions.ExpressionLog.Any(i => i.Contains("RemoteGroupKey")));

            Assert.Equal(2, result.totalCount);
            Assert.Null(result.summary);
            Assert.Equal(-1, result.groupCount);
        }

        [Fact]
        public void RequireGroupCount() {
            var data = new[] {
                new { G = 1 },
                new { G = 1 },
                new { G = 2 },
                new { G = 2 }
            };

            var loadOptions = new SampleLoadOptions {
                RemoteGrouping = true,
                RequireTotalCount = true,
                RequireGroupCount = true,
                Skip = 0,
                Take = 1,
                Group = new[] {
                    new GroupingInfo { Selector = "G", IsExpanded = false }
                }
            };

            var result = DataSourceLoader.Load(data, loadOptions);

            Assert.Equal(4, result.totalCount);

            var groups = result.data.Cast<Group>().ToArray();

            Assert.Equal(1, groups.Count());
            Assert.Equal(2, result.groupCount);
        }
    }

}
