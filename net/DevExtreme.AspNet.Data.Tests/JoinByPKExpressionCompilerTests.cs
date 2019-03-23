using System;
using System.Linq;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class JoinByPKExpressionCompilerTests {

        class DataItem {
            public int K1 { get; set; }
            public long K2 { get; set; }
            public string Prop { get; set; }
        }

        static readonly IQueryable<DataItem> SOURCE = new DataItem[0].AsQueryable();

        [Fact]
        public void MultiKey() {
            var compiler = new JoinByPKExpressionCompiler<DataItem>(false, null);

            var expr = compiler.Compile(
                SOURCE.Expression,
                SOURCE.Skip(1).Take(2).Expression,
                new[] { "K1", "K2" }
            );

            Assert.Equal(typeof(IQueryable<DataItem>), expr.Type);

            Assert.Equal(
                "source.Join(" +
                    "source.Skip(1).Take(2), " +
                    "obj => new AnonType`2(I0 = obj.K1, I1 = obj.K2), " +
                    "obj => new AnonType`2(I0 = obj.K1, I1 = obj.K2), " +
                    "(outer, inner) => outer" +
                ")",
                ShortenSource(expr.ToString())
            );
        }

        [Fact]
        public void SingleKey() {
            var compiler = new JoinByPKExpressionCompiler<DataItem>(false, null);

            var expr = compiler.Compile(
                SOURCE.Expression,
                SOURCE.Skip(1).Take(2).Expression,
                new[] { "K1" }
            );

            Assert.Equal(typeof(IQueryable<DataItem>), expr.Type);

            Assert.Equal(
                "source.Where(obj => source" +
                    ".Skip(1).Take(2)" +
                    ".Select(obj => obj.K1)" +
                    ".Contains(obj.K1)" +
                ")",
                ShortenSource(expr.ToString())
            );
        }


        static string ShortenSource(string expr) {
            return expr.Replace(typeof(DataItem).FullName + "[]", "source");
        }

    }

}
