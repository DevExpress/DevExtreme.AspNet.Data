using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EFCore2 {

    public class PaginateViaPrimaryKey {

        [Table(nameof(PaginateViaPrimaryKey) + "_" + nameof(DataItem))]
        public class DataItem : PaginateViaPrimaryKeyTestHelper.IDataItem {
            public int K1 { get; set; }
            public long K2 { get; set; }
        }

        [Fact]
        public void Scenario() {
            TestDbContext.Exec(context => {
                var set = context.Set<DataItem>();
                set.AddRange(
                    new DataItem { K1 = 1, K2 = 1 },
                    new DataItem { K1 = 2, K2 = 2 },
                    new DataItem { K1 = 3, K2 = 3 }
                );
                context.SaveChanges();

                PaginateViaPrimaryKeyTestHelper.Run(set);
                PaginateViaPrimaryKeyTestHelper.Run(set.Select(i => new { i.K1, i.K2 }));
            });
        }

    }

}
