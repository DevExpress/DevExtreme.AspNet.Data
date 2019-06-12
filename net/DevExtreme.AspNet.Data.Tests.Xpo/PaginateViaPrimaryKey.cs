using DevExpress.Xpo;
using System;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task Scenario() {
            await UnitOfWorkHelper.ExecAsync(uow => {
                foreach(var i in PaginateViaPrimaryKeyTestHelper.CreateTestData<DataItem>())
                    uow.Save(i);
                uow.CommitChanges();

                var query = uow.Query<DataItem>();
                PaginateViaPrimaryKeyTestHelper.Run(query);
                PaginateViaPrimaryKeyTestHelper.Run(query.Select(i => new { i.K1, i.K2 }));
            });
        }
    }

}
