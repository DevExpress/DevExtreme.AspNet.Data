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
        public void RequireGroupCount_MultiLevels() {
            var source = new[] {
                new { G1 = 1, G2 = 1 },
                new { G1 = 1, G2 = 2 },
                new { G1 = 2, G2 = 3 }
            };

            var loadOptions = new SampleLoadOptions {
                RemoteGrouping = true,
                RequireGroupCount = true,
                Group = new[] {
                    new GroupingInfo { Selector = "G1", IsExpanded = false },
                    new GroupingInfo { Selector = "G2", IsExpanded = false }
                },
                Take = 1
            };

            var loadResult = DataSourceLoader.Load(source, loadOptions);
            Assert.Equal(2, loadResult.groupCount);
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

        [Fact]
        public void Issue324() {
            var loadResult = DataSourceLoader.Load(new float?[] { 1, 3 }, new SampleLoadOptions {
                RemoteGrouping = true,
                TotalSummary = new[] {
                    new SummaryInfo { Selector = "this", SummaryType = "avg" }
                }
            });

            Assert.Equal(2d, loadResult.summary[0]);
        }

        [Fact]
        public void AggregateTranslationError() {
            // https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/331

            var data = new[] {
                new { P1 = "abc" }
            };

            var exception = Record.Exception(delegate {
                DataSourceLoader.Load(data, new SampleLoadOptions {
                    RemoteGrouping = true,
                    TotalSummary = new[] {
                        new SummaryInfo { Selector = "P1", SummaryType = "avg" }
                    }
                });
            });

            Assert.Equal("Failed to translate the 'sum' aggregate for the 'P1' member (System.String). See InnerException for details.", exception.Message);
            Assert.Contains("No coercion operator", exception.InnerException.Message);
        }

        [Fact]
        public void ExpandLinqSumType() {
            var sourceItem = new {
                SByte = SByte.MaxValue,
                Byte = Byte.MaxValue,

                Int16 = Int16.MaxValue,
                UInt16 = UInt16.MaxValue,

                Int32 = Int32.MaxValue,
                UInt32 = UInt32.MaxValue,

                Int64 = Int64.MaxValue / 2,
                UInt64 = UInt64.MaxValue,

                Single = Single.MaxValue,
                Double = Double.MaxValue / 2,
                Decimal = (Decimal.MaxValue - 1) / 2,

                SByteN = (SByte?)SByte.MaxValue,
                ByteN = (Byte?)Byte.MaxValue,

                Int16N = (Int16?)Int16.MaxValue,
                UInt16N = (UInt16?)UInt16.MaxValue,

                Int32N = (Int32?)Int32.MaxValue,
                UInt32N = (UInt32?)UInt32.MaxValue,

                Int64N = (Int64?)Int64.MaxValue / 2,
                UInt64N = (UInt64?)UInt64.MaxValue,

                SingleN = (Single?)Single.MaxValue,
                DoubleN = (Double?)Double.MaxValue / 2,
                DecimalN = ((Decimal?)Decimal.MaxValue - 1) / 2
            };

            var loadOptions = new SampleLoadOptions {
                GuardNulls = false,
                TotalSummary = new[] {
                    nameof(sourceItem.SByte),
                    nameof(sourceItem.Byte),
                    nameof(sourceItem.Int16),
                    nameof(sourceItem.UInt16),
                    nameof(sourceItem.Int32),
                    nameof(sourceItem.UInt32),
                    nameof(sourceItem.Int64),
                    nameof(sourceItem.UInt64),
                    nameof(sourceItem.Single),
                    nameof(sourceItem.Double),
                    nameof(sourceItem.Decimal),

                    nameof(sourceItem.SByteN),
                    nameof(sourceItem.ByteN),
                    nameof(sourceItem.Int16N),
                    nameof(sourceItem.UInt16N),
                    nameof(sourceItem.Int32N),
                    nameof(sourceItem.UInt32N),
                    nameof(sourceItem.Int64N),
                    nameof(sourceItem.UInt64N),
                    nameof(sourceItem.SingleN),
                    nameof(sourceItem.DoubleN),
                    nameof(sourceItem.DecimalN),
                }
                .Select(i => new SummaryInfo { Selector = i, SummaryType = "sum" })
                .ToArray(),
                RemoteGrouping = true,
                ExpandLinqSumType = true
            };

            var summary = DataSourceLoader.Load(new[] { sourceItem, sourceItem }, loadOptions).summary;

            var exprText = loadOptions.ExpressionLog.First(line => line.Contains("GroupBy"));
            Assert.Contains("Sum(obj => " + Compat.ExpectedConvert("obj.SByte", "Int64"), exprText);
            Assert.Contains("Sum(obj => " + Compat.ExpectedConvert("obj.Byte", "Int64"), exprText);
            Assert.Contains("Sum(obj => " + Compat.ExpectedConvert("obj.Int16", "Int64"), exprText);
            Assert.Contains("Sum(obj => " + Compat.ExpectedConvert("obj.UInt16", "Int64"), exprText);
            Assert.Contains("Sum(obj => " + Compat.ExpectedConvert("obj.Int32", "Int64"), exprText);
            Assert.Contains("Sum(obj => " + Compat.ExpectedConvert("obj.UInt32", "Int64"), exprText);
            Assert.Contains("Sum(obj => obj.Int64", exprText);
            Assert.Contains("Sum(obj => " + Compat.ExpectedConvert("obj.UInt64", "Decimal"), exprText);
            Assert.Contains("Sum(obj => " + Compat.ExpectedConvert("obj.Single", "Double"), exprText);
            Assert.Contains("Sum(obj => obj.Double", exprText);
            Assert.Contains("Sum(obj => obj.Decimal", exprText);

            Assert.Equal(2m * sourceItem.SByte, summary[0]);
            Assert.Equal(2m * sourceItem.Byte, summary[1]);
            Assert.Equal(2m * sourceItem.Int16, summary[2]);
            Assert.Equal(2m * sourceItem.UInt16, summary[3]);
            Assert.Equal(2m * sourceItem.Int32, summary[4]);
            Assert.Equal(2m * sourceItem.UInt32, summary[5]);
            Assert.Equal(2m * sourceItem.Int64, summary[6]);
            Assert.Equal(2m * sourceItem.UInt64, summary[7]);
            Assert.Equal(2d * sourceItem.Single, summary[8]);
            Assert.Equal(2d * sourceItem.Double, summary[9]);
            Assert.Equal(2m * sourceItem.Decimal, summary[10]);

            Assert.Equal(2m * sourceItem.SByteN, summary[11]);
            Assert.Equal(2m * sourceItem.ByteN, summary[12]);
            Assert.Equal(2m * sourceItem.Int16N, summary[13]);
            Assert.Equal(2m * sourceItem.UInt16N, summary[14]);
            Assert.Equal(2m * sourceItem.Int32N, summary[15]);
            Assert.Equal(2m * sourceItem.UInt32N, summary[16]);
            Assert.Equal(2m * sourceItem.Int64N, summary[17]);
            Assert.Equal(2m * sourceItem.UInt64N, summary[18]);
            Assert.Equal(2d * sourceItem.SingleN, summary[19]);
            Assert.Equal(2d * sourceItem.DoubleN, summary[20]);
            Assert.Equal(2m * sourceItem.DecimalN, summary[21]);
        }

        [Fact]
        public void T844633() {
            var source = new[] {
                new { G = 1, Value = 1 },
                new { G = 2, Value = 2 },
                new { G = 2, Value = 3 },
                new { G = 3, Value = 4 }
            };

            var loadOptions = new SampleLoadOptions {
                GuardNulls = false,
                Group = new[] {
                    new GroupingInfo { Selector = "G", IsExpanded = false }
                },
                RemoteGrouping = true,
                RequireGroupCount = true,
                Skip = 1,
                Take = 1
            };

            loadOptions.GroupSummary = loadOptions.TotalSummary = new[] {
                new SummaryInfo { Selector = "Value", SummaryType = "sum" },
                new SummaryInfo { SummaryType = "count" }
            };

            var loadResult = DataSourceLoader.Load(source, loadOptions);
            var group = loadResult.data.Cast<Group>().First();
            var summary = loadResult.summary;

            // groups
            Assert.EndsWith(
                ".GroupBy(obj => new AnonType`1(I0 = obj.G))" +
                ".OrderBy(g => g.Key.I0)" +
                ".Select(g => new AnonType`4(" +
                    "I0 = g.Count(), " +
                    "I1 = g.Key.I0, " +
                    "I2 = g.Sum(obj => obj.Value)" +
                "))" +
                ".Skip(1).Take(1)",
                loadOptions.ExpressionLog[0]
            );

            // totals
            Assert.EndsWith(
                ".GroupBy(obj => new AnonType())" +
                ".Select(g => new AnonType`2(" +
                    "I0 = g.Count(), " +
                    "I1 = g.Sum(obj => obj.Value)" +
                "))",
                loadOptions.ExpressionLog[1]
            );

            // group count
            Assert.EndsWith(
                ".Select(obj => obj.G).Distinct().Count()",
                loadOptions.ExpressionLog[2]
            );

            Assert.Equal(5m, group.summary[0]);
            Assert.Equal(2, group.summary[1]);

            Assert.Equal(10m, summary[0]);
            Assert.Equal(4, summary[1]);

            Assert.Equal(4, loadResult.totalCount);
            Assert.Equal(3, loadResult.groupCount);
        }

        [Fact]
        public void T844633_TotalsWoPaging() {
            var source = new[] {
                new { G = 0 }
            };

            var loadOptions = new SampleLoadOptions {
                Group = new[] {
                    new GroupingInfo { Selector = "G", IsExpanded = false }
                },
                RemoteGrouping = true,
                TotalSummary = new[] {
                    new SummaryInfo { Selector = "G", SummaryType = "sum" }
                }
            };

            DataSourceLoader.Load(source, loadOptions);

            Assert.Single(loadOptions.ExpressionLog.Where(line => line.Contains("GroupBy")));
        }
    }

}
