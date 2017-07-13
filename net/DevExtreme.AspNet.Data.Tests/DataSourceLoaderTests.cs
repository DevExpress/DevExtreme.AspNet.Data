using DevExtreme.AspNet.Data.ResponseModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

#if NET40
using Xunit.Extensions;
#endif

namespace DevExtreme.AspNet.Data.Tests {

    public class DataSourceLoaderTests {

        [Fact]
        public void TotalCount() {
            var data = new[] { 1, 3, 2 };
            var options = new SampleLoadOptions {
                Filter = new object[] { "this", "<>", 2 },
                Take = 1,
                IsCountQuery = true
            };

            Assert.Equal(2, DataSourceLoader.Load(data, options).totalCount);
        }

        [Fact]
        public void Load_NoRequireTotalCount() {
            var data = new[] { 1, 3, 5, 2, 4 };
            var options = new SampleLoadOptions {
                Skip = 1,
                Take = 2,
                Filter = new object[] { "this", "<>", 2 },
                Sort = new[] {
                    new SortingInfo { Selector = "this" }
                },
                RequireTotalCount = false
            };

            Assert.Equal(new[] { 3, 4 }, DataSourceLoader.Load(data, options).data.Cast<int>());
        }

        [Fact]
        public void Load_RequireTotalCount() {
            var data = new[] { 1, 3, 5, 2, 4 };
            var options = new SampleLoadOptions {
                Skip = 1,
                Take = 2,
                Filter = new object[] { "this", "<>", 2 },
                Sort = new[] {
                    new SortingInfo { Selector = "this" }
                },
                RequireTotalCount = true
            };

            var result = DataSourceLoader.Load(data, options);

            Assert.Equal(4, result.totalCount);
            Assert.Equal(new[] { 3, 4 }, result.data.Cast<int>());
        }

        [Fact]
        public void Load_GroupOnly() {
            var data = new[] {
                new {
                    G1 = 1,
                    G2 = 2
                }
            };

            var result = DataSourceLoader.Load(data, new SampleLoadOptions {
                Group = new[] {
                    new GroupingInfo { Selector = "G1" },
                    new GroupingInfo { Selector = "G2" }
                }
            });

            var g1 = (result.data as IEnumerable<Group>).First();
            var g2 = g1.items[0] as Group;

            Assert.Equal(1, g1.key);
            Assert.Equal(2, g2.key);
            Assert.Same(data[0], g2.items[0]);
        }

        [Fact]
        public void Load_GroupWithOtherOptions() {
            var data = new[] {
                new { g = "g2", a = 1 },
                new { g = "g2", a = 2 }, // filtered
                new { g = "g2", a = 3 },
                new { g = "g1", a = 0 }  // skipped
            };

            var result = DataSourceLoader.Load(data, new SampleLoadOptions {
                Filter = new[] { "a", "<>", "2" },
                Sort = new[] { new SortingInfo { Selector = "a", Desc = true } },
                Group = new[] { new GroupingInfo { Selector = "g" } },
                Skip = 1,
                Take = 1,
                RequireTotalCount = true
            });

            Assert.Equal(3, result.totalCount);

            var groups = result.data.Cast<Group>().ToArray();
            Assert.Equal(1, groups.Length);
            Assert.Equal(2, groups[0].items.Count);
            Assert.Same(data[2], groups[0].items[0]);
            Assert.Same(data[0], groups[0].items[1]);
        }

        [Fact]
        public void Load_GroupIsExpandedFalse() {
            var data = new[] {
                new { g1 = 1, g2 = 2 },
                new { g1 = 1, g2 = 2 }
            };

            var loadResult = DataSourceLoader.Load(data, new SampleLoadOptions {
                Group = new[] {
                    new GroupingInfo { Selector = "g1", IsExpanded = false },
                    new GroupingInfo { Selector = "g2", IsExpanded = false }
                }
            });

            var groups = (IList<Group>)loadResult.data;
            var nestedGroup = (groups[0].items[0] as Group);
            Assert.Equal(2, nestedGroup.count);
            Assert.Null(nestedGroup.items);
        }

        [Fact]
        public void Load_TotalSummary() {
            var data = new[] { 1, 2, 3 };

            var result = DataSourceLoader.Load(data, new SampleLoadOptions {
                TotalSummary = new[] {
                    new SummaryInfo { Selector = "this", SummaryType = "min" },
                    new SummaryInfo { Selector = "this", SummaryType = "max" }
                }
            });

            Assert.Equal(1, result.summary[0]);
            Assert.Equal(3, result.summary[1]);
        }

        [Fact]
        public void Load_GroupSummary() {
            var data = new[] {
                new { g = 1, value = 1 },
                new { g = 1, value = 2 },
                new { g = 2, value = 10 },
                new { g = 2, value = 20 }
            };

            var loadResult = DataSourceLoader.Load(data, new SampleLoadOptions {
                Group = new[] {
                    new GroupingInfo { Selector = "g" }
                },
                GroupSummary = new[] {
                    new SummaryInfo { Selector = "value", SummaryType = "sum" }
                }
            });

            var groups = (IList<Group>)loadResult.data;
            Assert.Equal(3M, groups[0].summary[0]);
            Assert.Equal(30M, groups[1].summary[0]);
        }

        [Fact]
        public void Load_TotalSummaryAndPaging() {
            var data = new[] { 1, 3, 5 };

            var result = DataSourceLoader.Load(data, new SampleLoadOptions {
                Skip = 1,
                Take = 1,
                TotalSummary = new[] {
                    new SummaryInfo { Selector = "this", SummaryType = "sum" }
                }
            });

            Assert.Equal(9M, result.summary[0]);
            Assert.Equal(1, result.data.Cast<object>().Count());
        }

        [Fact]
        public void Load_SummaryAndIsExpandedFalse() {
            var data = new[] {
                new { g = 1, value = 1 },
                new { g = 1, value = 2 }
            };

            var loadResult = DataSourceLoader.Load(data, new SampleLoadOptions {
                Group = new[] {
                    new GroupingInfo { Selector = "g", IsExpanded = false }
                },
                GroupSummary = new[] {
                    new SummaryInfo { Selector = "value",  SummaryType = "sum" }
                }
            });

            var groups = (IList<Group>)loadResult.data;
            Assert.Equal(3M, groups[0].summary[0]);
            Assert.Null(groups[0].items);
            Assert.Equal(2, groups[0].count);
        }

        [Fact]
        public void RequireGroupCountWhenGroupsAreExpanded() {
            var data = new[] {
                new { a = 1 },
                new { a = 2 },
                new { a = 2 }
            };

            var loadOptions = new SampleLoadOptions {
                RequireGroupCount = true,
                Group = new[] {
                    new GroupingInfo { Selector = "a", IsExpanded = true }
                },
                Skip = 1,
                Take = 1
            };

            var result = DataSourceLoader.Load(data, loadOptions);

            Assert.Equal(2, result.groupCount);
        }

        [Fact]
        public void Load_Select() {
            var data = new[] {
                new { f1 = 1, f2 = 2 }
            };

            var loadOptions = new SampleLoadOptions {
                Select = new[] { "f2" }
            };

            var item = DataSourceLoader.Load(data, loadOptions).data.Cast<IDictionary>().First();

            Assert.Equal(1, item.Keys.Count);
            Assert.Equal(2, item["f2"]);
        }

        [Fact]
        public void Load_SelectWithGrouping() {
            var data = new[] {
                new { g = 1, f = 1, waste = "any" }
            };

            var loadOptions = new SampleLoadOptions {
                Select = new[] { "g", "f" },
                Group = new[] {
                    new GroupingInfo { Selector = "g" }
                }
            };

            var groups = (IList<Group>)DataSourceLoader.Load(data, loadOptions).data;
            var item = (IDictionary)groups[0].items[0];

            Assert.Equal(2, item.Keys.Count);
            Assert.True(item.Contains("g"));
            Assert.True(item.Contains("f"));
        }

        [Fact]
        public void Load_SelectWithPaging() {
            var data = new[] {
                new { f = 1 },
                new { f = 2 }
            };

            var loadOptions = new SampleLoadOptions {
                Select = new[] { "f" },
                Skip = 1,
                Take = 1
            };

            var x = Record.Exception(delegate {
                DataSourceLoader.Load(data, loadOptions);
            });

            Assert.Null(x);
        }

        [Fact]
        public void Load_SelectNested() {
            var data = new[] {
                new {
                    Name = "Alex",
                    Address = new {
                        Zip = "89104",
                        Street = new {
                            Line1 = "2000 S Las Vegas Blvd",
                            Line2 = ""
                        },
                        City = "Las Vegas"
                    },
                    Contacts = new {
                        Phone = "phone",
                        Email = "email"
                    },
                    Waste = ""
                }
            };

            var loadOptions = new SampleLoadOptions {
                Select = new[] { "Name", "Address.City", "Address.Street.Line1", "Contacts.Email" }
            };

            var item = DataSourceLoader.Load(data, loadOptions).data.Cast<IDictionary>().First();

            var address = (IDictionary)item["Address"];
            var addressStreet = (IDictionary)address["Street"];
            var contacts = (IDictionary)item["Contacts"];

            Assert.Equal(3, item.Keys.Count);
            Assert.Equal(2, address.Keys.Count);
            Assert.Equal(1, addressStreet.Keys.Count);
            Assert.Equal(1, contacts.Keys.Count);

            Assert.Equal(data[0].Name, item["Name"]);
            Assert.Equal(data[0].Address.City, address["City"]);
            Assert.Equal(data[0].Address.Street.Line1, addressStreet["Line1"]);
            Assert.Equal(data[0].Contacts.Email, contacts["Email"]);
        }

        [Fact]
        public void Load_SelectWithConflict() {
            var result = DataSourceLoader.Load(
                new[] {
                    Tuple.Create(Tuple.Create("abc"))
                },
                new SampleLoadOptions {
                    Select = new[] { "Item1.Item1.Length", "Item1", "Item1.Item1.Length" }
                }
            );

            var item = result.data.Cast<IDictionary>().First();
            Assert.Equal(1, item.Keys.Count);
            Assert.True(item.Contains("Item1"));
        }

        [Fact]
        public void Load_SelectWithSummary_NoDoubleEnumeration() {
            var data = new[] {
                new { f = 1 }
            };

            var loadOptions = new SampleLoadOptions {
                Select = new[] { "f" },
                TotalSummary = new[] {
                    new SummaryInfo { Selector = "f", SummaryType = "sum" }
                }
            };

            var x = Record.Exception(delegate {
                var loadResult = DataSourceLoader.Load(data, loadOptions);
                loadResult.data.Cast<object>().ToArray();
            });

            Assert.Null(x);
        }

        [Theory]
        [InlineData(false, true)]
        [InlineData(false, false)]
        [InlineData(true, false)]
        public void Issue132(bool remoteGrouping, bool requireTotalCount) {
            var count = 123;
            var data = Enumerable.Repeat(new { }, count);

            var loadOptions = new SampleLoadOptions {
                RemoteGrouping = remoteGrouping,
                RequireTotalCount = requireTotalCount,
                Take = 10,
                TotalSummary = new[] {
                    new SummaryInfo { SummaryType = "count" },
                    new SummaryInfo { SummaryType = "count" }
                }
            };

            var loadResult = DataSourceLoader.Load(data, loadOptions);

            Assert.Contains(".Take(10)", loadOptions.ExpressionLog[0].ToString());
            Assert.Contains(".Count()", loadOptions.ExpressionLog[1].ToString());

            Assert.Equal(count, loadResult.summary[0]);
            Assert.Equal(count, loadResult.summary[1]);
        }
    }

}
