using Newtonsoft.Json;
using System;
using System.Linq;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class PaginateViaPrimaryKeyTests {

        [Fact]
        public void SingleKey() {
            var data = Enumerable.Range(0, 5)
                .Select(i => new { ID = i })
                .ToArray();

            var loadOptions = new SampleLoadOptions {
                SuppressGuardNulls = true,

                PrimaryKey = new[] { "ID" },
                PaginateViaPrimaryKey = true,

                Filter = new[] { "ID", ">", "0" },

                Skip = 2,
                Take = 2
            };

            var loadResult = DataSourceLoader.Load(data, loadOptions);

            Assert.Equal(
                "[{ID:3},{ID:4}]",
                DataToString(loadResult.data)
            );

            var log = loadOptions.ExpressionLog;

            Assert.EndsWith(
                ".Where(obj => (obj.ID > 0))" +
                ".OrderBy(obj => obj.ID)" +
                ".Select(obj => new AnonType`1(I0 = obj.ID))" +
                ".Skip(2).Take(2)",
                log[0]
            );

            Assert.EndsWith(
                ".Where(obj => ((obj.ID == 3) OrElse (obj.ID == 4)))" +
                ".OrderBy(obj => obj.ID)",
                log[1]
            );
        }

        static string DataToString(object data) {
            return JsonConvert.SerializeObject(data).Replace("\"", "");
        }

    }

}
