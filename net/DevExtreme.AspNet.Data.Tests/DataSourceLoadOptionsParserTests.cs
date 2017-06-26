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
                { DataSourceLoadOptionsParser.KEY_REQUIRE_GROUP_COUNT, "true" },
                { DataSourceLoadOptionsParser.KEY_SKIP, "42" },
                { DataSourceLoadOptionsParser.KEY_TAKE, "43" },
                { DataSourceLoadOptionsParser.KEY_SORT, @"[ { ""selector"": ""foo"", ""desc"": true } ]" },
                { DataSourceLoadOptionsParser.KEY_GROUP, @"[ { ""selector"": ""g"" } ]" },
                { DataSourceLoadOptionsParser.KEY_FILTER, @" [ ""foo"", ""bar"" ] " },
                { DataSourceLoadOptionsParser.KEY_TOTAL_SUMMARY, @"[ { ""selector"": ""total"", ""summaryType"": ""min"" } ]" },
                { DataSourceLoadOptionsParser.KEY_GROUP_SUMMARY, @"[ { ""selector"": ""group"", ""summaryType"": ""max"" } ]" },
                { DataSourceLoadOptionsParser.KEY_SELECT, @"[ ""f1"" ]" }
            };

            DataSourceLoadOptionsParser.Parse(opts, key => values[key]);

            Assert.True(opts.IsCountQuery);
            Assert.True(opts.RequireTotalCount);
            Assert.True(opts.RequireGroupCount);
            Assert.Equal(42, opts.Skip);
            Assert.Equal(43, opts.Take);
            Assert.Equal("foo", opts.Sort[0].Selector);
            Assert.True(opts.Sort[0].Desc);
            Assert.Equal("g", opts.Group[0].Selector);
            Assert.Equal(new[] { "foo", "bar" }, opts.Filter.Cast<string>());

            Assert.Equal("total", opts.TotalSummary[0].Selector);
            Assert.Equal("min", opts.TotalSummary[0].SummaryType);

            Assert.Equal("group", opts.GroupSummary[0].Selector);
            Assert.Equal("max", opts.GroupSummary[0].SummaryType);

            Assert.Equal("f1", opts.Select[0]);
        }

        [Fact]
        public void MustNotParseDates() {
            var opts = new SampleLoadOptions();

            DataSourceLoadOptionsParser.Parse(opts, key => {
                if(key == DataSourceLoadOptionsParser.KEY_FILTER)
                    return @"[ ""d"", ""2011-12-13T14:15:16Z"" ]";
                return null;
            });

            Assert.IsType<string>(opts.Filter[1]);
        }

    }

}
