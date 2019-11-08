using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EFCore {

    public class RemoteGrouping : IAsyncLifetime {

        [Table(nameof(RemoteGrouping) + "_" + nameof(DataItem))]
        public class DataItem {
            public int ID { get; set; }
            public int Group { get; set; }
        }

        async Task IAsyncLifetime.InitializeAsync() {
            await TestDbContext.ExecAsync(async context => {
                var dbSet = context.Set<DataItem>();
                dbSet.Add(new DataItem { Group = 1 });
                dbSet.Add(new DataItem { Group = 2 });
                await context.SaveChangesAsync();
            });
        }

        async Task IAsyncLifetime.DisposeAsync() {
            await TestDbContext.ExecAsync(async context => {
                var dbSet = context.Set<DataItem>();
                dbSet.RemoveRange(dbSet);
                await context.SaveChangesAsync();
            });
        }

#if EFCORE1

        [Fact]
        public async Task DisabledByDefault() {
            await TestDbContext.ExecAsync(context => {
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

#else

        [Fact]
        public async Task EnabledByDefault() {
            await TestDbContext.ExecAsync(context => {
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
        public async Task TotalSummary() {
            // aka empty group key
            // https://github.com/aspnet/EntityFrameworkCore/issues/11905
            // https://github.com/aspnet/EntityFrameworkCore/issues/11993

            await TestDbContext.ExecAsync(context => {
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

#endif
    }

}
