using DevExtreme.AspNet.Data.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class RemoteGroupCountTestHelper {

        public interface IEntity {
            int G1 { get; set; }
            int G2 { get; set; }
        }

        public static IEnumerable<T> GenerateTestData<T>(Func<T> itemFactory) where T : IEntity {
            T CreateItem(int g1, int g2) {
                var item = itemFactory();
                item.G1 = g1;
                item.G2 = g2;
                return item;
            }
            yield return CreateItem(1, 0);
            yield return CreateItem(2, 1);
            yield return CreateItem(2, 1);
            yield return CreateItem(2, 2);
            yield return CreateItem(3, 0);
        }

        public static void Run<T>(IQueryable<T> data) {
            Run(data, new[] {
                new GroupingInfo { Selector = "G1", IsExpanded = false }
            });
            Run(data, new[] {
                new GroupingInfo { Selector = "G1", IsExpanded = false },
                new GroupingInfo { Selector = "G2", IsExpanded = false }
            });
        }

        static void Run<T>(IQueryable<T> data, GroupingInfo[] group) {
            var loadOptions = new SampleLoadOptions {
                RemoteGrouping = true,
                RequireTotalCount = false,
                RequireGroupCount = true,
                Group = group,
                Skip = 1,
                Take = 1
            };

            var loadResult = DataSourceLoader.Load(data, loadOptions);
            Assert.Equal(3, loadResult.groupCount);

            var log = loadOptions.ExpressionLog;

            if(group.Length == 1) {
                Assert.Equal(-1, loadResult.totalCount); // not requested
                Assert.Equal(2, log.Count);
                Assert.Contains(log, line => line.Contains(".Distinct().Count()"));
            } else {
                Assert.Equal(data.Count(), loadResult.totalCount); // bonus because all groups are loaded
                Assert.Single(log);
            }
        }
    }

}
