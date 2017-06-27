using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EFCore {
    public class Bug120 {

        public class DataItem {
            [Key]
            public long ID { get; set; }
        }

        public class TestContext : DbContext {
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
                if(!optionsBuilder.IsConfigured)
                    optionsBuilder.UseInMemoryDatabase(nameof(Bug120));
            }

            public DbSet<DataItem> Data { get; set; }
        }

        [Fact]
        public void Scenario() {
            using(var context = new TestContext()) {
                var result = DataSourceLoader.Load(context.Data, new SampleLoadOptions { RequireTotalCount = true });
                Assert.Equal(0, result.totalCount);
            }
        }

    }
}
