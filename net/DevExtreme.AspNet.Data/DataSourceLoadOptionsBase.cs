using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data {

    /// <summary>
    /// A class with properties that specify data processing settings.
    /// </summary>
    public class DataSourceLoadOptionsBase {
        /// <summary>
        /// A flag indicating whether the total number of data objects is required.
        /// </summary>
        public bool RequireTotalCount { get; set; }

        /// <summary>
        /// A flag indicating whether the number of top-level groups is required.
        /// </summary>
        public bool RequireGroupCount { get; set; }

        /// <summary>
        /// A flag indicating whether the current query is made to get the total number of data objects.
        /// </summary>
        public bool IsCountQuery { get; set; }

        /// <summary>
        /// The number of data objects to be skipped from the start of the resulting set.
        /// </summary>
        public int Skip { get; set; }

        /// <summary>
        /// The number of data objects to be loaded.
        /// </summary>
        public int Take { get; set; }

        /// <summary>
        /// A sort expression.
        /// </summary>
        public SortingInfo[] Sort { get; set; }

        /// <summary>
        /// A group expression.
        /// </summary>
        public GroupingInfo[] Group { get; set; }

        /// <summary>
        /// A filter expression.
        /// </summary>
        public IList Filter { get; set; }

        /// <summary>
        /// A total summary expression.
        /// </summary>
        public SummaryInfo[] TotalSummary { get; set; }

        /// <summary>
        /// A group summary expression.
        /// </summary>
        public SummaryInfo[] GroupSummary { get; set; }

        /// <summary>
        /// A select expression.
        /// </summary>
        public string[] Select { get; set; }

        /// <summary>
        /// An array of data fields that constrains the <see cref="Select" /> expression.
        /// The effective select expression is the intersection of <see cref="PreSelect" /> and <see cref="Select" />.
        /// </summary>
        public string[] PreSelect { get; set; }

        /// <summary>
        /// A flag that indicates whether the select expression must be executed by the LINQ provider.
        /// If set to false, select is performed in memory.
        /// </summary>
        public bool? RemoteSelect { get; set; }

        /// <summary>
        /// A flag that indicates whether grouping must be executed by the LINQ provider.
        /// If set to false, data is fully loaded and grouped in memory.
        /// </summary>
        public bool? RemoteGrouping { get; set; }

        /// <summary>
        /// An array of primary keys.
        /// </summary>
        public string[] PrimaryKey { get; set; }

        /// <summary>
        /// The data field to be used for sorting by default.
        /// </summary>
        public string DefaultSort { get; set; }

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
