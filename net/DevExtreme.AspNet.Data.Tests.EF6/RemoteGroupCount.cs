using System;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EF6 {
    using DataItem = RemoteGroupCount_DataItem;

    public class RemoteGroupCount_DataItem : RemoteGroupCountTestHelper.IEntity {
        public int ID { get; set; }
        public int G1 { get; set; }
        public int G2 { get; set; }
    }

    public class RemoteGroupCount {

        [Fact]
        public async Task Scenario() {
            await TestDbContext.ExecAsync(context => {
                var dbSet = context.Set<DataItem>();

                dbSet.AddRange(RemoteGroupCountTestHelper.GenerateTestData(() => new DataItem()));
                context.SaveChanges();

                RemoteGroupCountTestHelper.Run(dbSet);
            });
        }

    }

}
