using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data {

    public abstract class DataSourceLoadOptionsBase {
        public const string
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
        public static T Parse<T>(IDictionary<string, string> values) where T : DataSourceLoadOptionsBase, new() {
            var result = new T();

            if(HasKey(values, KEY_REQUIRE_TOTAL_COUNT))
                result.RequireTotalCount = Convert.ToBoolean(values[KEY_REQUIRE_TOTAL_COUNT]);

            if(HasKey(values, KEY_IS_COUNT_QUERY))
                result.IsCountQuery = Convert.ToBoolean(values[KEY_IS_COUNT_QUERY]);

            if(HasKey(values, KEY_SKIP))
                result.Skip = Convert.ToInt32(values[KEY_SKIP]);

            if(HasKey(values, KEY_TAKE))
                result.Take = Convert.ToInt32(values[KEY_TAKE]);

            if(HasKey(values, KEY_SORT))
                result.Sort = JsonConvert.DeserializeObject<SortingInfo[]>(values[KEY_SORT]);

            if(HasKey(values, KEY_FILTER))
                result.Filter = JsonConvert.DeserializeObject<IList>(values[KEY_FILTER]);

            return result;
        }

        static bool HasKey(IDictionary<string, string> values, string key) {
            return values.ContainsKey(key) && !String.IsNullOrEmpty(values[key]);
        }
    }

}
