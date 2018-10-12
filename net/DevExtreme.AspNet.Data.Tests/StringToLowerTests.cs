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
            AssertFilter(false, true, op, expectedExpr);
        }

        [Fact]
        public void ImplicitTrueForL2O() {
            var loadResult = DataSourceLoader.Load(new[] { "T" }, new SampleLoadOptions {
                Filter = new[] { "this", "t" },
                IsCountQuery = true
            });

            Assert.Equal(1, loadResult.totalCount);
        }

        void AssertFilter(bool guardNulls, bool stringToLower, string op, string expectedExpr) {
            expectedExpr = expectedExpr.Replace("'", "\"");

            Assert.Equal(
                expectedExpr,
                new FilterExpressionCompiler<string>(guardNulls, stringToLower)
                    .Compile(new[] { "this", op, "T" })
                    .Body.ToString()
            );
        }


    }

}
