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

            public DateTime dateTime = new DateTime(2003, 3, 30);
            public DateTimeOffset dateTimeOffset = new DateTimeOffset(new DateTime(2003, 3, 30));
            public TimeSpan timeSpan = TimeSpan.FromMinutes(3);

            public Guid guid = new Guid(TEST_GUID);

            public DayOfWeek @enum = DayOfWeek.Wednesday;
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

            public DateTime? dateTime = new DateTime(2003, 3, 30);
            public DateTimeOffset? dateTimeOffset = new DateTimeOffset(new DateTime(2003, 3, 30));
            public TimeSpan? timeSpan = TimeSpan.FromMinutes(3);

            public Guid? guid = new Guid(TEST_GUID);

            public DayOfWeek? @enum = DayOfWeek.Wednesday;
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

            AssertEvaluation(obj, "dateTime", "3/30/2003");
            AssertEvaluation(obj, "dateTimeOffset", "3/30/2003");
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

            AssertEvaluation(obj, "dateTime", "3/30/2003");
            AssertEvaluation(obj, "dateTimeOffset", "3/30/2003");
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
            var compiler = new FilterExpressionCompiler<Structs>(false);

            Assert.Equal("False", compiler.Compile(new[] { "byte", "-3" }).Body.ToString());
            Assert.Equal("False", compiler.Compile(new[] { "byte", "257" }).Body.ToString());
            Assert.Equal("False", compiler.Compile(new[] { "int", "not-int" }).Body.ToString());
        }


        [Fact]
        public void DatesWithTimeComponent() {
            var testDate = new DateTime(2000, 1, 2, 3, 4, 5, 6);
            var testDateString = testDate.ToString("M/dd/yyyy HH:mm:ss.fff");

            var obj = new Structs {
                dateTime = testDate,
                dateTimeOffset = (DateTimeOffset)testDate
            };

            AssertEvaluation(obj, "dateTime", testDateString);
            AssertEvaluation(obj, "dateTimeOffset", testDateString);
        }

        [Fact]
        public void DatesFromIsoFormatWithUtc() {
            var testDate = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc);
            var testDateString = "2000-01-02T03:04:05Z";

            var obj = new Structs {
                dateTime = testDate,
                dateTimeOffset = (DateTimeOffset)testDate
            };

            AssertEvaluation(obj, "dateTime", testDateString);
            AssertEvaluation(obj, "dateTimeOffset", testDateString);
        }

        [Fact]
        public void FilterByEnumField() {
            var structObj = new Structs();
            AssertEvaluation(structObj, "enum", "=", 3);
            AssertEvaluation(structObj, "enum", "=", "3");
            AssertEvaluation(structObj, "enum", "=", "wednesday");

            var nullableObj = new NullableStructs();
            AssertEvaluation(nullableObj, "enum", "=", 3);
            AssertEvaluation(nullableObj, "enum", "=", "3");
            AssertEvaluation(nullableObj, "enum", "=", "wednesday");
        }

        [Fact]
        public void StringFuncOnTimeSpan() {
            // Regression test for the fix of https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/112

            var obj = new {
                Time = TimeSpan.Parse("01:23"),
                NullableTime = new TimeSpan?(TimeSpan.Parse("01:23"))
            };

            AssertEvaluation(obj, new[] { "Time", "contains", "23" });
            AssertEvaluation(obj, new[] { "NullableTime", "contains", "23" });
        }

    }

}
