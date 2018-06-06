using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EF6 {
    using DataItem = Bug235_DataItem;

    class Bug235_DataItem {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public int Prop { get; set; }
    }

    public class Bug235 {

        [Fact]
        public void Scenario() {
            TestDbContext.Exec(context => {
                var dbSet = context.Set<DataItem>();

                dbSet.Add(new DataItem { ID = 1, Prop = 0 });
                dbSet.Add(new DataItem { ID = 2, Prop = 2 });
                dbSet.Add(new DataItem { ID = 0, Prop = 1 });
                context.SaveChanges();

                var projection = dbSet.Select(i => new { i.ID, i.Prop });

                // Default settings: auto-sort by first sortable member (ID)
                {
                    var loadResult = DataSourceLoader.Load(projection, new SampleLoadOptions {
                        Skip = 1,
                        Take = 99
                    });

                    var data = ImplicitCast(projection, loadResult.data);

                    Assert.Equal(
                        new[] { 1, 2 },
                        data.Select(i => i.ID)
                    );
                }

                // User-provided DefaultSort
                {
                    var loadResult = DataSourceLoader.Load(projection, new SampleLoadOptions {
                        Skip = 1,
                        Take = 999,
                        DefaultSort = "Prop"
                    });

                    var data = ImplicitCast(projection, loadResult.data);

                    Assert.Equal(
                        new[] { 1, 2 },
                        data.Select(i => i.Prop)
                    );
                }
            });
        }

        IEnumerable<T> ImplicitCast<T>(IEnumerable<T> typeReference, IEnumerable data) {
            return data.Cast<T>();
        }

    }

}
