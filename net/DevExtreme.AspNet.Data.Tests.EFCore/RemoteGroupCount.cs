#if !EFCORE1
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EFCore {

    public class RemoteGroupCount {

        [Table(nameof(RemoteGroupCount) + "_" + nameof(DataItem))]
        public class DataItem : RemoteGroupCountTestHelper.IEntity {
            public int ID { get; set; }
            public int G1 { get; set; }
            public int G2 { get; set; }
        }

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
#endif
