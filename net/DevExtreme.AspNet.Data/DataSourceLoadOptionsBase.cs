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

        public bool? RemoteGrouping;
        public string[] PrimaryKey;

#if DEBUG
        internal Action<Expression> ExpressionWatcher;
        internal bool UseQueryableOnce;
#endif

        internal bool HasGroups {
            get { return Group != null && Group.Length > 0; }
        }

        internal bool HasSort {
            get { return Sort != null && Sort.Length > 0; }
        }

        internal bool HasPrimaryKey {
            get { return PrimaryKey != null && PrimaryKey.Length > 0; }
        }

        internal bool HasSummary {
            get { return TotalSummary != null && TotalSummary.Length > 0 || GroupSummary != null && GroupSummary.Length > 0; }
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

            if(HasPrimaryKey)
                return Utils.AddRequiredSort(result, PrimaryKey);

            if(result.Count < 1 && HasObsoleteDefaultSort)
                result.Add(new SortingInfo { Selector = ObsoleteDefaultSort });

            return result;
        }

        #region Obsolete

        [Obsolete("DefaultSort is obsolete. Use PrimaryKey instead.")]
        public string DefaultSort {
            get { return ObsoleteDefaultSort; }
            set { ObsoleteDefaultSort = value; }
        }

        internal string ObsoleteDefaultSort;

        internal bool HasObsoleteDefaultSort {
            get { return !String.IsNullOrEmpty(ObsoleteDefaultSort); }
        }

        #endregion
    }

}
