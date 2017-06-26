using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Helpers {

    public static class DataSourceLoadOptionsParser {
        public const string
            KEY_REQUIRE_TOTAL_COUNT = "requireTotalCount",
            KEY_REQUIRE_GROUP_COUNT = "requireGroupCount",
            KEY_IS_COUNT_QUERY = "isCountQuery",
            KEY_SKIP = "skip",
            KEY_TAKE = "take",
            KEY_SORT = "sort",
            KEY_GROUP = "group",
            KEY_FILTER = "filter",
            KEY_TOTAL_SUMMARY = "totalSummary",
            KEY_GROUP_SUMMARY = "groupSummary",
            KEY_SELECT = "select";

        public static void Parse(DataSourceLoadOptionsBase loadOptions, Func<string, string> valueSource) {
            var requireTotalCount = valueSource(KEY_REQUIRE_TOTAL_COUNT);
            var requireGroupCount = valueSource(KEY_REQUIRE_GROUP_COUNT);
            var isCountQuery = valueSource(KEY_IS_COUNT_QUERY);
            var skip = valueSource(KEY_SKIP);
            var take = valueSource(KEY_TAKE);
            var sort = valueSource(KEY_SORT);
            var group = valueSource(KEY_GROUP);
            var filter = valueSource(KEY_FILTER);
            var totalSummary = valueSource(KEY_TOTAL_SUMMARY);
            var groupSummary = valueSource(KEY_GROUP_SUMMARY);
            var select = valueSource(KEY_SELECT);

            if(!String.IsNullOrEmpty(requireTotalCount))
                loadOptions.RequireTotalCount = Convert.ToBoolean(requireTotalCount);

            if(!String.IsNullOrEmpty(requireGroupCount))
                loadOptions.RequireGroupCount = Convert.ToBoolean(requireGroupCount);

            if (!String.IsNullOrEmpty(isCountQuery))
                loadOptions.IsCountQuery = Convert.ToBoolean(isCountQuery);

            if(!String.IsNullOrEmpty(skip))
                loadOptions.Skip = Convert.ToInt32(skip);

            if(!String.IsNullOrEmpty(take))
                loadOptions.Take = Convert.ToInt32(take);

            if(!String.IsNullOrEmpty(sort))
                loadOptions.Sort = JsonConvert.DeserializeObject<SortingInfo[]>(sort);

            if(!String.IsNullOrEmpty(group))
                loadOptions.Group = JsonConvert.DeserializeObject<GroupingInfo[]>(group);

            if(!String.IsNullOrEmpty(filter)) {
                loadOptions.Filter = JsonConvert.DeserializeObject<IList>(filter, new JsonSerializerSettings {
                    DateParseHandling = DateParseHandling.None
                });
            }

            if(!String.IsNullOrEmpty(totalSummary))
                loadOptions.TotalSummary = JsonConvert.DeserializeObject<SummaryInfo[]>(totalSummary);

            if(!String.IsNullOrEmpty(groupSummary))
                loadOptions.GroupSummary = JsonConvert.DeserializeObject<SummaryInfo[]>(groupSummary);

            if(!String.IsNullOrEmpty(select))
                loadOptions.Select = JsonConvert.DeserializeObject<string[]>(select);
        }
    }

}
