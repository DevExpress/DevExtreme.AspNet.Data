using DevExpress.Xpo;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.Xpo {

    public class ExpandLinqSumType {

        [Persistent(nameof(ExpandLinqSumType) + "_" + nameof(DataItem))]
        public class DataItem : ExpandLinqSumTypeTestHelper.IEntity {
            [Key(AutoGenerate = true)]
            public int ID { get; set; }
            public int? Int32Prop { get; set; }
            public float? SingleProp { get; set; }
        }

        [Fact]
        public async Task Scenario() {
            await UnitOfWorkHelper.ExecAsync(uow => {
                foreach(var i in ExpandLinqSumTypeTestHelper.GenerateTestData(() => new DataItem()))
                    uow.Save(i);
                uow.CommitChanges();

                ExpandLinqSumTypeTestHelper.Run(uow.Query<DataItem>());
            });
        }

    }

}
