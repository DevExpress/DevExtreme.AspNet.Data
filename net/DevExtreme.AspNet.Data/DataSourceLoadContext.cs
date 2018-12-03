using DevExtreme.AspNet.Data.Aggregation;
using DevExtreme.AspNet.Data.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DevExtreme.AspNet.Data {

    class DataSourceLoadContext {
        readonly DataSourceLoadOptionsBase _options;
        readonly QueryProviderInfo _providerInfo;
        readonly Type _itemType;

        bool _primaryKeyAndDefaultSortEnsured;
        string[] _primaryKey;
        string _defaultSort;

        bool?
            _shouldEmptyGroups,
            _useRemoteGrouping,
            _summaryIsTotalCountOnly;

        public DataSourceLoadContext(DataSourceLoadOptionsBase options, QueryProviderInfo providerInfo, Type itemType) {
            _options = options;
            _providerInfo = providerInfo;
            _itemType = itemType;
        }

        public bool RequireTotalCount => _options.RequireTotalCount;

        public bool RequireGroupCount => _options.RequireGroupCount;

        public bool IsCountQuery => _options.IsCountQuery;

        public int Skip => _options.Skip;

        public int Take => _options.Take;

        public IReadOnlyList<GroupingInfo> Group => _options.Group;

        public IList Filter => _options.Filter;

        public IReadOnlyList<SummaryInfo> TotalSummary => _options.TotalSummary;

        public IReadOnlyList<SummaryInfo> GroupSummary => _options.GroupSummary;

        public bool HasFilter => _options.Filter != null && _options.Filter.Count > 0;

        public bool HasGroups => !Empty(Group);

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

        bool HasSort => !Empty(_options.Sort);

        IReadOnlyList<string> PrimaryKey {
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

        bool HasPrimaryKey => !Empty(PrimaryKey);

        bool HasDefaultSort => !String.IsNullOrEmpty(DefaultSort);

        public bool HasSummary => !Empty(TotalSummary) || HasGroupSummary;

        public bool HasGroupSummary => !Empty(GroupSummary);

        public bool SummaryIsTotalCountOnly {
            get {
                if(!_summaryIsTotalCountOnly.HasValue)
                    _summaryIsTotalCountOnly = !HasGroupSummary && HasSummary && TotalSummary.All(i => i.SummaryType == AggregateName.COUNT);
                return _summaryIsTotalCountOnly.Value;
            }
        }

        public bool HasAnySort => HasGroups || HasSort || HasPrimaryKey || HasDefaultSort;

        public bool HasAnySelect => HasPreSelect || HasSelect;

        bool HasPreSelect => !Empty(_options.PreSelect);

        bool HasSelect => !Empty(_options.Select);

        public bool UseRemoteSelect => _options.RemoteSelect.GetValueOrDefault(true);

        public bool UseStringToLower => _options.StringToLower.GetValueOrDefault(_providerInfo.IsLinqToObjects);

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

            if(HasPrimaryKey)
                requiredSort = requiredSort.Concat(PrimaryKey);

            return Utils.AddRequiredSort(result, requiredSort);
        }

        public IEnumerable<string> GetFullSelect() {
            if(HasPreSelect && HasSelect)
                return Enumerable.Intersect(_options.PreSelect, _options.Select);

            if(HasPreSelect)
                return _options.PreSelect;

            if(HasSelect)
                return _options.Select;

            return Enumerable.Empty<string>();
        }

        void EnsurePrimaryKeyAndDefaultSort() {
            if(_primaryKeyAndDefaultSortEnsured)
                return;

            var primaryKey = _options.PrimaryKey;
            var defaultSort = _options.DefaultSort;

            if(Empty(primaryKey))
                primaryKey = Utils.GetPrimaryKey(_itemType);

            if((Skip > 0 || Take > 0) && String.IsNullOrEmpty(defaultSort) && Empty(primaryKey)) {
                if(_providerInfo.IsEFClassic || _providerInfo.IsEFCore)
                    defaultSort = EFSorting.FindSortableMember(_itemType);
                else if(_providerInfo.IsXPO)
                    defaultSort = "this";
            }

            _primaryKey = primaryKey;
            _defaultSort = defaultSort;
            _primaryKeyAndDefaultSortEnsured = true;
        }

        static bool Empty<T>(IReadOnlyCollection<T> collection) {
            return collection == null || collection.Count < 1;
        }
    }

}
