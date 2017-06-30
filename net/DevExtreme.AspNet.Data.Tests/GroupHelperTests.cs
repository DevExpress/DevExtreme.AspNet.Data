using DevExtreme.AspNet.Data.ResponseModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class GroupHelperTests {

        static GroupHelper<T> CreateHelper<T>(IEnumerable<T> any) {
            return new GroupHelper<T>(new DefaultAccessor<T>());
        }

        [Fact]
        public void TwoLevelGrouping() {
            var item_15_1_1 = new {
                Year = 2015,
                Q = 1
            };

            var item_15_1_2 = new {
                Year = 2015,
                Q = 1
            };

            var item_15_2_1 = new {
                Year = 2015,
                Q = 2
            };

            var item_16_1_1 = new {
                Year = 2016,
                Q = 1
            };

            var data = new[] {
                item_15_1_1,
                item_16_1_1,
                item_15_2_1,
                item_15_1_2
            };

            var groups = CreateHelper(data).Group(data, new[] {
                new GroupingInfo { Selector = "Year" },
                new GroupingInfo { Selector = "Q" }
            });

            Assert.Equal(2015, groups[0].key);
            Assert.Equal(2016, groups[1].key);

            var g_2015_1 = groups[0].items[0] as Group;
            var g_2015_2 = groups[0].items[1] as Group;
            var g_2016_1 = groups[1].items[0] as Group;

            Assert.Equal(1, g_2015_1.key);
            Assert.Equal(2, g_2015_2.key);
            Assert.Equal(1, g_2016_1.key);

            Assert.Same(item_15_1_1, g_2015_1.items[0]);
            Assert.Same(item_15_1_2, g_2015_1.items[1]);
            Assert.Same(item_15_2_1, g_2015_2.items[0]);
            Assert.Same(item_16_1_1, g_2016_1.items[0]);
        }

        [Fact]
        public void GroupInterval_Numeric() {
            var data = new[] {
                new { n = (object)1.0 },
                new { n = (object)4 },
                new { n = (object)11M },
            };

            var groups = CreateHelper(data).Group(data, new[] {
                new GroupingInfo { Selector = "n", GroupInterval = "5" }
            });

            // [0, 5)   -   1, 4
            // [5, 10)  -   none
            // [10, 15) -   11

            Assert.Equal(2, groups.Count);

            Assert.Equal(0M, groups[0].key);
            Assert.Equal(10M, groups[1].key);

            Assert.Same(data[0], groups[0].items[0]);
            Assert.Same(data[1], groups[0].items[1]);
            Assert.Same(data[2], groups[1].items[0]);
        }

        [Fact]
        public void GroupInterval_Dates() {
            var data = new[] {
                new { d = new DateTime(2011, 12, 13, 14, 15, 16) }
            };

            var groups = CreateHelper(data).Group(data, new[] {
                new GroupingInfo { Selector = "d", GroupInterval = "year" },
                new GroupingInfo { Selector = "d", GroupInterval = "quarter" },
                new GroupingInfo { Selector = "d", GroupInterval = "month" },
                new GroupingInfo { Selector = "d", GroupInterval = "day" },
                new GroupingInfo { Selector = "d", GroupInterval = "dayOfWeek" },
                new GroupingInfo { Selector = "d", GroupInterval = "hour" },
                new GroupingInfo { Selector = "d", GroupInterval = "minute" },
                new GroupingInfo { Selector = "d", GroupInterval = "second" },
            });

            var g_year = groups[0];
            var g_quarter = g_year.items[0] as Group;
            var g_month = g_quarter.items[0] as Group;
            var g_day = g_month.items[0] as Group;
            var g_dayOfWeek = g_day.items[0] as Group;
            var g_hour = g_dayOfWeek.items[0] as Group;
            var g_minute = g_hour.items[0] as Group;
            var g_second = g_minute.items[0] as Group;

            Assert.Equal(2011, g_year.key);
            Assert.Equal(4, g_quarter.key);
            Assert.Equal(12, g_month.key);
            Assert.Equal(2, g_dayOfWeek.key);
            Assert.Equal(13, g_day.key);
            Assert.Equal(14, g_hour.key);
            Assert.Equal(15, g_minute.key);
            Assert.Equal(16, g_second.key);
        }

        [Fact]
        public void GroupInterval_NullDates() {
            var data = new[] {
                new { d = new DateTime?() },
                new { d = new DateTime?() }
            };

            var groups = CreateHelper(data).Group(data, new[] {
                new GroupingInfo { Selector = "d", GroupInterval = "year", IsExpanded = false },
                new GroupingInfo { Selector = "d", GroupInterval = "month", IsExpanded = false },
                new GroupingInfo { Selector = "d", GroupInterval = "day", IsExpanded = false }
            });

            var g_year = groups[0];
            var g_month = g_year.items[0] as Group;
            var g_day = g_month.items[0] as Group;

            Assert.Null(g_year.key);
            Assert.Null(g_month.key);
            Assert.Null(g_day.key);
        }

        [Fact]
        public void NullKey() {
            var data = new[] {
                null,
                "not null"
            };

            var groups = CreateHelper(data).Group(data, new[] { new GroupingInfo { Selector = "this" } });
            Assert.Null(groups[0].key);
        }

    }

}
