using DevExpress.Xpo;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.Xpo {

    public class T931030 {

        [Persistent(nameof(T931030) + "_" + nameof(DataItem))]
        public class DataItem : T931030_TestHelper.IEntity {
            [Key(AutoGenerate = true)]
            public int ID { get; set; }
            public int? Value { get; set; }
        }

        [Fact]
        public async Task Scenario() {
            await UnitOfWorkHelper.ExecAsync(uow => {
                foreach(var obj in T931030_TestHelper.GenerateTestData(() => new DataItem()))
                    uow.Save(obj);
                uow.CommitChanges();

                T931030_TestHelper.Run(uow.Query<DataItem>());
            });
        }

    }

}
