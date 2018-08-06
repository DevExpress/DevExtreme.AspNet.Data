using FluentNHibernate.Mapping;
using System;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.NH {

    public class Summary {

        public class DataItem : SummaryTestHelper.IEntity {
            public virtual int Id { get; set; }
            public virtual string Group1 { get; set; }
            public virtual string Group2 { get; set; }
            public virtual int? Value { get; set; }
        }

        public class DataItemMap : ClassMap<DataItem> {
            public DataItemMap() {
                Table(nameof(Summary) + "_" + nameof(DataItem));
                Id(i => i.Id);
                Map(i => i.Group1);
                Map(i => i.Group2);
                Map(i => i.Value);
            }
        }

        [Fact]
        public void Scenario() {
            SessionFactoryHelper.Exec(session => {
                using(var tx = session.BeginTransaction()) {
                    foreach(var i in SummaryTestHelper.GenerateTestData(() => new DataItem()))
                        session.Save(i);
                    tx.Commit();
                }

                SummaryTestHelper.Run(session.Query<DataItem>());
            });
        }

    }

}
