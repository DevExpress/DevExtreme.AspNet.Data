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

            Assert.Single(loadOptions.ExpressionLog);
            Assert.Matches(@"AnonType`2\(I0 = .+?, I1 = .+?\)", loadOptions.ExpressionLog[0]);

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
            Assert.Single(result.data.Cast<object>());
            Assert.Equal(2, result.totalCount);
        }

        [Fact]
        public void GroupSummaryAndTotalCount() {
            var result = DataSourceLoader.Load(
                new[] {
                    new { g = 1, v = 1 },
                    new { g = 1, v = 9 }
                },
                new SampleLoadOptions {
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

            Assert.DoesNotContain(loadOptions.ExpressionLog, expr => expr.Contains("RemoteGroupKey"));

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

            Assert.Single(groups);
            Assert.Equal(2, result.groupCount);
        }

        [Fact]
        public void Summary_MissingOverload() {
            // Neither of Min, Max, Sum, Average provides an overload for byte sequences
            var data = new byte[] { 1, 3, 5 };

            var loadOptions = new SampleLoadOptions {
                RemoteGrouping = true,
                TotalSummary = new[] { "sum", "min", "max", "avg" }
                    .Select(i => new SummaryInfo { Selector = "this", SummaryType = i })
                    .ToArray()
            };

            var summary = DataSourceLoader.Load(data, loadOptions).summary;

            Assert.Equal(9M, summary[0]);
            Assert.Equal((byte)1, summary[1]);
            Assert.Equal((byte)5, summary[2]);
            Assert.Equal(3M, summary[3]);
        }

        [Fact]
        public void Summary_MissingOverload_SumBrute() {
            var values = new[] {
                new {
                    p1 = byte.MaxValue,
                    p2 = sbyte.MaxValue,
                    p3 = short.MaxValue,
                    p4 = ushort.MaxValue,
                    p5 = uint.MaxValue,
                    p6 = ulong.MaxValue
                }
            };

            var nullableValues = new[] {
                new {
                    p1 = new byte?(byte.MaxValue),
                    p2 = new sbyte?(sbyte.MaxValue),
                    p3 = new short?(short.MaxValue),
                    p4 = new ushort?(ushort.MaxValue),
                    p5 = new uint?(uint.MaxValue),
                    p6 = new ulong?(ulong.MaxValue)
                }
            };

            var nulls = new[] {
                new {
                    p1 = new byte?(),
                    p2 = new sbyte?(),
                    p3 = new short?(),
                    p4 = new ushort?(),
                    p5 = new uint?(),
                    p6 = new ulong?()
                }
            };

            var valuesExpectation = new object[] {
                (decimal)byte.MaxValue,
                (decimal)sbyte.MaxValue,
                (decimal)short.MaxValue,
                (decimal)ushort.MaxValue,
                (decimal)uint.MaxValue,
                (decimal)ulong.MaxValue,
            };

            var nullsExpectation = new object[] { 0m, 0m, 0m, 0m, 0m, 0m };

            var loadOptions = new SampleLoadOptions {
                RemoteGrouping = true,
                TotalSummary = Enumerable.Range(1, 6)
                    .Select(i => new SummaryInfo { Selector = "p" + i, SummaryType = "sum" })
                    .ToArray()
            };

            Assert.Equal(valuesExpectation, DataSourceLoader.Load(values, loadOptions).summary);
            Assert.Equal(valuesExpectation, DataSourceLoader.Load(nullableValues, loadOptions).summary);
            Assert.Equal(nullsExpectation, DataSourceLoader.Load(nulls, loadOptions).summary);
        }

        [Fact]
        public void Summary_Empty() {
            // https://stackoverflow.com/a/1122839
            // See also AggregateCalculatorTests.Calculation_Empty

            var loadResult = DataSourceLoader.Load(new object[0], new SampleLoadOptions {
                RemoteGrouping = true,
                TotalSummary = new[] {
                    new SummaryInfo { Selector = "any", SummaryType = "sum" },
                    new SummaryInfo { Selector = "any", SummaryType = "min" },
                    new SummaryInfo { Selector = "any", SummaryType = "max" },
                    new SummaryInfo { Selector = "any", SummaryType = "avg" },
                    new SummaryInfo { Selector = "any", SummaryType = "count" }
                }
            });

            Assert.Equal(
                new object[] { 0m /* SumFix */, null, null, null, 0 },
                loadResult.summary
            );
        }

        [Fact]
        public void Summary_Average_EmptyWithZeroSum() {
            // Remote SUM is not necessary NULL for empty sets
            // https://github.com/aspnet/EntityFrameworkCore/issues/12307

            var result = RemoteGrouping.RemoteGroupTransformer.Run(
                typeof(Object),
                new[] { new Types.AnonType<int, int, int, int>(
                    0, // count
                    0, // sum
                    0  // count not null
                ) },
                0,
                new[] { new SummaryInfo { SummaryType = "avg" } },
                null
            );

            Assert.Null(result.Totals[0]);
        }

    }

}
