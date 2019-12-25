using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EFCore {

    public class Async {

        [Table(nameof(Async) + "_" + nameof(DataItem))]
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
