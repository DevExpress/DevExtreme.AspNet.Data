using DevExtreme.AspNet.Data.Aggregation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class AggregateCalculatorTests {

        [Fact]
        public void Minimal() {
            var data = new[] {
                new DevExtremeGroup {
                    items = new object[] { 1, 5 }
                },
                new DevExtremeGroup {
                    items = new object[] { 7 }
                }
            };

            var calculator = new AggregateCalculator<int>(data, new Accessor<int>(),
                new[] { new SummaryInfo { Selector = "this", SummaryType = "sum" } },
                new[] { new SummaryInfo { Selector = "this", SummaryType = "sum" } }
            );

            var totals = calculator.Run();

            Assert.Equal(13M, totals[0]);
            Assert.Equal(6M, data[0].Summary[0]);
            Assert.Equal(7M, data[1].Summary[0]);
        }

        [Fact]
        public void SummaryTypes() {
            var data = new object[] { 1, 3, 5 };
            var calculator = new AggregateCalculator<int>(data, new Accessor<int>(), new[] {
                new SummaryInfo { Selector = "this", SummaryType = "sum" },
                new SummaryInfo { Selector = "this", SummaryType = "min" },
                new SummaryInfo { Selector = "this", SummaryType = "max" },
                new SummaryInfo { Selector = "this", SummaryType = "avg" },
                new SummaryInfo { SummaryType = "count" },
            }, null);
            
            var totals = calculator.Run();

            Assert.Equal(9M, totals[0]);
            Assert.Equal(1, totals[1]);
            Assert.Equal(5, totals[2]);
            Assert.Equal(3M, totals[3]);
            Assert.Equal(3, totals[4]);
        }

    }

}
