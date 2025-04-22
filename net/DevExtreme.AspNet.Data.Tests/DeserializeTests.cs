using DevExtreme.AspNet.Data.Helpers;

using System.Collections;
using System.Text.Json;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class DeserializeTests {
        [Theory]
        [InlineData(@"[""fieldName"",""="",null]")]
        [InlineData(@"[[""fieldName1"",""="",""""],""and"",[""fieldName2"",""="",null]]")]
        public void FilterOperandValueCanBeNull(string rawJsonCriteria) {
            var deserializedList = JsonSerializer.Deserialize<IList>(rawJsonCriteria, DataSourceLoadOptionsParser.DEFAULT_SERIALIZER_OPTIONS);
            Assert.Equal(3, deserializedList.Count);
        }

        [Fact]
        public void FilterOperandValueCanBeObject() {
            var deserializedList = JsonSerializer.Deserialize<IList>(@"[""fieldName1"",""="",{""Value"":0}]", DataSourceLoadOptionsParser.DEFAULT_SERIALIZER_OPTIONS);
            Assert.Equal(3, deserializedList.Count);
            Assert.Equal("{\"Value\":0}", deserializedList[2].ToString());
        }
    }

}
