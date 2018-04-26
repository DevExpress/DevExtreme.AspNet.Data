using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

namespace DevExtreme.AspNet.Data.Tests.EFCore1 {

    partial class TestDbContext : DbContext {
        static readonly object LOCK = new object();
        static TestDbContext INSTANCE;

        static readonly string DB_NAME = typeof(TestDbContext).GetTypeInfo().Assembly.GetName().Name;
        static readonly string DB_FILE_PATH = Path.Combine(Path.GetTempPath(), DB_NAME + ".mdf");


        private TestDbContext() {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options) {
            options.UseSqlServer($"Data Source=(localdb)\\MSSQLLocalDB; AttachDbFileName={DB_FILE_PATH}; Initial Catalog={DB_NAME}");
        }

        public static void Exec(Action<TestDbContext> action) {
            lock(LOCK) {
                if(INSTANCE == null) {
                    ResetTestDatabase();
                    INSTANCE = new TestDbContext();
                    INSTANCE.Database.EnsureCreated();
                }

                action(INSTANCE);
            }
        }

        static void ResetTestDatabase() {
            // Possibly related: https://stackoverflow.com/a/46142857

            using(var conn = new SqlConnection($"Data Source=(localdb)\\MSSQLLocalDB")) {
                conn.Open();

                void Exec(string sql) {
                    using(var cmd = conn.CreateCommand()) {
                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                    }
                }


                try {
                    Exec($"drop database [{DB_NAME}]");
                } catch {
                }

                Exec($"create database [{DB_NAME}] on (name='{DB_NAME}', filename='{DB_FILE_PATH}')");
            }
        }
    }

}
