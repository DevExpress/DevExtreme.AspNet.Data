using FluentNHibernate.Mapping;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.NH {

    public class Async {

        public class DataItem : AsyncTestHelper.IDataItem {
            public virtual int Id { get; set; }
            public virtual int Value { get; set; }
        }

        public class DataItemMap : ClassMap<DataItem> {
            public DataItemMap() {
                Table(nameof(Async) + "_" + nameof(DataItem));
                Id(p => p.Id);
                Map(p => p.Value);
            }
        }

        [Fact]
        public async Task Scenario() {
            await SessionFactoryHelper.ExecAsync(async session => {
                using(var tx = session.BeginTransaction()) {
                    foreach(var i in AsyncTestHelper.CreateTestData(() => new DataItem()))
                        session.Save(i);
                    await tx.CommitAsync();
                }

                await AsyncTestHelper.RunAsync(session.Query<DataItem>());
            });
        }

    }

}
