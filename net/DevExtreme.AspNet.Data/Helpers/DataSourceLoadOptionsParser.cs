using System;
using System.Collections;
using System.Text.Json;

namespace DevExtreme.AspNet.Data.Helpers {

    /// <summary>
    /// A parser for the data processing settings.
    /// </summary>
    public static class DataSourceLoadOptionsParser {
        static readonly JsonSerializerOptions DEFAULT_SERIALIZER_OPTIONS = new JsonSerializerOptions(JsonSerializerDefaults.Web);

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

        /// <summary>
        /// Converts the string representations of the data processing settings to equivalent values of appropriate types.
        /// </summary>
        /// <param name="loadOptions">An object that will contain converted values.</param>
        /// <param name="valueSource">A function that accepts names of the data source options (such as "filter", "sort", etc.) and returns corresponding values.</param>
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

            if(!String.IsNullOrEmpty(isCountQuery))
                loadOptions.IsCountQuery = Convert.ToBoolean(isCountQuery);

            if(!String.IsNullOrEmpty(skip))
                loadOptions.Skip = Convert.ToInt32(skip);

            if(!String.IsNullOrEmpty(take))
                loadOptions.Take = Convert.ToInt32(take);

            if(!String.IsNullOrEmpty(sort))
                loadOptions.Sort = JsonSerializer.Deserialize<SortingInfo[]>(sort, DEFAULT_SERIALIZER_OPTIONS);

            if(!String.IsNullOrEmpty(group))
                loadOptions.Group = JsonSerializer.Deserialize<GroupingInfo[]>(group, DEFAULT_SERIALIZER_OPTIONS);

            if(!String.IsNullOrEmpty(filter)) {
                loadOptions.Filter = JsonSerializer.Deserialize<IList>(filter, new JsonSerializerOptions {
                    //TODO:
                    //DateParseHandling = DateParseHandling.None
                });
            }

            if(!String.IsNullOrEmpty(totalSummary))
                loadOptions.TotalSummary = JsonSerializer.Deserialize<SummaryInfo[]>(totalSummary, DEFAULT_SERIALIZER_OPTIONS);

            if(!String.IsNullOrEmpty(groupSummary))
                loadOptions.GroupSummary = JsonSerializer.Deserialize<SummaryInfo[]>(groupSummary, DEFAULT_SERIALIZER_OPTIONS);

            if(!String.IsNullOrEmpty(select))
                loadOptions.Select = JsonSerializer.Deserialize<string[]>(select);
        }
    }

}
