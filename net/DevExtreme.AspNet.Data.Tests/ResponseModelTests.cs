using DevExtreme.AspNet.Data.ResponseModel;

using System.Text.Json;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class ResponseModelTests {
#pragma warning disable xUnit1004 // skip until external / dependency reason is resolved
        [Fact(Skip = "Skip until consolidation or target bump to net7 and ShouldSerialize")]
#pragma warning restore xUnit1004
        public void EmptyLoadResultSerialization() {
            //https://github.com/dotnet/runtime/issues/41630
            //https://github.com/dotnet/runtime/issues/36236
            Assert.Equal(
                "{\"data\":null,\"totalCount\":-1,\"groupCount\":-1}",
                JsonSerializer.Serialize(new LoadResult())
            );
        }

        [Fact]
        public void EmptyGroupSerialization() {
            var json = JsonSerializer.Serialize(new Group());

            // these must always be present
            Assert.Contains("\"key\":", json);
            Assert.Contains("\"items\":", json);

            Assert.DoesNotContain("\"count\":", json);
            Assert.DoesNotContain("\"summary\":", json);
        }

#if NET4
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
