using System;
using System.Collections.Generic;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EFCore2 {
    using DataItem = RemoteGrouping_DataItem;

    class RemoteGrouping_DataItem {
        public int ID { get; set; }
        public int Group { get; set; }
    }

    public class RemoteGrouping {

        [Fact]
        public void DisabledByDefault() {
            TestDbContext.Exec(context => {
                var dbSet = context.Set<DataItem>();

                var loadOptions = new SampleLoadOptions {
                    Group = new[] {
                        new GroupingInfo {
                            Selector = "Group",
                            IsExpanded = false
                        }
                    }
                };

                DataSourceLoader.Load(dbSet, loadOptions);

                Assert.NotEmpty(loadOptions.ExpressionLog);
                Assert.DoesNotContain(loadOptions.ExpressionLog, i => i.Contains(".GroupBy"));
            });
        }

    }

}
