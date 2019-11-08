using DevExpress.Xpo;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.Xpo {

    public class Bug339 {

        [Persistent(nameof(Bug339) + "_" + nameof(DataItem))]
        public class DataItem {
            [Key]
            public Guid ID { get; set; }
        }

        [Fact]
        public async Task Scenario() {
            var guid1 = "".PadLeft(32, '1');
            var guid2 = "".PadLeft(32, '2');

            await UnitOfWorkHelper.ExecAsync(uow => {
                uow.Save(new DataItem { ID = new Guid(guid1) });
                uow.Save(new DataItem { ID = new Guid(guid2) });

                uow.CommitChanges();

                var loadResult = DataSourceLoader.Load(uow.Query<DataItem>(), new SampleLoadOptions {
                    Filter = new[] { "ID", "<", guid2 },
                    RequireTotalCount = true
                });

                Assert.Equal(1, loadResult.totalCount);
            });

        }

    }

}
