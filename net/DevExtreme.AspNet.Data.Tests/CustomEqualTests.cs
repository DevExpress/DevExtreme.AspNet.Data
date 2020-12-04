using System;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class CustomEqualTests {

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void NoOperator(bool not) {
            var source = new[] {
                new DataItemNoOperator { Value = 1 },
                new DataItemNoOperator { Value = 2 }
            };

            var loadOptions = new SampleLoadOptions {
                GuardNulls = false,
                Filter = new object[] {
                    "this",
                    not ? "<>" : "=",
                    new DataItemNoOperator { Value = 1 }
                },
                IsCountQuery = true
            };

            var loadResult = DataSourceLoader.Load(source, loadOptions);

            Assert.Equal(1, loadResult.totalCount);
            Assert.Contains(
                not
                    ? ".Where(obj => Not(Equals(obj, {1})))"
                    : ".Where(obj => Equals(obj, {1}))",
                loadOptions.ExpressionLog[0]
            );
        }

        [Fact]
        public void WithOperator() {
            var source = new[] {
                new DataItemWithOperator { Value = 1 },
                new DataItemWithOperator { Value = 2 }
            };

            var loadOptions = new SampleLoadOptions {
                GuardNulls = false,
                Filter = new object[] { "this", new DataItemWithOperator { Value = 1 } },
                IsCountQuery = true
            };

            var loadResult = DataSourceLoader.Load(source, loadOptions);

            Assert.Equal(1, loadResult.totalCount);
            Assert.Contains(
                ".Where(obj => (obj == {1}))",
                loadOptions.ExpressionLog[0]
            );
        }

        [Fact]
        public void Struct() {
            var source = new[] {
                new DataItemStruct { Value = 1 },
                new DataItemStruct { Value = 2 }
            };

            var loadOptions = new SampleLoadOptions {
                GuardNulls = false,
                Filter = new object[] { "this", new DataItemStruct { Value = 1 } },
                IsCountQuery = true
            };

            var loadResult = DataSourceLoader.Load(source, loadOptions);

            Assert.Equal(1, loadResult.totalCount);
            Assert.Contains(
                ".Where(obj => Equals(" + Compat.ExpectedConvert("obj", "Object") +  ", " + Compat.ExpectedConvert("{1}", "Object") + "))",
                loadOptions.ExpressionLog[0]
            );
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
