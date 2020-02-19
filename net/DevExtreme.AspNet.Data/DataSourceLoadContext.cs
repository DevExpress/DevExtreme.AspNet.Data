using DevExtreme.AspNet.Data.Aggregation;
using DevExtreme.AspNet.Data.Helpers;
using DevExtreme.AspNet.Data.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DevExtreme.AspNet.Data {

    partial class DataSourceLoadContext {
        readonly DataSourceLoadOptionsBase _options;
        readonly QueryProviderInfo _providerInfo;
        readonly Type _itemType;

        public DataSourceLoadContext(DataSourceLoadOptionsBase options, QueryProviderInfo providerInfo, Type itemType) {
            _options = options;
            _providerInfo = providerInfo;
            _itemType = itemType;
        }

        public bool GuardNulls {
            get {
#if DEBUG
                if(_options.GuardNulls.HasValue)
                    return _options.GuardNulls.Value;
#endif
                return _providerInfo.IsLinqToObjects;
            }
        }

        public bool RequireQueryableChainBreak {
            get {
                if(_providerInfo.IsXPO) {
                    // 1. XPQuery is stateful
                    // 2. CreateQuery(expr) and Execute(expr) don't spawn intermediate query instances for Queryable calls within expr.
                    //    This can put XPQuery into an invalid state. Example: Distinct().Count()
                    // 3. XPQuery is IQueryProvider itself
                    return true;
                }

                return false;
            }
        }

        public AnonTypeNewTweaks CreateAnonTypeNewTweaks() => new AnonTypeNewTweaks {
            AllowEmpty = !_providerInfo.IsL2S,
            AllowUnusedMembers = !_providerInfo.IsL2S
        };

        static bool IsEmpty<T>(IReadOnlyCollection<T> collection) {
            return collection == null || collection.Count < 1;
        }

        static bool IsEmptyList(IList list) {
            return list == null || list.Count < 1;
        }
    }

    // Total count
    partial class DataSourceLoadContext {
        public bool RequireTotalCount => _options.RequireTotalCount;
        public bool IsCountQuery => _options.IsCountQuery;
    }

    // Paging
    partial class DataSourceLoadContext {
        public int Skip => _options.Skip;
        public int Take => _options.Take;
        public bool HasPaging => Skip > 0 || Take > 0;
        public bool PaginateViaPrimaryKey => _options.PaginateViaPrimaryKey.GetValueOrDefault(false);
    }

    // Filter
    partial class DataSourceLoadContext {
        public IList Filter => _options.Filter;
        public bool HasFilter => !IsEmptyList(_options.Filter);
        public bool UseStringToLower => _options.StringToLower ?? DataSourceLoadOptionsBase.StringToLowerDefault ?? _providerInfo.IsLinqToObjects;
    }

    // Grouping
    partial class DataSourceLoadContext {
        bool?
            _shouldEmptyGroups,
            _useRemoteGrouping;

        public bool RequireGroupCount => _options.RequireGroupCount;

        public IReadOnlyList<GroupingInfo> Group => _options.Group;

        public bool HasGroups => !IsEmpty(Group);

        public bool ShouldEmptyGroups {
            get {
                if(!_shouldEmptyGroups.HasValue)
                    _shouldEmptyGroups = HasGroups && !Group.Last().GetIsExpanded();
                return _shouldEmptyGroups.Value;
            }
        }

        public bool UseRemoteGrouping {
            get {

                bool HasAvg(IEnumerable<SummaryInfo> summary) {
                    return summary != null && summary.Any(i => i.SummaryType == "avg");
                }

                bool ShouldUseRemoteGrouping() {
                    if(_providerInfo.IsLinqToObjects)
                        return false;

                    if(_providerInfo.IsEFCore) {
                        // https://github.com/aspnet/EntityFrameworkCore/issues/2341
                        // https://github.com/aspnet/EntityFrameworkCore/issues/11993
                        // https://github.com/aspnet/EntityFrameworkCore/issues/11999
                        if(_providerInfo.Version < new Version(2, 2, 0))
                            return false;

                        #warning Remove with https://github.com/aspnet/EntityFrameworkCore/issues/11711 fix
                        if(HasAvg(TotalSummary) || HasAvg(GroupSummary))
                            return false;
                    }

                    return true;
                }

                if(!_useRemoteGrouping.HasValue)
                    _useRemoteGrouping = _options.RemoteGrouping ?? ShouldUseRemoteGrouping();

                return _useRemoteGrouping.Value;
            }
        }
    }

    // Sorting & Primary Key
    partial class DataSourceLoadContext {
        bool _primaryKeyAndDefaultSortEnsured;
        string[] _primaryKey;
        string _defaultSort;

        public bool HasAnySort => HasGroups || HasSort || ShouldSortByPrimaryKey || HasDefaultSort;

        bool HasSort => !IsEmpty(_options.Sort);

        public IReadOnlyList<string> PrimaryKey {
            get {
                EnsurePrimaryKeyAndDefaultSort();
                return _primaryKey;
            }
        }

        string DefaultSort {
            get {
                EnsurePrimaryKeyAndDefaultSort();
                return _defaultSort;
            }
        }

        public bool HasPrimaryKey => !IsEmpty(PrimaryKey);

        bool HasDefaultSort => !String.IsNullOrEmpty(DefaultSort);

        bool ShouldSortByPrimaryKey => HasPrimaryKey && _options.SortByPrimaryKey.GetValueOrDefault(true);

        public IEnumerable<SortingInfo> GetFullSort() {
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
                foreach(var s in _options.Sort) {
                    if(memo.Contains(s.Selector))
                        continue;

                    memo.Add(s.Selector);
                    result.Add(s);
                }
            }

            IEnumerable<string> requiredSort = new string[0];

            if(HasDefaultSort)
                requiredSort = requiredSort.Concat(new[] { DefaultSort });

            if(ShouldSortByPrimaryKey)
                requiredSort = requiredSort.Concat(PrimaryKey);

            return Utils.AddRequiredSort(result, requiredSort);
        }

        void EnsurePrimaryKeyAndDefaultSort() {
            if(_primaryKeyAndDefaultSortEnsured)
                return;

            var primaryKey = _options.PrimaryKey;
            var defaultSort = _options.DefaultSort;

            if(IsEmpty(primaryKey))
                primaryKey = Utils.GetPrimaryKey(_itemType);

            if(HasPaging && String.IsNullOrEmpty(defaultSort) && IsEmpty(primaryKey)) {
                if(_providerInfo.IsEFClassic || _providerInfo.IsEFCore)
                    defaultSort = EFSorting.FindSortableMember(_itemType);
                else if(_providerInfo.IsXPO)
                    defaultSort = "this";
            }

            _primaryKey = primaryKey;
            _defaultSort = defaultSort;
            _primaryKeyAndDefaultSortEnsured = true;
        }
    }

    // Summary
    partial class DataSourceLoadContext {
        bool? _summaryIsTotalCountOnly;

        public IReadOnlyList<SummaryInfo> TotalSummary => _options.TotalSummary;

        public IReadOnlyList<SummaryInfo> GroupSummary => _options.GroupSummary;

        public bool HasSummary => HasTotalSummary || HasGroupSummary;

        public bool HasTotalSummary => !IsEmpty(TotalSummary);

        public bool HasGroupSummary => !IsEmpty(GroupSummary);

        public bool SummaryIsTotalCountOnly {
            get {
                if(!_summaryIsTotalCountOnly.HasValue)
                    _summaryIsTotalCountOnly = !HasGroupSummary && HasTotalSummary && TotalSummary.All(i => i.SummaryType == AggregateName.COUNT);
                return _summaryIsTotalCountOnly.Value;
            }
        }

        public bool ExpandLinqSumType {
            get {
                if(_options.ExpandLinqSumType.HasValue)
                    return _options.ExpandLinqSumType.Value;

                // NH 5.2.6: https://github.com/nhibernate/nhibernate-core/issues/2029
                // EFCore 2: https://github.com/aspnet/EntityFrameworkCore/issues/14851

                return _providerInfo.IsEFClassic
                    || _providerInfo.IsEFCore && _providerInfo.Version.Major > 2
                    || _providerInfo.IsXPO;
            }
        }
    }

    // Select
    partial class DataSourceLoadContext {
        string[] _fullSelect;
        bool? _useRemoteSelect;

        public bool HasAnySelect => FullSelect.Count > 0;

        public bool UseRemoteSelect {
            get {
                if(!_useRemoteSelect.HasValue)
                    _useRemoteSelect = _options.RemoteSelect ?? (!_providerInfo.IsLinqToObjects && FullSelect.Count <= AnonType.MAX_SIZE);

                return _useRemoteSelect.Value;
            }
        }

        public IReadOnlyList<string> FullSelect {
            get {
                string[] Init() {
                    var hasSelect = !IsEmpty(_options.Select);
                    var hasPreSelect = !IsEmpty(_options.PreSelect);

                    if(hasPreSelect && hasSelect)
                        return Enumerable.Intersect(_options.PreSelect, _options.Select, StringComparer.OrdinalIgnoreCase).ToArray();

                    if(hasPreSelect)
                        return _options.PreSelect;

                    if(hasSelect)
                        return _options.Select;

                    return new string[0];
                }

                if(_fullSelect == null)
                    _fullSelect = Init();

                return _fullSelect;
            }
        }
    }
}
