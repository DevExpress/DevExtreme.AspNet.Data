using MongoDB.Bson;
using System;
using System.Collections;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class BsonBindingTests {

        [Fact]
        public void Filter() {
            string Compile(IList filter, bool stringToLower = false)
                => new FilterExpressionCompiler(typeof(BsonDocument), false, stringToLower).Compile(filter).Body.ToString();

            var left = "obj.get_Item(\"p\")";
            var rightString = Compat.ExpectedConvert("\"v\"", "BsonValue");

            Assert.Equal(
                $"({left} == {rightString})",
                Compile(new[] { "p", "v" })
            );

            Assert.Equal(
                $"({left} == BsonNull)",
                Compile(new[] { "p", null })
            );

            var leftAsString = Compat.ExpectedConvert(left, "String");

            Assert.Equal(
                $"IsMatch({leftAsString}, \"\\.\", Singleline)",
                Compile(new[] { "p", "contains", "." })
            );

            Assert.Equal(
                $"Not(IsMatch({leftAsString}, \"\\.\", IgnoreCase, Singleline))",
                Compile(new[] { "p", "notcontains", "." }, true)
            );

            Assert.Equal(
                $"IsMatch({leftAsString}, \"^\\.\", Singleline)",
                Compile(new[] { "p", "startswith", "." })
            );

            Assert.Equal(
                $"IsMatch({leftAsString}, \"\\.$\", Singleline)",
                Compile(new[] { "p", "endswith", "." })
            );
        }

    }

}
