using DevExpress.Xpo;
using System;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.Xpo {

    public class RemoteGroupingStress {

        [Persistent(nameof(RemoteGroupingStress) + "_" + nameof(DataItem))]
        public class DataItem : XPLiteObject, RemoteGroupingStressHelper.IEntity {
            int _id;
            int _num;
            int? _nullNum;
            DateTime _date;
            DateTime? _nullDate;

            public DataItem(Session session)
                : base(session) {
            }

            [Key(AutoGenerate = true)]
            public int ID {
                get { return _id; }
                set { SetPropertyValue(nameof(ID), ref _id, value); }
            }

            public int Num {
                get { return _num; }
                set { SetPropertyValue(nameof(Num), ref _num, value); }
            }

            public int? NullNum {
                get { return _nullNum; }
                set { SetPropertyValue(nameof(NullNum), ref _nullNum, value); }
            }

            public DateTime Date {
                get { return _date; }
                set { SetPropertyValue(nameof(Date), ref _date, value); }
            }

            public DateTime? NullDate {
                get { return _nullDate; }
                set { SetPropertyValue(nameof(NullDate), ref _nullDate, value); }
            }

        }

        [Fact]
        public void Scenario() {
            UnitOfWorkHelper.Exec(uow => {
                new DataItem(uow);
                uow.CommitChanges();

                RemoteGroupingStressHelper.Run(uow.Query<DataItem>());
            });
        }

    }

}
