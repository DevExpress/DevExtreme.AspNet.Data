using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Tests.EF6 {

    partial class TestDbContext : DbContext {
        static readonly object LOCK = new object();
        static TestDbContext INSTANCE;

        private TestDbContext(params Type[] knownEntities)
            : base($"Data Source=(localdb)\\MSSQLLocalDB; Initial Catalog={typeof(TestDbContext).Assembly.GetName().Name}; Integrated Security=True") {
        }

        public static void Exec(Action<TestDbContext> action) {
            lock(LOCK) {
                if(INSTANCE == null) {
                    INSTANCE = new TestDbContext();
                    INSTANCE.Database.Delete();
                    INSTANCE.Database.CreateIfNotExists();
                }

                action(INSTANCE);
            }
        }

    }

}
