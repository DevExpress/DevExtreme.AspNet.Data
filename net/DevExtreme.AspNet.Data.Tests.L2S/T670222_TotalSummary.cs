using System;
using System.Linq;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.L2S {

    public class T670222_TotalSummary {

        [Fact]
        public void Scenario() {
            TestDataContext.Exec(context => {
                context.PurgeGenericTestTable();

                var table = context.GenericTestDataItems;
                table.InsertOnSubmit(new GenericTestDataItem { Num = 1 });
                table.InsertOnSubmit(new GenericTestDataItem { Num = 2 });
                context.SubmitChanges();

                var loadResult = DataSourceLoader.Load(table, new SampleLoadOptions {
                    TotalSummary = new[] {
                        new SummaryInfo { Selector = "Num", SummaryType = "sum" }
                    }
                });

                Assert.Equal(3m, loadResult.summary[0]);
            });
        }

    }

}
