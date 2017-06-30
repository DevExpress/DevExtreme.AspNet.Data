using DevExtreme.AspNet.Data.ResponseModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class ResponseModelTests {

        [Fact]
        public void EmptyLoadResultSerialization() {
            Assert.Equal(
                "{\"data\":null}",
                JsonConvert.SerializeObject(new LoadResult())
            );
        }

        [Fact]
        public void EmptyGroupSerialization() {
            var json = JsonConvert.SerializeObject(new Group());

            // these must always be present
            Assert.Contains("\"key\":", json);
            Assert.Contains("\"items\":", json);

            Assert.DoesNotContain("\"count\":", json);
            Assert.DoesNotContain("\"summary\":", json);
        }

#if NET40
        [Fact]
        public void JavaScriptSerializer() {
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            var loadResultJson = serializer.Serialize(new LoadResult());
            Assert.Contains("\"data\":", loadResultJson);
            Assert.Contains("\"totalCount\":", loadResultJson);
            Assert.Contains("\"groupCount\":", loadResultJson);
            Assert.Contains("\"summary\":", loadResultJson);

            var groupJson = serializer.Serialize(new Group());
            Assert.Contains("\"key\":", groupJson);
            Assert.Contains("\"items\":", groupJson);
            Assert.Contains("\"count\":", groupJson);
            Assert.Contains("\"summary\":", groupJson);
        }
#endif

    }

}
