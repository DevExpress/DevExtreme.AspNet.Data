using DevExtreme.AspNet.Data.ResponseModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class DynamicBindingTests {
        const string P1 = "p1";

        static ExpandoObject CreateExpando(object p1) {
            dynamic obj = new ExpandoObject();
            obj.p1 = p1;
            return obj;
        }

        static IDictionary<string, object>[] ToDictArray(IEnumerable data) {
            return data
                .Cast<IDictionary<string, object>>()
                .ToArray();
        }

        [Fact]
        public void Sort() {
            var loadOptions = new SampleLoadOptions {
                Sort = new[] {
                    new SortingInfo { Selector = P1 }
                }
            };

            var data = new[] {
                CreateExpando(2),
                CreateExpando(1)
            };

            var objectResult = ToDictArray(DataSourceLoader.Load<object>(data, loadOptions).data);
            var expandoResult = ToDictArray(DataSourceLoader.Load<ExpandoObject>(data, loadOptions).data);

            Assert.Equal(1, objectResult[0][P1]);
            Assert.Equal(2, objectResult[1][P1]);

            Assert.Equal(1, expandoResult[0][P1]);
            Assert.Equal(2, expandoResult[1][P1]);
        }

        [Fact]
        public void Filter() {
            var loadOptions = new SampleLoadOptions {
                Filter = new object[] { P1, ">", 1 }
            };

            var data = new[] {
                CreateExpando(1m),
                CreateExpando(2d)
            };

            var objectResult = ToDictArray(DataSourceLoader.Load<object>(data, loadOptions).data);
            var expandoResult = ToDictArray(DataSourceLoader.Load<ExpandoObject>(data, loadOptions).data);

            Assert.Single(objectResult);
            Assert.Equal(2d, objectResult[0][P1]);

            Assert.Single(expandoResult);
            Assert.Equal(2d, expandoResult[0][P1]);
        }

        [Fact]
        public void Filter_JValueNull() {
            var result = ToDictArray(DataSourceLoader.Load(
                new[] {
                    CreateExpando(null)
                },
                new SampleLoadOptions {
                    Filter = new object[] { P1, JValue.CreateNull() }
                }
            ).data);

            Assert.Single(result);
        }

        [Fact]
        public void Filter_Null() {
            var result = ToDictArray(DataSourceLoader.Load(
                new[] {
                    CreateExpando(1),
                    CreateExpando(null)
                },
                new SampleLoadOptions {
                    Filter = new object[] { P1, ">=", null }
                }
            ).data);

            Assert.Equal(2, result.Length);
        }

        [Fact]
        public void TotalSummary() {
            var loadOptions = new SampleLoadOptions {
                TotalSummary = new[] {
                    new SummaryInfo { Selector = P1, SummaryType = "sum" }
                }
            };

            var data = new[] {
                CreateExpando(1),
                CreateExpando(2)
            };

            var objectResult = DataSourceLoader.Load<object>(data, loadOptions);
            var expandoResult = DataSourceLoader.Load<ExpandoObject>(data, loadOptions);

            Assert.Equal(3m, objectResult.summary[0]);
            Assert.Equal(3m, expandoResult.summary[0]);
        }

        [Fact]
        public void Grouping() {
            var loadOptions = new SampleLoadOptions {
                Group = new[] {
                    new GroupingInfo { Selector = P1 }
                },
                GroupSummary = new[] {
                    new SummaryInfo { Selector= P1, SummaryType = "sum" }
                }
            };

            var data = new[] {
                CreateExpando(1),
                CreateExpando(2),
                CreateExpando(1),
                CreateExpando(2)
            };

            var objectResult = (IList<Group>)DataSourceLoader.Load<object>(data, loadOptions).data;
            var expandoResult = (IList<Group>)DataSourceLoader.Load<ExpandoObject>(data, loadOptions).data;

            Assert.Equal(2m, objectResult[0].summary[0]);
            Assert.Equal(4m, objectResult[1].summary[0]);

            Assert.Equal(2m, expandoResult[0].summary[0]);
            Assert.Equal(4m, expandoResult[1].summary[0]);
        }

        [Fact]
        public void JArray() {
            var sourceData = JsonConvert.DeserializeObject<JArray>(@"[
                { ""p1"": 2 },
                { ""p1"": 3 },
                { ""p1"": 1 },
            ]");

            var result = DataSourceLoader.Load(sourceData, new SampleLoadOptions {
                Filter = new object[] { P1, "<>", 2 },
                Sort = new[] { new SortingInfo { Selector = P1 } },
                TotalSummary = new[] { new SummaryInfo { Selector = P1, SummaryType = "sum" } }
            });

            var resultData = result.data.Cast<JObject>().ToArray();
            Assert.Equal(1, resultData[0][P1]);
            Assert.Equal(3, resultData[1][P1]);

            Assert.Equal(4m, result.summary[0]);
        }

        [Fact]
        public void T598818() {
            var data = new[] {
                CreateExpando(1),
                CreateExpando(3),
                CreateExpando(5),
                CreateExpando(null),
            };

            var loadOptions = new SampleLoadOptions {
                RemoteGrouping = true,
                TotalSummary = new[] {
                    new SummaryInfo { SummaryType = "sum", Selector = P1 },
                    new SummaryInfo { SummaryType = "min", Selector = P1 },
                    new SummaryInfo { SummaryType = "max", Selector = P1 },
                    new SummaryInfo { SummaryType = "avg", Selector = P1 }
                }
            };

            var summary = DataSourceLoader.Load(data, loadOptions).summary;

            Assert.Contains(".GroupBy", loadOptions.ExpressionLog[1]);

            Assert.Equal(9m, summary[0]);
            Assert.Equal(1, summary[1]);
            Assert.Equal(5, summary[2]);
            Assert.Equal(3m, summary[3]);
        }

        [Fact]
        public void Issue227() {
            dynamic
                dataItem = new ExpandoObject(),
                company = new ExpandoObject();

            dataItem.Company = company;
            company.Name = "abc";

            var loadResult = DataSourceLoader.Load(new[] { dataItem }, new SampleLoadOptions {
                Filter = new[] { "Company.Name", "abc" }
            });

            Assert.Single(loadResult.data);
        }

        [Fact]
        public void NoToStringForNumbers() {
            var compiler = new FilterExpressionCompiler(typeof(object), false);

            void Case(IList clientFilter, string expectedExpr, object trueTestValue) {
                var expr = compiler.Compile(clientFilter);
                Assert.Equal(expectedExpr, expr.Body.ToString());
                Assert.True((bool)expr.Compile().DynamicInvoke(trueTestValue));
            }

            Case(
                new object[] { "this", ">", new JValue(9) },
                "(DynamicCompare(obj, 9, False) > 0)",
                10
            );
        }

        [Fact]
        public void T714342() {
            var data = System.Linq.Dynamic.Core.DynamicQueryableExtensions.Select(
                new[] { new { CategoryID = 1 } }.AsQueryable(),
                "new { CategoryID }"
            );

            Assert.False(DynamicBindingHelper.ShouldUseDynamicBinding(data.ElementType));
        }

        [Fact]
        public void DBNull() {
            dynamic item1 = new ExpandoObject();
            dynamic item2 = new ExpandoObject();
            item1.p = 123;
            item2.p = System.DBNull.Value;

            var source = new[] { item1, item2 };

            Assert.Equal(1, DataSourceLoader.Load(source, new SampleLoadOptions {
                Filter = new object[] { "p", 123 },
                RequireTotalCount = true
            }).totalCount);

            Assert.Equal(1, DataSourceLoader.Load(source, new SampleLoadOptions {
                Filter = new object[] { "p", null },
                RequireTotalCount = true
            }).totalCount);
        }

        [Fact]
        public void T819075() {
            dynamic sourceItem = new ExpandoObject();
            sourceItem.p = new DateTime(2011, 11, 11);

            Assert.Equal(1, DataSourceLoader.Load(new[] { sourceItem }, new SampleLoadOptions {
                Filter = new[] { "p", "11/11/2011" },
                RequireTotalCount = true
            }).totalCount);
        }

        [Fact]
        public void Issue413() {
            // https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/413#issuecomment-580766581

            IQueryable<object> projection = new[] { new { A = 1 } }
                .AsQueryable()
                .Select(i => new { i.A });

            var loadResult = DataSourceLoader.Load(projection, new SampleLoadOptions {
                Filter = new[] { "A", "1" },
                RequireTotalCount = true
            });

            Assert.Equal(1, loadResult.totalCount);
        }
    }

}
