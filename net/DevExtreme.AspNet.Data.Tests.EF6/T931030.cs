using System;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EF6 {
    using DataItem = T931030_DataItem;

    class T931030_DataItem : T931030_TestHelper.IEntity {
        public int ID { get; set; }
        public int? Value { get; set; }
    }

    public class T931030 {

        [Fact]
        public async Task Scenario() {
            await TestDbContext.ExecAsync(context => {
                var dbSet = context.Set<DataItem>();

                dbSet.AddRange(T931030_TestHelper.GenerateTestData(() => new DataItem()));
                context.SaveChanges();

                T931030_TestHelper.Run(dbSet);
            });
        }

    }

}
