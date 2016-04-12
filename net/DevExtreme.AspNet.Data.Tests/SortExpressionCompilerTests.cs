using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class SortExpressionCompilerTests {
        SortExpressionCompiler<DataItem1> _compiler;
        Expression _targetExpr;

        class DataItem1 {
            public int Prop1 { get; set; }
            public double Prop2 { get; set; }
        }

        class DataItem2 {
            public DataItem1 Inner { get; set; }
        }

        public SortExpressionCompilerTests() {
            _compiler = new SortExpressionCompiler<DataItem1>(false);
            _targetExpr = Expression.Parameter(typeof(IQueryable<DataItem1>), "data");
        }

        [Fact]
        public void SingleSort() {
            var clientExpr = new[] {
                new SortingInfo { Selector = "Prop1" }
            };

            var expr = _compiler.Compile(_targetExpr, clientExpr);
            Assert.Equal("data.OrderBy(obj => obj.Prop1)", expr.ToString());
        }

        [Fact]
        public void SingleSortDesc() {
            var clientExpr = new[] {
                new SortingInfo {
                    Selector = "Prop2",
                    Desc = true
                },
            };

            var expr = _compiler.Compile(_targetExpr, clientExpr);
            Assert.Equal("data.OrderByDescending(obj => obj.Prop2)", expr.ToString());
        }

        [Fact]
        public void MultipleSort() {
            var clientExpr = new[] {
                new SortingInfo {
                    Selector="Prop1"
                },
                new SortingInfo {
                    Selector="Prop2"
                },
                new SortingInfo {
                    Selector="Prop1",
                    Desc= true
                }
            };

            var expr = _compiler.Compile(_targetExpr, clientExpr);
            Assert.Equal("data.OrderBy(obj => obj.Prop1).ThenBy(obj => obj.Prop2).ThenByDescending(obj => obj.Prop1)", expr.ToString());
        }

        [Fact]
        public void EmptySort() {
            var expr = _compiler.Compile(_targetExpr, new SortingInfo[0]);
            Assert.Same(_targetExpr, expr);
        }

        [Fact]
        public void SortByThis() {
            var clientExpr = new[] {
                new SortingInfo { Selector = "this" }
            };

            var expr = _compiler.Compile(_targetExpr, clientExpr);
            Assert.Equal("data.OrderBy(obj => obj)", expr.ToString());
        }

        [Fact]
        public void SortByNestedProp() {
            var clientExpr = new[] {
                new SortingInfo { Selector = "Inner.Prop1" }
            };

            var targetExpr = Expression.Parameter(typeof(IQueryable<DataItem2>), "data");
            var expr = new SortExpressionCompiler<DataItem2>(false).Compile(targetExpr, clientExpr);
            Assert.Equal("data.OrderBy(obj => obj.Inner.Prop1)", expr.ToString());
        }

        [Fact]
        public void T361903() {
            Assert.Equal("data", _compiler.Compile(_targetExpr, new[] { new SortingInfo { Selector = "" } }).ToString());
            Assert.Equal("data", _compiler.Compile(_targetExpr, new[] { new SortingInfo { Selector = null } }).ToString());
        }
    }

}
