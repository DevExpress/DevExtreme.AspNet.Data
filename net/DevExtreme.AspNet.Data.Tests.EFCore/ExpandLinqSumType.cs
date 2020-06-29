#if !EFCORE1 && !EFCORE2
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EFCore {

    public class ExpandLinqSumType {

        [Table(nameof(ExpandLinqSumType) + "_" + nameof(DataItem))]
        public class DataItem : ExpandLinqSumTypeTestHelper.IEntity {
            public int ID { get; set; }
            public int? Int32Prop { get; set; }
            public float? SingleProp { get; set; }
        }

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
#endif
