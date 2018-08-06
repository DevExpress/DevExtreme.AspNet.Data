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
                    var sqlHelper = new SqlServerTestDbHelper("NH");
                    sqlHelper.ResetDatabase();

                    FACTORY = Fluently.Configure()
                        .Database(MsSqlConfiguration.MsSql2012.ConnectionString(sqlHelper.ConnectionString))
                        .Mappings(m => m.FluentMappings.AddFromAssembly(typeof(SessionFactoryHelper).Assembly))
                        .ExposeConfiguration(config => new SchemaExport(config).Create(false, true))
                        .BuildSessionFactory();
                }

                using(var session = FACTORY.OpenSession()) {
                    action(session);
                }
            }
        }

    }

}
