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

    public class DynamicObjectTests {
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

            Assert.Equal(1, objectResult.Length);
            Assert.Equal(2d, objectResult[0][P1]);

            Assert.Equal(1, expandoResult.Length);
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

            Assert.Equal(1, result.Length);
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

    }

}
