using System;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.L2S {
    using DataItem = RemoteGroupCount_DataItem;

    public class RemoteGroupCount {

        [Fact]
        public void Scenario() {
            TestDataContext.Exec(context => {
                var table = context.RemoteGroupCount_DataItems;

                foreach(var i in RemoteGroupCountTestHelper.GenerateTestData(() => new DataItem()))
                    table.InsertOnSubmit(i);
                context.SubmitChanges();

                RemoteGroupCountTestHelper.Run(table);
            });
        }

    }

}
