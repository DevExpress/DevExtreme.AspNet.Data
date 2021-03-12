using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class IsSummaryQueryTests {

        [Theory]
        [MemberData(nameof(CombinatorialBool), 4)]
        public void Case(bool remoteGrouping, bool requireTotalCount, bool hasGroups, bool hasSummary) {
            var loadOptions = new SampleLoadOptions {
                IsSummaryQuery = true,
                RemoteGrouping = remoteGrouping,
                RequireTotalCount = requireTotalCount,
                Filter = new[] { "this", "<", "100" }
            };
            if(hasGroups) {
                loadOptions.Group = new[] {
                    new GroupingInfo { Selector = "any", IsExpanded = !remoteGrouping }
                };
                loadOptions.GroupSummary = new[] {
                    new SummaryInfo { Selector = "any", SummaryType = "any" }
                };
            }
            if(hasSummary) {
                loadOptions.TotalSummary = new[] {
                    new SummaryInfo { Selector = "this", SummaryType = "sum" }
                };
            }

            var loadResult = DataSourceLoader.Load(new[] { 1, 1, 2, 2, 100 }, loadOptions);

            Assert.Null(loadResult.data);

            if(hasSummary)
                Assert.Equal(6m, loadResult.summary[0]);
            else
                Assert.Null(loadResult.summary);

            // Refer to RemoteGroupExpressionCompiler.MakeAggregatingProjection
            var implicitTotalCount = hasSummary && remoteGrouping;

            if(requireTotalCount || implicitTotalCount)
                Assert.True(loadResult.totalCount > 0);
            else
                Assert.Equal(-1, loadResult.totalCount);

            var expectedExpressionCount = 0;

            if(requireTotalCount && !implicitTotalCount) {
                expectedExpressionCount++;
                Assert.Contains(loadOptions.ExpressionLog, line => line.EndsWith(".Where(obj => (obj < 100)).Count()"));
            }

            if(hasSummary) {
                expectedExpressionCount++;
                if(remoteGrouping)
                    Assert.Contains(loadOptions.ExpressionLog, line => line.Contains(".GroupBy(obj => new AnonType())"));
                else
                    Assert.Contains(loadOptions.ExpressionLog, line => line.EndsWith(".Where(obj => (obj < 100))"));
            }

            Assert.Equal(expectedExpressionCount, loadOptions.ExpressionLog.Count);
        }

        public static IEnumerable<object[]> CombinatorialBool(int count) {
            var combinationCount = 1 << count;
            for(var i = 0; i < combinationCount; i++) {
                yield return Enumerable.Range(0, count)
                    .Select(bit => (i & 1 << bit) != 0)
                    .Cast<object>()
                    .ToArray();
            }
        }
    }

}
