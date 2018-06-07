using DevExtreme.AspNet.Data.RemoteGrouping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class RemoteGroupExpressionCompilerTests {

        class DataItem {
            public int G1 = 0;
            public int G2 = 0;
            public int Value = 0;
            public int? Nullable = null;
        }

        static ParameterExpression CreateTargetParam<T>() {
            return Expression.Parameter(typeof(IQueryable<T>), "data");
        }

        [Fact]
        public void Compile() {
            var compiler = new RemoteGroupExpressionCompiler<DataItem>(
                new[] {
                    new GroupingInfo { Selector = "G1" },
                    new GroupingInfo { Selector = "G2", Desc = true }
                },
                new[] {
                    new SummaryInfo { Selector = "Value", SummaryType = "sum" },
                    new SummaryInfo { Selector = "Value", SummaryType = "min" },
                    new SummaryInfo { Selector = "Value", SummaryType = "max" },
                    new SummaryInfo { Selector = "Value", SummaryType = "avg" },
                    new SummaryInfo { SummaryType = "count" }
                },
                new[] {
                    new SummaryInfo { SummaryType = "count" },
                    new SummaryInfo { Selector = "Nullable", SummaryType = "avg" },
                    new SummaryInfo { Selector = "Nullable", SummaryType = "max" },
                    new SummaryInfo { Selector = "Nullable", SummaryType = "min" },
                    new SummaryInfo { Selector = "Nullable", SummaryType = "sum" }
                }
            );

            var expr = compiler.Compile(CreateTargetParam<DataItem>());

            Assert.Equal(
                "data"
                + ".GroupBy(obj => new Tuple`2(Item1 = obj.G1, Item2 = obj.G2))"
                + ".OrderBy(g => g.Key.Item1)"
                + ".ThenByDescending(g => g.Key.Item2)"
                + ".Select(g => new Tuple`8("

                // count
                + "Item1 = g.Count(), "

                // keys
                + "Item2 = g.Key.Item1, "
                + "Item3 = g.Key.Item2, "

                // total summary
                + "Item4 = g.Sum(obj => obj.Value), "
                + "Item5 = g.Min(obj => obj.Value), "
                + "Item6 = g.Max(obj => obj.Value), "
                + "Item7 = g.Sum(obj => obj.Value), Rest = new Tuple`6("    // avg sum
                + "Item1 = g.Count(), "                                     // avg count
                // (count skipped)

                // group summary
                // (count skipped)
                + "Item2 = g.Sum(obj => obj.Nullable), "               // avg sum
                + "Item3 = g.Count(obj => (obj.Nullable != null)), "   // avg count
                + "Item4 = g.Max(obj => obj.Nullable), "
                + "Item5 = g.Min(obj => obj.Nullable), "
                + "Item6 = g.Sum(obj => obj.Nullable)"
                + ")))",
                expr.ToString()
            );
        }

        [Fact]
        public void CompileEmpty() {
            var expr = new RemoteGroupExpressionCompiler<DataItem>(null, null, null).Compile(CreateTargetParam<DataItem>());
            Assert.Equal(
                "data"
                    + ".GroupBy(obj => 1)"
                    + ".Select(g => new Tuple`1(Item1 = g.Count()))",
                expr.ToString()
            );
        }

        [Fact]
        public void IgnoreGroupSummaryIfNoGroups() {
            var compiler = new RemoteGroupExpressionCompiler<DataItem>(null, null, new[] {
                new SummaryInfo { Selector = "ignore me", SummaryType = "ignore me" }
            });

            compiler.Compile(CreateTargetParam<DataItem>());
        }

        [Fact]
        public void GroupInterval_Numeric() {

            string Compile<T>(string selector, bool guardNulls) {
                var compiler = new RemoteGroupExpressionCompiler<T>(
                    guardNulls,
                    new[] {
                        new GroupingInfo { Selector = selector, GroupInterval = "123" }
                    },
                    null, null
                );

                return compiler.Compile(CreateTargetParam<T>()).ToString();
            }

            Assert.Contains("Item1 = (obj - (obj % 123)", Compile<double>("this", false));
            Assert.Contains("Item1 = (obj - (obj % 123)", Compile<double?>("this", false));

            Assert.Contains(
                $"Item1 = IIF(((obj == null) OrElse (obj.Item1 == null)), null, {Compat.ExpectedConvert("(obj.Item1.Length - (obj.Item1.Length % 123))", "Nullable`1")})",
                Compile<Tuple<string>>("Item1.Length", true)
            );
        }

        [Fact]
        public void GroupInterval_Date() {

            string Compile<T>(string selector, bool guardNulls) {
                var compiler = new RemoteGroupExpressionCompiler<T>(
                    guardNulls,
                    new[] {
                        new GroupingInfo { Selector = selector, GroupInterval = "year" },
                        new GroupingInfo { Selector = selector, GroupInterval = "quarter" },
                        new GroupingInfo { Selector = selector, GroupInterval = "month" },
                        new GroupingInfo { Selector = selector, GroupInterval = "day" },
                        new GroupingInfo { Selector = selector, GroupInterval = "dayOfWeek" },
                        new GroupingInfo { Selector = selector, GroupInterval = "hour" },
                        new GroupingInfo { Selector = selector, GroupInterval = "minute" },
                        new GroupingInfo { Selector = selector, GroupInterval = "second" }
                    },
                    null, null
                );

                return compiler.Compile(CreateTargetParam<T>()).ToString();
            }

            {
                var expr = Compile<DateTime>("this", false);

                Assert.Contains("Item1 = obj.Year", expr);
                Assert.Contains("Item2 = ((obj.Month + 2) / 3)", expr);
                Assert.Contains("Item3 = obj.Month", expr);
                Assert.Contains("Item4 = obj.Day", expr);
                Assert.Contains("Item5 = " + Compat.ExpectedConvert("obj.DayOfWeek", "Int32"), expr);
                Assert.Contains("Item6 = obj.Hour", expr);
                Assert.Contains("Item7 = obj.Minute", expr);
                Assert.Contains("Item1 = obj.Second", expr);
            }

            {
                var expr = Compile<DateTime?>("this", false);

                string Wrap(string coreSelector) {
                    return Compat.ExpectedConvert(coreSelector, "Nullable`1");
                }

                Assert.Contains("Item1 = " + Wrap("obj.Value.Year"), expr);
                Assert.Contains("Item2 = " + Wrap("((obj.Value.Month + 2) / 3)"), expr);
                Assert.Contains("Item3 = " + Wrap("obj.Value.Month"), expr);
                Assert.Contains("Item4 = " + Wrap("obj.Value.Day"), expr);
                Assert.Contains("Item5 = " + Wrap("obj.Value.DayOfWeek"), expr);
                Assert.Contains("Item6 = " + Wrap("obj.Value.Hour"), expr);
                Assert.Contains("Item7 = " + Wrap("obj.Value.Minute"), expr);
                Assert.Contains("Item1 = " + Wrap("obj.Value.Second"), expr);
            }

            {
                var expr = Compile<Tuple<DateTime?>>("Item1", true);

                string Wrap(string coreSelector) {
                    return $"IIF(((obj == null) OrElse (obj.Item1 == null)), null, {Compat.ExpectedConvert(coreSelector, "Nullable`1")})";
                }

                Assert.Contains("Item1 = " + Wrap("obj.Item1.Value.Year"), expr);
                Assert.Contains("Item2 = " + Wrap("((obj.Item1.Value.Month + 2) / 3)"), expr);
                Assert.Contains("Item3 = " + Wrap("obj.Item1.Value.Month"), expr);
                Assert.Contains("Item4 = " + Wrap("obj.Item1.Value.Day"), expr);
                Assert.Contains("Item5 = " + Wrap("obj.Item1.Value.DayOfWeek"), expr);
                Assert.Contains("Item6 = " + Wrap("obj.Item1.Value.Hour"), expr);
                Assert.Contains("Item7 = " + Wrap("obj.Item1.Value.Minute"), expr);
                Assert.Contains("Item1 = " + Wrap("obj.Item1.Value.Second"), expr);
            }
        }

        [Fact]
        public void Bug100() {
            Bug100Core<Tuple<int, int, int>>("Item1", "Item2", "Item3");
            Bug100Core<Tuple<int, int, int, int, int>>("Item1", "Item2", "Item3", "Item4", "Item5");
            Bug100Core<Tuple<int, int, int, int, int, int>>("Item1", "Item2", "Item3", "Item4", "Item5", "Item6");
            Bug100Core<Tuple<int, int, int, int, int, int, int>>("Item1", "Item2", "Item3", "Item4", "Item5", "Item6", "Item7");
        }

        void Bug100Core<T>(params string[] memberNames) {
            var compiler = new RemoteGroupExpressionCompiler<T>(
                memberNames.Select(i => new GroupingInfo { Selector = i }).ToArray(),
                null,
                null
            );

            Assert.Contains(
                "(" + String.Join(", ", memberNames.Select((i, index) => $"Item{1 + index} = obj.{i}")) + ")",
                compiler.Compile(CreateTargetParam<T>()).ToString()
            );
        }

    }

}
