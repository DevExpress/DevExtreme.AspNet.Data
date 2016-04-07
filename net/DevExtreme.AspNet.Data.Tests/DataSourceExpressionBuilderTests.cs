using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class DataSourceExpressionBuilderTests {
        public void Build_SkipTake() {
            var builder = new DataSourceExpressionBuilder<int> {
                Skip = 111,
                Take = 222
            };

            var expr = builder.BuildLoadExpr();

            Assert.Equal("data.Skip(111).Take(222)", expr.Body.ToString());
        }

        [Fact]
        public void Build_Filter() {
            var builder = new DataSourceExpressionBuilder<int> {
                Filter = new object[] { "this", ">", 123 }
            };

            var expr = builder.BuildLoadExpr();

            Assert.Equal("data.Where(obj => (obj > 123))", expr.Body.ToString());
        }

        [Fact]
        public void Build_CountQuery() {
            var builder = new DataSourceExpressionBuilder<int> {
                Skip = 111,
                Take = 222,
                Filter = new object[] { "this", 123 },
                Sort = new[] {
                    new SortingInfo { Selector = "this" }
                },
            };

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
            var builder = new DataSourceExpressionBuilder<Tuple<int, string>> {
                Sort = new[] {
                    new SortingInfo {
                        Selector="Item1"
                    },
                    new SortingInfo {
                        Selector = "Item2",
                        Desc=true
                    }
                }
            };

            var expr = builder.BuildLoadExpr();
            Assert.Equal("data.OrderBy(obj => obj.Item1).ThenByDescending(obj => obj.Item2)", expr.Body.ToString());
        }

        [Fact]
        public void GroupingAddedToSorting() {
            var builder = new DataSourceExpressionBuilder<Tuple<int, int, int>> {
                Sort = new[] {
                    new SortingInfo { Selector = "Item2" },
                    new SortingInfo { Selector = "Item3" }
                },
                Group = new[] {
                    new GroupingInfo { Selector = "Item1" },
                    new GroupingInfo { Selector = "Item2", Desc = true  } // this must win
                }
            };

            Assert.Equal(
                "data.OrderBy(obj => obj.Item1).ThenByDescending(obj => obj.Item2).ThenBy(obj => obj.Item3)",
                builder.BuildLoadExpr().Body.ToString()
            );
        }
    }

}
