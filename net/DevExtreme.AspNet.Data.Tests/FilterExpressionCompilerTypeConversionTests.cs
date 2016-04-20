using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class FilterExpressionCompilerTypeConversionTests {
        const string TEST_GUID = "01234567-0123-0123-0123-012345670000";

        void AssertEvaluation<T>(T dataItem, params object[] clientFilter) {
            var expr = new FilterExpressionCompiler<T>(false).Compile(clientFilter);
            Assert.Equal(true, expr.Compile().DynamicInvoke(dataItem));
        }

        class Structs {
            public sbyte @sbyte = 3;
            public int @int = 3;
            public long @long = 3;

            public byte @byte = 3;
            public uint @uint = 3;
            public ulong @ulong = 3;

            public float @float = 3;
            public double @double = 3;
            public decimal @decimal = 3;

            public char @char = '3';
            public bool @bool = true;

            public DateTime dateTime = new DateTime(2003, 3, 3);
            public DateTimeOffset dateTimeOffset = new DateTimeOffset(new DateTime(2003, 3, 3));
            public TimeSpan timeSpan = TimeSpan.FromMinutes(3);

            public Guid guid = new Guid(TEST_GUID);
        }

        class NullableStructs {
            public sbyte? @sbyte = 3;
            public int? @int = 3;
            public long? @long = 3;

            public byte? @byte = 3;
            public uint? @uint = 3;
            public ulong? @ulong = 3;

            public float? @float = 3;
            public double? @double = 3;
            public decimal? @decimal = 3;

            public char? @char = '3';
            public bool? @bool = true;

            public DateTime? dateTime = new DateTime(2003, 3, 3);
            public DateTimeOffset? dateTimeOffset = new DateTimeOffset(new DateTime(2003, 3, 3));
            public TimeSpan? timeSpan = TimeSpan.FromMinutes(3);

            public Guid? guid = new Guid(TEST_GUID);
        }

        [Fact]
        public void StructFromString() {
            var obj = new Structs();

            AssertEvaluation(obj, "sbyte", "3");
            AssertEvaluation(obj, "int", "3");
            AssertEvaluation(obj, "long", "3");

            AssertEvaluation(obj, "byte", "3");
            AssertEvaluation(obj, "uint", "3");
            AssertEvaluation(obj, "ulong", "3");

            AssertEvaluation(obj, "float", "3");
            AssertEvaluation(obj, "double", "3");
            AssertEvaluation(obj, "decimal", "3");

            AssertEvaluation(obj, "bool", "true");
            AssertEvaluation(obj, "char", "3");

            AssertEvaluation(obj, "dateTime", "2003/3/3");
            AssertEvaluation(obj, "dateTimeOffset", "2003/3/3");
            AssertEvaluation(obj, "timeSpan", "00:03");

            AssertEvaluation(obj, "guid", TEST_GUID);
        }

        [Fact]
        public void NullableStructFromString() {
            var obj = new NullableStructs();

            AssertEvaluation(obj, "sbyte", "3");
            AssertEvaluation(obj, "int", "3");
            AssertEvaluation(obj, "long", "3");

            AssertEvaluation(obj, "byte", "3");
            AssertEvaluation(obj, "uint", "3");
            AssertEvaluation(obj, "ulong", "3");

            AssertEvaluation(obj, "float", "3");
            AssertEvaluation(obj, "double", "3");
            AssertEvaluation(obj, "decimal", "3");

            AssertEvaluation(obj, "bool", "true");
            AssertEvaluation(obj, "char", "3");

            AssertEvaluation(obj, "dateTime", "2003/3/3");
            AssertEvaluation(obj, "dateTimeOffset", "2003/3/3");
            AssertEvaluation(obj, "timeSpan", "00:03");

            AssertEvaluation(obj, "guid", TEST_GUID);
        }

        [Fact]
        public void StringFromNonString() {
            AssertEvaluation(new { s = "123" }, "s", 123);
        }

        [Fact]
        public void IntegralFromFloat() {
            var obj = new Structs();

            AssertEvaluation(obj, "sbyte", 2.9);
            AssertEvaluation(obj, "int", 2.9);
            AssertEvaluation(obj, "long", 2.9);

            AssertEvaluation(obj, "byte", 2.9);
            AssertEvaluation(obj, "uint", 2.9);
            AssertEvaluation(obj, "ulong", 2.9);
        }

        [Fact]
        public void IntegralFromFloatString() {
            var obj = new Structs();

            AssertEvaluation(obj, "sbyte", "2.9");
            AssertEvaluation(obj, "int", "2.9");
            AssertEvaluation(obj, "long", "2.9");

            AssertEvaluation(obj, "byte", "2.9");
            AssertEvaluation(obj, "uint", "2.9");
            AssertEvaluation(obj, "ulong", "2.9");
        }

        [Fact]
        public void InvalidValueHandling() {
            var obj = new Structs();

            var compiler = new FilterExpressionCompiler<Structs>(false);

            Assert.Equal("False", compiler.Compile(new[] { "byte", "-3" }).Body.ToString());
            Assert.Equal("False", compiler.Compile(new[] { "byte", "257" }).Body.ToString());
            Assert.Equal("False", compiler.Compile(new[] { "int", "not-int" }).Body.ToString());
        }


    }

}
