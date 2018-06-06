using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EFCore1 {

    public class Bug120 {

        [Table(nameof(Bug120) + "_" + nameof(DataItem))]
        public class DataItem {
            public long ID { get; set; }
        }

        [Fact]
        public void Scenario() {
            TestDbContext.Exec(context => {
                var dbSet = context.Set<DataItem>();
                var result = DataSourceLoader.Load(dbSet, new SampleLoadOptions { RequireTotalCount = true });
                Assert.Equal(0, result.totalCount);
            });
        }

    }
}
