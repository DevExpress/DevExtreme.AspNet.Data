using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using System;

namespace DevExtreme.AspNet.Data.Tests.NH {

    static class SessionFactoryHelper {
        static ISessionFactory FACTORY;
        static object LOCK = new object();

        public static void Exec(Action<ISession> action) {
            lock(LOCK) {
                if(FACTORY == null) {
                    var sqlHelper = new SqlServerTestDbHelper("DevExtreme_AspNet_Data_Tests_NH_DB");
                    sqlHelper.ResetDatabase();

                    FACTORY = Fluently.Configure()
                        .Database(MsSqlConfiguration.MsSql2012
                            .ConnectionString(sqlHelper.ConnectionString)
                            .ShowSql()
                        )
                        .Mappings(m => m.FluentMappings.AddFromAssembly(typeof(SessionFactoryHelper).Assembly))
                        .ExposeConfiguration(config => new SchemaExport(config).Create(true, true))
                        .BuildSessionFactory();
                }

                using(var session = FACTORY.OpenSession()) {
                    action(session);
                }
            }
        }

    }

}
