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

        private TestDbContext(string connectionString)
            : base(connectionString) {
        }

        public static void Exec(Action<TestDbContext> action) {
            lock(LOCK) {
                if(INSTANCE == null) {
                    var helper = new SqlServerTestDbHelper("DevExtreme_AspNet_Data_Tests_EF6_DB");
                    helper.ResetDatabase();

                    INSTANCE = new TestDbContext(helper.ConnectionString);
                    INSTANCE.Database.CreateIfNotExists();
                }

                action(INSTANCE);
            }
        }

    }

}
