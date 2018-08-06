using System;
using System.Linq;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EF6 {
    using DataItem = RemoteGroupingStress_DataItem;

    class RemoteGroupingStress_DataItem : RemoteGroupingStressHelper.IEntity {
        public int ID { get; set; }
        public int Num { get; set; }
        public int? NullNum { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public DateTime? NullDate { get; set; }
    }

    public class RemoteGroupingStress {

        [Fact]
        public void Scenario() {
            TestDbContext.Exec(context => {
                var dbSet = context.Set<DataItem>();

                dbSet.Add(new DataItem());
                context.SaveChanges();

                RemoteGroupingStressHelper.Run(dbSet);
            });
        }

    }

}
