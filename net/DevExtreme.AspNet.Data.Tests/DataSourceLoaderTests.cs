using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class DataSourceLoaderTests {

        [Fact]
        public void TotalCount() {
            var data = new[] { 1, 3, 2 };
            var options = new DataSourceLoadOptions {
                Filter = new object[] { "this", "<>", 2 },
                Take = 1,
                IsCountQuery = true
            };

            Assert.Equal(2, new DataSourceLoader().Load(data, options));
        }

        [Fact]
        public void Load_NoRequireTotalCount() {
            var data = new[] { 1, 3, 5, 2, 4 };
            var options = new DataSourceLoadOptions {
                Skip = 1,
                Take = 2,
                Filter = new object[] { "this", "<>", 2 },
                Sort = new[] {
                    new SortingInfo { Selector = "this" }
                },
                RequireTotalCount = false
            };

            Assert.Equal(new[] { 3, 4 }, new DataSourceLoader().Load(data, options) as IEnumerable<int>);
        }

        [Fact]
        public void Load_RequireTotalCount() {
            var data = new[] { 1, 3, 5, 2, 4 };
            var options = new DataSourceLoadOptions {
                Skip = 1,
                Take = 2,
                Filter = new object[] { "this", "<>", 2 },
                Sort = new[] {
                    new SortingInfo { Selector = "this" }
                },
                RequireTotalCount = true
            };

            var result = new DataSourceLoader().Load(data, options) as IDictionary;
            Assert.NotNull(result);

            Assert.Equal(4, result["totalCount"]);
            Assert.Equal(new[] { 3, 4 }, result["data"] as IEnumerable<int>);
        }
    }

}
