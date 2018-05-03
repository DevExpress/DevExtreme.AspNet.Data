﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EFCore2 {

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
                var exprLog = new List<string>();

                var loadOptions = new SampleLoadOptions {
                    Group = new[] {
                        new GroupingInfo {
                            Selector = "Group",
                            IsExpanded = false
                        }
                    },
                    ExpressionWatcher = x => exprLog.Add(x.ToString())
                };

                Assert.DoesNotContain(exprLog, i => i.Contains(".GroupBy"));
            });
        }

    }

}
