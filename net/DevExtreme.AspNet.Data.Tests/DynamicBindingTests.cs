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

        static IDictionary<string, object>[] ToDictArray(object obj) {
            return ((IEnumerable)obj)
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
            
            var resultObject = ToDictArray(DataSourceLoader.Load(
                new object[] {
                    CreateExpando(2),
                    CreateExpando(1)
                }, 
                loadOptions
            ));

            var resultExpando = ToDictArray(DataSourceLoader.Load(
                new[] {
                    CreateExpando(2),
                    CreateExpando(1)
                }, 
                loadOptions
            ));

            Assert.Equal(1, resultObject[0][P1]);
            Assert.Equal(2, resultObject[1][P1]);

            Assert.Equal(1, resultExpando[0][P1]);
            Assert.Equal(2, resultExpando[1][P1]);
        }

        [Fact]
        public void Filter() {
            var loadOptions = new SampleLoadOptions {
                Filter = new object[] { P1, ">", 1 }
            };

            var resultObject = ToDictArray(DataSourceLoader.Load(
                new object[] {
                    CreateExpando(1),
                    CreateExpando(2)
                },
                loadOptions
            ));

            var resultExpando = ToDictArray(DataSourceLoader.Load(
                new[] {
                    CreateExpando(1),
                    CreateExpando(2)
                },
                loadOptions
            ));

            Assert.Equal(1, resultObject.Length);
            Assert.Equal(2, resultObject[0][P1]);

            Assert.Equal(1, resultExpando.Length);
            Assert.Equal(2, resultExpando[0][P1]);
        }

        [Fact]
        public void TotalSummary() {
            var loadOptions = new SampleLoadOptions {
                TotalSummary = new[] {
                    new SummaryInfo { Selector = P1, SummaryType = "sum" }
                }
            };

            var resultObject = (DataSourceLoadResult)DataSourceLoader.Load(
                new object[] {
                    CreateExpando(1),
                    CreateExpando(2)
                },
                loadOptions
            );

            var resultExpando = (DataSourceLoadResult)DataSourceLoader.Load(
                new[] {
                    CreateExpando(1),
                    CreateExpando(2)
                },
                loadOptions
            );

            Assert.Equal(3m, resultObject.summary[0]);
            Assert.Equal(3m, resultExpando.summary[0]);
        }

    }

}
