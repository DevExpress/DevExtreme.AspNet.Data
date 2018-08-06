using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data {

    /// <summary>
    /// An abstract class with fields that specify data processing settings.
    /// </summary>
    public abstract class DataSourceLoadOptionsBase {
        /// <summary>
        /// A flag indicating whether the total number of data objects is required.
        /// </summary>
        public bool RequireTotalCount;

        /// <summary>
        /// A flag indicating whether the number of top-level groups is required.
        /// </summary>
        public bool RequireGroupCount;

        /// <summary>
        /// A flag indicating whether the current query is made to get the total number of data objects.
        /// </summary>
        public bool IsCountQuery;

        /// <summary>
        /// The number of data objects to be skipped from the start of the resulting set.
        /// </summary>
        public int Skip;

        /// <summary>
        /// The number of data objects to be loaded.
        /// </summary>
        public int Take;

        /// <summary>
        /// A sort expression.
        /// </summary>
        public SortingInfo[] Sort;

        /// <summary>
        /// A group expression.
        /// </summary>
        public GroupingInfo[] Group;

        /// <summary>
        /// A filter expression.
        /// </summary>
        public IList Filter;

        /// <summary>
        /// A total summary expression.
        /// </summary>
        public SummaryInfo[] TotalSummary;

        /// <summary>
        /// A group summary expression.
        /// </summary>
        public SummaryInfo[] GroupSummary;

        /// <summary>
        /// A select expression.
        /// </summary>
        public string[] Select;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public string[] PreSelect;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool? RemoteSelect;

        /// <summary>
        /// A flag that indicates whether grouping parameters must be translated into LINQ.
        /// </summary>
        public bool? RemoteGrouping;

        /// <summary>
        /// An array of primary keys.
        /// </summary>
        public string[] PrimaryKey;

        /// <summary>
        /// The data field to be used for sorting by default.
        /// </summary>
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

        internal bool HasAnySelect {
            get { return HasPreSelect || HasSelect; }
        }

        internal bool HasPreSelect {
            get { return PreSelect != null && PreSelect.Length > 0; }
        }

        internal bool HasSelect {
            get { return Select != null && Select.Length > 0; }
        }

        internal bool UseRemoteSelect {
            get { return RemoteSelect.GetValueOrDefault(true); }
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

        internal IEnumerable<string> GetFullSelect() {
            if(HasPreSelect && HasSelect)
                return Enumerable.Intersect(PreSelect, Select);

            if(HasPreSelect)
                return PreSelect;

            if(HasSelect)
                return Select;

            return Enumerable.Empty<string>();
        }
    }

}
