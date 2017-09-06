using DevExtreme.AspNet.Data.Aggregation;
using DevExtreme.AspNet.Data.ResponseModel;
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
                new Group {
                    items = new object[] { 1, 5 }
                },
                new Group {
                    items = new object[] { 7 }
                }
            };

            var calculator = new AggregateCalculator<int>(data, new DefaultAccessor<int>(),
                new[] { new SummaryInfo { Selector = "this", SummaryType = "sum" } },
                new[] { new SummaryInfo { Selector = "this", SummaryType = "sum" } }
            );

            var totals = calculator.Run();

            Assert.Equal(13M, totals[0]);
            Assert.Equal(6M, data[0].summary[0]);
            Assert.Equal(7M, data[1].summary[0]);
        }

        void AssertCalculation(object[] data, object expectedSum, object expectedMin, object expectedMax, object expectedAvg, object expectedCount) {
            /*
                SQL script to validate

                drop table if exists t1;

                #create table t1 (a int);
                #insert into t1 (a) values (1), (3), (5);

                #create table t1 (a varchar(32));
                #insert into t1 (a) values ('a'), ('z');

                #insert into t1 (a) values (null);

                select concat(
                    " sum=",   coalesce(sum(a),   'N'),
                    " min=",   coalesce(min(a),   'N'),
                    " max=",   coalesce(max(a),   'N'),
                    " avg=",   coalesce(avg(a),   'N'),
                    " count=", coalesce(count(*), 'N')
                ) from t1;

            */

            var summaries = new[] {
                new SummaryInfo { Selector = "this", SummaryType = "sum" },
                new SummaryInfo { Selector = "this", SummaryType = "min" },
                new SummaryInfo { Selector = "this", SummaryType = "max" },
                new SummaryInfo { Selector = "this", SummaryType = "avg" },
                new SummaryInfo { SummaryType = "count" },
            };

            var totals = new AggregateCalculator<object>(data, new DefaultAccessor<object>(), summaries, null).Run();

            Assert.Equal(expectedSum, totals[0]);
            Assert.Equal(expectedMin, totals[1]);
            Assert.Equal(expectedMax, totals[2]);
            Assert.Equal(expectedAvg, totals[3]);
            Assert.Equal(expectedCount, totals[4]);
        }

        [Fact]
        public void Calculation_Empty() {
            AssertCalculation(
                new object[0],
                // SQL: sum=N min=N max=N avg=N count=0

                expectedSum: null,
                expectedMin: null,
                expectedMax: null,
                expectedAvg: null,
                expectedCount: 0
            );
        }

        [Fact]
        public void Calculation_NoNulls() {
            AssertCalculation(
                new object[] { 1, 3, 5 },
                // SQL: sum=9 min=1 max=5 avg=3.0000 count=3

                expectedSum: 9M,
                expectedMin: 1,
                expectedMax: 5,
                expectedAvg: 3M,
                expectedCount: 3
            );
        }

        [Fact]
        public void Calculation_Nulls() {
            // "Summaries of the count type do not skip empty values regardless of the skipEmptyValues"
            // http://js.devexpress.com/Documentation/ApiReference/UI_Widgets/dxDataGrid/Configuration/summary/totalItems/?version=15_2#skipEmptyValues

            AssertCalculation(
                new object[] { null },
                // SQL:  sum=N min=N max=N avg=N count=1

                expectedSum: null,
                expectedMin: null,
                expectedMax: null,
                expectedAvg: null,
                expectedCount: 1
            );

            AssertCalculation(
                new object[] { 1, 3, 5, null },
                // SQL: sum=9 min=1 max=5 avg=3.0000 count=4

                expectedSum: 9M,
                expectedMin: 1,
                expectedMax: 5,
                expectedAvg: 3M,
                expectedCount: 4
            );
        }

        [Fact]
        public void Calculation_Strings() {
            AssertCalculation(
                new object[] { "a", "z", null },
                // SQL: sum=0 min=a max=z avg=0 count=3

                expectedSum: 0M,
                expectedMin: "a",
                expectedMax: "z",
                expectedAvg: 0M,
                expectedCount: 3
            );
        }

        [Fact]
        public void Calculation_NonComparable() {
            AssertCalculation(
                new[] { new object() },

                // NOTE sum and avg by analogy with strings case
                expectedSum: 0M,
                expectedMin: null,
                expectedMax: null,
                expectedAvg: 0M,
                expectedCount: 1
            );
        }

        [Fact]
        public void NestedGroups() {
            var data = new[] {
                new Group {
                    items = new[] {
                        new Group {
                            items = new object[] { 1, 5 }
                        }
                    }
                }
            };

            var calculator = new AggregateCalculator<int>(data, new DefaultAccessor<int>(), null, new[] {
                new SummaryInfo { Selector = "this", SummaryType = "sum" }
            });

            var totals = calculator.Run();
            Assert.Null(totals);

            Assert.Equal(6M, data[0].summary[0]);
            Assert.Equal(6M, (data[0].items[0] as Group).summary[0]);
        }

        [Fact]
        public void IgnoreGroupSummaryIfNoGroups() {
            var data = new object[] { 1 };

            var calculator = new AggregateCalculator<int>(data, new DefaultAccessor<int>(), null, new[] {
                new SummaryInfo { Selector = "ignore me", SummaryType = "ignore me" }
            });

            calculator.Run();
        }

        [Fact]
        public void Issue147() {
            var data = Enumerable.Repeat(Double.MaxValue / 3, 2);

            var calculator = new AggregateCalculator<double>(data, new DefaultAccessor<double>(),
                new[] { new SummaryInfo { Selector = "this", SummaryType = "sum" } },
                null
            );

            Assert.Equal(data.Sum(), calculator.Run()[0]);
        }

    }

}
