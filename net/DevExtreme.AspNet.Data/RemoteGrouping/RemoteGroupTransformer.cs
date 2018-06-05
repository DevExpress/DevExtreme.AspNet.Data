using DevExtreme.AspNet.Data.Aggregation;
using DevExtreme.AspNet.Data.Helpers;
using DevExtreme.AspNet.Data.ResponseModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.RemoteGrouping {

    class RemoteGroupTransformer {

        public static RemoteGroupingResult Run(IEnumerable<object> flatGroups, int groupCount, SummaryInfo[] totalSummary, SummaryInfo[] groupSummary) {
            List<Group> hierGroups = null;

            if(groupCount > 0) {
                hierGroups = new GroupHelper<object>(TupleUtils.ACCESSOR).Group(
                    flatGroups,
                    Enumerable.Range(1, groupCount).Select(i => new GroupingInfo { Selector = i.ToString() }).ToArray()
                );
            }

            IEnumerable dataToAggregate = hierGroups;
            if(dataToAggregate == null)
                dataToAggregate = flatGroups;

            var fieldIndex = 1 + groupCount;
            var transformedTotalSummary = TransformSummary(totalSummary, ref fieldIndex);
            var transformedGroupSummary = TransformSummary(groupSummary, ref fieldIndex);

            transformedTotalSummary.Add(new SummaryInfo { SummaryType = AggregateName.REMOTE_COUNT });

            var totals = new AggregateCalculator<object>(dataToAggregate, TupleUtils.ACCESSOR, transformedTotalSummary, transformedGroupSummary).Run();
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
                        Selector = fieldIndex.ToString()
                    });
                    fieldIndex += 2;
                } else {
                    result.Add(new SummaryInfo {
                        SummaryType = originalType,
                        Selector = fieldIndex.ToString()
                    });
                    fieldIndex++;
                }
            }

            return result;
        }

    }

}
