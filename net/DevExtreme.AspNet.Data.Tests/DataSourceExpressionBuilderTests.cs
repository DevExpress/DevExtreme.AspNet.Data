using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class DataSourceExpressionBuilderTests {

        static DataSourceExpressionBuilder<T> CreateBuilder<T>(DataSourceLoadOptionsBase loadOptions, bool guardNulls = false) {
            return new DataSourceExpressionBuilder<T>(loadOptions, guardNulls);
        }

        [Fact]
        public void Build_SkipTake() {
            var builder = CreateBuilder<int>(new SampleLoadOptions {
                Skip = 111,
                Take = 222
            });

            var expr = builder.BuildLoadExpr();

            Assert.Equal("data.Skip(111).Take(222)", expr.Body.ToString());
        }

        [Fact]
        public void Build_Filter() {
            var builder = CreateBuilder<int>(new SampleLoadOptions {
                Filter = new object[] { "this", ">", 123 }
            });

            var expr = builder.BuildLoadExpr();

            Assert.Equal("data.Where(obj => (obj > 123))", expr.Body.ToString());
        }

        [Fact]
        public void Build_CountQuery() {
            var builder = CreateBuilder<int>(new SampleLoadOptions {
                Skip = 111,
                Take = 222,
                Filter = new object[] { "this", 123 },
                Sort = new[] {
                    new SortingInfo { Selector = "this" }
                },
            });

            var expr = builder.BuildCountExpr();
            var text = expr.ToString();

            Assert.Contains("Where", text);
            Assert.DoesNotContain("Skip", text);
            Assert.DoesNotContain("Take", text);
            Assert.DoesNotContain("OrderBy", text);
            Assert.EndsWith(".Count()", text);
        }

        [Fact]
        public void Build_Sorting() {
            var builder = CreateBuilder<Tuple<int, string>>(new SampleLoadOptions {
                Sort = new[] {
                    new SortingInfo {
                        Selector="Item1"
                    },
                    new SortingInfo {
                        Selector = "Item2",
                        Desc=true
                    }
                }
            });

            var expr = builder.BuildLoadExpr();
            Assert.Equal("data.OrderBy(obj => obj.Item1).ThenByDescending(obj => obj.Item2)", expr.Body.ToString());
        }

        [Fact]
        public void GroupingAddedToSorting() {
            var loadOptions = new SampleLoadOptions {
                Sort = new[] {
                    new SortingInfo { Selector = "Item2" },
                    new SortingInfo { Selector = "Item3" }
                },
                Group = new[] {
                    new GroupingInfo { Selector = "Item1" },
                    new GroupingInfo { Selector = "Item2", Desc = true  } // this must win
                }
            };

            var builder = CreateBuilder<Tuple<int, int, int>>(loadOptions);

            Assert.Equal(
                "data.OrderBy(obj => obj.Item1).ThenByDescending(obj => obj.Item2).ThenBy(obj => obj.Item3)",
                builder.BuildLoadExpr().Body.ToString()
            );

            loadOptions.Sort = null;
            Assert.Contains("OrderBy", builder.BuildLoadExpr().Body.ToString());
        }

        [Fact]
        public void MultiIntervalGroupsSortedOnce() {
            var builder = CreateBuilder<int>(new SampleLoadOptions {
                Group = new[] {
                    new GroupingInfo { Selector = "this", GroupInterval = "a" },
                    new GroupingInfo { Selector = "this", GroupInterval = "b" }
                }
            });

            Assert.Equal("data.OrderBy(obj => obj)", builder.BuildLoadExpr().Body.ToString());
        }

        [Fact]
        public void GuardNulls() {
            var builder = CreateBuilder<Tuple<int?, string, DateTime?>>(new SampleLoadOptions {
                Filter = new[] {
                    new[] { "Item1", ">", "0" },
                    new[] { "Item2", "contains", "z" },
                    new[] { "Item2.Length", ">", "1" },                    
                    new[] { "Item3.Year", ">", "0" }
                },
                Sort = new[] {
                    new SortingInfo { Selector = "Item1" },
                    new SortingInfo { Selector = "Item2.Length" },
                    new SortingInfo { Selector = "Item3.Year" },
                }
            }, true);

            var expr = builder.BuildLoadExpr();
            var query = expr.Compile();

            var data = new[] {
                // filtered out
                null,
                Tuple.Create<int?, string, DateTime?>(null, "zz", new DateTime(2000, 1, 1)),
                Tuple.Create<int?, string, DateTime?>(1, null, new DateTime(2000, 1, 1)),
                Tuple.Create<int?, string, DateTime?>(1, "zz", null),
                

                // kept
                Tuple.Create<int?, string, DateTime?>(1, "zz", new DateTime(2000, 1, 2)),
                Tuple.Create<int?, string, DateTime?>(1, "zz", new DateTime(2000, 1, 1))                
            };

            var result = query(data.AsQueryable()).ToArray();
            Assert.Equal(2, result.Length);
        }
    }

}
