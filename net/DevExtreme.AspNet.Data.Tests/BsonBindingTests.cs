using MongoDB.Bson;
using System;
using System.Collections;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class BsonBindingTests {

        [Fact]
        public void Filter() {
            var compiler = new FilterExpressionCompiler(typeof(BsonDocument), false);

            string Compile(IList filter) => compiler.Compile(filter).Body.ToString();

            var left = "obj.get_Item(\"p\")";
            var rightString = Compat.ExpectedConvert("\"v\"", "BsonValue");

            Assert.Equal(
                $"({left} == {rightString})",
                Compile(new[] { "p", "v" })
            );

            Assert.Equal(
                $"({left} == null)",
                Compile(new[] { "p", null })
            );
        }

    }

}
