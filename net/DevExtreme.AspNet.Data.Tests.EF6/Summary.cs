using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EF6 {
    using DataItem = Summary_DataItem;

    class Summary_DataItem : SummaryTestHelper.IEntity {
        public int ID { get; set; }
        public string Group1 { get; set; }
        public string Group2 { get; set; }
        public int? Value { get; set; }
    }

    public class Summary {

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
