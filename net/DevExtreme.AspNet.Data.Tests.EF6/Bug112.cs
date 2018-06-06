using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EF6 {
    using DataItem = Bug112_DataItem;

    class Bug112_DataItem {
        public int ID { get; set; }
        public TimeSpan? Duration { get; set; }
    }

    public class Bug112 {

        [Fact]
        public void Scenario() {
            TestDbContext.Exec(context => {
                var dbSet = context.Set<DataItem>();

                dbSet.AddRange(new[] {
                    new DataItem { Duration = TimeSpan.Parse("01:23") },
                    new DataItem { Duration = TimeSpan.Parse("02:23") },
                    new DataItem { Duration = TimeSpan.Zero }
                });

                context.SaveChanges();

                var loadResult = DataSourceLoader.Load(dbSet, new SampleLoadOptions {
                    Filter = new[] { "Duration", "contains", "23" }
                });

                var data = (IEnumerable<DataItem>)loadResult.data;
                Assert.Equal(2, data.Count());
            });
        }

    }

}
