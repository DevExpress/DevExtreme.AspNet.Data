using System;
using System.Linq;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class StringToLowerTests {

        [Theory]
        [InlineData("=", "(obj.ToLower() == 't')")]
        [InlineData("<>", "(obj.ToLower() != 't')")]
        [InlineData(">", "(Compare(obj.ToLower(), 't') > 0)")]
        [InlineData("<", "(Compare(obj.ToLower(), 't') < 0)")]
        [InlineData(">=", "(Compare(obj.ToLower(), 't') >= 0)")]
        [InlineData("<=", "(Compare(obj.ToLower(), 't') <= 0)")]
        [InlineData("startswith", "obj.ToLower().StartsWith('t')")]
        [InlineData("endswith", "obj.ToLower().EndsWith('t')")]
        [InlineData("contains", "obj.ToLower().Contains('t')")]
        [InlineData("notcontains", "Not(obj.ToLower().Contains('t'))")]
        public void True(string op, string expectedExpr) {
            AssertFilter<string>(false, true, op, expectedExpr);
        }

        [Fact]
        public void ImplicitTrueForL2O() {
            var loadResult = DataSourceLoader.Load(new[] { "T" }, new SampleLoadOptions {
                Filter = new[] { "this", "t" },
                IsCountQuery = true
            });

            Assert.Equal(1, loadResult.totalCount);
        }

        [Fact]
        public void ForceToString_ToLower_GuardNulls() {
            AssertFilter<int?>(true, true, "contains", "(IIF((obj == null), null, obj.ToString().ToLower()) ?? '').Contains('t')");
        }

        [Fact]
        public void Dynamic() {
            var compiler = new FilterExpressionCompiler<dynamic>(true, true);
            var expr = compiler.Compile(new object[] {
                new[] { "this", "startswith", "1" },
                "or",
                new[] { "this", "b" },
                "or",
                new[] { "this", ">=", "c" }
            });

            var expectedExpr = "(((IIF((obj == null), null, obj.ToString().ToLower()) ?? '').StartsWith('1')"
                    + " OrElse (IIF((obj == null), null, obj.ToString().ToLower()) == 'b'))"
                    + " OrElse (Compare(IIF((obj == null), null, obj.ToString().ToLower()), 'c') >= 0))";

            Assert.Equal(
                expectedExpr.Replace("'", "\""),
                expr.Body.ToString()
            );

            var method = expr.Compile();
            Assert.True((bool)method.DynamicInvoke(1));
            Assert.True((bool)method.DynamicInvoke("b"));
            Assert.True((bool)method.DynamicInvoke('c'));
        }

        void AssertFilter<T>(bool guardNulls, bool stringToLower, string op, string expectedExpr) {
            expectedExpr = expectedExpr.Replace("'", "\"");

            Assert.Equal(
                expectedExpr,
                new FilterExpressionCompiler<T>(guardNulls, stringToLower)
                    .Compile(new[] { "this", op, "T" })
                    .Body.ToString()
            );
        }


    }

}
