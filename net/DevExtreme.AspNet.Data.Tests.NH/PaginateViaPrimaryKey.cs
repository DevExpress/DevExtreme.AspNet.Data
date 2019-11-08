using FluentNHibernate.Mapping;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.NH {

    public class PaginateViaPrimaryKey {

        public class DataItem : PaginateViaPrimaryKeyTestHelper.IDataItem {
            public virtual int K1 { get; set; }
            public virtual long K2 { get; set; }
        }

        public class DataItemMap : ClassMap<DataItem> {
            public DataItemMap() {
                Table(nameof(PaginateViaPrimaryKey) + "_" + nameof(DataItem));
                Id(p => p.K1);
                Map(p => p.K2);
            }
        }

        [Fact]
        public async Task Scenario() {
            await SessionFactoryHelper.ExecAsync(session => {
                foreach(var i in PaginateViaPrimaryKeyTestHelper.CreateTestData<DataItem>())
                    session.Save(i);

                var query = session.Query<DataItem>();
                PaginateViaPrimaryKeyTestHelper.Run(query);
                PaginateViaPrimaryKeyTestHelper.Run(query.Select(i => new { i.K1, i.K2 }));
            });
        }

    }

}
