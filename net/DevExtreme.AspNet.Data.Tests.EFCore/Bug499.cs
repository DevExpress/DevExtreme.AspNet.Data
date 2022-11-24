using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EFCore {

    public class Bug499 {

        [Table(nameof(Bug499) + "_" + nameof(Entity))]
        public class Entity {
            public int ID { get; set; }
            public DayOfWeek Day { get; set; }
            public DayOfWeek? NullableDay { get; set; }
        }

        [Fact]
        public async Task Scenario() {
            await TestDbContext.ExecAsync(context => {
                var dbSet = context.Set<Entity>();

                var entity1 = new Entity {
                    Day = DayOfWeek.Monday,
                    NullableDay = DayOfWeek.Monday,
                };

                var entity6 = new Entity {
                    Day = DayOfWeek.Saturday,
                    NullableDay = DayOfWeek.Saturday,
                };

                dbSet.AddRange(entity1, entity6);
                context.SaveChanges();

                var loadOptions = new SampleLoadOptions {
                    Filter = new[] {
                        new object[] { "Day", ">=", 1 },
                        new object[] { "Day", "<", "saturday" },
                        new object[] { "NullableDay", ">=", DayOfWeek.Monday },
                        new object[] { "NullableDay", "<", "saturday" },
                    }
                };

                var loadResult = DataSourceLoader.Load(dbSet, loadOptions);

                Assert.Equal(
                    new[] { entity1 },
                    loadResult.data
                );
            });
        }

    }
}
