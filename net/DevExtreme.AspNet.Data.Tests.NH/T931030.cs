using FluentNHibernate.Mapping;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.NH {

    public class T931030 {

        public class DataItem : T931030_TestHelper.IEntity {
            public virtual int ID { get; set; }
            public virtual int? Value { get; set; }
        }

        public class DataItemMap : ClassMap<DataItem> {
            public DataItemMap() {
                Table(nameof(T931030) + "_" + nameof(DataItem));
                Id(i => i.ID);
                Map(i => i.Value);
            }
        }

        [Fact]
        public async Task Scenario() {
            await SessionFactoryHelper.ExecAsync(session => {
                using(var tx = session.BeginTransaction()) {
                    foreach(var i in T931030_TestHelper.GenerateTestData(() => new DataItem()))
                        session.Save(i);
                    tx.Commit();
                }

                T931030_TestHelper.Run(session.Query<DataItem>());
            });
        }

    }

}
