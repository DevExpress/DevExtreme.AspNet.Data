using System;
using System.Linq;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.L2S {
    using DataItem = RemoteGroupingStress_DataItem;

    public class RemoteGroupingStress {

        [Fact]
        public void Scenario() {
            TestDataContext.Exec(context => {
                var table = context.RemoteGroupingStress_DataItems;

                table.InsertOnSubmit(new DataItem());
                context.SubmitChanges();

                RemoteGroupingStressHelper.Run(table);
            });
        }

    }

}
