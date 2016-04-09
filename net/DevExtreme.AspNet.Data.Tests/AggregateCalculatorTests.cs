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
        public void Calculation() {
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

            {
                var data = new object[0];
                var totals = new AggregateCalculator<object>(data, new Accessor<object>(), summaries, null).Run();
                // SQL: sum=N min=N max=N avg=N count=0

                Assert.Null(totals[0]);
                Assert.Null(totals[1]);
                Assert.Null(totals[2]);
                Assert.Null(totals[3]);
                Assert.Equal(0, totals[4]);
            }

            {
                var data = new object[] { null };
                var totals = new AggregateCalculator<object>(data, new Accessor<object>(), summaries, null).Run();
                // SQL:  sum=N min=N max=N avg=N count=1

                Assert.Null(totals[0]);
                Assert.Null(totals[1]);
                Assert.Null(totals[2]);
                Assert.Null(totals[3]);

                // "Summaries of the count type do not skip empty values regardless of the skipEmptyValues"
                // http://js.devexpress.com/Documentation/ApiReference/UI_Widgets/dxDataGrid/Configuration/summary/totalItems/?version=15_2#skipEmptyValues
                Assert.Equal(1, totals[4]);
            }

            {
                var data = new object[] { 1, 3, 5 };
                var totals = new AggregateCalculator<object>(data, new Accessor<object>(), summaries, null).Run();
                // SQL: sum=9 min=1 max=5 avg=3.0000 count=3

                Assert.Equal(9M, totals[0]);
                Assert.Equal(1, totals[1]);
                Assert.Equal(5, totals[2]);
                Assert.Equal(3M, totals[3]);
                Assert.Equal(3, totals[4]);
            }

            {
                var data = new object[] { 1, 3, 5, null };
                var totals = new AggregateCalculator<object>(data, new Accessor<object>(), summaries, null).Run();
                // SQL: sum=9 min=1 max=5 avg=3.0000 count=4

                Assert.Equal(9M, totals[0]);
                Assert.Equal(1, totals[1]);
                Assert.Equal(5, totals[2]);
                Assert.Equal(3M, totals[3]);
                Assert.Equal(4, totals[4]);
            }

            {
                var data = new object[] { "a", "z", null };
                var totals = new AggregateCalculator<object>(data, new Accessor<object>(), summaries, null).Run();
                // SQL: sum=0 min=a max=z avg=0 count=3

                Assert.Equal(0M, totals[0]);
                Assert.Equal("a", totals[1]);
                Assert.Equal("z", totals[2]);
                Assert.Equal(0M, totals[3]);
                Assert.Equal(3, totals[4]);
            }
        }

        [Fact]
        public void NestedGroups() {
            var data = new[] {
                new DevExtremeGroup {
                    items = new[] {
                        new DevExtremeGroup {
                            items = new object[] { 1, 5 }
                        }
                    }
                }
            };

            var calculator = new AggregateCalculator<int>(data, new Accessor<int>(), null, new[] {
                new SummaryInfo { Selector = "this", SummaryType = "sum" }
            });

            var totals = calculator.Run();
            Assert.Null(totals);

            Assert.Equal(6M, data[0].Summary[0]);
            Assert.Equal(6M, (data[0].items[0] as DevExtremeGroup).Summary[0]);
        }

    }

}
