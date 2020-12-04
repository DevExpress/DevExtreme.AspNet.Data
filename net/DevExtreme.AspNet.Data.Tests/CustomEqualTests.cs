using System;
using System.Collections.Generic;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class CustomEqualTests {

        [Fact]
        public void NoOperator() {
            var source = new[] {
                new DataItemNoOperator { Value = 1 },
                new DataItemNoOperator { Value = 2 },
            };
            var operand = new DataItemNoOperator { Value = 1 };

            TestFilter(source, "=", operand, ".Where(obj => Equals(obj, {1}))");
            TestFilter(source, "<>", operand, ".Where(obj => Not(Equals(obj, {1})))");
        }

        [Fact]
        public void WithOperator() {
            TestFilter(
                new[] {
                    new DataItemWithOperator { Value = 1 },
                    new DataItemWithOperator { Value = 2 },
                },
                "=", new DataItemWithOperator { Value = 1 },
                ".Where(obj => (obj == {1}))"
            );
        }

        [Fact]
        public void Struct() {
            TestFilter(
                new[] {
                    new DataItemStruct { Value = 1 },
                    new DataItemStruct { Value = 2 },
                },
                "=", new DataItemStruct { Value = 1 },
                ".Where(obj => Equals("
                    + Compat.ExpectedConvert("obj", "Object") + ", "
                    + Compat.ExpectedConvert("{1}", "Object")
                    + "))"
            );
        }

        static void TestFilter<T>(IEnumerable<T> source, string operation, T operand, string expectedExprPart) {
            var loadOptions = new SampleLoadOptions {
                GuardNulls = false,
                Filter = new object[] { "this", operation, operand },
                IsCountQuery = true
            };

            var loadResult = DataSourceLoader.Load(source, loadOptions);

            Assert.Equal(1, loadResult.totalCount);
            Assert.Contains(expectedExprPart, loadOptions.ExpressionLog[0]);
        }

        class DataItemNoOperator {
            public int Value;
            public override bool Equals(object obj)
                => Value == (obj as DataItemNoOperator).Value;
            public override int GetHashCode()
                => Value.GetHashCode();
            public override string ToString()
                => "{" + Value + "}";
        }

        class DataItemWithOperator {
            public int Value;
            public static bool operator ==(DataItemWithOperator l, DataItemWithOperator r)
                => l.Value == r.Value;
            public static bool operator !=(DataItemWithOperator l, DataItemWithOperator r)
                => l.Value != r.Value;
            public override bool Equals(object obj)
                => throw new NotImplementedException();
            public override int GetHashCode()
                => throw new NotImplementedException();
            public override string ToString()
                => "{" + Value + "}";
        }

        struct DataItemStruct {
            public int Value;
            public override bool Equals(object obj)
                => Value == ((DataItemStruct)obj).Value;
            public override int GetHashCode()
                => Value.GetHashCode();
            public override string ToString()
                => "{" + Value + "}";
        }
    }

}
