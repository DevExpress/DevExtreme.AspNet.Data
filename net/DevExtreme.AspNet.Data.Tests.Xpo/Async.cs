using DevExpress.Xpo;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.Xpo {

    public class Async {

        [Persistent(nameof(Async) + "_" + nameof(DataItem))]
        public class DataItem : AsyncTestHelper.IDataItem {
            [Key(AutoGenerate = true)]
            public int ID { get; set; }
            public int Value { get; set; }
        }

        [Fact]
        public async Task Scenario() {
            await UnitOfWorkHelper.ExecAsync(async uow => {
                foreach(var i in AsyncTestHelper.CreateTestData(() => new DataItem()))
                    uow.Save(i);

                uow.CommitChanges();
                await AsyncTestHelper.RunAsync(uow.Query<DataItem>());
            });
        }

    }

}
