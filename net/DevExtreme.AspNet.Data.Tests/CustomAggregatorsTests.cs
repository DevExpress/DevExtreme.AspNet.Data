using DevExtreme.AspNet.Data.Aggregation;
using DevExtreme.AspNet.Data.Helpers;
using System;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {
    public class CustomAggregatorsTests {

        [Fact]
        public void ShouldNotReturnUnexistingAggregator() {
            StaticBarrier.Run(delegate {
                Assert.Null(CustomAggregators.CreateAggregator("custom", new DefaultAccessor<int>()));
            });
        }

        [Fact]
        public void ShouldCreateRegisteredAggregator() {
            StaticBarrier.Run(delegate {
                CustomAggregators.RegisterAggregator(AggregateName.SUM, typeof(SumAggregator<>));
                var aggregator = CustomAggregators.CreateAggregator(AggregateName.SUM, new DefaultAccessor<int>());
                Assert.NotNull(aggregator);
                Assert.IsType<SumAggregator<int>>(aggregator);
            });
        }

        [Fact]
        public void ShouldSupportMultipleAggregatorRegistrations() {
            StaticBarrier.Run(delegate {
                CustomAggregators.RegisterAggregator("any", typeof(SumAggregator<>));
                CustomAggregators.RegisterAggregator("any", typeof(MinAggregator<>));
                var aggregator = CustomAggregators.CreateAggregator("any", new DefaultAccessor<int>());
                Assert.NotNull(aggregator);
                Assert.IsType<MinAggregator<int>>(aggregator);
            });
        }
    }
}
