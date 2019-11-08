using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EF6 {
    using DataItem = Async_DataItem;

    public class Async_DataItem : AsyncTestHelper.IDataItem {
        public int ID { get; set; }
        public int Value { get; set; }
    }

    public class Async {

        [Fact]
        public async Task Scenario() {
            await TestDbContext.ExecAsync(async context => {
                var set = context.Set<DataItem>();
                set.AddRange(AsyncTestHelper.CreateTestData(() => new DataItem()));
                await context.SaveChangesAsync();

                await AsyncTestHelper.RunAsync(set);
            });
        }

    }

}
