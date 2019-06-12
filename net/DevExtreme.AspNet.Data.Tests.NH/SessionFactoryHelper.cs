using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Tests.NH {

    static class SessionFactoryHelper {
        static ISessionFactory FACTORY;

        public static async Task ExecAsync(Func<ISession, Task> action) {
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
                await action(session);
            }
        }

        public static async Task ExecAsync(Action<ISession> action) {
            await ExecAsync(context => {
                action(context);
                return Task.CompletedTask;
            });
        }

    }

}
