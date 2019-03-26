using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class PaginateViaPrimaryKeyTestHelper {

        public interface IDataItem {
            int K1 { get; set; }
            long K2 { get; set; }
        }

        public static IEnumerable<T> CreateTestData<T>() where T : IDataItem, new() {
            for(var i = 1; i <= 3; i++)
                yield return new T { K1 = i, K2 = i };
        }

        public static void Run<T>(IQueryable<T> source) {
            Run(source, new[] { "K1" });
            Run(source, new[] { "K1", "K2" });
        }

        static void Run<T>(IQueryable<T> source, string[] pk) {
            var loadOptions = new SampleLoadOptions {
                PrimaryKey = pk,
                PaginateViaPrimaryKey = true,
                Filter = new[] { "K1", ">", "1" },
                Select = new[] { "K1", "K2" },
                Sort = new[] {
                    new SortingInfo { Selector = "K1" }
                },
                Skip = 1,
                Take = 1,
                RequireTotalCount = true
            };

            var loadResult = DataSourceLoader.Load(source, loadOptions);
            var data = loadResult.data.Cast<IDictionary<string, object>>().ToArray();

            Assert.Single(data);
            Assert.Equal(3, data[0]["K1"]);
        }

    }

}
