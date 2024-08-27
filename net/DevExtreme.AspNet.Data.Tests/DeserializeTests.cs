using System.Collections;
using System.Text.Json;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class DeserializeTests {
        static readonly JsonSerializerOptions TESTS_DEFAULT_SERIALIZER_OPTIONS = new JsonSerializerOptions(JsonSerializerDefaults.Web) {
            Converters = { new ListConverter() }
        };

        [Theory]
        [InlineData(@"[""fieldName"",""="",null]")]
        [InlineData(@"[[""fieldName1"",""="",""""],""and"",[""fieldName2"",""="",null]]")]
        public void FilterOperandValueCanBeNull(string rawJsonCriteria) {
            var deserializedList = JsonSerializer.Deserialize<IList>(rawJsonCriteria, TESTS_DEFAULT_SERIALIZER_OPTIONS);
            Assert.Equal(3, deserializedList.Count);
        }

        [Fact]
        public void FilterOperandValueCanBeObject() {
            var deserializedList = JsonSerializer.Deserialize<IList>(@"[""fieldName1"",""="",{""Value"":0}]", TESTS_DEFAULT_SERIALIZER_OPTIONS);
            Assert.Equal(3, deserializedList.Count);
            Assert.Equal("{\"Value\":0}", deserializedList[2].ToString());
        }
    }

}
