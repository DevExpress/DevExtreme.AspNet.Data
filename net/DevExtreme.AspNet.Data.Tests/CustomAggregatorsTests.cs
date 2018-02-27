using DevExtreme.AspNet.Data.Aggregation;
using System;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {
    public class CustomAggregatorsTests {
        private static readonly IAccessor<int> defaultAccessor = new DefaultAccessor<int>();

        [Fact]
        public void ShouldNotReturnUnexistingAggregator() {
            Assert.Null(CustomAggregators.CreateAggregator("custom", defaultAccessor));
        }

        [Fact]
        public void ShouldCreateRegisteredAggregator() {
            CustomAggregators.RegisterAggregator(AggregateName.SUM, typeof(SumAggregator<>));
            var aggregator = CustomAggregators.CreateAggregator(AggregateName.SUM, defaultAccessor);
            Assert.NotNull(aggregator);
            Assert.IsType<SumAggregator<int>>(aggregator);
        }

        [Fact]
        public void ShouldSupportMultipleAggregatorRegistrations() {
            CustomAggregators.RegisterAggregator("any", typeof(SumAggregator<>));
            CustomAggregators.RegisterAggregator("any", typeof(MinAggregator<>));
            var aggregator = CustomAggregators.CreateAggregator("any", defaultAccessor);
            Assert.NotNull(aggregator);
            Assert.IsType<MinAggregator<int>>(aggregator);
        }
    }
}
