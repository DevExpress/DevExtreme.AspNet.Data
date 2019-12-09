using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public static class ExpandLinqSumTypeTestHelper {

        public interface IEntity {
            Int32? Int32Prop { get; set; }
            Single? SingleProp { get; set; }
        }

        public static IEnumerable<T> GenerateTestData<T>(Func<T> itemFactory) where T : IEntity {
            for(var i = 0; i < 2; i++) {
                var item = itemFactory();
                item.Int32Prop = Int32.MaxValue;
                item.SingleProp = Single.MaxValue;
                yield return item;
            }
        }

        public static void Run<T>(IQueryable<T> data) {
            var loadResult = DataSourceLoader.Load(data, new SampleLoadOptions {
                RemoteGrouping = true,
                TotalSummary = new[] {
                    new SummaryInfo { SummaryType = "sum", Selector = nameof(IEntity.Int32Prop) },
                    new SummaryInfo { SummaryType = "sum", Selector = nameof(IEntity.SingleProp) }
                }
            });

            var summary = loadResult.summary;

            Assert.Equal(2m * Int32.MaxValue, summary[0]);
            Assert.Equal(2d * Single.MaxValue, summary[1]);
        }
    }

}
