using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using System;

namespace DevExtreme.AspNet.Data.Tests.Xpo {

    static class UnitOfWorkHelper {
        static readonly object LOCK = new object();
        static IDataLayer DATA_LAYER;

        public static void Exec(Action<UnitOfWork> action) {
            lock(LOCK) {
                XpoDefault.Session = null;

                if(DATA_LAYER == null) {
                    var sqlHelper = new SqlServerTestDbHelper("DevExtreme_AspNet_Data_Tests_Xpo_DB");
                    sqlHelper.ResetDatabase();

                    var dict = new ReflectionDictionary();
                    dict.GetDataStoreSchema(
                        typeof(DefaultSort.DataItem)
                    );

                    var provider = XpoDefault.GetConnectionProvider(
                        sqlHelper.ConnectionString,
                        AutoCreateOption.SchemaOnly
                    );

                    DATA_LAYER = new SimpleDataLayer(dict, provider);
                }

                using(var uow = new UnitOfWork(DATA_LAYER)) {
                    action(uow);
                }
            }
        }
    }

}
