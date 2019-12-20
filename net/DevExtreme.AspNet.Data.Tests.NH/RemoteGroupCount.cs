using FluentNHibernate.Mapping;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.NH {

    public class RemoteGroupCount {

        public class DataItem : RemoteGroupCountTestHelper.IEntity {
            public virtual int Id { get; set; }
            public virtual int G1 { get; set; }
            public virtual int G2 { get; set; }
        }

        public class DataItemMap : ClassMap<DataItem> {
            public DataItemMap() {
                Table(nameof(RemoteGroupCount) + "_" + nameof(DataItem));
                Id(i => i.Id);
                Map(i => i.G1);
                Map(i => i.G2);
            }
        }

        [Fact]
        public async Task Scenario() {
            await SessionFactoryHelper.ExecAsync(session => {
                foreach(var i in RemoteGroupCountTestHelper.GenerateTestData(() => new DataItem()))
                    session.Save(i);

                RemoteGroupCountTestHelper.Run(session.Query<DataItem>());
            });
        }

    }

}
