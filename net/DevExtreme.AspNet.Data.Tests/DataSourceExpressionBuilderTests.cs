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

            Assert.Equal("data.Skip(111).Take(222)", expr.ToString());
        }

        [Fact]
        public void Build_Filter() {
            var builder = CreateBuilder<int>(new SampleLoadOptions {
                Filter = new object[] { "this", ">", 123 }
            });

            var expr = builder.BuildLoadExpr();

            Assert.Equal("data.Where(obj => (obj > 123))", expr.ToString());
        }

        [Fact]
        public void Build_FilterAsEmptyList() {
            // To mitigate cases like https://devexpress.com/issue=T483154

            var builder = CreateBuilder<int>(new SampleLoadOptions {
                Filter = new object[0]
            });

            Assert.DoesNotContain(".Where", builder.BuildLoadExpr().ToString());
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
            Assert.Contains(".Count()", text);
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
            Assert.Equal("data.OrderBy(obj => obj.Item1).ThenByDescending(obj => obj.Item2)", expr.ToString());
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
                builder.BuildLoadExpr().ToString()
            );

            loadOptions.Sort = null;
            Assert.Contains("OrderBy", builder.BuildLoadExpr().ToString());
        }

        [Fact]
        public void MultiIntervalGroupsSortedOnce() {
            var builder = CreateBuilder<int>(new SampleLoadOptions {
                Group = new[] {
                    new GroupingInfo { Selector = "this", GroupInterval = "a" },
                    new GroupingInfo { Selector = "this", GroupInterval = "b" }
                }
            });

            Assert.Equal("data.OrderBy(obj => obj)", builder.BuildLoadExpr().ToString());
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

            var data = new[] {
                // filtered out
                null,
                Tuple.Create<int?, string, DateTime?>(null, "zz", new DateTime(2000, 1, 1)),
                Tuple.Create<int?, string, DateTime?>(1, null, new DateTime(2000, 1, 1)),
                Tuple.Create<int?, string, DateTime?>(1, "zz", null),


                // kept
                Tuple.Create<int?, string, DateTime?>(1, "zz", new DateTime(2000, 1, 2)),
                Tuple.Create<int?, string, DateTime?>(1, "zz", new DateTime(2000, 1, 1))
            }.AsQueryable();

            var expr = builder.BuildLoadExpr(data.Expression);
            var result = data.Provider.CreateQuery<object>(expr).ToArray();
            Assert.Equal(2, result.Length);
        }

        [Fact]
        public void DefaultSort() {
            var options = new SampleLoadOptions {
                DefaultSort = "Item1"
            };

            var builder = new DataSourceExpressionBuilder<Tuple<int, int>>(options, false);

            Assert.Equal("data.OrderBy(obj => obj.Item1)", builder.BuildLoadExpr(false).ToString());

            options.Sort = new[] {
                new SortingInfo { Selector = "Item2" }
            };

            Assert.Equal("data.OrderBy(obj => obj.Item2).ThenBy(obj => obj.Item1)", builder.BuildLoadExpr(false).ToString());

            options.Sort[0].Selector = "Item1";
            Assert.Equal("data.OrderBy(obj => obj.Item1)", builder.BuildLoadExpr(false).ToString());
        }

        [Fact]
        public void NoUnnecessaryOrderingForRemoteGroups() {
            var options = new SampleLoadOptions {
                RemoteGrouping = true,
                Group = new[] {
                    new GroupingInfo { Selector = "Item1" }
                },
                Sort = new[] {
                    new SortingInfo { Selector = "Item2" }
                }
            };

            var builder = new DataSourceExpressionBuilder<Tuple<int, int>>(options, false);
            var expr = builder.BuildLoadGroupsExpr().ToString();

            Assert.True(expr.StartsWith("data.GroupBy"));
        }

        [Fact]
        public void AlwaysOrderDataByPrimaryKey() {
            var options = new SampleLoadOptions {
                PrimaryKey = new[] { "Item2", "Item1" }
            };

            var builder = new DataSourceExpressionBuilder<Tuple<int, int>>(options, false);

            Assert.Equal(
                "data.OrderBy(obj => obj.Item2).ThenBy(obj => obj.Item1)",
                builder.BuildLoadExpr().ToString()
            );
        }

        [Fact]
        public void DefaultSortAndPrimaryKey() {
            var options = new SampleLoadOptions {
                PrimaryKey = new[] { "Item1" },
                DefaultSort = "Item1",
                Sort = new[] { new SortingInfo { Selector = "Item1" } }
            };

            var builder = new DataSourceExpressionBuilder<Tuple<int, int, int>>(options, false);

            Assert.Equal(
                "data.OrderBy(obj => obj.Item1)",
                builder.BuildLoadExpr().ToString()
            );

            options.DefaultSort = "Item2";
            options.Sort[0].Selector = "Item3";

            Assert.Equal(
                "data.OrderBy(obj => obj.Item3).ThenBy(obj => obj.Item2).ThenBy(obj => obj.Item1)",
                builder.BuildLoadExpr().ToString()
            );
        }
    }

}
