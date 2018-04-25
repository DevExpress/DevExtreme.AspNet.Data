using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Tests.EF6 {

    partial class TestDbContext : DbContext {
        static readonly object LOCK = new object();
        static readonly string DB_FILE_PATH = Path.Combine(Path.GetTempPath(), typeof(TestDbContext).Assembly.GetName().Name + ".mdf");
        static TestDbContext INSTANCE;

        private TestDbContext()
            : base($"Data Source=(localdb)\\MSSQLLocalDB; AttachDbFileName={DB_FILE_PATH}; Integrated Security=True") {
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
