using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class FilterExpressionCompilerTests {

        class DataItem1 {
            public int IntProp { get; set; }
            public string StringProp { get; set; }
            public int? NullableProp { get; set; }
            public DateTime Date { get; set; }
        }

        LambdaExpression Compile<T>(IList criteria, bool guardNulls = false) {
            return new FilterExpressionCompiler<T>(guardNulls).Compile(criteria);
        }

        [Fact]
        public void ImplicitEquals() {
            var expr = Compile<DataItem1>(new object[] { "IntProp", 123 });
            Assert.Equal("(obj.IntProp == 123)", expr.Body.ToString());
        }

        [Fact]
        public void ExplicitEquals() {
            var expr = Compile<DataItem1>(new object[] { "IntProp", "=", 1225 });
            Assert.Equal("(obj.IntProp == 1225)", expr.Body.ToString());
        }

        [Fact]
        public void DoesNotEqual() {
            var expr = Compile<DataItem1>(new object[] { "IntProp", "<>", 1 });
            Assert.Equal("(obj.IntProp != 1)", expr.Body.ToString());
        }

        [Fact]
        public void ComparisonOperations() {
            foreach(var op in new[] { ">", "<", ">=", "<=" }) {
                var expr = Compile<DataItem1>(new object[] { "IntProp", op, 9 });
                Assert.Equal("(obj.IntProp " + op + " 9)", expr.Body.ToString());
            }
        }

        [Fact]
        public void StringContains() {
            var expr = Compile<DataItem1>(new[] { "StringProp", "contains", "Abc" });
            Assert.Equal("obj.StringProp.ToLower().Contains(\"abc\")", expr.Body.ToString());
        }

        [Fact]
        public void StringNotContains() {
            var expr = Compile<DataItem1>(new[] { "StringProp", "notContains", "Abc" });
            Assert.Equal("Not(obj.StringProp.ToLower().Contains(\"abc\"))", expr.Body.ToString());
        }

        [Fact]
        public void StartsWith() {
            var expr = Compile<DataItem1>(new[] { "StringProp", "startsWith", "Prefix" });
            Assert.Equal("obj.StringProp.ToLower().StartsWith(\"prefix\")", expr.Body.ToString());
        }

        [Fact]
        public void EndsWith() {
            var expr = Compile<DataItem1>(new[] { "StringProp", "endsWith", "Postfix" });
            Assert.Equal("obj.StringProp.ToLower().EndsWith(\"postfix\")", expr.Body.ToString());
        }

        [Fact]
        public void StringFunctionOnNonStringData() {
            var expr = Compile<DataItem1>(new[] { "IntProp", "contains", "Abc" });
            Assert.Equal("obj.IntProp.ToString().ToLower().Contains(\"abc\")", expr.Body.ToString());
        }

        [Fact]
        public void StringFunctionGuardNulls() {
            Assert.Equal(
                @"(IIF((obj == null), null, obj.StringProp) ?? """").ToLower().StartsWith(""abc"")",
                Compile<DataItem1>(new[] { "StringProp", "startswith", "abc" }, true).Body.ToString()
            );

            Assert.Equal(
                @"(IIF((obj == null), null, obj.IntProp.ToString()) ?? """").ToLower().StartsWith(""abc"")",
                Compile<DataItem1>(new[] { "IntProp", "startswith", "abc" }, true).Body.ToString()
            );
        }

        [Fact]
        public void ImplicitAndOfTwo() {
            var crit = new[] {
                new object[] { "IntProp", ">", 1 },
                new object[] { "IntProp", "<", 10 }
            };

            var expr = Compile<DataItem1>(crit);
            Assert.Equal("((obj.IntProp > 1) AndAlso (obj.IntProp < 10))", expr.Body.ToString());
        }

        [Fact]
        public void ExplicitAndOfTwo() {
            var crit = new object[] {
                new object[] { "IntProp", ">", 1 },
                "and",
                new object[] { "IntProp", "<", 10 }
            };

            var expr = Compile<DataItem1>(crit);
            Assert.Equal("((obj.IntProp > 1) AndAlso (obj.IntProp < 10))", expr.Body.ToString());
        }

        [Fact]
        public void OrOfTwo() {
            var crit = new object[] {
                new object[] { "IntProp", 1 },
                "or",
                new object[] { "IntProp", 2 }
            };


            var expr = Compile<DataItem1>(crit);
            Assert.Equal("((obj.IntProp == 1) OrElse (obj.IntProp == 2))", expr.Body.ToString());
        }

        [Fact]
        public void Not() {
            var crit = new object[] {
                "!",
                new object[] {
                    new object[] { "IntProp", ">", 1 },
                    "and",
                    new object[] { "IntProp", "<", 10 }
                }
            };

            var expr = Compile<DataItem1>(crit);
            Assert.Equal("Not(((obj.IntProp > 1) AndAlso (obj.IntProp < 10)))", expr.Body.ToString());
        }

        [Fact]
        public void IsUnaryWithJsonCriteria() {
            var crit = JsonConvert.DeserializeObject<IList>("[\"!\", []]");
            var compiler = new FilterExpressionCompiler<object>(false);
            Assert.True(compiler.IsUnary(crit));
        }

        [Fact]
        public void GroupOfMany() {
            var crit = new object[] {
                new object[] { "IntProp", ">", 1 },
                new object[] { "IntProp", "<", 10 },
                "and",
                new[] { "StringProp", "<>", "abc" }

            };

            var expr = Compile<DataItem1>(crit);
            Assert.Equal("(((obj.IntProp > 1) AndAlso (obj.IntProp < 10)) AndAlso (obj.StringProp != \"abc\"))", expr.Body.ToString());
        }

        [Fact]
        public void NestedGroups() {
            var crit = new object[] {
                new object[] { "IntProp", 1 },
                "||",
                new[] {
                    new object[] { "IntProp", ">", 1 },
                    new object[] { "IntProp", "<", 10 }
                }
            };


            var expr = Compile<DataItem1>(crit);
            Assert.Equal("((obj.IntProp == 1) OrElse ((obj.IntProp > 1) AndAlso (obj.IntProp < 10)))", expr.Body.ToString());
        }

        [Fact]
        public void MixedGroupOperatorsWithoutBrackets() {
            var crit = new object[] {
                new object[] { "IntProp", ">", 1 },
                new object[] { "IntProp", "<", 10 },
                "||",
                new object[] { "IntProp", "=", 100 },
            };

            var e = Record.Exception(() => Compile<DataItem1>(crit));

            Assert.Contains("Mixing", e.Message);
        }

        [Fact]
        public void MultipleOrRegression() {
            var crit = new object[] {
                new object[] { "IntProp", 1 },
                "or",
                new object[] { "IntProp", 2 },
                "or",
                new object[] { "IntProp", 3 }
            };

            var expr = Compile<DataItem1>(crit);
            Assert.Equal("(((obj.IntProp == 1) OrElse (obj.IntProp == 2)) OrElse (obj.IntProp == 3))", expr.Body.ToString());
        }

        [Fact]
        public void ThisAsLeftValue() {
            var expr = Compile<int>(new object[] { "this", 1 });
            Assert.Equal("(obj == 1)", expr.Body.ToString());
        }

        [Fact]
        public void NullablePropertyAndPureValue() {
            var expr = Compile<DataItem1>(new object[] { "NullableProp", 1 });
            Assert.Equal("(obj.NullableProp == 1)", expr.Body.ToString());

            var method = expr.Compile();

            var result = (bool)method.DynamicInvoke(new DataItem1 { NullableProp = 1 });
            Assert.True(result);
        }

        [Fact]
        public void NoConvertWhenCompareWithNull() {
            var expr = Compile<DataItem1>(new[] { "StringProp", null });
            Assert.Equal("(obj.StringProp == null)", expr.Body.ToString());
        }

        [Fact]
        public void T105740() {
            var data = new[] {
                new DataItem1{ Date = new DateTime(2011, 12, 13) }
            };

            Assert.True((bool)Compile<DataItem1>(new object[] { "Date", "12/13/2011 00:00:00" }).Compile().DynamicInvoke(data[0]));
        }

        [Fact]
        public void JsonObjects() {
            var crit = (IList)JsonConvert.DeserializeObject(@"[ [ ""StringProp"", ""abc"" ], [ ""NullableProp"", null ] ]");
            var expr = Compile<DataItem1>(crit);
            Assert.Equal(@"((obj.StringProp == ""abc"") AndAlso (obj.NullableProp == null))", expr.Body.ToString());
        }

        [Fact]
        public void StringInequality() {
            foreach(var op in new[] { "<", "<=", ">=", ">" }) {
                Assert.Equal(
                    $@"(Compare(obj.StringProp, ""a"") {op} 0)",
                    Compile<DataItem1>(new[] { "StringProp", op, "a" }).Body.ToString()
                );
            }

            Assert.Equal(
                "(Compare(obj.StringProp, null) > 0)",
                Compile<DataItem1>(new[] { "StringProp", ">", null }).Body.ToString()
            );
        }

        [Fact]
        public void Issue136() {
            var x = Record.Exception(delegate {
                Compile<Tuple<int>>(new[] { "Item99", "1" });
            });

            Assert.True(x is ArgumentException);
        }

        [Fact]
        public void ValueTypeAndNull() {
            // Part of https://devexpress.com/issue=T616169 fix

            string CompileOperation(string op) {
                return Compile<Tuple<int>>(new[] { "Item1", op, null }).Body.ToString();
            }

            var expectedConvert = Compat.ExpectedConvert("obj.Item1", "Nullable`1");

            Assert.Equal($"({expectedConvert} == null)", CompileOperation("="));
            Assert.Equal($"({expectedConvert} != null)", CompileOperation("<>"));

            // https://stackoverflow.com/q/4399932
            Assert.Equal("False", CompileOperation(">"));
            Assert.Equal("False", CompileOperation(">="));
            Assert.Equal("False", CompileOperation("<"));
            Assert.Equal("False", CompileOperation("<="));
        }

    }

}
