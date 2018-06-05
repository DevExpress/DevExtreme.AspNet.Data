using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class SelectExpressionCompilerTests {

        Expression Compile<T>(params string[] selectors) {
            return new SelectExpressionCompiler<T>(false).Compile(
                Expression.Parameter(typeof(IQueryable<T>), "data"),
                selectors
            );
        }

        [Fact]
        public void SimpleSelector() {
            var expr = Compile<Tuple<string>>("Item1");
            Assert.Equal("data.Select(obj => new Tuple`1(Item1 = obj.Item1))", expr.ToString());
        }

    }

}
