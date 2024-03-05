#if NEWTONSOFT_TESTS
using DevExtreme.AspNet.Data.ResponseModel;

using System.ComponentModel;
using Xunit;

using Newtonsoft.Json;

namespace DevExtreme.AspNet.Data.Tests {
    
    public class ResponseModelTestsEx {

        class LoadResultEx : LoadResult {
            [DefaultValue(-1), JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
            public new int totalCount { get; set; } = -1;
            [DefaultValue(-1), JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
            public new int groupCount { get; set; } = -1;
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
            public new object[] summary { get; set; }
        }

        [Fact]
        public void EmptyLoadResultSerialization() {
            Assert.Equal(
                "{\"data\":null}",
                JsonConvert.SerializeObject(new LoadResultEx())
            );
        }

    }

}
#endif
