using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class DataSourceExpressionBuilderTests {

        [Fact]
        public void Build_SkipTake() {
            var builder = Compat.CreateDataSourceExpressionBuilder<int>(new SampleLoadOptions {
                Skip = 111,
                Take = 222,
                GuardNulls = false
            });

            var expr = builder.BuildLoadExpr();

            Assert.Equal("data.Skip(111).Take(222)", expr.ToString());
        }

        [Fact]
        public void Build_Filter() {
            var builder = Compat.CreateDataSourceExpressionBuilder<int>(new SampleLoadOptions {
                Filter = new object[] { "this", ">", 123 },
                GuardNulls = false
            });

            var expr = builder.BuildLoadExpr();

            Assert.Equal("data.Where(obj => (obj > 123))", expr.ToString());
        }

        [Fact]
        public void Build_FilterAsEmptyList() {
            // To mitigate cases like https://devexpress.com/issue=T483154

            var builder = Compat.CreateDataSourceExpressionBuilder<int>(new SampleLoadOptions {
                Filter = new object[0],
                GuardNulls = false
            });

            Assert.DoesNotContain(".Where", builder.BuildLoadExpr().ToString());
        }

        [Fact]
        public void Build_CountQuery() {
            var builder = Compat.CreateDataSourceExpressionBuilder<int>(new SampleLoadOptions {
                Skip = 111,
                Take = 222,
                Filter = new object[] { "this", 123 },
                Sort = new[] {
                    new SortingInfo { Selector = "this" }
                },
                GuardNulls = false
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
            var builder = Compat.CreateDataSourceExpressionBuilder<Tuple<int, string>>(new SampleLoadOptions {
                Sort = new[] {
                    new SortingInfo {
                        Selector="Item1"
                    },
                    new SortingInfo {
                        Selector = "Item2",
                        Desc=true
                    }
                },
                GuardNulls = false
            });

            var expr = builder.BuildLoadExpr();
            Assert.Equal("data.OrderBy(obj => obj.Item1).ThenByDescending(obj => obj.Item2)", expr.ToString());
        }

        [Fact]
        public void SortByPrimaryKey() {

            void Case(Action<DataSourceLoadOptionsBase> initOptions, Action<string> assert) {
                var source = new[] {
                    new { ID = 1, Value = "A" }
                };

                var loadOptions = new SampleLoadOptions {
                    GuardNulls = false,
                    PrimaryKey = new[] { "ID" },
                    SortByPrimaryKey = false
                };

                initOptions?.Invoke(loadOptions);

                assert(Compat.CreateDataSourceExpressionBuilder(source.AsQueryable(), loadOptions).BuildLoadExpr().ToString());
            }

            Case(
                null,
                expr => Assert.DoesNotContain("OrderBy", expr)
            );

            Case(
                options => options.DefaultSort = "Value",
                expr => {
                    Assert.Contains(".OrderBy(obj => obj.Value)", expr);
                    Assert.DoesNotContain("ThenBy", expr);
                }
            );
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
                },
                GuardNulls = false
            };

            string BuildLoadExpr() => Compat.CreateDataSourceExpressionBuilder<Tuple<int, int, int>>(loadOptions).BuildLoadExpr().ToString();

            Assert.Equal(
                "data.OrderBy(obj => obj.Item1).ThenByDescending(obj => obj.Item2).ThenBy(obj => obj.Item3)",
                BuildLoadExpr()
            );

            loadOptions.Sort = null;
            Assert.Contains("OrderBy", BuildLoadExpr());
        }

        [Fact]
        public void MultiIntervalGroupsSortedOnce() {
            var builder = Compat.CreateDataSourceExpressionBuilder<int>(new SampleLoadOptions {
                Group = new[] {
                    new GroupingInfo { Selector = "this", GroupInterval = "a" },
                    new GroupingInfo { Selector = "this", GroupInterval = "b" }
                },
                GuardNulls = false
            });

            Assert.Equal("data.OrderBy(obj => obj)", builder.BuildLoadExpr().ToString());
        }

        [Fact]
        public void GuardNulls() {
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

            var builder = Compat.CreateDataSourceExpressionBuilder(data, new SampleLoadOptions {
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
                },
                GuardNulls = true
            });

            var expr = builder.BuildLoadExpr();
            var result = data.Provider.CreateQuery<object>(expr).ToArray();
            Assert.Equal(2, result.Length);
        }

        [Fact]
        public void DefaultSort() {
            var options = new SampleLoadOptions {
                DefaultSort = "Item1",
                GuardNulls = false
            };

            string BuildLoadExpr() => Compat.CreateDataSourceExpressionBuilder<Tuple<int, int>>(options).BuildLoadExpr(false).ToString();

            Assert.Equal("data.OrderBy(obj => obj.Item1)", BuildLoadExpr());

            options.Sort = new[] {
                new SortingInfo { Selector = "Item2" }
            };

            Assert.Equal("data.OrderBy(obj => obj.Item2).ThenBy(obj => obj.Item1)", BuildLoadExpr());

            options.Sort[0].Selector = "Item1";
            Assert.Equal("data.OrderBy(obj => obj.Item1)", BuildLoadExpr());
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
                },
                GuardNulls = false
            };

            var builder = Compat.CreateDataSourceExpressionBuilder<Tuple<int, int>>(options);
            var expr = builder.BuildLoadGroupsExpr().ToString();

            Assert.StartsWith("data.GroupBy", expr);
        }

        [Fact]
        public void AlwaysOrderDataByPrimaryKey() {
            var options = new SampleLoadOptions {
                PrimaryKey = new[] { "Item2", "Item1" },
                GuardNulls = false
            };

            var builder = Compat.CreateDataSourceExpressionBuilder<Tuple<int, int>>(options);

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
                Sort = new[] { new SortingInfo { Selector = "Item1" } },
                GuardNulls = false
            };

            {
                var builder = Compat.CreateDataSourceExpressionBuilder<Tuple<int, int, int>>(options);

                Assert.Equal(
                    "data.OrderBy(obj => obj.Item1)",
                    builder.BuildLoadExpr().ToString()
                );
            }

            options.DefaultSort = "Item2";
            options.Sort[0].Selector = "Item3";

            {
                var builder = Compat.CreateDataSourceExpressionBuilder<Tuple<int, int, int>>(options);

                Assert.Equal(
                    "data.OrderBy(obj => obj.Item3).ThenBy(obj => obj.Item2).ThenBy(obj => obj.Item1)",
                    builder.BuildLoadExpr().ToString()
                );
            }
        }

        [Fact]
        public void PR202() {
            // https://github.com/DevExpress/DevExtreme.AspNet.Data/pull/202

            var options = new SampleLoadOptions {
                DefaultSort = "item1",
                Sort = new[] {
                    new SortingInfo { Selector = "ITEM1" }
                },
                GuardNulls = false
            };

            var builder = Compat.CreateDataSourceExpressionBuilder<Tuple<int>>(options);

            Assert.Equal(
                "data.OrderBy(obj => obj.Item1)",
                builder.BuildLoadExpr().ToString()
            );
        }

        [Fact]
        public void RemoteSelectFalse() {
            var options = new SampleLoadOptions {
                Select = new[] { "abc" },
                RemoteSelect = false,
                GuardNulls = false
            };

            Assert.Equal(
                "data",
                Compat.CreateDataSourceExpressionBuilder<object>(options).BuildLoadExpr().ToString()
            );
        }

        [Fact]
        public void BuildGroupCountExpr() {
            string BuildExpr(DataSourceLoadOptionsBase options) => Compat.CreateDataSourceExpressionBuilder<Tuple<int, int>>(options)
                .BuildGroupCountExpr()
                .ToString();

            var error = Record.Exception(delegate {
                BuildExpr(new SampleLoadOptions {
                    Group = new[] {
                        new GroupingInfo { Selector = "Item1" },
                        new GroupingInfo { Selector = "Item2" }
                    }
                });
            });
            Assert.True(error is InvalidOperationException);

            Assert.Equal(
                "data.Where(obj => (obj.Item2 == 1)).Select(obj => obj.Item1).Distinct().Count()",
                BuildExpr(new SampleLoadOptions {
                    GuardNulls = false,
                    Filter = new[] { "Item2", "1" },
                    Group = new[] {
                        new GroupingInfo { Selector = "Item1" }
                    }
                })
            );
        }
    }

}
