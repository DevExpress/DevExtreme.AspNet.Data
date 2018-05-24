using DevExpress.Xpo;
using System;
using System.Linq;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.Xpo {

    public class Tests {

        [Fact]
        public void DefaultSort_Projection() {
            UnitOfWorkHelper.Exec(uow => {
                new GenericTestEntity(uow) { Oid = MakeGuid('a'), Text = "a" };
                new GenericTestEntity(uow) { Oid = MakeGuid('c'), Text = "c" };
                new GenericTestEntity(uow) { Oid = MakeGuid('b'), Text = "b" };
                uow.CommitChanges();

                var projection = uow.Query<GenericTestEntity>().Select(i => new { i.Text });
                var loadResult = DataSourceLoader.Load(projection, new SampleLoadOptions {
                    Skip = 1,
                    Take = 1
                });

                dynamic data = loadResult.data.Cast<object>().ToArray();
                Assert.Equal("b", data[0].Text);
            });
        }

        Guid MakeGuid(char ch) {
            return new Guid("".PadLeft(32, ch));
        }
    }

}
