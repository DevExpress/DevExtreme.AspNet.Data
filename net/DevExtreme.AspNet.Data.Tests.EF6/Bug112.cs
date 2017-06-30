using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EF6 {

    class Bug112_DataItem {
        [Key]
        public int ID { get; set; }
        public TimeSpan? Duration { get; set; }
    }

    partial class TestDbContext {
        public DbSet<Bug112_DataItem> Bug112_Data { get; set; }
    }

    public class Bug112 {

        [Fact]
        public void Scenario() {
            TestDbContext.Exec(context => {
                var dbSet = context.Bug112_Data;

                dbSet.AddRange(new[] {
                    new Bug112_DataItem { ID = 1, Duration = TimeSpan.Parse("01:23") },
                    new Bug112_DataItem { ID = 2, Duration = TimeSpan.Parse("02:23") },
                    new Bug112_DataItem { ID = 3, Duration = TimeSpan.Zero }
                });

                context.SaveChanges();

                var loadResult = DataSourceLoader.Load(dbSet, new SampleLoadOptions {
                    Filter = new[] { "Duration", "contains", "23" }
                });

                var data = (IEnumerable<Bug112_DataItem>)loadResult.data;
                Assert.Equal(2, data.Count());
            });
        }

    }

}
