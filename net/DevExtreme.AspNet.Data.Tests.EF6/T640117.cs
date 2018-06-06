﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EF6 {

    class T640117_ParentItem {

        public T640117_ParentItem() {
            Children = new HashSet<T640117_ChildItem>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        [ForeignKey("ParentID")]
        public ICollection<T640117_ChildItem> Children { get; set; }
    }

    class T640117_ChildItem {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        public int? ParentID { get; set; }

        public T640117_ParentItem Parent { get; set; }
    }

    partial class TestDbContext {
        public DbSet<T640117_ParentItem> T640117_Parents { get; set; }
        public DbSet<T640117_ChildItem> T640117_Children { get; set; }
    }

    public class T640117 {

        [Fact]
        public void Scenario() {
            TestDbContext.Exec(context => {
                var parent = new T640117_ParentItem { ID = 123 };
                var child = new T640117_ChildItem { ID = 1 };
                var orphan = new T640117_ChildItem { ID = 2 };

                parent.Children.Add(child);
                context.T640117_Parents.Add(parent);
                context.T640117_Children.Add(orphan);
                context.SaveChanges();

                {
                    var loadResut = DataSourceLoader.Load(context.T640117_Children, new SampleLoadOptions {
                        Select = new[] { "Parent.ID" },
                        Sort = new[] {
                            new SortingInfo { Selector = "Parent.ID" }
                        }
                    });

                    dynamic items = loadResut.data.Cast<object>().ToArray();
                    Assert.Null(items[0].Parent.ID);
                    Assert.Equal(123, items[1].Parent.ID);
                }

                {
                    var loadResult = DataSourceLoader.Load(context.T640117_Children, new SampleLoadOptions {
                        Filter = new[] { "Parent.ID", null }
                    });

                    Assert.Single(loadResult.data);
                }

                {
                    var loadResult = DataSourceLoader.Load(context.T640117_Children, new SampleLoadOptions {
                        Filter = new[] { "Parent.ID", "<>", null }
                    });

                    Assert.Single(loadResult.data);
                }

                {
                    var loadResult = DataSourceLoader.Load(context.T640117_Children, new SampleLoadOptions {
                        Filter = new[] { "ID", null }
                    });

                    Assert.Empty(loadResult.data);
                }
            });
        }
    }
}


