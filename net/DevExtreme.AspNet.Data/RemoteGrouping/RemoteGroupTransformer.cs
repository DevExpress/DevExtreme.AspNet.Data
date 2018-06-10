using DevExtreme.AspNet.Data.Aggregation;
using DevExtreme.AspNet.Data.ResponseModel;
using DevExtreme.AspNet.Data.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.RemoteGrouping {

    class RemoteGroupTransformer {

        public static RemoteGroupingResult Run(Type sourceItemType, IEnumerable<AnonType> flatGroups, int groupCount, SummaryInfo[] totalSummary, SummaryInfo[] groupSummary) {
            List<Group> hierGroups = null;

            if(groupCount > 0) {
                hierGroups = new GroupHelper<AnonType>(AnonTypeAccessor.Instance).Group(
                    flatGroups,
                    Enumerable.Range(0, groupCount).Select(i => new GroupingInfo { Selector = AnonType.IndexToField(1 + i) }).ToArray()
                );
            }

            IEnumerable dataToAggregate = hierGroups;
            if(dataToAggregate == null)
                dataToAggregate = flatGroups;

            var fieldIndex = 1 + groupCount;
            var transformedTotalSummary = TransformSummary(totalSummary, ref fieldIndex);
            var transformedGroupSummary = TransformSummary(groupSummary, ref fieldIndex);

            transformedTotalSummary.Add(new SummaryInfo { SummaryType = AggregateName.REMOTE_COUNT });

            var sumFix = new SumFix(sourceItemType, totalSummary, groupSummary);
            var totals = new AggregateCalculator<AnonType>(dataToAggregate, AnonTypeAccessor.Instance, transformedTotalSummary, transformedGroupSummary, sumFix).Run();
            var totalCount = (int)totals.Last();

            totals = totals.Take(totals.Length - 1).ToArray();
            if(totals.Length < 1)
                totals = null;

            return new RemoteGroupingResult {
                Groups = hierGroups,
                Totals = totals,
                TotalCount = totalCount
            };
        }

        static List<SummaryInfo> TransformSummary(SummaryInfo[] original, ref int fieldIndex) {
            var result = new List<SummaryInfo>();
            if(original == null)
                return result;

            for(var originalIndex = 0; originalIndex < original.Length; originalIndex++) {
                var originalType = original[originalIndex].SummaryType;

                if(originalType == AggregateName.COUNT) {
                    result.Add(new SummaryInfo {
                        SummaryType = AggregateName.REMOTE_COUNT
                    });
                } else if(originalType == AggregateName.AVG) {
                    result.Add(new SummaryInfo {
                        SummaryType = AggregateName.REMOTE_AVG,
                        Selector = AnonType.IndexToField(fieldIndex)
                    });
                    fieldIndex += 2;
                } else {
                    result.Add(new SummaryInfo {
                        SummaryType = originalType,
                        Selector = AnonType.IndexToField(fieldIndex)
                    });
                    fieldIndex++;
                }
            }

            return result;
        }

    }

}
