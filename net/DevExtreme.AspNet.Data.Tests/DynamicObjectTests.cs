using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class DynamicObjectTests {


        IQueryable<ExpandoObject> CreateTestData() {
            dynamic
                item1 = new ExpandoObject(),
                item2 = new ExpandoObject(),
                item3 = new ExpandoObject();

            item1.p = 1;
            item2.p = 2;
            item3.p = 3;

            return new ExpandoObject[] {
                item3, item1, item2
            }.AsQueryable();
        }


        [Fact]
        public void Sort_DynamicKeyword() {
            var options = new SampleLoadOptions {
                Sort = new[] {
                    new SortingInfo { Selector = "p" }
                }
            };
            
            var builder = new DataSourceExpressionBuilder<dynamic>(options, true);
            var func = builder.BuildLoadExpr().Compile();

            dynamic input = CreateTestData();
            dynamic output = Enumerable.ToArray(func(input));

            Assert.Equal(1, output[0].p);
            Assert.Equal(3, output[2].p);
        }

        [Fact]
        public void Filter_DynamicKeyword() {
            var options = new SampleLoadOptions {
                Filter = new object[] { "p", ">", 2 }
            };

            var builder = new DataSourceExpressionBuilder<dynamic>(options, true);
            var func = builder.BuildLoadExpr().Compile();

            dynamic input = CreateTestData();
            dynamic output = Enumerable.ToArray(func(input));

            Assert.Equal(1, output.Length);
            Assert.Equal(3, output[0].p);
        }

    }

}
