using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EFCore1 {
    using DataItem = Bug120_DataItem;

    class Bug120_DataItem {
        public long ID { get; set; }
    }

    public class Bug120 {

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
