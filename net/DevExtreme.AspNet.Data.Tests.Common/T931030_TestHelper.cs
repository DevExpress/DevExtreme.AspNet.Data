using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public static class T931030_TestHelper {

        public interface IEntity {
            int? Value { get; set; }
        }

        public static IEnumerable<T> GenerateTestData<T>(Func<T> itemFactory) where T : IEntity {
            foreach(var i in new int?[] { null, 1, 2 }) {
                var item = itemFactory();
                item.Value = i;
                yield return item;
            }
        }

        public static void Run<T>(IQueryable<T> source) {
            var loadResult = DataSourceLoader.Load(source, new SampleLoadOptions {
                IsCountQuery = true,
                Filter = new object[] {
                    "!",
                    new object[] {
                        new object[] { "Value", 1 },
                        "or",
                        new object[] { "Value", 2 },
                    }
                }
            });

            Assert.Equal(1, loadResult.totalCount);
        }
    }

}
