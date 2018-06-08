using DevExpress.Xpo;
using System;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.Xpo {

    public class Summary {

        [Persistent(nameof(Summary) + "_" + nameof(DataItem))]
        public class DataItem : XPLiteObject, SummaryTestHelper.IEntity {
            int _id;
            string _group1, _group2;
            int? _value;

            public DataItem(Session session)
                : base(session) {
            }

            [Key(AutoGenerate = true)]
            public int ID {
                get { return _id; }
                set { SetPropertyValue(nameof(ID), ref _id, value); }
            }

            public string Group1 {
                get { return _group1; }
                set { SetPropertyValue(nameof(Group1), ref _group1, value); }
            }

            public string Group2 {
                get { return _group2; }
                set { SetPropertyValue(nameof(Group2), ref _group2, value); }
            }

            public int? Value {
                get { return _value; }
                set { SetPropertyValue(nameof(Value), ref _value, value); }
            }

        }

        [Fact]
        public void Scenario() {
            UnitOfWorkHelper.Exec(uow => {
                SummaryTestHelper.GenerateTestData(() => new DataItem(uow));
                uow.CommitChanges();

                SummaryTestHelper.Run(uow.Query<DataItem>());
            });
        }

    }
}
