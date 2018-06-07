using DevExpress.Xpo;
using System;
using System.Linq;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.Xpo {

    public class DefaultSort {

        [Persistent(nameof(DefaultSort) + "_" + nameof(DataItem))]
        public class DataItem : XPLiteObject {
            int _id;
            string _text;

            public DataItem(Session session)
                : base(session) {
            }

            [Key(AutoGenerate = false)]
            public int ID {
                get { return _id; }
                set { SetPropertyValue(nameof(ID), ref _id, value); }
            }

            public string Text {
                get { return _text; }
                set { SetPropertyValue(nameof(Text), ref _text, value); }
            }
        }

        [Fact]
        public void Scenario() {
            UnitOfWorkHelper.Exec(uow => {
                new DataItem(uow) { ID = 1, Text = "1" };
                new DataItem(uow) { ID = 3, Text = "3" };
                new DataItem(uow) { ID = 2, Text = "2" };
                uow.CommitChanges();

                {
                    var loadResult = DataSourceLoader.Load(uow.Query<DataItem>(), new SampleLoadOptions {
                        Skip = 1,
                        Take = 1
                    });

                    var data = loadResult.data.Cast<DataItem>().ToArray();
                    Assert.Equal(2, data[0].ID);
                }

                {
                    var projection = uow.Query<DataItem>().Select(i => new { i.Text });
                    var loadResult = DataSourceLoader.Load(projection, new SampleLoadOptions {
                        Skip = 1,
                        Take = 1
                    });

                    dynamic data = loadResult.data.Cast<object>().ToArray();
                    Assert.Equal("2", data[0].Text);
                }
            });

        }

    }

}
