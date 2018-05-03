using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EFCore1 {

    class RemoteGrouping_DataItem {
        public int ID { get; set; }
        public int Group { get; set; }
    }

    partial class TestDbContext {
        public DbSet<RemoteGrouping_DataItem> RemoteGrouping_Data { get; set; }
    }

    public class RemoteGrouping {

        [Fact]
        public void DisabledByDefault() {
            TestDbContext.Exec(context => {
                var dbSet = context.RemoteGrouping_Data;

                var loadOptions = new SampleLoadOptions {
                    Group = new[] {
                        new GroupingInfo {
                            Selector = "Group",
                            IsExpanded = false
                        }
                    }
                };

                Assert.DoesNotContain(loadOptions.ExpressionLog, i => i.Contains(".GroupBy"));
            });
        }

    }

}
