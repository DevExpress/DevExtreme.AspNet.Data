using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EFCore {

    class MsSqlGroupBy_DataItem {
        public int ID { get; set; }
        public int Group { get; set; }
    }

    partial class TestDbContext {
        public DbSet<MsSqlGroupBy_DataItem> MsSqlGroupBy_Data { get; set; }
    }

    public class MsSqlGroupBy {

        [Fact]
        public void Scenario() {
            TestDbContext.Exec(context => {
                var dbSet = context.MsSqlGroupBy_Data;

                dbSet.AddRange(
                    new MsSqlGroupBy_DataItem { Group = 1 },
                    new MsSqlGroupBy_DataItem { Group = 1 }
                );

                context.SaveChanges();

#warning TODO
            });
        }

    }

}
