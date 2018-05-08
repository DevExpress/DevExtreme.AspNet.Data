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
                + ".GroupBy(obj => new AnonType`2(I0 = obj.G1, I1 = obj.G2))"
                + ".OrderBy(g => g.Key.I0)"
                + ".ThenByDescending(g => g.Key.I1)"
                + ".Select(g => new AnonType`16() {"

                // count
                + "I0 = g.Count(), "

                // keys
                + "I1 = g.Key.I0, "
                + "I2 = g.Key.I1, "

                // total summary
                + "I3 = g.Sum(obj => obj.Value), "
                + "I4 = g.Min(obj => obj.Value), "
                + "I5 = g.Max(obj => obj.Value), "
                + "I6 = g.Sum(obj => obj.Value), "  // avg sum
                + "I7 = g.Count(), "                // avg count
                // (count skipped)

                // group summary
                // (count skipped)
                + "I8 = g.Sum(obj => obj.Nullable), "               // avg sum
                + "I9 = g.Count(obj => (obj.Nullable != null)), "   // avg count
                + "I10 = g.Max(obj => obj.Nullable), "
                + "I11 = g.Min(obj => obj.Nullable), "
                + "I12 = g.Sum(obj => obj.Nullable)"
                + "})",
                expr.ToString()
            );
        }

        [Fact]
        public void CompileEmpty() {
            var expr = new RemoteGroupExpressionCompiler<DataItem>(null, null, null).Compile(CreateTargetParam<DataItem>());
            Assert.Equal(
                "data"
                    + ".GroupBy(obj => new AnonType())"
                    + ".Select(g => new AnonType`1() {I0 = g.Count()})",
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

            Assert.Contains("I0 = (obj - (obj % 123)", Compile<double>("this", false));
            Assert.Contains("I0 = (obj - (obj % 123)", Compile<double?>("this", false));

            Assert.Contains(
                $"I0 = IIF(((obj == null) OrElse (obj.Item1 == null)), null, {Compat.ExpectedConvert("(obj.Item1.Length - (obj.Item1.Length % 123))", "Nullable`1")})",
                Compile<Tuple<string>>("Item1.Length", true)
            );
        }

        [Fact]
        public void GroupInterval_Date() {
            var compiler = new RemoteGroupExpressionCompiler<DateTime>(
                new[] {
                    new GroupingInfo { Selector = "this", GroupInterval = "year" },
                    new GroupingInfo { Selector = "this", GroupInterval = "quarter" },
                    new GroupingInfo { Selector = "this", GroupInterval = "month" },
                    new GroupingInfo { Selector = "this", GroupInterval = "day" },
                    new GroupingInfo { Selector = "this", GroupInterval = "dayOfWeek" },
                    new GroupingInfo { Selector = "this", GroupInterval = "hour" },
                    new GroupingInfo { Selector = "this", GroupInterval = "minute" },
                    new GroupingInfo { Selector = "this", GroupInterval = "second" }
                },
                null, null
            );

            var expr = compiler.Compile(CreateTargetParam<DateTime>()).ToString();

            Assert.Contains("I0 = obj.Year", expr);
            Assert.Contains("I1 = ((obj.Month + 2) / 3)", expr);
            Assert.Contains("I2 = obj.Month", expr);
            Assert.Contains("I3 = obj.Day", expr);
            Assert.Contains("I4 = " + Compat.ExpectedConvert("obj.DayOfWeek", "Int32"), expr);
            Assert.Contains("I5 = obj.Hour", expr);
            Assert.Contains("I6 = obj.Minute", expr);
            Assert.Contains("I7 = obj.Second", expr);
        }

        [Fact]
        public void GroupInterval_NullableDate() {
            var compiler = new RemoteGroupExpressionCompiler<Nullable<DateTime>>(
                new[] {
                    new GroupingInfo { Selector = "this", GroupInterval = "year" }
                },
                null, null
            );

            var expr = compiler.Compile(CreateTargetParam<Nullable<DateTime>>()).ToString();

            Assert.Contains("I0 = " +  Compat.ExpectedConvert("obj.Value.Year", "Nullable`1"), expr);
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
                "(" + String.Join(", ", memberNames.Select((i, index) => $"I{index} = obj.{i}")) + ")",
                compiler.Compile(CreateTargetParam<T>()).ToString()
            );
        }

    }

}
