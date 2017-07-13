using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data {

    public abstract class DataSourceLoadOptionsBase {
        public bool RequireTotalCount;
        public bool RequireGroupCount;
        public bool IsCountQuery;
        public int Skip;
        public int Take;
        public SortingInfo[] Sort;
        public GroupingInfo[] Group;
        public IList Filter;
        public SummaryInfo[] TotalSummary;
        public SummaryInfo[] GroupSummary;
        public string[] Select;

        public bool? RemoteGrouping;
        public string[] PrimaryKey;
        public string DefaultSort;

#if DEBUG
        internal Action<Expression> ExpressionWatcher;
        internal bool UseEnumerableOnce;
#endif

        internal bool HasFilter {
            get { return Filter != null && Filter.Count > 0; }
        }

        internal bool HasGroups {
            get { return Group != null && Group.Length > 0; }
        }

        internal bool HasSort {
            get { return Sort != null && Sort.Length > 0; }
        }

        internal bool HasPrimaryKey {
            get { return PrimaryKey != null && PrimaryKey.Length > 0; }
        }

        internal bool HasDefaultSort {
            get { return !String.IsNullOrEmpty(DefaultSort); }
        }

        internal bool HasSummary {
            get { return TotalSummary != null && TotalSummary.Length > 0 || HasGroupSummary; }
        }

        internal bool HasGroupSummary {
            get { return GroupSummary != null && GroupSummary.Length > 0; }
        }

        internal bool HasAnySort {
            get { return HasGroups || HasSort || HasPrimaryKey || HasDefaultSort; }
        }

        internal bool HasSelect {
            get { return Select != null && Select.Length > 0; }
        }

        internal IEnumerable<SortingInfo> GetFullSort() {
            var memo = new HashSet<string>();
            var result = new List<SortingInfo>();

            if(HasGroups) {
                foreach(var g in Group) {
                    if(memo.Contains(g.Selector))
                        continue;

                    memo.Add(g.Selector);
                    result.Add(g);
                }
            }

            if(HasSort) {
                foreach(var s in Sort) {
                    if(memo.Contains(s.Selector))
                        continue;

                    memo.Add(s.Selector);
                    result.Add(s);
                }
            }

            IEnumerable<string> requiredSort = new string[0];

            if(HasDefaultSort)
                requiredSort = requiredSort.Concat(new[] { DefaultSort });

            if(HasPrimaryKey)
                requiredSort = requiredSort.Concat(PrimaryKey);

            return Utils.AddRequiredSort(result, requiredSort);
        }
    }

}
