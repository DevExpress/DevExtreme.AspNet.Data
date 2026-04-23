using DevExtreme.AspNet.Data.ResponseModel;
using DevExtreme.AspNet.Data.Types;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class DynamicClassTests {

        [Fact]
        public void Totals() {
            var summaryInfo = new SummaryInfo {
                Selector = "p",
                SummaryType = "sum"
            };

            var source = new[] {
                new { p = 1 },
                new { p = 2 }
            };

            var loadOptions = new SampleLoadOptions {
                GuardNulls = false,
                RemoteGrouping = true,
                TotalSummary = Enumerable
                    .Repeat(summaryInfo, 123)
                    .ToArray()
            };

            var loadResult = DataSourceLoader.Load(source, loadOptions);

            Assert.Equal(3m, loadResult.summary[122]);

            Assert.Contains(loadOptions.ExpressionLog, line =>
                line.Contains(".Select(g => new <>f__AnonymousType") &&
                line.Contains("I100 = g.Sum(obj => obj.p)")
            );
        }

        [Fact]
        public void GroupSummary() {
            var summaryInfo = new SummaryInfo {
                Selector = "p",
                SummaryType = "sum"
            };

            var loadOptions = new SampleLoadOptions {
                GuardNulls = false,
                RemoteGrouping = true,
                Group = new[] {
                    new GroupingInfo { Selector = "g", IsExpanded = false },
                },
                GroupSummary = Enumerable
                    .Repeat(summaryInfo, 123)
                    .ToArray()
            };

            var source = new[] {
                new { g = 1, p = 1 },
                new { g = 1, p = 2 },
            };

            var loadResult = DataSourceLoader.Load(source, loadOptions);
            var group = loadResult.data.Cast<Group>().First();

            Assert.Equal(3m, group.summary[122]);

            Assert.Contains(loadOptions.ExpressionLog, line =>
                line.Contains(".Select(g => new <>f__AnonymousType") &&
                line.Contains("I100 = g.Sum(obj => obj.p)")
            );
        }

        [Fact]
        public void Select() {
            var select = Enumerable.Repeat("p", 123).ToArray();

            var source = new[] {
                new { p = 1 }
            };

            var loadOptions = new SampleLoadOptions {
                GuardNulls = false,
                RemoteSelect = true,
                Select = select
            };

            var loadResult = DataSourceLoader.Load(source, loadOptions);

            Assert.Contains(loadOptions.ExpressionLog, line =>
                line.Contains(".Select(obj => new <>f__AnonymousType") &&
                line.Contains("I100 = obj.p")
            );
        }

        [Fact]
        public void Select_WithMoreThan32Fields_ReturnsValues() {
            var source = new[] { new { p = 42 } };

            var loadOptions = new SampleLoadOptions {
                GuardNulls = false,
                RemoteSelect = true,
                Select = Enumerable.Repeat("p", 33).ToArray()
            };

            var loadResult = DataSourceLoader.Load(source, loadOptions);
            var item = loadResult.data.Cast<IDictionary<string, object>>().First();

            Assert.Equal(42, item["p"]);
        }

        [Fact]
        public void DynamicClassBridge_Indexer_GetMember_RoundTripsValue() {
            var expectedValue = "test";
            var dynamicType = CallDynamicClassBridgeCreateType(new[] { typeof(string) });
            var instance = Activator.CreateInstance(dynamicType, expectedValue);
            Assert.Equal(expectedValue, CallDynamicClassBridgeGetIndexerMember(instance, 0));
        }

        static MethodInfo GetDynamicClassBridgeMethod(string methodName) {
            var bridgeType = typeof(AnonType).Assembly.GetType("DevExtreme.AspNet.Data.Types.DynamicClassBridge");
            return bridgeType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
        }

        static Type CallDynamicClassBridgeCreateType(Type[] memberTypes) {
            return (Type)GetDynamicClassBridgeMethod("CreateType").Invoke(null, new object[] { memberTypes });
        }

        static object CallDynamicClassBridgeGetIndexerMember(object obj, int index) {
            return GetDynamicClassBridgeMethod("GetMember").Invoke(null, new object[] { obj, index });
        }

    }

}
