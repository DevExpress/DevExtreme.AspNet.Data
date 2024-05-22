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
    }

}
#endif
