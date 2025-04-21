//using DevExtreme.AspNet.Data.Helpers;

using System;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class SerializeTests {
        [Fact]
        public void SerializeEmptyOptions() {
            Assert.Equal(@"{""RequireTotalCount"":false,""RequireGroupCount"":false,""IsCountQuery"":false,""IsSummaryQuery"":false,""Skip"":0,""Take"":0,""Sort"":null,""Group"":null,""Filter"":null,""TotalSummary"":null,""GroupSummary"":null,""Select"":null,""PreSelect"":null,""RemoteSelect"":null,""RemoteGrouping"":null,""ExpandLinqSumType"":null,""PrimaryKey"":null,""DefaultSort"":null,""StringToLower"":null,""PaginateViaPrimaryKey"":null,""SortByPrimaryKey"":null,""AllowAsyncOverSync"":false}",
                JsonSerializer.Serialize(new DataSourceLoadOptionsBase()));
        }

        [Fact]
        public void SerializeDeserializeConvertersAffectedOptions() {
            var loadOptionsStrGroup = @"""Group"":[{""GroupInterval"":""100"",""IsExpanded"":null,""Selector"":""freight"",""Desc"":false}]";
            var loadOptionsStrFilter = @"""Filter"":[[""orderDate"",""\u003E="",""2011-12-13T14:15:16""],""and"",[""orderDate"",""\u003C"",""2011-12-13T14:15:17""]]";

            var loadOptions = new DataSourceLoadOptionsBase() {
                Group = new GroupingInfo[] {
                    new GroupingInfo() {
                        GroupInterval = "100",
                        Selector = "freight"
                    }
                },
                Filter = new List<object>() {
                    new List<object>() { "orderDate", ">=", new DateTime(2011, 12, 13, 14, 15, 16) },
                    "and",
                    new List<object>() { "orderDate", "<", new DateTime(2011, 12, 13, 14, 15, 17) }
                }
            };

            var loadOptionsSerialized = JsonSerializer.Serialize(loadOptions);
            Assert.Contains($"{loadOptionsStrGroup},{loadOptionsStrFilter}", loadOptionsSerialized);

            var loadOptionsDeserialized = JsonSerializer.Deserialize<DataSourceLoadOptionsBase>(
                loadOptionsSerialized
                //, DataSourceLoadOptionsParser.DEFAULT_SERIALIZER_OPTIONS
                // does not require options, because deserializes from just serialized instance
                );
            Assert.Equal(loadOptions.Group[0].GroupInterval, loadOptionsDeserialized.Group[0].GroupInterval);
            Assert.Equal(loadOptions.Group[0].Selector, loadOptionsDeserialized.Group[0].Selector);
            Assert.Equal(loadOptions.Filter.Count, loadOptionsDeserialized.Filter.Count);
            var filter0Orig = (IList<object>)loadOptions.Filter[0];
            var filter0STJ = (IList<object>)loadOptionsDeserialized.Filter[0];
            Assert.Equal(filter0Orig[0], filter0STJ[0]);
            Assert.Equal(filter0Orig[1], filter0STJ[1]);
            //TODO:
            //https://github.com/dotnet/runtime/issues/31423
            //https://learn.microsoft.com/en-us/dotnet/standard/datetime/system-text-json-support
            //https://github.com/DevExpress/DevExtreme.AspNet.Data/blob/master/js/dx.aspnet.data.js -> serializeDate
            //Assert.Equal(filter0Orig[2], filter0STJ[2]);
            Assert.Equal("2011-12-13T14:15:16", filter0STJ[2]);

        }
    }

}
