using DevExtreme.AspNet.Data.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class CustomSortCompilersTests {

        [Fact]
        public void CustomSorting() {
            try {
                CustomSortCompilers.RegisterBinaryExpressionCompiler((target, info) => {
                    if(info.AccessorText == "Name") {
                        var prop = Expression.Property(info.DataItemExpression, nameof(Product.Name));

                        var body1 = Expression.Condition(Expression.Equal(prop, Expression.Constant("Lightsaber")), Expression.Constant(1), Expression.Constant(2));
                        var expr1 = Expression.Lambda(body1, (ParameterExpression)info.DataItemExpression);
                        var expr2 = Expression.Lambda(prop, (ParameterExpression)info.DataItemExpression);

                        target = Expression.Call(typeof(Queryable), Utils.GetSortMethod(info.First, info.Desc),
                            new[] { info.DataItemExpression.Type, expr1.ReturnType }, target, Expression.Quote(expr1));
                        target = Expression.Call(typeof(Queryable), Utils.GetSortMethod(false, info.Desc),
                            new[] { info.DataItemExpression.Type, expr2.ReturnType }, target, Expression.Quote(expr2));

                        return target;
                    }

                    return null;
                });

                var source = new[] {
                    new Product { Name = "Box", Price = 5 },
                    new Product { Name = "Pen", Price = 2 },
                    new Product { Name = "Bike", Price = 2000 },
                    new Product { Name = "Lightsaber", Price = 5000000 }
                };


                var loadOptions = new SampleLoadOptions {
                    Sort = new[] { new SortingInfo { Selector = "Name", Desc = false } },
                    RequireTotalCount = true
                };

                var loadResult = DataSourceLoader.Load(source, loadOptions);
                var data = loadResult.data.Cast<Product>().ToArray();
                Assert.Equal("Lightsaber", data[0].Name);
                Assert.Equal("Bike", data[1].Name);
                Assert.Equal("Box", data[2].Name);
                Assert.Equal("Pen", data[3].Name);
            } finally {
                CustomSortCompilers.Sort.CompilerFuncs.Clear();
            }
        }

        class Product {
            public string Name { get; set; }
            public decimal Price { get; set; }
        }
    }
}
