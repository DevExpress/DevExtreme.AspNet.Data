using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EFCore {
    public class Bug326 {

        public abstract class EntityBase {
            public int ID { get; set; }
        }

        [Table(nameof(Bug326) + "_" + nameof(Entity))]
        public class Entity : EntityBase {
            public string Prop { get; set; }
        }

        public class DTO : EntityBase {
            public string Prop { get; set; }
        }

        [Fact]
        public async Task Scenario() {
            await TestDbContext.ExecAsync(context => {
                var dtoQuery = context.Set<Entity>().Select(i => new DTO { ID = i.ID, Prop = i.Prop });

                Assert.Null(Record.Exception(delegate {
                    var loadResult = DataSourceLoader.Load(dtoQuery, new SampleLoadOptions {
                        Take = 1
                    });

                    loadResult.data.Cast<object>().ToArray();
                }));
            });
        }

    }

}
