#if NEWTONSOFT_TESTS
using System.Collections;
using Newtonsoft.Json;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class DeserializeTestsEx {
        [Theory]
        [InlineData(@"[""fieldName"",""="",null]")]
        [InlineData(@"[[""fieldName1"",""="",""""],""and"",[""fieldName2"",""="",null]]")]
        public void FilterOperandValueCanBeNull(string rawJsonCriteria) {
            var deserializedList = JsonConvert.DeserializeObject<IList>(rawJsonCriteria);
            Assert.Equal(3, deserializedList.Count);
        }

        [Fact]
        public void FilterOperandValueCanBeObject() {
            var deserializedList = JsonConvert.DeserializeObject<IList>(@"[""fieldName1"",""="",{""Value"":0}]");
            Assert.Equal(3, deserializedList.Count);
            Assert.Equal("{\r\n  \"Value\": 0\r\n}", deserializedList[2].ToString());
        }
    }

}
#endif
