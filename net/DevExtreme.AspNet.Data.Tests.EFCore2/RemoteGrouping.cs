using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EFCore2 {

    public class RemoteGrouping : IDisposable {

        [Table(nameof(RemoteGrouping) + "_" + nameof(DataItem))]
        public class DataItem {
            public int ID { get; set; }
            public int Group { get; set; }
        }

        public RemoteGrouping() {
            TestDbContext.Exec(context => {
                var dbSet = context.Set<DataItem>();
                dbSet.Add(new DataItem { Group = 1 });
                dbSet.Add(new DataItem { Group = 2 });
                context.SaveChanges();
            });
        }

        public void Dispose() {
            TestDbContext.Exec(context => {
                var dbSet = context.Set<DataItem>();
                dbSet.RemoveRange(dbSet);
                context.SaveChanges();
            });
        }

        [Fact]
        public void EnabledByDefault() {
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
                Assert.Contains(loadOptions.ExpressionLog, i => i.Contains(".GroupBy"));
            });
        }

        [Fact]
        public void TotalSummary() {
            // aka empty group key
            // https://github.com/aspnet/EntityFrameworkCore/issues/11905
            // https://github.com/aspnet/EntityFrameworkCore/issues/11993

            TestDbContext.Exec(context => {
                var dbSet = context.Set<DataItem>();

                var loadOptions = new SampleLoadOptions {
                    TotalSummary = new[] {
                        new SummaryInfo { Selector = "Group", SummaryType = "sum" }
                    }
                };

                Assert.Equal(
                    3m,
                    DataSourceLoader.Load(dbSet, loadOptions).summary[0]
                );

                Assert.Equal(
                    3m,
                    DataSourceLoader.Load(dbSet.Select(i => new { i.ID, i.Group }), loadOptions).summary[0]
                );
            });
        }

    }

}
