using System;
using System.Linq;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.L2S {
    using DataItem = Summary_DataItem;

    public class Summary {

        [Fact]
        public void Scenario() {
            TestDataContext.Exec(context => {
                var table = context.Summary_DataItems;

                table.InsertAllOnSubmit(SummaryTestHelper.GenerateTestData(() => new DataItem()));
                context.SubmitChanges();

                SummaryTestHelper.Run(table);
            });
        }

    }

}
