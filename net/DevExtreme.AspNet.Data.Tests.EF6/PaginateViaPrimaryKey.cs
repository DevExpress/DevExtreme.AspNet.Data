using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EF6 {

    public class PaginateViaPrimaryKey_DataItem : PaginateViaPrimaryKeyTestHelper.IDataItem {
        public int K1 { get; set; }
        public long K2 { get; set; }
    }

    public class PaginateViaPrimaryKey {

        [Fact]
        public async Task Scenario() {
            await TestDbContext.ExecAsync(context => {
                var set = context.Set<PaginateViaPrimaryKey_DataItem>();
                foreach(var i in PaginateViaPrimaryKeyTestHelper.CreateTestData<PaginateViaPrimaryKey_DataItem>())
                    set.Add(i);
                context.SaveChanges();

                PaginateViaPrimaryKeyTestHelper.Run(set);
                PaginateViaPrimaryKeyTestHelper.Run(set.Select(i => new { i.K1, i.K2 }));
            });
        }

    }

}
