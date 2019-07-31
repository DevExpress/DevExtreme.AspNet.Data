using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public static class AsyncTestHelper {

        public interface IDataItem {
            int Value { get; set; }
        }

        public static IEnumerable<T> CreateTestData<T>(Func<T> itemFactory) where T : IDataItem {
            for(var i = 1; i <= 3; i++) {
                var item = itemFactory();
                item.Value = i;
                yield return item;
            }
        }

        public static async Task RunAsync<T>(IQueryable<T> data) {
            var loadOptions = new SampleLoadOptions {
                RequireTotalCount = true,
                Take = 1,
                RemoteGrouping = true
            };

            {
                var loadResult = await DataSourceLoader.LoadAsync(data, loadOptions);
                Assert.Equal(3, loadResult.totalCount);
                Assert.Single(loadResult.data);
            }

            {
                loadOptions.TotalSummary = new[] {
                    new SummaryInfo { SummaryType = "sum", Selector = nameof(IDataItem.Value) }
                };

                var loadResult = await DataSourceLoader.LoadAsync(data, loadOptions);
                Assert.Equal(6m, loadResult.summary[0]);
            }

            Assert.Contains(loadOptions.ExpressionLog, i => i.EndsWith(".Count()"));
            Assert.Contains(loadOptions.ExpressionLog, i => i.EndsWith(".Take(1)"));
            Assert.Contains(loadOptions.ExpressionLog, i => i.Contains(".GroupBy"));
        }

    }

}
