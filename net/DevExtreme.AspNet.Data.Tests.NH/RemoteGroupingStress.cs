using FluentNHibernate.Mapping;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.NH {

    public class RemoteGroupingStress {

        public class DataItem : RemoteGroupingStressHelper.IEntity {
            public virtual int Id { get; set; }
            public virtual int Num { get; set; }
            public virtual int? NullNum { get; set; }
            public virtual DateTime Date { get; set; }
            public virtual DateTime? NullDate { get; set; }
//#if EFCORE8 || EFCORE9
            // dummy interface implementation
            public virtual DateOnly DateO { get; set; }
//#endif
        }

        public class DataItemMap : ClassMap<DataItem> {
            public DataItemMap() {
                Table(nameof(RemoteGroupingStress) + "_" + nameof(DataItem));
                Id(i => i.Id);
                Map(i => i.Num);
                Map(i => i.NullNum);
                Map(i => i.Date);
                Map(i => i.NullDate);
                //Map(i => i.DateO); //used by all fixtures, requires nh feature support (see skip)
            }
        }
#pragma warning disable xUnit1004 // skip until external / dependency reason is resolved
        [Fact(Skip = "Skip until https://github.com/nhibernate/nhibernate-core/issues/2912 is implemented?")]
#pragma warning restore xUnit1004
        public async Task Scenario() {
            await SessionFactoryHelper.ExecAsync(session => {
                session.Save(new DataItem());
                RemoteGroupingStressHelper.Run(session.Query<DataItem>());
            });
        }

    }

}
