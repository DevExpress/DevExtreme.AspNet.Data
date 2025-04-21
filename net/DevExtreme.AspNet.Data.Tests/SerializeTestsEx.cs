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
            Assert.Equal(loadOptions.Group[0].GroupInterval, loadOptionsDeserialized.Group[0].GroupInterval);
            Assert.Equal(loadOptions.Group[0].Selector, loadOptionsDeserialized.Group[0].Selector);
            Assert.Equal(loadOptions.Filter.Count, loadOptionsDeserialized.Filter.Count);
            var filter0Orig = (IList<object>)loadOptions.Filter[0];
            var filter0NTSF = (JArray)loadOptionsDeserialized.Filter[0];
            Assert.Equal(filter0Orig[0], ((JValue)filter0NTSF[0]).Value);
            Assert.Equal(filter0Orig[1], ((JValue)filter0NTSF[1]).Value);
            Assert.Equal(filter0Orig[2], ((JValue)filter0NTSF[2]).Value);
        }
    }

}
#endif
