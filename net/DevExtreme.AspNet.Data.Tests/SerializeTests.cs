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
            Assert.Equal("100", loadOptionsDeserialized.Group[0].GroupInterval);
            Assert.Equal("freight", loadOptionsDeserialized.Group[0].Selector);
            Assert.Equal(3, loadOptionsDeserialized.Filter.Count);
            var strFilter0STJ = (IList<object>)loadOptionsDeserialized.Filter[0];
            Assert.Equal("orderDate", strFilter0STJ[0]);
            Assert.Equal(">=", strFilter0STJ[1]);
            Assert.Equal("2011-12-13T14:15:16", strFilter0STJ[2]);
        }
    }

}
