using DevExtreme.AspNet.Data.Aggregation;
using DevExtreme.AspNet.Data.Helpers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {
    public class CustomAggregatorsTests {

        [Fact]
        public async Task ShouldNotReturnUnexistingAggregator() {
            await StaticBarrier.RunAsync(delegate {
                Assert.Null(CustomAggregators.CreateAggregator("custom", new DefaultAccessor<int>()));
            });
        }

        [Fact]
        public async Task ShouldCreateRegisteredAggregator() {
            await StaticBarrier.RunAsync(delegate {
                CustomAggregators.RegisterAggregator(AggregateName.SUM, typeof(SumAggregator<>));
                var aggregator = CustomAggregators.CreateAggregator(AggregateName.SUM, new DefaultAccessor<int>());
                Assert.NotNull(aggregator);
                Assert.IsType<SumAggregator<int>>(aggregator);
            });
        }

        [Fact]
        public async Task ShouldSupportMultipleAggregatorRegistrations() {
            await StaticBarrier.RunAsync(delegate {
                CustomAggregators.RegisterAggregator("any", typeof(SumAggregator<>));
                CustomAggregators.RegisterAggregator("any", typeof(MinAggregator<>));
                var aggregator = CustomAggregators.CreateAggregator("any", new DefaultAccessor<int>());
                Assert.NotNull(aggregator);
                Assert.IsType<MinAggregator<int>>(aggregator);
            });
        }
    }
}
