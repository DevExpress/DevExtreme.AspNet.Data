using DevExtreme.AspNet.Data.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.L2S {

    public class T670222_UnusedAnonTypeMembers {

        [Fact]
        public void Scenario() {
            TestDataContext.Exec(context => {
                context.PurgeGenericTestTable();

                var table = context.GenericTestDataItems;
                table.InsertOnSubmit(new GenericTestDataItem { Num = 123 });
                context.SubmitChanges();

                var error = Record.Exception(delegate {
                    DataSourceLoader.Load(table, new SampleLoadOptions {
                        Group = new[] {
                        new GroupingInfo { Selector = nameof(GenericTestDataItem.Num), IsExpanded = false },
                        new GroupingInfo { Selector = nameof(GenericTestDataItem.Num), IsExpanded = false },
                        new GroupingInfo { Selector = nameof(GenericTestDataItem.Num), IsExpanded = false }
                    }
                    });
                });

                Assert.Null(error);
            });
        }

    }

}
