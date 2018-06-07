using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text;

namespace DevExtreme.AspNet.Data.Tests {

    public class SqlServerTestDbHelper {
        const string LOCAL_DB = "(localdb)\\MSSQLLocalDB";

        readonly string _dbName;
        readonly string _dbFilePath;

        public SqlServerTestDbHelper(string id) {
            _dbName = $"DevExtreme_AspNet_Data_Tests_{id}_DB";
            _dbFilePath = Path.Combine(
                Path.GetDirectoryName(typeof(SqlServerTestDbHelper).GetTypeInfo().Assembly.Location),
                _dbName + ".mdf"
            );
        }

        public string ConnectionString {
            get { return $"Data Source={LOCAL_DB}; AttachDbFileName={_dbFilePath}; Initial Catalog={_dbName}"; }
        }

        public void ResetDatabase() {
            // Possibly related: https://stackoverflow.com/a/46142857

            using(var conn = new SqlConnection($"Data Source={LOCAL_DB}")) {
                conn.Open();

                void Exec(string sql) {
                    using(var cmd = conn.CreateCommand()) {
                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                    }
                }

                try {
                    Exec($"drop database [{_dbName}]");
                } catch {
                }

                Exec($"create database [{_dbName}] on (name='{_dbName}', filename='{_dbFilePath}')");
            }
        }

    }

}
