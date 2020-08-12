using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EFCore {

    public class Summary {

        [Table(nameof(Summary) + "_" + nameof(DataItem))]
        public class DataItem : SummaryTestHelper.IEntity {
            public int ID { get; set; }
            public string Group1 { get; set; }
            public string Group2 { get; set; }
            public int? Value { get; set; }
        }

        [Fact]
        public async Task Scenario() {
            await TestDbContext.ExecAsync(context => {
                var dbSet = context.Set<DataItem>();

                dbSet.AddRange(SummaryTestHelper.GenerateTestData(() => new DataItem()));
                context.SaveChanges();

                SummaryTestHelper.Run(dbSet);
            });
        }

    }

}
