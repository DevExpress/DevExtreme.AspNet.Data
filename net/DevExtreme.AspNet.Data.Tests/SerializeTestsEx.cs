#if NEWTONSOFT_TESTS
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class SerializeTestsEx {
        [Fact]
        public void SerializeEmptyOptions() {
            Assert.Equal(@"{""RequireTotalCount"":false,""RequireGroupCount"":false,""IsCountQuery"":false,""IsSummaryQuery"":false,""Skip"":0,""Take"":0,""Sort"":null,""Group"":null,""Filter"":null,""TotalSummary"":null,""GroupSummary"":null,""Select"":null,""PreSelect"":null,""RemoteSelect"":null,""RemoteGrouping"":null,""ExpandLinqSumType"":null,""PrimaryKey"":null,""DefaultSort"":null,""StringToLower"":null,""PaginateViaPrimaryKey"":null,""SortByPrimaryKey"":null,""AllowAsyncOverSync"":false}",
                JsonConvert.SerializeObject(new DataSourceLoadOptionsBase()));
        }

        [Fact]
        public void SerializeDeserializeConvertersAffectedOptions() {
            var loadOptionsStrGroup = @"""Group"":[{""GroupInterval"":""100"",""IsExpanded"":null,""Selector"":""freight"",""Desc"":false}]";
            var loadOptionsStrFilter = @"""Filter"":[[""orderDate"","">="",""2011-12-13T14:15:16""],""and"",[""orderDate"",""<"",""2011-12-13T14:15:17""]]";

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

            var loadOptionsSerialized = JsonConvert.SerializeObject(loadOptions);
            Assert.Contains($"{loadOptionsStrGroup},{loadOptionsStrFilter}", loadOptionsSerialized);

            var loadOptionsDeserialized = JsonConvert.DeserializeObject<DataSourceLoadOptionsBase>(loadOptionsSerialized);
            Assert.Equal("100", loadOptionsDeserialized.Group[0].GroupInterval);
            Assert.Equal("freight", loadOptionsDeserialized.Group[0].Selector);
            Assert.Equal(3, loadOptionsDeserialized.Filter.Count);
            var strFilter0NTSF = (JArray)loadOptionsDeserialized.Filter[0];
            Assert.Equal("orderDate", strFilter0NTSF[0]);
            Assert.Equal(">=", strFilter0NTSF[1]);
            Assert.Equal("2011-12-13T14:15:16", strFilter0NTSF[2]);
        }
    }

}
#endif
