using DevExtreme.AspNet.Data.Aggregation;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {
    public class AggregateRegistryTests {
        private static readonly IAccessor<int> defaultAccessor = new DefaultAccessor<int>();

        [Fact]
        public void ShouldNotReturnUnexistingAggregator() {
            var aggregatorRegistry = new AggregatorRegistry<int>();
            Assert.Null(aggregatorRegistry.CreateAggregator("custom", defaultAccessor));
        }

        [Fact]
        public void ShouldCreateAggregatorByConstructor() {
            var aggregatorRegistry = new AggregatorRegistry<int>();
            aggregatorRegistry.RegisterAggregator<SumAggregator<int>>(AggregateName.SUM);
            var aggregator = aggregatorRegistry.CreateAggregator(AggregateName.SUM, defaultAccessor);
            Assert.NotNull(aggregator);
            Assert.IsType<SumAggregator<int>>(aggregator);
        }

        [Fact]
        public void ShouldCreateAggregatorByFunction() {
            var aggregatorRegistry = new AggregatorRegistry<int>();
            aggregatorRegistry.RegisterAggregator(AggregateName.COUNT, accessor => new CountAggregator<int>(accessor, false));
            var aggregator = aggregatorRegistry.CreateAggregator(AggregateName.COUNT, defaultAccessor);
            Assert.NotNull(aggregator);
            Assert.IsType<CountAggregator<int>>(aggregator);
        }

        [Fact]
        public void ShouldSupportMultipleAggregatorRegistrations() {
            var aggregatorRegistry = new AggregatorRegistry<int>();
            aggregatorRegistry.RegisterAggregator<SumAggregator<int>>("any");
            aggregatorRegistry.RegisterAggregator("any", accessor => new CountAggregator<int>(accessor, false));
            var aggregator = aggregatorRegistry.CreateAggregator("any", defaultAccessor);
            Assert.NotNull(aggregator);
            Assert.IsType<CountAggregator<int>>(aggregator);
        }
    }

}
