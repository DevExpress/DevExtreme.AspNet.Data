using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class DynamicObjectTests {

        static IEnumerable<T> CreateTestData<T>() {
            dynamic
                item1 = new ExpandoObject(),
                item2 = new ExpandoObject(),
                item3 = new ExpandoObject();

            item1.p = 1;
            item2.p = 2;
            item3.p = 3;


            return new T[] { item3, item1, item2 };
        }

        static T[] LoadDataOnly<T>(DataSourceLoadOptionsBase loadOptions) {
            var opaqueResult = DataSourceLoader.Load(CreateTestData<T>(), loadOptions);
            return ((IEnumerable<T>)opaqueResult).ToArray();
        }

        [Fact]
        public void Sort() {
            var loadOptions = new SampleLoadOptions {
                Sort = new[] {
                    new SortingInfo { Selector = "p" }
                }
            };

            dynamic resultDynamic = LoadDataOnly<dynamic>(loadOptions);
            dynamic resultExpando = LoadDataOnly<ExpandoObject>(loadOptions);

            Assert.Equal(1, resultDynamic[0].p);
            Assert.Equal(3, resultDynamic[2].p);

            Assert.Equal(1, resultExpando[0].p);
            Assert.Equal(3, resultExpando[2].p);
        }

        [Fact]
        public void Filter() {
            var loadOptions = new SampleLoadOptions {
                Filter = new object[] { "p", ">", 2 }
            };

            dynamic resultDynamic = LoadDataOnly<dynamic>(loadOptions);
            dynamic resultExpando = LoadDataOnly<ExpandoObject>(loadOptions);

            Assert.Equal(1, resultDynamic.Length);
            Assert.Equal(3, resultDynamic[0].p);

            Assert.Equal(1, resultExpando.Length);
            Assert.Equal(3, resultExpando[0].p);
        }

        [Fact]
        public void TotalSummary() {
            var loadOptions = new SampleLoadOptions {
                TotalSummary = new[] {
                    new SummaryInfo { Selector = "p", SummaryType = "sum" }
                }
            };

            var resultDynamic = (DataSourceLoadResult)DataSourceLoader.Load(CreateTestData<dynamic>(), loadOptions);
            var resultExpando = (DataSourceLoadResult)DataSourceLoader.Load(CreateTestData<ExpandoObject>(), loadOptions);

            Assert.Equal(6m, resultDynamic.summary[0]);
            Assert.Equal(6m, resultExpando.summary[0]);
        }

    }

}
