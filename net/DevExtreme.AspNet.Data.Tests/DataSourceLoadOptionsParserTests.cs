using DevExtreme.AspNet.Data.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class DataSourceLoadOptionsParserTests {

        [Fact]
        public void AllKeys() {
            var opts = new SampleLoadOptions();
            var values = new Dictionary<string, string> {
                { DataSourceLoadOptionsParser.KEY_IS_COUNT_QUERY, "true" },
                { DataSourceLoadOptionsParser.KEY_REQUIRE_TOTAL_COUNT, "true" },
                { DataSourceLoadOptionsParser.KEY_SKIP, "42" },
                { DataSourceLoadOptionsParser.KEY_TAKE, "43" },
                { DataSourceLoadOptionsParser.KEY_SORT, @"[ { ""selector"": ""foo"", ""desc"": true } ]" },
                { DataSourceLoadOptionsParser.KEY_FILTER, @" [ ""foo"", ""bar"" ] " },
            };

            DataSourceLoadOptionsParser.Parse(opts, key => values[key]);

            Assert.True(opts.IsCountQuery);
            Assert.True(opts.RequireTotalCount);
            Assert.Equal(42, opts.Skip);
            Assert.Equal(43, opts.Take);
            Assert.Equal("foo", opts.Sort[0].Selector);
            Assert.True(opts.Sort[0].Desc);
            Assert.Equal(new[] { "foo", "bar" }, opts.Filter.Cast<string>());
        }

    }

}
