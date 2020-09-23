using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EFCore {

    public class T931030 {

        [Table(nameof(T931030) + "_" + nameof(DataItem))]
        public class DataItem : T931030_TestHelper.IEntity {
            public int ID { get; set; }
            public int? Value { get; set; }
        }

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
