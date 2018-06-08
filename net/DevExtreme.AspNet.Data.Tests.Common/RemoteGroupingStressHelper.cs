using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public static class RemoteGroupingStressHelper {
        const string PROP_NUM = nameof(IEntity.Num);
        const string PROP_NULL_NUM = nameof(IEntity.NullNum);
        const string PROP_DATE = nameof(IEntity.Date);
        const string PROP_NULL_DATE = nameof(IEntity.NullDate);

        public interface IEntity {
            int Num { get; }
            int? NullNum { get; }
            DateTime Date { get; }
            DateTime? NullDate { get; }
        }

        public static void Run<T>(IQueryable<T> data) where T : IEntity {
            var loadOptions = new SampleLoadOptions {
                RemoteGrouping = true,
                Group = BuildGroupParams(),
                GroupSummary = BuildSummaryParams()
            };

            Assert.Null(Record.Exception(delegate {
                DataSourceLoader.Load(data, loadOptions);
            }));
        }

        static GroupingInfo[] BuildGroupParams() {
            var list = new List<GroupingInfo> {
                new GroupingInfo { Selector = PROP_NUM },
                new GroupingInfo { Selector = PROP_NULL_NUM },
                new GroupingInfo { Selector = PROP_DATE },
                new GroupingInfo { Selector = PROP_NULL_DATE }
            };

            foreach(var interval in Enumerable.Range(1, 3).Select(i => (100 * i).ToString())) {
                list.Add(new GroupingInfo { Selector = PROP_NUM, GroupInterval = interval });
                list.Add(new GroupingInfo { Selector = PROP_NULL_NUM, GroupInterval = interval });
            }

            foreach(var interval in new[] { "year", "month", "day" }) {
                list.Add(new GroupingInfo { Selector = PROP_DATE, GroupInterval = interval });
                list.Add(new GroupingInfo { Selector = PROP_NULL_DATE, GroupInterval = interval });
            }

            foreach(var item in list)
                item.IsExpanded = false;

            return list.ToArray();
        }

        static SummaryInfo[] BuildSummaryParams() {
            var list = new List<SummaryInfo>();

            foreach(var type in new[] { "count", "min", "max", "sum", "avg" }) {
                list.Add(new SummaryInfo { Selector = PROP_NUM, SummaryType = type });
                list.Add(new SummaryInfo { Selector = PROP_NULL_NUM, SummaryType = type });
            }

            foreach(var type in new[] { "count", "min", "max" }) {
                list.Add(new SummaryInfo { Selector = PROP_DATE, SummaryType = type });
                list.Add(new SummaryInfo { Selector = PROP_NULL_DATE, SummaryType = type });
            }

            return list.ToArray();
        }

    }

}
