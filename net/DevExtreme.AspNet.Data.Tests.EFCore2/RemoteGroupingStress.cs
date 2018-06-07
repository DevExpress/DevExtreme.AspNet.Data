using System;
using System.ComponentModel.DataAnnotations.Schema;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EFCore2 {

    public class RemoteGroupingStress {

        [Table(nameof(RemoteGroupingStress) + "_" + nameof(DataItem))]
        public class DataItem : RemoteGroupingStressHelper.IEntity {
            public int ID { get; set; }
            public int Num { get; set; }
            public int? NullNum { get; set; }
            public DateTime Date { get; set; }
            public DateTime? NullDate { get; set; }
        }

        [Fact]
        public void Scenario() {
            TestDbContext.Exec(context => {
                var dbSet = context.Set<DataItem>();

                dbSet.Add(new DataItem());
                context.SaveChanges();

                Assert.Null(Record.Exception(delegate {
                    RemoteGroupingStressHelper.Run(dbSet);
                }));
            });
        }

    }

}
