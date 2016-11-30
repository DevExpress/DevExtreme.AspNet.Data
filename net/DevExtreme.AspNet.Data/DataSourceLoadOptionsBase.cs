﻿using System;
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
        public string DefaultSort;

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

        internal bool HasDefaultSort {
            get { return !String.IsNullOrEmpty(DefaultSort); }
        }

        internal bool HasSummary {
            get { return TotalSummary != null && TotalSummary.Length > 0 || GroupSummary != null && GroupSummary.Length > 0; }
        }

        internal bool HasAnySort {
            get { return HasGroups || HasSort || HasPrimaryKey || HasDefaultSort; }
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

            if(HasDefaultSort) {
                if(!result.Any(i => i.Selector == DefaultSort))
                    result.Add(new SortingInfo { Selector = DefaultSort });
            }

            if(HasPrimaryKey)
                return Utils.AddRequiredSort(result, PrimaryKey);

            return result;
        }
    }

}
