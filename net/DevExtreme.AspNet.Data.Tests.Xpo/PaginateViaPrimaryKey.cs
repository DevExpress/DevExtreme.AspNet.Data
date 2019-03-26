using DevExpress.Xpo;
using System;
using System.Linq;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.Xpo {

    public class PaginateViaPrimaryKey {

        [Persistent(nameof(PaginateViaPrimaryKey) + "_" + nameof(DataItem))]
        public class DataItem : PaginateViaPrimaryKeyTestHelper.IDataItem {
            [Key]
            public int K1 { get; set; }
            public long K2 { get; set; }
        }

        [Fact]
        public void Scenario() {
            UnitOfWorkHelper.Exec(uow => {
                uow.Save(new DataItem { K1 = 1, K2 = 1 });
                uow.Save(new DataItem { K1 = 2, K2 = 2 });
                uow.Save(new DataItem { K1 = 3, K2 = 3 });
                uow.CommitChanges();

                var query = uow.Query<DataItem>();
                PaginateViaPrimaryKeyTestHelper.Run(query);
                PaginateViaPrimaryKeyTestHelper.Run(query.Select(i => new { i.K1, i.K2 }));
            });
        }
    }

}
