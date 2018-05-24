using DevExpress.Xpo;
using System;
using System.Linq;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.Xpo {

    public class Tests {

        [Fact]
        public void DefaultSort_Projection() {
            UnitOfWorkHelper.Exec(uow => {
                new GenericTestEntity(uow) { Text = "a" };
                new GenericTestEntity(uow) { Text = "c" };
                new GenericTestEntity(uow) { Text = "b" };
                uow.CommitChanges();

                var projection = uow.Query<GenericTestEntity>().Select(i => new { i.Text });
                var loadResult = DataSourceLoader.Load(projection, new SampleLoadOptions {
                    Skip = 1,
                    Take = 1
                });

                var data = loadResult.data.Cast<object>().ToArray();
#warning TODO
            });
        }
    }

}
