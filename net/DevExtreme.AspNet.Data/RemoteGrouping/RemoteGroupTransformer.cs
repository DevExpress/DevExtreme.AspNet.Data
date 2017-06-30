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

        public static RemoteGroupingResult Run(IEnumerable<AnonType> flatGroups, int groupCount, SummaryInfo[] totalSummary, SummaryInfo[] groupSummary) {
            var markup = new RemoteGroupTypeMarkup(
                groupCount,
                totalSummary != null ? totalSummary.Length : 0,
                groupSummary != null ? groupSummary.Length : 0
            );

            List<Group> hierGroups = null;

            if(groupCount > 0) {
                hierGroups = new GroupHelper<AnonType>(Accessors.AnonType).Group(
                    flatGroups,
                    Enumerable.Range(0, groupCount).Select(i => new GroupingInfo { Selector = AnonType.ITEM_PREFIX + (RemoteGroupTypeMarkup.KeysStartIndex + i) }).ToArray()
                );
            }

            IEnumerable dataToAggregate = hierGroups;
            if(dataToAggregate == null)
                dataToAggregate = flatGroups;

            var transformedTotalSummary = TransformSummary(totalSummary, markup.TotalSummaryStartIndex);
            var transformedGroupSummary = TransformSummary(groupSummary, markup.GroupSummaryStartIndex);

            transformedTotalSummary.Add(new SummaryInfo { SummaryType = "remoteCount" });

            var totals = new AggregateCalculator<AnonType>(dataToAggregate, Accessors.AnonType, transformedTotalSummary, transformedGroupSummary).Run();
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

        static List<SummaryInfo> TransformSummary(SummaryInfo[] original, int fieldStartIndex) {
            var result = new List<SummaryInfo>();

            if(original != null) {
                for(var i = 0; i < original.Length; i++) {
                    result.Add(new SummaryInfo {
                        Selector = AnonType.ITEM_PREFIX + (fieldStartIndex + i),
                        SummaryType = TransformSummaryType(original[i].SummaryType)
                    });
                }
            }

            return result;
        }

        static string TransformSummaryType(string type) {
            if(type == "count")
                return "remoteCount";

            if(type == "avg")
                return "remoteAvg";

            return type;
        }


    }

}
