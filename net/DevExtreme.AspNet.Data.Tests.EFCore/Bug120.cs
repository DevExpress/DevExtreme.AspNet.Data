using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EFCore {

    public class Bug120 {

        [Table(nameof(Bug120) + "_" + nameof(DataItem))]
        public class DataItem {
            public long ID { get; set; }
        }

        [Fact]
        public async Task Scenario() {
            await TestDbContext.ExecAsync(context => {
                var dbSet = context.Set<DataItem>();
                var result = DataSourceLoader.Load(dbSet, new SampleLoadOptions { RequireTotalCount = true });
                Assert.Equal(0, result.totalCount);
            });
        }

    }
}
