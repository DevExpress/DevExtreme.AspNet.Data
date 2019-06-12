using DevExtreme.AspNet.Data.Aggregation;
using DevExtreme.AspNet.Data.Helpers;
using System;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {
    public class CustomAggregatorsTests {

        [Fact]
        public void ShouldNotReturnUnexistingAggregator() {
            Assert.Null(CustomAggregators.CreateAggregator("custom", new DefaultAccessor<int>()));
        }

        [Fact]
        public void ShouldCreateRegisteredAggregator() {
            try {
                CustomAggregators.RegisterAggregator(AggregateName.SUM, typeof(SumAggregator<>));
                var aggregator = CustomAggregators.CreateAggregator(AggregateName.SUM, new DefaultAccessor<int>());
                Assert.NotNull(aggregator);
                Assert.IsType<SumAggregator<int>>(aggregator);
            } finally {
                CustomAggregators.Clear();
            }
        }

        [Fact]
        public void ShouldSupportMultipleAggregatorRegistrations() {
            try {
                CustomAggregators.RegisterAggregator("any", typeof(SumAggregator<>));
                CustomAggregators.RegisterAggregator("any", typeof(MinAggregator<>));
                var aggregator = CustomAggregators.CreateAggregator("any", new DefaultAccessor<int>());
                Assert.NotNull(aggregator);
                Assert.IsType<MinAggregator<int>>(aggregator);
            } finally {
                CustomAggregators.Clear();
            }
        }
    }
}
