using DevExpress.Xpo;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.Xpo {

    public class RemoteGroupCount {
        [Persistent(nameof(RemoteGroupCount) + "_" + nameof(DataItem))]
        public class DataItem : RemoteGroupCountTestHelper.IEntity {
            [Key(AutoGenerate = true)]
            public int ID { get; set; }
            public int G1 { get; set; }
            public int G2 { get; set; }
        }

        [Fact]
        public async Task Scenario() {
            await UnitOfWorkHelper.ExecAsync(uow => {
                foreach(var i in RemoteGroupCountTestHelper.GenerateTestData(() => new DataItem()))
                    uow.Save(i);
                uow.CommitChanges();

                RemoteGroupCountTestHelper.Run(uow.Query<DataItem>());
            });
        }
    }

}
