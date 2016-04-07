using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class GroupHelperTests {

        static GroupHelper<T> CreateHelper<T>(IEnumerable<T> data) {
            return new GroupHelper<T>(data);
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

            var helper = CreateHelper(data);
            var groups = helper.Group("Year", "Q");

            Assert.Equal(2015, groups[0].key);
            Assert.Equal(2016, groups[1].key);

            var g_2015_1 = groups[0].items[0] as DevExtremeGroup;
            var g_2015_2 = groups[0].items[1] as DevExtremeGroup;
            var g_2016_1 = groups[1].items[0] as DevExtremeGroup;

            Assert.Equal(1, g_2015_1.key);
            Assert.Equal(2, g_2015_2.key);
            Assert.Equal(1, g_2016_1.key);

            Assert.Same(item_15_1_1, g_2015_1.items[0]);
            Assert.Same(item_15_1_2, g_2015_1.items[1]);
            Assert.Same(item_15_2_1, g_2015_2.items[0]);
            Assert.Same(item_16_1_1, g_2016_1.items[0]);
        }

    }

}
