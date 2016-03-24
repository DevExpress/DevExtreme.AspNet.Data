using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data {

    public abstract class DataSourceLoadOptionsBase {
        const string
            KEY_REQUIRE_TOTAL_COUNT = "requireTotalCount",
            KEY_IS_COUNT_QUERY = "isCountQuery",
            KEY_SKIP = "skip",
            KEY_TAKE = "take",
            KEY_SORT = "sort",
            KEY_FILTER = "filter";

        public static readonly string[] ALL_KEYS = new[] {
            KEY_REQUIRE_TOTAL_COUNT,
            KEY_IS_COUNT_QUERY,
            KEY_SKIP,
            KEY_TAKE,
            KEY_SORT,
            KEY_FILTER
        };


        public bool RequireTotalCount { get; set; }
        public bool IsCountQuery { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public SortingInfo[] Sort { get; set; }
        public IList Filter { get; set; }

#warning TODO add test
        public static T Parse<T>(Func<string, string> valueSource) where T : DataSourceLoadOptionsBase, new() {
            var requireTotalCount = valueSource(KEY_REQUIRE_TOTAL_COUNT);
            var isCountQuery = valueSource(KEY_IS_COUNT_QUERY);
            var skip = valueSource(KEY_SKIP);
            var take = valueSource(KEY_TAKE);
            var sort = valueSource(KEY_SORT);
            var filter = valueSource(KEY_FILTER);

            var result = new T();

            if(!String.IsNullOrEmpty(requireTotalCount))
                result.RequireTotalCount = Convert.ToBoolean(requireTotalCount);

            if(!String.IsNullOrEmpty(isCountQuery))
                result.IsCountQuery = Convert.ToBoolean(isCountQuery);

            if(!String.IsNullOrEmpty(skip))
                result.Skip = Convert.ToInt32(skip);

            if(!String.IsNullOrEmpty(take))
                result.Take = Convert.ToInt32(take);

            if(!String.IsNullOrEmpty(sort))
                result.Sort = JsonConvert.DeserializeObject<SortingInfo[]>(sort);

            if(!String.IsNullOrEmpty(filter))
                result.Filter = JsonConvert.DeserializeObject<IList>(filter);

            return result;
        }

    }

}
