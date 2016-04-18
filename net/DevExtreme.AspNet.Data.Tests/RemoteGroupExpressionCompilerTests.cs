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
        }

        static ParameterExpression CreateTargetParam<T>() {
            return Expression.Parameter(typeof(IQueryable<T>), "data");
        }

        [Fact]
        public void Compile() {
            var compiler = new RemoteGroupExpressionCompiler<DataItem>(
                new[] {
                    new GroupingInfo { Selector = "G1" },
                    new GroupingInfo { Selector = "G2" }
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
                    new SummaryInfo { Selector = "Value", SummaryType = "avg" },
                    new SummaryInfo { Selector = "Value", SummaryType = "max" },
                    new SummaryInfo { Selector = "Value", SummaryType = "min" },
                    new SummaryInfo { Selector = "Value", SummaryType = "sum" }
                }
            );

            var expr = compiler.Compile(CreateTargetParam<DataItem>());

            Assert.Equal(
                "data"
                + ".GroupBy(obj => new RemoteGroupKey`8(K0 = obj.G1, K1 = obj.G2))"
                + ".Select(g => new RemoteGroup`24() {"
                + "K0 = g.Key.K0, "
                + "K1 = g.Key.K1, "
                + "Count = g.Count(), "
                + "G1 = g.Sum(obj => obj.Value), "
                + "G2 = g.Max(obj => obj.Value), "
                + "G3 = g.Min(obj => obj.Value), "
                + "G4 = g.Sum(obj => obj.Value), "
                + "T0 = g.Sum(obj => obj.Value), "
                + "T1 = g.Min(obj => obj.Value), "
                + "T2 = g.Max(obj => obj.Value), "
                + "T3 = g.Sum(obj => obj.Value)"
                + "})",
                expr.ToString()
            );
        }

        [Fact]
        public void CompileEmpty() {
            var expr = new RemoteGroupExpressionCompiler<DataItem>(null, null, null).Compile(CreateTargetParam<DataItem>());
            Assert.Equal(
                "data"
                    + ".GroupBy(obj => new RemoteGroupKey`8())"
                    + ".Select(g => new RemoteGroup`24() {Count = g.Count()})",
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
            var compiler = new RemoteGroupExpressionCompiler<double>(
                new[] {
                    new GroupingInfo { Selector = "this", GroupInterval = "123" }
                },
                null, null
            );

            var expr = compiler.Compile(CreateTargetParam<double>()).ToString();
            Assert.Contains("K0 = (obj - (obj % Convert(123))", expr);
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

            Assert.Contains("K0 = obj.Year", expr);
            Assert.Contains("K1 = ((obj.Month + 2) / 3)", expr);
            Assert.Contains("K2 = obj.Month", expr);
            Assert.Contains("K3 = obj.Day", expr);
            Assert.Contains("K4 = Convert(obj.DayOfWeek)", expr);
            Assert.Contains("K5 = obj.Hour", expr);
            Assert.Contains("K6 = obj.Minute", expr);
            Assert.Contains("K7 = obj.Second", expr);
        }

        [Fact]
        public void RemoteGroupKeyClass() {
            var key1 = new RemoteGroupKey<int, int, int, int, int, int, int, int>(0, 1, 2, 3, 4, 5, 6, 7);

            Assert.Equal(0, key1.K0);
            Assert.Equal(1, key1.K1);
            Assert.Equal(2, key1.K2);
            Assert.Equal(3, key1.K3);
            Assert.Equal(4, key1.K4);
            Assert.Equal(5, key1.K5);
            Assert.Equal(6, key1.K6);
            Assert.Equal(7, key1.K7);

            Assert.False(Equals(key1, new object()));

            var key2 = new RemoteGroupKey<int, int, int, int, int, int, int, int>(0, 1, 2, 3, 4, 5, 6, 7);

            Assert.True(Equals(key1, key2));
            Assert.Equal(key1.GetHashCode(), key2.GetHashCode());
        }

        [Fact]
        public void RemoteGroupClass() {
            var group = new RemoteGroup<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>();
            var type = group.GetType().GetTypeInfo();
            var accessor = new RemoteGroupAccessor();

            var prefixes = new Dictionary<string, int> {
                ["K"] = 8,
                ["T"] = 8,
                ["G"] = 8
            };

            foreach(var prefix in prefixes.Keys) {
                for(var index = 0; index < prefixes[prefix]; index++) {
                    var value = new Object();
                    type.GetDeclaredField(prefix + index).SetValue(group, value);
                    Assert.Same(value, accessor.Read(group, prefix + index));
                }
            }
        }

    }

}
