using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Helpers {

    public static class DataSourceLoadOptionsParser {
        public const string
            KEY_REQUIRE_TOTAL_COUNT = "requireTotalCount",
            KEY_IS_COUNT_QUERY = "isCountQuery",
            KEY_SKIP = "skip",
            KEY_TAKE = "take",
            KEY_SORT = "sort",
            KEY_FILTER = "filter";

#warning TODO add test
        public static void Parse(DataSourceLoadOptionsBase loadOptions, Func<string, string> valueSource) {
            var requireTotalCount = valueSource(KEY_REQUIRE_TOTAL_COUNT);
            var isCountQuery = valueSource(KEY_IS_COUNT_QUERY);
            var skip = valueSource(KEY_SKIP);
            var take = valueSource(KEY_TAKE);
            var sort = valueSource(KEY_SORT);
            var filter = valueSource(KEY_FILTER);

            if(!String.IsNullOrEmpty(requireTotalCount))
                loadOptions.RequireTotalCount = Convert.ToBoolean(requireTotalCount);

            if(!String.IsNullOrEmpty(isCountQuery))
                loadOptions.IsCountQuery = Convert.ToBoolean(isCountQuery);

            if(!String.IsNullOrEmpty(skip))
                loadOptions.Skip = Convert.ToInt32(skip);

            if(!String.IsNullOrEmpty(take))
                loadOptions.Take = Convert.ToInt32(take);

            if(!String.IsNullOrEmpty(sort))
                loadOptions.Sort = JsonConvert.DeserializeObject<SortingInfo[]>(sort);

            if(!String.IsNullOrEmpty(filter))
                loadOptions.Filter = JsonConvert.DeserializeObject<IList>(filter);
        }
    }

}
