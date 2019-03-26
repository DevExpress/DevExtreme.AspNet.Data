using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EF6 {

    public class PaginateViaPrimaryKey_DataItem : PaginateViaPrimaryKeyTestHelper.IDataItem {
        public int K1 { get; set; }
        public long K2 { get; set; }
    }

    public class PaginateViaPrimaryKey {

        [Fact]
        public void Scenario() {
            TestDbContext.Exec(context => {
                var set = context.Set<PaginateViaPrimaryKey_DataItem>();
                set.AddRange(new[] {
                    new PaginateViaPrimaryKey_DataItem { K1 = 1, K2 = 1 },
                    new PaginateViaPrimaryKey_DataItem { K1 = 2, K2 = 2 },
                    new PaginateViaPrimaryKey_DataItem { K1 = 3, K2 = 3 }
                });
                context.SaveChanges();

                PaginateViaPrimaryKeyTestHelper.Run(set);
                PaginateViaPrimaryKeyTestHelper.Run(set.Select(i => new { i.K1, i.K2 }));
            });
        }


    }

}
