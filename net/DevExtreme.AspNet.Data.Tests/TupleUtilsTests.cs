using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class TupleUtilsTests {

        [Fact]
        public void CreateType_Short() {
            Assert.Equal(
                typeof(Tuple<int>),
                TupleUtils.CreateType(new[] { typeof(int) })
            );

            Assert.Equal(
                typeof(Tuple<int, int, int, int, int, int, string>),
                TupleUtils.CreateType(new[] {
                    typeof(int), typeof(int), typeof(int), typeof(int),
                    typeof(int), typeof(int), typeof(string)
                })
            );
        }

        [Fact]
        public void CreateNewExpr_Short() {
            var initializer = TupleUtils.CreateNewExpr(
                typeof(Tuple<bool, int>),
                new[] {
                    Expression.Constant(true),
                    Expression.Constant(1)
                }
            );

            Assert.Equal(
                "new Tuple`2(Item1 = True, Item2 = 1)",
                initializer.ToString()
            );
        }

        [Fact]
        public void CreateNewExpr_Long() {
            var accessors = new List<Expression>();

            for(var i = 0; i < 5; i++)
                accessors.Add(Expression.Constant(true));

            for(var i = 0; i < 5; i++)
                accessors.Add(Expression.Constant(1));

            for(var i = 0; i < 5; i++)
                accessors.Add(Expression.Constant(null, typeof(string)));

            var tupleType = TupleUtils.CreateType(accessors.Select(i => i.Type).ToArray());

            Assert.Equal(
                "new Tuple`8("
                    + "Item1 = True, "
                    + "Item2 = True, "
                    + "Item3 = True, "
                    + "Item4 = True, "
                    + "Item5 = True, "
                    + "Item6 = 1, "
                    + "Item7 = 1, "
                    + "Rest = new Tuple`8("
                        + "Item1 = 1, "
                        + "Item2 = 1, "
                        + "Item3 = 1, "
                        + "Item4 = null, "
                        + "Item5 = null, "
                        + "Item6 = null, "
                        + "Item7 = null, "
                        + "Rest = new Tuple`1("
                            + "Item1 = null"
                        + ")"
                    + ")"
                + ")",
                TupleUtils.CreateNewExpr(tupleType, accessors).ToString()
            );
        }

        [Fact]
        public void CreateReadItemExpr() {
            var param = Expression.Parameter(typeof(Tuple<int, int, int, int, int, int, int, Tuple<int, int, int, int, int, int, int, Tuple<int, int>>>), "t");

            Assert.Equal(
                "t.Item7",
                TupleUtils.CreateReadItemExpr(param, 6).ToString()
            );

            Assert.Equal(
                "t.Rest.Item1",
                TupleUtils.CreateReadItemExpr(param, 7).ToString()
            );

            Assert.Equal(
                "t.Rest.Rest.Item2",
                TupleUtils.CreateReadItemExpr(param, 15).ToString()
            );
        }

        [Theory]
        [InlineData(true), InlineData(false)]
        public void ReadItem(bool useIndexer) {
            var tuple = (0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16).ToTuple();

            Assert.Equal(6, TupleUtils.ReadItem(tuple, 6, useIndexer));
            Assert.Equal(7, TupleUtils.ReadItem(tuple, 7, useIndexer));
            Assert.Equal(15, TupleUtils.ReadItem(tuple, 15, useIndexer));
        }

    }

}
