using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using System;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Tests.Xpo {

    static class UnitOfWorkHelper {
        static IDataLayer DATA_LAYER;

        public static async Task ExecAsync(Func<UnitOfWork, Task> action) {
            XpoDefault.Session = null;

            if(DATA_LAYER == null) {
                var sqlHelper = new SqlServerTestDbHelper("Xpo");
                sqlHelper.ResetDatabase();

                var dict = new ReflectionDictionary();
                dict.GetDataStoreSchema(
                    typeof(DefaultSort.DataItem),
                    typeof(RemoteGroupingStress.DataItem),
                    typeof(Summary.DataItem),
                    typeof(Bug339.DataItem),
                    typeof(PaginateViaPrimaryKey.DataItem),
                    typeof(Async.DataItem),
                    typeof(ExpandLinqSumType.DataItem)
                );

                var provider = XpoDefault.GetConnectionProvider(
                    sqlHelper.ConnectionString,
                    AutoCreateOption.SchemaOnly
                );

                DATA_LAYER = new SimpleDataLayer(dict, provider);
            }

            using(var uow = new UnitOfWork(DATA_LAYER)) {
                await action(uow);
            }
        }

        public static async Task ExecAsync(Action<UnitOfWork> action) {
            await ExecAsync(context => {
                action(context);
                return Task.CompletedTask;
            });
        }
    }

}
