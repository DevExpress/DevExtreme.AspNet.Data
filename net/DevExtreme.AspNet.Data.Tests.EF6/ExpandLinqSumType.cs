using System;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EF6 {
    using DataItem = ExpandLinqSumType_DataItem;

    class ExpandLinqSumType_DataItem : ExpandLinqSumTypeTestHelper.IEntity {
        public int ID { get; set; }
        public int? Int32Prop { get; set; }
        public float? SingleProp { get; set; }
    }

    public class ExpandLinqSumType {

        [Fact]
        public async Task Scenario() {
            await TestDbContext.ExecAsync(context => {
                var dbSet = context.Set<DataItem>();

                dbSet.AddRange(ExpandLinqSumTypeTestHelper.GenerateTestData(() => new DataItem()));
                context.SaveChanges();

                ExpandLinqSumTypeTestHelper.Run(dbSet);
            });
        }

    }

}
