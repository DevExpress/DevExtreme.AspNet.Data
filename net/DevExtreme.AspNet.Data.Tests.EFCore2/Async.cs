using System;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EFCore2 {

    public class Async {

        public class DataItem : AsyncTestHelper.IDataItem {
            public int ID { get; set; }
            public int Value { get; set; }
        }

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
