using FluentNHibernate.Mapping;
using System;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.NH {

    public class RemoteGroupingStress {

        public class DataItem : RemoteGroupingStressHelper.IEntity {
            public virtual int Id { get; set; }
            public virtual int Num { get; set; }
            public virtual int? NullNum { get; set; }
            public virtual DateTime Date { get; set; }
            public virtual DateTime? NullDate { get; set; }
        }

        public class DataItemMap : ClassMap<DataItem> {
            public DataItemMap() {
                Table(nameof(RemoteGroupingStress) + "_" + nameof(DataItem));
                Id(i => i.Id);
                Map(i => i.Num);
                Map(i => i.NullNum);
                Map(i => i.Date);
                Map(i => i.NullDate);
            }
        }

        [Fact]
        public void Scenario() {
            SessionFactoryHelper.Exec(session => {
                session.Save(new DataItem());
                RemoteGroupingStressHelper.Run(session.Query<DataItem>());
            });
        }

    }

}
