using DevExtreme.AspNet.Data.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

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
            Assert.Single(groups);
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
            Assert.Single(result.data.Cast<object>());
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

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Load_EmptySummary(bool remoteGrouping) {
            var data = new[] {
                new { g = 1 }
            };

            var loadOptions = new SampleLoadOptions {
                RemoteGrouping = remoteGrouping,
                Group = new[] {
                    new GroupingInfo { Selector = "g", IsExpanded = false }
                }
            };

            void Run() {
                var loadResult = DataSourceLoader.Load(data, loadOptions);
                Assert.Null(loadResult.summary);
                Assert.Null(loadResult.data.Cast<Group>().First().summary);
            }

            Assert.Null(loadOptions.TotalSummary);
            Assert.Null(loadOptions.GroupSummary);
            Run();

            loadOptions.TotalSummary = Array.Empty<SummaryInfo>();
            loadOptions.GroupSummary = Array.Empty<SummaryInfo>();
            Run();
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

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Load_Select(bool remoteSelect) {
            var data = new[] {
                new { f1 = 1, f2 = 2 }
            };

            var loadOptions = new SampleLoadOptions {
                Select = new[] { "f2" },
                RemoteSelect = remoteSelect
            };

            var item = DataSourceLoader.Load(data, loadOptions).data.Cast<IDictionary<string, object>>().First();

            Assert.Equal(1, item.Keys.Count);
            Assert.Equal(2, item["f2"]);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Load_SelectWithGrouping(bool remoteSelect) {
            var data = new[] {
                new { g = 1, f = 1, waste = "any" }
            };

            var loadOptions = new SampleLoadOptions {
                Select = new[] { "g", "f" },
                RemoteSelect = remoteSelect,
                Group = new[] {
                    new GroupingInfo { Selector = "g" }
                }
            };

            var groups = (IList<Group>)DataSourceLoader.Load(data, loadOptions).data;
            var item = (IDictionary<string, object>)groups[0].items[0];

            Assert.Equal(2, item.Keys.Count);
            Assert.True(item.ContainsKey("g"));
            Assert.True(item.ContainsKey("f"));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Load_SelectWithPaging(bool remoteSelect) {
            var data = new[] {
                new { f = 1 },
                new { f = 2 }
            };

            var loadOptions = new SampleLoadOptions {
                Select = new[] { "f" },
                RemoteSelect = remoteSelect,
                Skip = 1,
                Take = 1
            };

            var x = Record.Exception(delegate {
                DataSourceLoader.Load(data, loadOptions);
            });

            Assert.Null(x);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Load_SelectNested(bool remoteSelect) {
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
                Select = new[] { "Name", "Address.City", "Address.Street.Line1", "Contacts.Email" },
                RemoteSelect = remoteSelect
            };

            var item = DataSourceLoader.Load(data, loadOptions).data.Cast<IDictionary<string, object>>().First();

            var address = (IDictionary<string, object>)item["Address"];
            var addressStreet = (IDictionary<string, object>)address["Street"];
            var contacts = (IDictionary<string, object>)item["Contacts"];

            Assert.Equal(3, item.Keys.Count);
            Assert.Equal(2, address.Keys.Count);
            Assert.Equal(1, addressStreet.Keys.Count);
            Assert.Equal(1, contacts.Keys.Count);

            Assert.Equal(data[0].Name, item["Name"]);
            Assert.Equal(data[0].Address.City, address["City"]);
            Assert.Equal(data[0].Address.Street.Line1, addressStreet["Line1"]);
            Assert.Equal(data[0].Contacts.Email, contacts["Email"]);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Load_SelectWithConflict(bool remoteSelect) {
            var result = DataSourceLoader.Load(
                new[] {
                    Tuple.Create(Tuple.Create("abc"))
                },
                new SampleLoadOptions {
                    Select = new[] { "Item1.Item1.Length", "Item1", "Item1.Item1.Length" },
                    RemoteSelect = remoteSelect
                }
            );

            var item = result.data.Cast<IDictionary<string, object>>().First();
            Assert.Equal(1, item.Keys.Count);
            Assert.True(item.ContainsKey("Item1"));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Load_SelectWithSummary_NoDoubleEnumeration(bool remoteSelect) {
            var data = new[] {
                new { f = 1 }
            };

            var loadOptions = new SampleLoadOptions {
                Select = new[] { "f" },
                RemoteSelect = remoteSelect,
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
        [InlineData(false)]
        [InlineData(true)]
        public void Load_PreSelect(bool remoteSelect) {
            var data = new[] {
                new { a = 1, b = 2, c = 3 }
            };

            IDictionary<string, object> Load(string[] preSelect, string[] select) {
                var loadResult = DataSourceLoader.Load(data, new SampleLoadOptions {
                    PreSelect = preSelect,
                    Select = select,
                    RemoteSelect = remoteSelect
                });

                return loadResult.data.Cast<IDictionary<string, object>>().First();
            }

            var item = Load(
                preSelect: new[] { "a", "b" },
                select: null
            );

            Assert.Equal(2, item.Keys.Count);
            Assert.True(item.ContainsKey("a"));
            Assert.True(item.ContainsKey("b"));

            item = Load(
                preSelect: new[] { "a", "b" },
                select: new[] { "b", "c" }
            );

            Assert.Equal(1, item.Keys.Count);
            Assert.True(item.ContainsKey("b"));
        }

        [Fact]
        public void Load_PreSelect_CI() {
            // https://github.com/DevExpress/DevExtreme.AspNet.Data/pull/400

            var loadOptions = new SampleLoadOptions {
                Select = new[] { "prop" },
                PreSelect = new[] { "Prop" }
            };

            var loadResult = DataSourceLoader.Load(new[] { new { Prop = 123 } }, loadOptions);
            var items = loadResult.data.Cast<IDictionary<string, object>>().ToArray();

            Assert.Equal(123, items.First()["Prop"]);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(null)]
        public void Load_Select_NoAnonTypeLimits(bool? remoteSelect) {
            var loadResult = DataSourceLoader.Load(new[] { "a" }, new SampleLoadOptions {
                Select = Enumerable.Repeat("this", 123).ToArray(),
                RemoteSelect = remoteSelect
            });

            Assert.All(
                loadResult.data.Cast<IDictionary<string, object>>(),
                i => Assert.Equal("a", i["this"])
            );
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

        [Fact]
        public void Load_GridFilter_T616169() {
            var loadOptions = new SampleLoadOptions {
                Filter = new object[] {
                    new object[] { "f", ">", 0 },
                    "or",
                    new object[] { "f", "=", null }
                }
            };

            var data = new[] {
                new { f = 1 }
            };

            var loadResult = DataSourceLoader.Load(data, loadOptions);
            Assert.NotEmpty(loadResult.data);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Issue246(bool remoteSelect) {
            var data = new[] {
                new {
                    Department = new { Title = "abc" }
                }
            };

            var loadOptions = new SampleLoadOptions {
                Select = new[] { "Department.Title" },
                RemoteSelect = remoteSelect,
                Group = new[] {
                    new GroupingInfo { Selector = "Department.Title.Length" }
                }
            };

            var groups = (IList<Group>)DataSourceLoader.Load(data, loadOptions).data;
            Assert.Equal(3, groups[0].key);

            var item = (IDictionary<string, object>)groups[0].items[0];
            var department = (IDictionary<string, object>)item["Department"];
            Assert.Equal("abc", department["Title"]);
        }

        [Fact]
        public void CustomAggregatorWithRemoteGrouping() {
            // https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/341

            try {
                Aggregation.CustomAggregators.RegisterAggregator("my1", typeof(Object));

                var exception = Record.Exception(delegate {
                    DataSourceLoader.Load(new object[0], new SampleLoadOptions {
                        RemoteGrouping = true,
                        TotalSummary = new[] {
                            new SummaryInfo { Selector = "this", SummaryType = "my1" }
                        }
                    });
                });

                Assert.Contains("custom aggregate 'my1' cannot be translated", exception.Message);
            } finally {
                Aggregation.CustomAggregators.Clear();
            }
        }
    }

}
